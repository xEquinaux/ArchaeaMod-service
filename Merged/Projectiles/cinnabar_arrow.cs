using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Buffs;
using ArchaeaMod.Projectiles;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_arrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mercury Arrow");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.damage = 12;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.hostile  = false;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Default;
            Projectile.CountsAsClass(DamageClass.Ranged);
            Projectile.arrow = true;
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
                Player player = Main.player[Projectile.owner];
                Angle = (float)Math.Atan2(player.Center.Y - Main.MouseWorld.Y, player.Center.X - Main.MouseWorld.X);

                if (Projectile.velocity.X < 0f)
                {
                    Projectile.spriteDirection = -1;
                    Angle += radians * -90f;
                }
                else Angle += radians * -90f;
                Projectile.netUpdate = true;
                init = true;
            }

            ticks++;
            if (ticks >= 20)
                Projectile.velocity.Y += 0.10f;

            Projectile.rotation = Projectile.velocity.ToRotation() + Draw.radian * 90f;

            if (Projectile.velocity.X < 0f && Projectile.oldVelocity.X >= 0f || Projectile.velocity.X > 0f && Projectile.oldVelocity.X <= 0f || Projectile.velocity.Y < 0f && Projectile.oldVelocity.Y >= 0f || Projectile.velocity.Y > 0f && Projectile.oldVelocity.Y <= 0f)
                Projectile.netUpdate = true;

            int DustType = ModContent.DustType<Merged.Dusts.c_silver_dust>();
            int Dust1 = Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 1, 1, DustType, 0f, 0f, 0, Color.White, 1.4f); // old dust: 159, Color.OrangeRed
            Main.dust[Dust1].noGravity = true;
        }
        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 4, 0f, 0f, 0, default(Color), 1f);
            }
            if (ArchaeaPlayer.IsEquipped(Main.player[Projectile.owner], ModContent.ItemType<Items.Armors.magnoheadgear>(), ModContent.ItemType<Items.Armors.magnoplate>(), ModContent.ItemType<Items.Armors.magnogreaves>()))
            {
                if (Main.rand.NextFloat() < 0.15f)
                {
                    ArchaeaProjectiles.Explode(Projectile, ModContent.DustType<Dusts.cinnabar_dust>(), 30, Projectile.damage, Projectile.knockBack, true, ModContent.BuffType<mercury>(), 180, true, 10);
                    ArchaeaProjectiles.Explode(Projectile, DustID.Smoke, 36, Projectile.damage, Projectile.knockBack, false);
                }
            }
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }

        public void SyncProj(int netID)
        {
            if (Main.netMode == netID)
            {
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI, Projectile.position.X, Projectile.position.Y, Projectile.rotation);
                Projectile.netUpdate = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                target.AddBuff(ModContent.BuffType<mercury>(), 300);
            }
        }
        /*  public override void SendExtraAI(BinaryWriter writer)
            {
                writer.Write(Angle);
            }
            public override void ReceiveExtraAI(BinaryReader reader)
            {
                projectile.rotation = reader.ReadSingle();
            }   */
    }
}
