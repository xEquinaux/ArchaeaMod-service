using System;
using ArchaeaMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.Merged.Projectiles
{
    public class cinnabar_spore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cinnabar Spore");
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.scale = 1f;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.damage = 10;
            Projectile.knockBack = 0f;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.CountsAsClass(DamageClass.Magic);
        }

        bool spawnOK = false;
        int buffer = 256;
        int x, y;
        public void Initialize()
        {
            maxTime = Projectile.timeLeft;

            Player player = Main.player[Projectile.owner];

            foreach (NPC n in Main.npc)
            {
                if (!target && n.active && !n.friendly && !n.dontTakeDamage && !n.immortal && n.target == player.whoAmI && ((n.lifeMax >= 10 && !Main.expertMode) || (n.lifeMax >= 30 && (Main.expertMode || Main.hardMode))))
                {
                    npcTarget = n.whoAmI;
                    target = true;
                }
            }
            if (target && Vector2.Distance(player.position - Main.npc[npcTarget].position, Vector2.Zero) <= 512)
            {
                Projectile.Center = Main.npc[npcTarget].Center;
            }
            else NewPosition(player, 128);
        }
        bool init = false;
        bool target = false;
        int ticks = 0;
        int maxTime;
        int npcTarget;
        float rot = 0;
        const float radians = 0.017f;
        Vector2 npcCenter;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ArchaeaMod.Buffs.mercury>(), 450);
            if (Main.netMode == 2)
                NetMessage.SendData(MessageID.NPCBuffs, -1, -1, null, target.whoAmI);
        }
        public override void AI()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }

            Player player = Main.player[Projectile.owner];

            NPC nme = Main.npc[npcTarget];

            int direction = 0;
            if (Projectile.velocity.X < 0)
                direction = -1;
            else direction = 1;

            Projectile.velocity.Y = 1f;

            foreach (NPC n in Main.npc)
            {
                if (n.active && !n.friendly && !n.dontTakeDamage && !n.immortal)
                {
                    if (Projectile.Hitbox.Intersects(n.Hitbox))
                    {
                        if (ticks % 60 == 0)
                        {
                            ArchaeaNPC.StrikeNPC(n, Projectile.damage, Projectile.knockBack, direction, false);
                        }
                    }
                }
            }

            ticks++;

            if (!IsTile(Projectile.position.X, Projectile.position.Y))
            {
                Projectile.alpha = 255 * (maxTime - ticks) / maxTime;
                Lighting.AddLight(Projectile.Center, new Vector3(0.804f, 0.361f, 0.361f));
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter == 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
            }

            Tile tile = Main.tile[(int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16];
            if (!Main.tileSolid[tile.TileType] || !tile.HasTile)
                Projectile.timeLeft = 5;
        }
        public override void OnKill(int timeLeft)
        {
            int DustType = ModContent.DustType<Merged.Dusts.cinnabar_dust>();
            int dustType2 = ModContent.DustType<Merged.Dusts.c_silver_dust>();
            for (int k = 0; k < 3; k++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 0, default(Color), 1.2f);
                Main.dust[killDust].noGravity = false;
            }
            for (int k = 0; k < 2; k++)
            {
                int killDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType2, 0f, 0f, 0, default(Color), 1.2f);
                Main.dust[killDust2].noGravity = false;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public void SyncProj(float x, float y)
        {
            if (Main.netMode == 2)
            {
                Projectile.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI, Projectile.position.X, Projectile.position.Y);
                Projectile.netUpdate = true;
            }
            else if (Main.netMode == 0)
            {
                Projectile.position = new Vector2(x, y);
            }
        }

        public void NewPosition(Player player, int maxTries)
        {
            float PosX = Main.rand.Next((int)player.position.X - buffer, (int)player.position.X + buffer);
            float PosY = Main.rand.Next((int)player.position.Y - (int)(buffer * 1.67f), (int)player.position.Y - buffer);
            for (int i = 0; i < maxTries; i++)
            {
                if(!IsTile(PosX, PosY))
                {
                    SyncProj(PosX, PosY);
                    break;
                }
            }
        }
        public bool IsTile(float x, float y)
        {
            int i = (int)x / 16;
            int j = (int)y / 16;
            bool Active = Main.tile[i, j].HasTile == true;
            bool Solid = Main.tileSolid[Main.tile[i, j].TileType] == true;

            if (Solid && Active) return true;
            else return false;
        }
    }
}
