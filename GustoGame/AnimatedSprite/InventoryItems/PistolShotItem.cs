using Gusto.Models;
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
    public class PistolShotItem : InventoryItem
    {
        public PistolShotItem(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            Texture2D textureTribalTokens = content.Load<Texture2D>("PistolShot");
            Texture2D textureTribalTokensBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTribalTokensBB = new Texture2D(graphics, textureTribalTokens.Width, textureTribalTokens.Height);
            Asset baseTribalTokensAsset = new Asset(textureTribalTokens, textureTribalTokensBB, 2, 1, 1.0f, "pistolShotItem", region);
            SetSpriteAsset(baseTribalTokensAsset, location);
            stackable = true;
        }
    }
}
