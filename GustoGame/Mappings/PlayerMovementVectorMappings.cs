using Gusto.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustoGame.Mappings
{
    class PlayerMovementVectorMappings
    {
        public static Dictionary<int, Tuple<float, float>> PlayerDirectionVectorValues = new Dictionary<int, Tuple<float, float>>()
        {
            {0, new Tuple<float, float>(0, -PhysicsUtility.basePlayerMovementSpeed)},
            {1, new Tuple<float, float>(-(PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg), -PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg) },
            {2, new Tuple<float, float>(-(PhysicsUtility.basePlayerMovementSpeed), 0) },
            {3, new Tuple<float, float>(-PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg, PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg) },
            {4, new Tuple<float, float>(0, (PhysicsUtility.basePlayerMovementSpeed))},
            {5, new Tuple<float, float>(PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg, PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg) },
            {6, new Tuple<float, float>(PhysicsUtility.basePlayerMovementSpeed, 0) },
            {7, new Tuple<float, float>(PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg, -PhysicsUtility.basePlayerMovementSpeed * PhysicsUtility.sin45deg) }
        };
    }
}
