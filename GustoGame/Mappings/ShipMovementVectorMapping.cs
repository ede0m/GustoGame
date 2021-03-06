﻿using Gusto.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Mappings
{
    public class ShipMovementVectorMapping
    {
        // map ship direction sprite frames (ROWS) to base movement values
        public static Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues = new Dictionary<int, Tuple<float, float>>()
        {
            {0, new Tuple<float, float>(0, -PhysicsUtility.baseShipMovementSpeed)},
            {1, new Tuple<float, float>(-(PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg), -PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {2, new Tuple<float, float>(-(PhysicsUtility.baseShipMovementSpeed), 0) },
            {3, new Tuple<float, float>(-PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg, PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {4, new Tuple<float, float>(0, (PhysicsUtility.baseShipMovementSpeed))},
            {5, new Tuple<float, float>(PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg, PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {6, new Tuple<float, float>(PhysicsUtility.baseShipMovementSpeed, 0) },
            {7, new Tuple<float, float>(PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg, -PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) }
        };

        public static Dictionary<int, Tuple<float, float>> InverseShipDirectionVectorValues = new Dictionary<int, Tuple<float, float>>()
        {
            {0, new Tuple<float, float>(0, (PhysicsUtility.baseShipMovementSpeed))},
            {1, new Tuple<float, float>((PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg), PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {2, new Tuple<float, float>((PhysicsUtility.baseShipMovementSpeed), 0) },
            {3, new Tuple<float, float>(PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg, -PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {4, new Tuple<float, float>(0, -(PhysicsUtility.baseShipMovementSpeed))},
            {5, new Tuple<float, float>(-(PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg), -PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) },
            {6, new Tuple<float, float>(-(PhysicsUtility.baseShipMovementSpeed), 0) },
            {7, new Tuple<float, float>(-PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg, PhysicsUtility.baseShipMovementSpeed * PhysicsUtility.sin45deg) }
        };
    }
}
