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
    public class Mercury : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mercury Shards");
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.damage = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        private int ai = -1;
        public const int
            Ground = 0,
            Falling = 1;
        public float velX;
        public float velY;
        public Vector2 start;
        private Dust dust;
        public Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        public override bool PreAI()
        {
            switch (ai)
            {
                case -1:
                    Initialize();
                    goto case 0;
                case 0:
                    ai = 0;
                    break;
            }
            return true;
        }
        public override void AI()
        {
            switch ((int)Projectile.ai[0])
            {
                case Ground:
                    dust.velocity = Projectile.velocity;
                    Projectile.velocity.Y = velY;
                    Projectile.rotation -= 0.017f * 5f;
                    break;
                case Falling:
                    Projectile.rotation += 0.017f * 5f;
                    Projectile.velocity = new Vector2(velX, velY);
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if ((int)Projectile.ai[0] != Ground)
                NPCs.ArchaeaNPC.DustSpread(Projectile.Center, 1, 1, 6, 3, 2f);
        }
        protected void Initialize()
        {
            switch ((int)Projectile.ai[0])
            {
                case Ground:
                    velY = -8f;
                    dust = defaultDust;
                    Projectile.timeLeft = 30;
                    Projectile.tileCollide = false;
                    Projectile.friendly = true;
                    NPCs.ArchaeaNPC.DustSpread(Projectile.Center, 1, 1, 6, 4, 2f);
                    break;
                case Falling:
                    start = new Vector2(Projectile.ai[1], owner.position.Y - 600f);
                    Projectile.position = start;
                    velX = Main.rand.NextFloat(-2f, 2f);
                    velY = 12f;
                    break;
            }
        }
        private Dust defaultDust
        {
            get { return Dust.NewDustDirect(Projectile.Center, 1, 1, 6); }
        }
    }
}
