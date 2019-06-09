﻿using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Bounding
{
    public class BoundingBoxLocations
    {
        public static Dictionary<TeamType, List<Tuple<int,int>>> BoundingBoxLocationMap = new Dictionary<TeamType, List<Tuple<int, int>>>()
        {
            {TeamType.Player, new List<Tuple<int,int>>()},
            {TeamType.A, new List<Tuple<int, int>>() },
            {TeamType.B, new List<Tuple<int, int>>() }
        };

        public static Dictionary<string, List<Sprite>> RegionMap = new Dictionary<string, List<Sprite>>();

        public static HashSet<Sprite> LandTileLocationList = new HashSet<Sprite>();
    }
}
