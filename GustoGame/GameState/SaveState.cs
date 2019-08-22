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
    [KnownType(typeof(Vector2))]
    [DataContract]
    public class ShipState : ISaveState
    {
        [DataMember]
        public string region { get; set; }
        [DataMember]
        public string objKey { get; set; }
        [DataMember]
        public Vector2 location { get; set; }
        [DataMember]
        public List<string> inventoryKeys { get; set; }
        [DataMember]
        public List<string> inventoryCounts { get; set; }
        [DataMember]
        public bool anchored { get; set; }
        [DataMember]
        public bool playerAboard { get; set; }
        [DataMember]
        public float health { get; set; }
    }

    // ... TODO: more states - weather, stuffInBoundingBoxLocations?(treasuremaps), etc
    


}
