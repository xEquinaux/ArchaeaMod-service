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
    public class Javelin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Javelin");
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.damage = 28;
            Projectile.knockBack = 0f;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Throwing;
        }
        private int ai = -1;
        private Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    ThrowAngle(); 
                    for (int n = 0 ; n < 2; n++)
                    {
                        int index = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity, this.Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10, n);
                        Main.projectile[index].localAI[0] = 1;
                    }
                    goto case 1;
                case 1:
                    Projectile.ai[0] = 1;
                    return true;
                case 10:
                    if (Projectile.localAI[0] == 1)
                    {
                        ThrowAngle();
                        int i = (int)Projectile.ai[1];
                        double cos = 6f * (Math.Cos(NPCs.ArchaeaNPC.AngleTo(Main.player[Projectile.owner].Center, Main.MouseWorld) - (i == 0 ? -(Draw.radian * 10f) : Draw.radian * 10f)));
                        double sine = 6f * (Math.Sin(NPCs.ArchaeaNPC.AngleTo(Main.player[Projectile.owner].Center, Main.MouseWorld) - (i == 0 ? -(Draw.radian * 10f) : Draw.radian * 10f)));
                        Projectile.velocity = new Vector2((float)cos, (float)sine);
                        Projectile.netUpdate = true;
                        Projectile.localAI[0] = 2;
                    }
                    return true;
                default:
                    return true;
            }
        }
        private void ThrowAngle()
        {
            Projectile.rotation = NPCs.ArchaeaNPC.AngleTo(owner.Center, Main.MouseWorld) + (float)(Math.PI / 4f);
            rotate = Projectile.rotation;
            rotate += (float)Math.PI / 4f;
            Projectile.position = new Vector2(ArchaeaItem.StartThrowX(owner), Projectile.position.Y - 16f);
        }
        private int time = 90;
        private float rotate;
        private float x;
        private float y;
        private Vector2 hit;
        private Target target;
        private NPC npc;
        public override void AI()
        {
            if (ArchaeaItem.Elapsed(5))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 0, default(Color), 2f);
        }
    }
}
