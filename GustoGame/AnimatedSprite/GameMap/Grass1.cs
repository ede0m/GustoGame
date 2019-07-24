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

namespace Gusto.AnimatedSprite.GameMap
{
    public class Grass1 : Grass
    {
        public Grass1(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            Texture2D textureGrass1 = content.Load<Texture2D>("Grass1");
            Texture2D textureGrass1BB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureGrass1BB = new Texture2D(graphics, textureGrass1.Width, textureGrass1.Height);
            Asset Grass1Asset = new Asset(textureGrass1, textureGrass1BB, 2, 1, 1.5f, "grass1", region);
            SetSpriteAsset(Grass1Asset, location);
        }
    }
}
