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
    public class SoftWood : InventoryItem
    {
        public SoftWood(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "softWood";

            Texture2D textureSoftWood = content.Load<Texture2D>("TribalTokens");
            Texture2D textureSoftWoodBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureSoftWoodBB = new Texture2D(graphics, textureSoftWood.Width, textureSoftWood.Height);
            Asset baseTribalTokensAsset = new Asset(textureSoftWood, textureSoftWoodBB, 1, 1, 0.5f, "softWood", region);
            SetSpriteAsset(baseTribalTokensAsset, location);
            stackable = true;
        }
    }
}

