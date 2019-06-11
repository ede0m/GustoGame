using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Utility
{
    public class ItemUtility
    {

        public static List<Sprite> ItemsToUpdate = new List<Sprite>();


        public static InventoryItem CreateInventoryItem(string key, int amountStacked, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            switch (key)
            {
                case ("tribalTokens"):
                    return new TribalTokens(team, region, location, content, graphics);
                    break;
                case ("shortSword"):
                    return new ShortSword(team, region, location, content, graphics);
                    break;
            }
            return null;
        }

    }
}
