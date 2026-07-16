using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;
using ArchaeaMod.Projectiles;
using ArchaeaMod.NPCs;

namespace ArchaeaMod.Merged.Projectiles
{
    public class magno_orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magno Orb");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1.2f;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3600;
            Projectile.damage = 28;
            Projectile.knockBack = 7.5f;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.CountsAsClass(DamageClass.Magic);
            Projectile.netImportant = true;
        }

        public void Initialize()
        {
            oldAngle = (float)Math.Atan2(Main.MouseWorld.Y - Projectile.position.Y, Main.MouseWorld.X - Projectile.position.X);
            Projectile.velocity *= 2f;
        }
        bool init = false;
        bool hitbox = false;
        int ticks = 0;
        float num, num2;
        int DustType;
        float oldAngle;
        float speed = 10f;
        int[] proj = new int[2];
        Rectangle mouseBox;
        float distance, radius = 0f, angle0 = 0f, angle1 = 0f;
        double cos = 0d;
        double sine = 0d;
        public override bool PreAI()
        {
            switch ((int)Projectile.ai[0])
            {
                case -1:
                    Projectile.ai[0] = 0;
                    for (int i = 0; i < 2; i++) { 
                        proj[i] = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, this.Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10f, i);
                        ArchaeaItem.SyncProj(Main.projectile[proj[i]]);
                    }
                    Main.projectile[proj[0]].localAI[0] = proj[1];
                    goto case 0;
                case 0:
                    return true;
                case 10:
                    Projectile.tileCollide = false;
                    int index = (int)Projectile.ai[1];
                    if (!hitbox) {
                        mouseBox = new Rectangle((int)Main.MouseWorld.X - 30, (int)Main.MouseWorld.Y - 30, 60, 60);
                        distance = Vector2.Distance(Main.player[Projectile.owner].Center, Main.MouseWorld);
                        radius = distance / 2f;
                        angle0 = Main.player[Projectile.owner].AngleTo(Main.MouseWorld);
                        angle1 = Main.player[Projectile.owner].AngleFrom(Main.MouseWorld);
                        hitbox = true;
                    }
                    Vector2 center = Main.player[Projectile.owner].Center + ArchaeaMod.NPCs.ArchaeaNPC.AngleToSpeed(angle0, radius);
                    if (index == 0)
                    {
                        angle1 += Draw.radian * 5f;
                        if (!Main.projectile[(int)Projectile.localAI[0]].active) {
                            Projectile.Kill();
                        }
                    }
                    else if (index == 1)
                    {
                        angle1 -= Draw.radian * 5f;
                        if (ticks++ > 180 / 5) {
                            Projectile.Kill();
                        }
                    }
                    cos = center.X + radius * Math.Cos(angle1);
                    sine = center.Y + radius * Math.Sin(angle1);
                    Projectile.position = new Vector2((float)cos, (float)sine);
                    dust();
                    if (Projectile.position.X < 0f && Projectile.oldPosition.X >= 0f || Projectile.position.X > 0f && Projectile.oldPosition.X <= 0f || Projectile.position.Y < 0f && Projectile.oldPosition.Y >= 0f || Projectile.position.Y > 0f && Projectile.oldPosition.Y <= 0f)
                        Projectile.netUpdate = true;
                    return false;
                default:
                    return false;
            }
        }
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            Player player = Main.player[Projectile.owner];

            float Angle = (float)Math.Atan2(Main.MouseWorld.Y - Projectile.position.Y, Main.MouseWorld.X - Projectile.position.X);
            if (Main.mouseLeft && Vector2.Distance(Main.MouseWorld - Projectile.position, Vector2.Zero) < 128)
            {
                Projectile.rotation = Angle;
                Rectangle mouseBox = new Rectangle((int)Main.MouseWorld.X - 8, (int)Main.MouseWorld.Y - 8, 16, 16);
                if (!mouseBox.Intersects(Projectile.Hitbox))
                    Projectile.velocity = Distance(null, Angle, speed);
                else Projectile.velocity = Vector2.Zero;

                oldAngle = (float)Math.Atan2(Main.MouseWorld.Y - Projectile.Center.Y, Main.MouseWorld.X - Projectile.Center.X);
            }
            else
            {
                Projectile.velocity = Distance(null, oldAngle, 16f);
                Projectile.rotation = oldAngle;
            }
            if (Vector2.Distance(player.position - Projectile.position, Vector2.Zero) > 1024)
            {
                Projectile.Kill();
            }
            dust();
            if (Projectile.velocity.X <= Projectile.oldVelocity.X || Projectile.velocity.X > Projectile.oldVelocity.X || Projectile.velocity.Y <= Projectile.oldVelocity.Y || Projectile.velocity.Y > Projectile.oldVelocity.Y)
                Projectile.netUpdate = true;
        }
        private void dust(int num = 1)
        {
            DustType = ModContent.DustType<Merged.Dusts.magno_dust>();
            for (int k = 0; k < num; k++)
            {
                int orbDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 0, default(Color), 1f);
                Main.dust[orbDust].noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int num = 48 * 3;
            int num2 = 24 * 3;
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center - new Vector2(num2, num2), num, num, ModContent.DustType<Dusts.magno_dust>(), 0, 0, 0, default, 2);
                Dust.NewDust(Projectile.Center - new Vector2(num2, num2), num, num, DustID.Smoke, 0, 0, 0, default, 2f);
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < num)
                {
                    ArchaeaNPC.StrikeNPC(npc, Projectile.damage, Projectile.knockBack, npc.position.X < Projectile.position.X ? -1 : 1, Main.rand.NextBool());
                }
            }
            //for (float k = 0; k < MathHelper.ToRadians(360); k += 0.017f * 9)
            //{
            //    int Proj1 = Projectile.NewProjectile(projectile.position + new Vector2(projectile.width / 2, projectile.height / 2), Distance(null, k, 16f), mod.ProjectileType("dust_diffusion"), projectile.damage, 7.5f, projectile.owner, projectile.Center.X, projectile.Center.Y);
            //    //  if (Main.netMode == 1) NetMessage.SendData(27, -1, -1, null, Proj1);
            //}
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }

        public void SyncProj(int netID)
        {
            if (Main.netMode == netID)
            {
            }
        }

        public Vector2 AB(int rise, int run)
        {
            return new Vector2(rise, run);
        }
        public Vector2 Distance(Player player, float Angle, float Radius)
        {
            float VelocityX = (float)(Radius * Math.Cos(Angle));
            float VelocityY = (float)(Radius * Math.Sin(Angle));

            return new Vector2(VelocityX, VelocityY);
        }
    }
}
