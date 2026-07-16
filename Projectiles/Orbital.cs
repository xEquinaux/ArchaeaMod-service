using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;

namespace ArchaeaMod.Projectiles
{
    public class Orbital : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Orb");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 44;
            Projectile.damage = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        private bool update = true;
        private int ai = -1;
        private float angle;
        private Vector2 center;
        private Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        private Target target;
        public override bool PreAI()
        {
            return PreAI(update && Projectile.timeLeft > 30);
        }
        public bool PreAI(bool update)
        {
            if (update)
            {
                switch (ai)
                {
                    case -1:
                        center = owner.Center;
                        goto case 0;
                    case 0:
                        ai = 0;
                        angle = Projectile.ai[0];
                        Projectile.penetrate = -1;
                        float a = NPCs.ArchaeaNPC.AngleTo(center, Main.MouseWorld);
                        if (Projectile.Distance(Main.MouseWorld) > Projectile.width)
                            center += NPCs.ArchaeaNPC.AngleToSpeed(a, 10f);
                        else center = Main.MouseWorld;
                        Projectile.Center = NPCs.ArchaeaNPC.AngleBased(center, angle, 45f);
                        if (owner.ownedProjectileCounts[Projectile.type] == 6)
                            goto case 1;
                        return false;
                    case 1:
                        ai = 1;
                        Projectile.penetrate = 1;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        private int type;
        public override void AI()
        {
            if (owner.inventory[owner.selectedItem].type != ModContent.ItemType<n_Staff>())
                Projectile.Kill();
            if (ArchaeaItem.Elapsed(30))
            {
                target = Target.GetClosest(owner, Target.GetTargets(Projectile, 300f).Where(t => t != null).ToArray());
                Projectile.netUpdate = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter == 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 6)
            {
                Projectile.frame = 0;
            }

            switch (type)
            {
                case 0:
                    if (target == null)
                    {
                        center = Main.MouseWorld;
                        angle += Draw.radian * 4f;
                        Projectile.Center = NPCs.ArchaeaNPC.AngleBased(center, angle, 45f);
                        return;
                    }
                    else type = 1;
                    break;
                case 1:
                    if (target == null || !target.npc.active || target.npc.life <= 0)
                        goto case 2;
                    float a = NPCs.ArchaeaNPC.AngleTo(Projectile.Center, target.npc.Center);
                    Projectile.velocity += Speed(a, target.npc.Center);
                    NPCs.ArchaeaNPC.VelocityClamp(Projectile, -8f, 8f);
                    break;
                case 2:
                    float a2 = NPCs.ArchaeaNPC.AngleTo(Projectile.Center, center);
                    center = Main.MouseWorld;
                    Projectile.velocity += Speed(a2, center);
                    NPCs.ArchaeaNPC.VelocityClamp(Projectile, -5f, 5f);
                    if (Projectile.Distance(Main.MouseWorld) < 90f)
                        type = 0;
                    break;
                default:
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            NPCs.ArchaeaNPC.DustSpread(Projectile.Center, Projectile.width, Projectile.height, 6, 4, 2f);
        }
        public Vector2 Speed(float angle, Vector2 target)
        {
            return NPCs.ArchaeaNPC.AngleToSpeed(angle, Projectile.Distance(target) * 0.5f);
        }
    }
}
