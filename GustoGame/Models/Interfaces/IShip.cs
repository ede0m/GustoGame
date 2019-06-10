using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Interfaces
{
    public interface IShip
    {
        Tuple<float, float> SetSailBonusMovement(Dictionary<int, Tuple<float, float>> ShipDirectionVectorValues,
            int windDirection, int windSpeed, float sailSpeedBonus, int sailRColumn, int sailLColumn);

    }
}
