using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Types
{
    public enum ActionState
    {
        Attack,
        PassiveRoam,
        DefenseRoam,
        Follow,
        IdleDefense,
        IdleFlee,
    }
}
