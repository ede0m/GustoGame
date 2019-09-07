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
                InventoryItem itm = CreateInventoryItem(key, team, region, location, content, graphics);
                if (itm != null)
                {
                    returnItems.Add(itm);
                    trackStackable.Add(key);
                }

                returnItems[index].inInventory = true;
                returnItems[index].amountStacked = amountDropped; // override CreateItem default amount created with random drop amount
                index++;
            }
            return returnItems;
        }

        public static List<Sprite> CreateInteriorItems(List<Tuple<string, int>> itemDrops, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            List<Sprite> returnItems = new List<Sprite>();
            int index = 0;
            foreach (var item in itemDrops)
            {
                string key = item.Item1;
                int amountDropped = item.Item2;
                Sprite itm = CreateItem(key, team, region, location, content, graphics);
                if (itm != null)
                {
                    returnItems.Add(itm);
                }
                index++;
            }
            return returnItems;
        }


        public static InventoryItem CreateInventoryItem(string key, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            InventoryItem item = null;
            int amountStacked = 1;
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
                case ("baseChestItem"):
                    item = new BaseChestItem(team, region, location, content, graphics);
                    item.placeableVersion = new BaseChest(team, region, location, content, graphics);
                    break;
                case ("nails"):
                    item = new Nails(team, region, location, content, graphics);
                    amountStacked = 5; 
                    break;
                case ("cannonBallItem"):
                    item = new CannonBallItem(team, region, location, content, graphics);
                    amountStacked = 3;
                    break;
                case ("pistolShotItem"):
                    item = new PistolShotItem(team, region, location, content, graphics);
                    amountStacked = 5;
                    break;
                case ("ironBar"):
                    item = new IronBar(team, region, location, content, graphics);
                    break;
                case ("treasureMapItem"):
                    item = new TreasureMapItem(null, team, region, location, content, graphics);
                    break;
                case ("chiliFish"):
                    item = new ChiliFish(team, region, location, content, graphics);
                    break;
                case ("chiliPepper"):
                    item = new ChiliPepper(team, region, location, content, graphics);
                    break;
                case ("cookedFish"):
                    item = new CookedFish(team, region, location, content, graphics);
                    break;
                case ("cookedMeat"):
                    item = new CookedMeat(team, region, location, content, graphics);
                    break;
                case ("rawFish"):
                    item = new RawFish(team, region, location, content, graphics);
                    break;
                case ("rawMeat"):
                    item = new RawMeat(team, region, location, content, graphics);
                    break;
                case ("spoiledFish"):
                    item = new SpoiledFish(team, region, location, content, graphics);
                    break;
                case ("spoiledMeat"):
                    item = new SpoiledMeat(team, region, location, content, graphics);
                    break;
                case ("feather"):
                    item = new Feather(team, region, location, content, graphics);
                    break;
                case ("scales"):
                    item = new Scales(team, region, location, content, graphics);
                    break;
                case ("fishOil"):
                    item = new FishOil(team, region, location, content, graphics);
                    break;
                case ("goldCoins"):
                    item = new GoldCoins(team, region, location, content, graphics);
                    break;
            }
            item.itemKey = key;
            item.amountStacked = amountStacked;
            return item;
        }

        public static Sprite CreateItem(string key, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            Sprite item = null;
            switch (key)
            {
                case ("baseBarrel"):
                    item = new BaseBarrel(team, region, location, content, graphics);
                    break;
                case ("baseChest"):
                    item = new BaseChest(team, region, location, content, graphics);
                    break;
            }
            return item;
        }
    }
}
