using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArchaeaMod.TakerylProject.Projectiles
{
    //  Possibility to thrown any held object
    public class ThrownSword : ModProjectile
    {
        private bool preAI;
        private short ai
        {
            get { return (short)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        private short graphic
        {
            get { return (short)Projectile.ai[1]; }
        }
        private int ticks
        {
            get { return (int)Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        }
        private const float gravityMax = 9.17f;
        private Texture2D texture;
        private Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        public const short
            AI_Default = 0,
            AI_Embed4Owner = 1,
            AI_Embed4All = 2,
            AI_Explode = 10;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Thrown Sword");
        }
        public override void SetDefaults()
        {
            Projectile.damage = 30;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool PreAI()
        {
            if (!preAI)
            {
                texture = TextureAssets.Item[graphic].Value;
                Projectile.width = texture.Width;
                Projectile.height = texture.Height;
                Projectile.knockBack = 5f;
                Projectile.position.Y += Projectile.width / 2;
                //  set variations for initialze speed adjustments
                //  flying (default) = 0
                if (ai == AI_Default)
                {
                    Projectile.velocity *= 2f;
                    Projectile.tileCollide = true;
                }
                preAI = true;
            }
            return true;
        }
        public override void AI()
        {
            ticks++;
            Func<int, bool> fall = delegate(int max) {
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 4f;
                if (ticks > max) {
                    //  Projectile.rotation += 0.017f * 5f;
                    if (Projectile.velocity.Y < gravityMax) {
                        Projectile.velocity.Y += gravityMax / 10f;
                    }
                    return true;
                }
                return false;
            };
            //  set variations based on when 'ticks' reaches a certain value for each ai
            //  Default = 0
            //  Sword explodes (no item) == 10
            if (ai == AI_Default)
            {
                fall(100);
            }
            if (ai == AI_Embed4Owner)
            {
                if (!TileCollide())
                    fall(100);
                else 
                {
                    Projectile.velocity = Vector2.Zero;
                    if (owner.Hitbox.Intersects(Projectile.Hitbox))
                        Projectile.Kill();
                }
            }
            if (ai == AI_Embed4All)
            {
                if (!TileCollide())
                    fall(100);
                else 
                {
                    Projectile.velocity = Vector2.Zero;
                    if (Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)].Hitbox.Intersects(Projectile.Hitbox))
                        Projectile.Kill();
                }
            }
            Projectile.timeLeft = 3;
        }
        
        public override void OnKill(int timeLeft)
        {
            //  need global explosion effect for one style
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, texture.Width, texture.Height, DustID.Stone);
            Item.NewItem(Projectile.GetSource_Death(), Projectile.position, texture.Width, texture.Height, graphic);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (texture != null)
                Main.spriteBatch.Draw(texture, Projectile.position - Main.screenPosition, null, Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            return false;
        } 

        private bool TileCollide()
        {
            int x = (int)Projectile.position.X / 16;
            int y = (int)Projectile.position.Y / 16;
            int w = texture.Width / 16 / 2;
            int h = texture.Height / 16;
            for (int i = x; i < x + w; i++)
            for (int j = y; j < y + h; j++)
            {   
                Tile tile = Main.tile[i, j];
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                    return true;
            }
            return false;
        }
    }
}