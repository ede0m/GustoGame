using Gusto.AnimatedSprite;
using Gusto.Mappings;
using Gusto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public static class RandomEvents
    {

        public static Random rand = new Random();

        public static float RandomSelectionRange(int n)
        {
            return rand.Next(-n, n);
        }

        // maxItemDrop is different than maxDrop key. maxItemDrop is number of attempts to get items. number of drops by inventory will  not exceed maxItemDrop.
        public static List<Tuple<string, int>> RandomNPDrops(string objKey, int maxItemDrop)
        {
            List<string> drops = new List<string>();
            List<int> dropAmounts = new List<int>();

            int itemSetCount = ItemMappings.ItemDrops[objKey].Count;
            List<string> itemSet = Enumerable.ToList(ItemMappings.ItemDrops[objKey].Keys);

            int i = 0;
            while (i < maxItemDrop && itemSetCount > 0)
            {
                string randomItemKey = itemSet[rand.Next(itemSetCount)];
                float percentWillDrop = ItemMappings.ItemDrops[objKey][randomItemKey]["percentDrop"];
                float maxDropAmount = ItemMappings.ItemDrops[objKey][randomItemKey]["maxDrop"];
                float dropP = rand.Next(0, 100);
                if (dropP <= percentWillDrop)
                {
                    float randomDropAmount = 1;
                    if (maxDropAmount > 1)
                    {
                        randomDropAmount = rand.Next(1, (int)maxDropAmount+1);
                    }

                    drops.Add(randomItemKey);
                    dropAmounts.Add((int)randomDropAmount);

                }
                i++;
            }
            List<Tuple<string, int>> returnDrops = new List<Tuple<string, int>>();
            for (int j = 0; j < drops.Count; j++)
                returnDrops.Add(new Tuple<string, int>(drops[j], dropAmounts[j]));
            return returnDrops;

        }

    }
}
