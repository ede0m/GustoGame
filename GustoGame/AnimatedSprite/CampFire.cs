using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
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
    public class CampFire : Cooker
    {
        public CampFire(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            craftSet = "cookT1";

            Texture2D texture = content.Load<Texture2D>("CampFire");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 6, 1, 0.3f, "campFire", region);

            Light lanternLight = new Light(content, graphics, 1.0f, Color.OrangeRed);
            emittingLight = lanternLight;

            SetSpriteAsset(asset, location);
        }
    }
}
