using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public interface IShip
    {
        void SetSailBonusMovement(Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues,
            int windDirection, int windSpeed, float sailSpeedBonus, int sailRColumn, int sailLColumn);

    }
}
