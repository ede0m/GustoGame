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
    public class TribalTokens : InventoryItem
    {
        public TribalTokens(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            Texture2D textureTribalTokens = content.Load<Texture2D>("TribalTokens");
            Texture2D textureTribalTokensBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTribalTokensBB = new Texture2D(graphics, textureTribalTokens.Width, textureTribalTokens.Height);
            Asset baseTribalTokensAsset = new Asset(textureTribalTokens, textureTribalTokensBB, 1, 1, 0.5f, "tribalTokens", region);
            SetSpriteAsset(baseTribalTokensAsset, location);
        }
    }
}
