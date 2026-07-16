using System;
using ArchaeaMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_orb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinnabar Orb");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 1f;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.damage = 10;
            Projectile.knockBack = 7.5f;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.CountsAsClass(DamageClass.Magic);
            Projectile.netImportant = true;
        }

        public float degrees
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        public void Initialize()
        {
            Player player = Main.player[Projectile.owner];

            center = new Vector2((player.position.X - Projectile.width / 2) + player.width / 2, (player.position.Y - Projectile.height / 2) + player.height / 2);

            ProjX = center.X + (float)(radius * Math.Cos(Projectile.ai[0]));
            ProjY = center.Y + (float)(radius * Math.Sin(Projectile.ai[0]));

            startAngle = (float)Math.Atan2(center.Y - ProjY, center.X - ProjX);

            Projectile.position = center;
        }
        bool init = false;
        bool target = false;
        int npcTarget = 0, oldNpcTarget;
        int ticks = 15;
        int timer;
        int DustType;
        float ProjX, ProjY;
        float startAngle, npcAngle;
        float radius = 16f;
        const float radians = 0.017f;
        Vector2 center;
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            Player player = Main.player[Projectile.owner];

            float Angle2 = (float)Math.Atan2(player.position.Y - Projectile.position.Y, player.position.X - Projectile.position.X);
            Projectile.rotation = Angle2 + (radians * -90f);

            if (ticks > 0)
            {
                ticks--;
                Projectile.position += Distance(null, startAngle, 8f);
            }

            if (ticks == 0)
            {
                if (!target)
                {
                    center = new Vector2((player.position.X - Projectile.width / 2) + player.width / 2, (player.position.Y - Projectile.height / 2) + player.height / 2);
                    radius = 128f;

                    degrees += radians * 3f;
                    Projectile.position.X = center.X + (float)(radius * Math.Cos(degrees));
                    Projectile.position.Y = center.Y + (float)(radius * Math.Sin(degrees));
                }
                foreach (NPC n in Main.npc)
                {
                    if((!target && npcTarget == 0f) && n.active && !n.friendly && !n.dontTakeDamage && !n.immortal && n.target == player.whoAmI && ((n.lifeMax >= 50 && (Main.expertMode || Main.hardMode)) || (n.lifeMax >= 15 && !Main.expertMode && !Main.hardMode)))
                    {
                        if (Vector2.Distance(n.position - Projectile.position, Vector2.Zero) < 256f)
                        {
                            oldNpcTarget = npcTarget;
                            npcTarget = n.whoAmI;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    NPC nme = Main.npc[npcTarget];
                    float npcAngle = (float)Math.Atan2(nme.position.Y - Projectile.position.Y, nme.position.X - Projectile.position.X);

                    Projectile.velocity = Distance(null, npcAngle, 16f);

                    int direction = 0;
                    if (Projectile.velocity.X < 0)
                        direction = -1;
                    else direction = 1;
                    if(Projectile.Hitbox.Intersects(nme.Hitbox))
                    {
                        ArchaeaNPC.StrikeNPC(nme, Projectile.damage, Projectile.knockBack, direction, false);
                        Projectile.Kill();
                    }

                    if (!nme.active || nme.life <= 0)
                    {
                        target = false;
                    } 
                }
            }
            timer++;
            DustType = ModContent.DustType<Merged.Dusts.cinnabar_dust>();
            if (timer % 6 == 0)
            {
                int orbDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 0, Color.White, 1f);
                Main.dust[orbDust].noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            DustType = ModContent.DustType<Merged.Dusts.cinnabar_dust>();
            for (int k = 0; k < 6; k++)
            {
                int Dust1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 0, Color.White, 2f);
                int Dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 2f);
            }
        }

        public Vector2 Distance(Player player, float Angle, float Radius)
        {
            float VelocityX = (float)(Radius * Math.Cos(Angle));
            float VelocityY = (float)(Radius * Math.Sin(Angle));

            return new Vector2(VelocityX, VelocityY);
        }
    }
}
