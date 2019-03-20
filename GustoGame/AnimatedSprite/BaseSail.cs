using Gusto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite
{
    public class BaseSail : Sail
    {
        public BaseSail()
        {
            sailIsLeftColumn = 2;
            sailIsRightColumn = 0;
            sailSpeed = 1.5f;
            windWindowAdd = 1;
            windWindowSub = 1;
        }
    }
}
