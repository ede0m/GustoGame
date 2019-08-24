using Comora;
using Gusto.AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class TreasureMap : InventoryItem
    {
        public string treasureInRegion;
        public Vector2 digTileLoc;
        public string storageTierType;
        public Storage rewarded;
        public bool solved;

        GraphicsDevice _graphics;
        ContentManager _content;

        public TreasureMap(TeamType type, ContentManager content, GraphicsDevice graphics) : base(type, content, graphics)
        {
            _graphics = graphics;
            _content = content;
            teamType = type;
            amountStacked = 1;
        }
    }
}
