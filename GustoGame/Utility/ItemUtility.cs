﻿using Gusto.AnimatedSprite;
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


        public static List<InventoryItem> CreateNPCInventory(List<Tuple<string, int>> itemDrops, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            List<InventoryItem> returnItems = new List<InventoryItem>();
            List<string> trackStackable = new List<string>();
            int index = 0;
            foreach (var item in itemDrops)
            {
                if (trackStackable.Contains(item.Item1))
                {
                    returnItems[trackStackable.IndexOf(item.Item1)].amountStacked += (int)item.Item2;
                    continue;
                }

                string key = item.Item1;
                int amountDropped = item.Item2;
                switch (key)
                {
                    case ("tribalTokens"):
                        TribalTokens tt = new TribalTokens(team, region, location, content, graphics);
                        tt.itemKey = "tribalTokens";
                        returnItems.Add(tt);
                        trackStackable.Add("tribalTokens");
                        break;
                    case ("shortSword"):
                        ShortSword ss = new ShortSword(team, region, location, content, graphics);
                        ss.itemKey = "shortSword";
                        returnItems.Add(ss);
                        trackStackable.Add("shortSword");
                        break;
                }
                returnItems[index].inInventory = true;
                returnItems[index].amountStacked = amountDropped;
                index++;
            }
            return returnItems;
        }

    }
}
