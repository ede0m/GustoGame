using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public class ItemUtility
    {

        // Models add to this global list when they drop items. Update order adds these to the UpdateOrder
        public static List<Sprite> ItemsToUpdate = new List<Sprite>();

        public static List<InventoryItem> CreateNPInventory(List<Tuple<string, int>> itemDrops, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            List<InventoryItem> returnItems = new List<InventoryItem>();
            List<string> trackStackable = new List<string>();
            int index = 0;
            foreach (var item in itemDrops)
            {
                if (trackStackable.Contains(item.Item1) && returnItems[trackStackable.IndexOf(item.Item1)].stackable)
                {
                    returnItems[trackStackable.IndexOf(item.Item1)].amountStacked += (int)item.Item2;
                    continue;
                }

                string key = item.Item1;
                int amountDropped = item.Item2;
                InventoryItem itm = CreateItem(key, team, region, location, content, graphics);
                if (itm != null)
                {
                    returnItems.Add(itm);
                    trackStackable.Add(key);
                }

                returnItems[index].inInventory = true;
                returnItems[index].amountStacked = amountDropped;
                index++;
            }
            return returnItems;
        }


        public static InventoryItem CreateItem(string key, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            InventoryItem item = null;
            switch (key)
            {
                case ("tribalTokens"):
                    item = new TribalTokens(team, region, location, content, graphics);
                    break;
                case ("basePlank"):
                    item = new BasePlank(team, region, location, content, graphics);
                    break;
                case ("shortSword"):
                    item = new ShortSword(team, region, location, content, graphics);
                    break;
                case ("softWood"):
                    item = new SoftWood(team, region, location, content, graphics);
                    break;
                case ("islandGrass"):
                    item = new IslandGrass(team, region, location, content, graphics);
                    break;
                case ("coal"):
                    item = new Coal(team, region, location, content, graphics);
                    break;
                case ("ironOre"):
                    item = new IronOre(team, region, location, content, graphics);
                    break;
                case ("baseSword"):
                    item = new BaseSword(team, region, location, content, graphics);
                    break;
                case ("anvilItem"):
                    item = new AnvilItem(team, region, location, content, graphics);
                    item.placeableVersion = new CraftingAnvil(team, region, location, content, graphics);
                    break;
            }
            item.itemKey = key;
            return item;
        }
    }
}
