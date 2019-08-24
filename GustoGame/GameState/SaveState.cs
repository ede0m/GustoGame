using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Gusto.AnimatedSprite;
using Gusto.GameMap;

namespace Gusto.SaveState
{
    public class InventoryItemSerialized
    {
        public string itemKey { get; set; }
        public int stackedAmount { get; set; }
        public Dictionary<int, List<InventoryItemSerialized>> storageItems { get; set; } // key is index of storage container in the inventory, value is the inventory of the storage
        public Dictionary<int, TreasureMapItemSerialized> treasureMaps { get; set; }
    }

    public class TreasureMapItemSerialized
    {
        public Vector2 digLocation { get; set; }
        public string treasureInRegion { get; set; }
        public string storageType { get; set; }
        public List<InventoryItemSerialized> reward { get; set; }
    }


    [DataContract]
    [KnownType(typeof(InventoryItemSerialized))]
    [KnownType(typeof(TreasureMapItemSerialized))]
    [KnownType(typeof(Vector2))]
    [KnownType(typeof(TeamType))]
    [KnownType(typeof(ShipState))]
    [KnownType(typeof(PlayerState))]
    public class PlayerState : ISaveState
    {
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public TeamType team { get; set; }
        [DataMember]
        public List<InventoryItemSerialized> inventory { get; set; }
        [DataMember]
        public bool onShip { get; set; }
        [DataMember]
        public Guid playerOnShipId { get; set; }
        [DataMember]
        public string inHandItemKey { get; set; }
        [DataMember]
        public float health { get; set; }
    }

    [DataContract]
    [KnownType(typeof(InventoryItemSerialized))]
    [KnownType(typeof(TreasureMapItemSerialized))]
    [KnownType(typeof(Vector2))]
    [KnownType(typeof(TeamType))]
    [KnownType(typeof(NpcState))]
    public class NpcState : ISaveState
    {
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public TeamType team { get; set; }
        [DataMember]
        public List<InventoryItemSerialized> inventory { get; set; }
        [DataMember]
        public bool onShip { get; set; }
        [DataMember]
        public Guid playerOnShipId { get; set; }
        [DataMember]
        public float health { get; set; }
    }


    [DataContract]
    [KnownType(typeof(InventoryItemSerialized))]
    [KnownType(typeof(TreasureMapItemSerialized))]
    [KnownType(typeof(Vector2))]
    [KnownType(typeof(TeamType))]
    [KnownType(typeof(ShipState))]
    public class ShipState : ISaveState
    {
        [DataMember]
        public Guid shipId { get; set; } // needed for player.playerOnShip
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public TeamType team { get; set; }
        [DataMember]
        public List<InventoryItemSerialized> inventory { get; set; }
        [DataMember]
        public bool anchored { get; set; }
        [DataMember]
        public bool playerAboard { get; set; }
        [DataMember]
        public float health { get; set; }
    }

    [DataContract]
    [KnownType(typeof(OnGroundState))]
    public class OnGroundState : ISaveState
    {
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public TeamType team { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public bool inventoryItem { get; set; }
        [DataMember]
        public int amountStacked { get; set; }
        [DataMember]
        public List<InventoryItemSerialized> inventory { get; set; }
    }

    [DataContract]
    [KnownType(typeof(RainState))]
    [KnownType(typeof(WeatherSaveState))]
    public class WeatherSaveState : ISaveState
    {
        [DataMember]
        public float currentMsOfDay;
        [DataMember]
        public float currentLightIntensity { get; set; }
        [DataMember]
        public float sunAngleX { get; set; }
        [DataMember]
        public float shadowTransparency { get; set; }
        [DataMember]
        public int nDays;
        [DataMember]
        public float weatherDuration { get; set; }
        [DataMember]
        public float msThroughWeather { get; set; }
        [DataMember]
        public RainState rainState { get; set; }
        [DataMember]
        public int rainIntensity { get; set; }
        [DataMember]
        public bool lightning { get; set; }
    }

    // ... TODO: more states - weather, stuffInBoundingBoxLocations?(treasuremaps), etc
}
