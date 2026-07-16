using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Buffs;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_dagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinnabar Dagger");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.scale = 1f;
            Projectile.CountsAsClass(DamageClass.Throwing);
        }

        public void Initialize()
        {
            Player player = Main.player[Projectile.owner];
            Angle = (float)Math.Atan2(Projectile.position.Y - Main.MouseWorld.Y, Projectile.position.X - Main.MouseWorld.X);

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Angle += radians * -90f;
                Projectile.rotation = Angle + radians;
            }
            else
            {
                Angle += radians * -90f;
                Projectile.rotation = Angle + radians;
            }
        }
        bool init = false;
        int ticks = 0;
        float Angle;
        float degrees = 0;
        const float radians = 0.017f;
        Player player;
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            
            ticks++;
            if (ticks >= 20)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.X < 0f)
                {
                    degrees = radians * 15f;
                    Projectile.rotation -= degrees;
                }
                else
                {
                    degrees = radians * 15f;
                    Projectile.rotation += degrees;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.rand.NextFloat() >= 0.75f)
            {
                int daggerDrop = Item.NewItem(Item.GetSource_None(), Projectile.Center, ModContent.ItemType<Merged.Items.cinnabar_dagger>(), 1, true, 0, false, false);
            }
            for (int k = 0; k < 8; k++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 4, 0f, 0f, 0, default(Color), 1f);
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(ModContent.BuffType<mercury>(), 300);
            }
        }
    }
}
