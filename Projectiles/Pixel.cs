using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using ArchaeaMod.Items;
using Terraria.Audio;

namespace ArchaeaMod.Projectiles
{
    public class Pixel : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shard");
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * alpha;
        }
        public override bool? CanCutTiles() 
            => type != Drip && type != Diffusion;
        private bool direction;
        private int ai;
        private bool factory => Projectile.localAI[0] == -100f;
        public const int
            None = -1,
            Default = 0,
            Sword = 1,
            Active = 2,
            Gravity = 3,
            AntiGravity = 4,
            Diffusion = 5,
            Drip = 6;
        private float rotate;
        private float alpha;
        private float rand 
        {
            get {return Projectile.localAI[1]; }
            set { Projectile.localAI[1] = value; }
        }
        private int type
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }
        private Dust dust;
        private Player owner
        {
            get { return Main.player[Projectile.owner]; }
        }
        private float endY;
        public override bool PreAI()
        {
            if (ai == 0)
            {
                if (type == Drip)
                { 
                    SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
                    Projectile.tileCollide = true;
                    Projectile.damage = 0;//30;
                    ai = 3;
                }
                if (type == Diffusion)
                {
                    Projectile.timeLeft = 12;
                    Projectile.tileCollide = false;
                    Projectile.damage = 35;
                    ai = 1;
                }
            }
            switch (ai)
            {
                case 0:
                    direction = owner.direction == 1 ? true : false;
                    rotate = direction ? 0f : (float)Math.PI;
                    dust = SetDust();
                    endY = owner.position.Y;
                    goto case 1;
                case 1:
                    ai = 1;
                    break;
                case 3:
                    if (Projectile.velocity.Y < 6.12f)
                    { 
                        Projectile.velocity.Y += 0.917f;
                    }
                    else Projectile.velocity.Y = 6.12f;
                    break;
            }
            return true;
        }
        public void _AIType()
        {
            switch (type)
            {
                case None:
                    Projectile.alpha = 0;
                    alpha = 1f;
                    Projectile.timeLeft = 100;
                    break;
                case Default:
                    dust.position = Projectile.position;
                    break;
                case Sword:
                    NPCs.ArchaeaNPC.RotateIncrement(true, ref rotate, (float)Math.PI / 2f, 0.15f, out rotate);
                    Projectile.velocity += NPCs.ArchaeaNPC.AngleToSpeed(rotate, 0.25f);
                    Projectile.tileCollide = Projectile.position.Y > endY;
                    dust.position = Projectile.position;
                    break;
                case Active:
                    dust = SetDust();
                    break;
                case AntiGravity:
                    if (alpha < 1f)
                    {
                        alpha += 0.02f;
                        Projectile.scale *= alpha;
                    }
                    Projectile.velocity.Y = -0.5f;
                    break;
                case Drip:
                    if (ArchaeaItem.Elapsed(5))
                    { 
                        Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<Merged.Dusts.c_silver_dust>(), 0f, Projectile.velocity.Y, Scale: Math.Abs(Projectile.velocity.Y - 6.12f) / 1.5f + 3f);
                    }
                    if (Main.LocalPlayer.Hitbox.Contains(Projectile.Center.ToPoint()))
                    {
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<Buffs.mercury>(), 60);
                    }
                    break;
                case Diffusion:
                    Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<Merged.Dusts.magno_dust>(), 0f, 0f, 0, default, 0.8f);
                    if (Main.LocalPlayer.Hitbox.Contains(Projectile.Center.ToPoint()))
                    {
                        if (ArchaeaItem.Elapsed(20))
                        {
                            Main.LocalPlayer.Hurt(PlayerDeathReason.ByProjectile(Main.LocalPlayer.whoAmI, Projectile.whoAmI), Projectile.damage, Projectile.Center.X < Main.LocalPlayer.Center.X ? 1 : -1);
                        }
                    }
                    break;
            }
        }
        public override void AI()
        {
            _AIType();
        }
        public override void OnKill(int timeLeft)
        {
            switch (type)
            {
                case Default:
                    break;
                case Sword:
                    NPCs.ArchaeaNPC.DustSpread(Projectile.Center, 1, 1, 6, 4, 2f);
                    if (Projectile.ai[0] == Mercury)
                        Projectile.NewProjectileDirect(Projectile.GetSource_Death(), new Vector2(owner.position.X, owner.position.Y - 600f), Vector2.Zero, ModContent.ProjectileType<Mercury>(), 20, 4f, owner.whoAmI, Projectiles.Mercury.Falling, Projectile.position.X);
                    break;
                case Drip:
                    int rand = Main.rand.Next(3, 6);
                    for (int i = 0; i < rand; i++)
                    {
                        float randX = 6 * Main.rand.NextFloat() * Main.rand.Next(new[] {-1, 1});
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, new Vector2(randX, -4f * Main.rand.NextFloat()), Projectile.type, 30, 2f, Main.myPlayer, 0f, Diffusion);
                        p.tileCollide = false;
                        p.ignoreWater = true;
                    }
                    //SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    break;
            }
        }
        public const int
            Fire = 1,
            Dark = 2,
            Mercury = 3,
            Electric = 4;
        private Dust defaultDust
        {
            get { return Dust.NewDustDirect(Vector2.Zero, 1, 1, 0); }
        }
        public Dust SetDust()
        {
            switch ((int)Projectile.ai[0])
            {
                case 0:
                    break;
                case Fire:
                    return Dust.NewDustDirect(Projectile.Center, 2, 2, 6, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
                case Mercury:
                    return Dust.NewDustDirect(Projectile.Center, 2, 2, 6, 0f, 0f, 0, default(Color), Main.rand.NextFloat(1f, 3f));
                case Electric:
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Electric, 0f, 0f, 0, default(Color), Projectile.localAI[1]);
                    dust.noGravity = true;
                    return dust;
            }
            return defaultDust;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return type != Drip && type != Diffusion && !factory;
        }
    }

}
