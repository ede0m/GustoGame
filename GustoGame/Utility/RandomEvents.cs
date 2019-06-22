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

        public static int RandomAimOffset(Random randomGeneration)
        {
            return randomGeneration.Next(-120, 120);
        }

        public static int RandomShotSpeed(Random randomGeneration)
        {
            return randomGeneration.Next(10, 25);
        }

        public static int RandomSelection(int nSelect, Random randomGeneration)
        {
            return randomGeneration.Next(0, nSelect);
        }

        public static List<Tuple<string, int>> RandomNPCDrops(TeamType team, Random rand, int maxItemDrop)
        {
            List<string> drops = new List<string>();
            List<int> dropAmounts = new List<int>();

            int itemSetCount = ItemDropMappings.ItemDrops[team].Count;
            List<string> itemSet = Enumerable.ToList(ItemDropMappings.ItemDrops[team].Keys);

            int i = 0;
            while (i < maxItemDrop)
            {
                string randomItemKey = itemSet[rand.Next(itemSetCount)];
                float percentWillDrop = ItemDropMappings.ItemDrops[team][randomItemKey]["percentDrop"];
                float maxDropAmount = ItemDropMappings.ItemDrops[team][randomItemKey]["maxDrop"];
                float dropP = rand.Next(0, 100);
                if (dropP <= percentWillDrop)
                {
                    float randomDropAmount = 1;
                    if (maxDropAmount > 1)
                    {
                        randomDropAmount = rand.Next(1, (int)maxDropAmount);
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
