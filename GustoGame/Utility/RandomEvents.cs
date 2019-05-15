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

        public static int RandomTilePiece(int nPieces, Random randomGeneration)
        {
            return randomGeneration.Next(0, nPieces);
        }

    }
}
