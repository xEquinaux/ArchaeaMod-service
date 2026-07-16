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
using ArchaeaMod.NPCs;

namespace ArchaeaMod.Projectiles
{
    public class Tomohawk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tomohawk");
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.damage = 24;
        }

        private int ai = -1;
        private int direction;
        private float yOffset = 16f;
        private Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        public override bool PreAI()
        {
            if ((int)Projectile.ai[0] == 10)
            {
                Projectile.tileCollide = false;
                return true;
            }
            switch ((int)Projectile.localAI[0])
            {
                case 0:
                    direction = owner.direction;
                    Projectile.position.Y -= yOffset;
                    goto case 1;
                case 1:
                    Projectile.localAI[0] = -1;
                    int index = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, this.Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10);
                    Main.projectile[index].timeLeft = 300;
                    Main.projectile[index].direction = owner.direction;
                    Main.projectile[index].localAI[1] = 60f;
                    return true;
                default:
                    return true;
            }
        }

        public override void AI()
        {
            Projectile.rotation -= Draw.radian * 5f * Projectile.direction;
            if ((int)Projectile.ai[0] == 10)
            {
                //Vector2 lerp = Vector2.Lerp(new Vector2(-2, 2), new Vector2(2, -2), Projectile.timeLeft / 600f);
                float radius = Projectile.localAI[1];
                double cos  = Main.player[Projectile.owner].Center.X - Projectile.width / 2 + radius * Math.Cos(-90f * Draw.radian);
                double sine = Main.player[Projectile.owner].Center.Y - Projectile.height / 2 + radius * Math.Sin(-90f * Draw.radian);
                Projectile.position = new Vector2((float)cos, (float)sine);
                if (Projectile.timeLeft < 180)
                {
                    Projectile.ai[0] = 15;
                    Projectile.tileCollide = false;
                }
            }
            if ((int)Projectile.ai[0] == 15)
            {
                NPC npc = Projectile.FindTargetWithinRange(1000f);
                if (npc != null)
                { 
                    Projectile.velocity = NPCs.ArchaeaNPC.AngleToSpeed(NPCs.ArchaeaNPC.AngleTo(Projectile.Center, npc.Center), 2f);
                }
                else Projectile.Kill();
            }
            Dusts(1);
            fadeOut();
            if (Projectile.velocity.X < 0f && Projectile.oldVelocity.X >= 0f || Projectile.velocity.X > 0f && Projectile.oldVelocity.X <= 0f || Projectile.velocity.Y < 0f && Projectile.oldVelocity.Y >= 0f || Projectile.velocity.Y > 0f && Projectile.oldVelocity.Y <= 0f)
                Projectile.netUpdate = true;
        }
        private void fadeOut()
        {
            if (Projectile.timeLeft < 10)
            {
                if ((Projectile.alpha += 20) < 200)
                    Projectile.timeLeft = 10;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Dusts(8, true);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ArchaeaNPC.StrikeNPC(target, Projectile.damage, Projectile.knockBack, target.position.X < Projectile.position.X ? -1 : 1, Main.rand.NextBool());
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.townNPC;
        }
        protected void Dusts(int amount, bool noGravity = false)
        {
            for (int i = 0; i < amount; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(amount, amount), amount * 2, amount * 2, 6, Projectile.velocity.X, Projectile.velocity.Y);
                dust.noGravity = noGravity;
            }
        }
    }
}
