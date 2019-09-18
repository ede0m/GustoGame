using Gusto.AnimatedSprite;
using Gusto.GameMap;
using Gusto.Models;
using Gusto.Models.Animated;
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
        public static Dictionary<TeamType, List<Target>> BoundingBoxLocationMap = new Dictionary<TeamType, List<Target>>()
        {
            {TeamType.Player, new List<Target>()},
            {TeamType.A, new List<Target>() },
            {TeamType.B, new List<Target>() },
            {TeamType.Animal, new List<Target>() },

        };

        public static Dictionary<string, Region> RegionMap = new Dictionary<string, Region>();
        public static Dictionary<Guid, Interior> interiorMap = new Dictionary<Guid, Interior>(); // map of all the interiors in the game

        public static List<TilePiece> TilesInView = new List<TilePiece>();
        public static HashSet<Sprite> LandTileLocationList = new HashSet<Sprite>(); // TODO: update to TilePiece
        public static HashSet<TilePiece> InteriorTileList = new HashSet<TilePiece>(); // Tiles for the interior that the player is in
        public static HashSet<Sprite> GroundObjectLocationList = new HashSet<Sprite>(); // World ground objects
        public static HashSet<Light> WorldLightLocationList = new HashSet<Light>(); // world light list
        public static List<TreasureMap> treasureLocationsList = new List<TreasureMap>();
    }
}
