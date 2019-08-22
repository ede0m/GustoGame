using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.SaveState
{

    public class ShipState : ISaveState
    {
        public Vector2 location { get; set; }
        public string region { get; set; }
        public string objKey { get; set; }
        public List<InventoryItem> inventory { get; set; }
        public bool roaming { get; set; }
        public bool anchored { get; set; }
        public bool playerAboard { get; set; }
        public float health { get; set; }
    }

    // ... TODO: more states
}
