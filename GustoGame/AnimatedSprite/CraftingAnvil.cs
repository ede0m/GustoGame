using Gusto.Models;
using Gusto.Models.Animated;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite
{
    public class CraftingAnvil : Anvil
    {
        public CraftingAnvil(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            Texture2D texture = content.Load<Texture2D>("Anvil");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 1, 1, 0.5f, "craftingAnvil", region);

            SetSpriteAsset(asset, location);
        }
    }
}
