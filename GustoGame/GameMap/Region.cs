using Gusto.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap
{
    public class Region
    {
        string regionName;
        public List<Sprite> RegionLandTiles { get; set; }
        public List<Sprite> RegionOceanTiles { get; set; }
        public HashSet<Region> Neighbors { get; set; }
        public Rectangle Bounds { get; set; }

        public Region(string name)
        {
            regionName = name;
            RegionLandTiles = new List<Sprite>();
            RegionOceanTiles = new List<Sprite>();
            Neighbors = new HashSet<Region>();
        }

    }
}
