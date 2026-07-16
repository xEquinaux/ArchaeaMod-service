using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ArchaeaMod.Merged;
namespace ArchaeaMod.Items.Tiles
{
    public class m_chandelier : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //tooltips.Add(new TooltipLine(Mod, "ItemName", "Magnoliac Chandelier"));
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = 1;
            Item.value = 1000;
            Item.rare = 1;
            Item.maxStack = 99;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Merged.Items.Materials.magno_bar>(), 3)
                .AddIngredient(ModContent.ItemType<Merged.Items.Tiles.magno_brick>(), 4)
                .AddIngredient((int)TileID.Torches, 4)
                .AddIngredient((int)TileID.Chain, 1)
                .AddTile(TileID.Anvils)
//            recipe.SetResult(Item.type);
                .Register();
        }
    }
}
