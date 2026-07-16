
using MonoMod.RuntimeDetour;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

using static System.Formats.Asn1.AsnWriter;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ArchaeaMod.Jobs.Items
{
    internal class Scroll_firestorm : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scroll of Firestorm");
            /* Tooltip.SetDefault("Blast from the above.\n" +
                "One use."); */
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.maxStack = 10;
            Item.consumable = true;
            Item.autoReuse = false;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.scale = 1;
            Item.value = 0;
            Item.rare = 3;
        }
        public override bool? UseItem(Player player)
        {     
            var modPlayer = player.GetModPlayer<ArchaeaPlayer>();
            if (!modPlayer.fireStorm)
            { 
                modPlayer.fireStorm = true;
                SoundEngine.PlaySound(SoundID.Item8, player.Center);
                if (Main.netMode == 1)
                {
                    NetHandler.Send(Packet.CastFireStorm, i: player.whoAmI, b: true);
                }
                return true;
            }
            return false;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.Fireblossom, 7)
                .AddIngredient(ItemID.Meteorite)
                .AddTile((int)ItemID.CrystalBall)
                .Register();
        }
    }
}