using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class ChiliFish : InventoryItem, ISpoiles
    {
        public ChiliFish(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "chiliFish";

            msCraftTime = 10000;

            Texture2D texture = content.Load<Texture2D>("ChiliFish");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 1, 1, 0.5f, "chiliFish", region);
            SetSpriteAsset(asset, location);
            stackable = true;
        }
    }
}


