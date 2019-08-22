using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Gusto.SaveState
{
    public class InventoryItemSerialized
    {
        public string itemKey { get; set; }
        public int stackedAmount { get; set; }
        public Dictionary<int, List<InventoryItemSerialized>> storageItems { get; set; } // key is index of storage container in the inventory, value is the inventory of the storage
        public Dictionary<int, List<TreasureMapSerialized>> treasureMaps { get; set; }
    }

    public class TreasureMapSerialized
    {
        public Vector2 digLocation { get; set; }
        public string region { get; set; }
        public List<InventoryItemSerialized> reward { get; set; }
    }


    [DataContract]
    [KnownType(typeof(InventoryItemSerialized))]
    [KnownType(typeof(TreasureMapSerialized))]
    [KnownType(typeof(Vector2))]
    [KnownType(typeof(ShipState))]
    public class ShipState : ISaveState
    {
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public List<InventoryItemSerialized> inventory { get; set; }
        [DataMember]
        public bool anchored { get; set; }
        [DataMember]
        public bool playerAboard { get; set; }
        [DataMember]
        public float health { get; set; }
    }

    // ... TODO: more states - weather, stuffInBoundingBoxLocations?(treasuremaps), etc
    


}
