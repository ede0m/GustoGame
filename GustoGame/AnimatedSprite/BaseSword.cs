using Gusto.Models;
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
    public class BaseSword : Sword
    {
        public BaseSword(TeamType team, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;

            Texture2D textureBaseSword = content.Load<Texture2D>("BaseSword");
            Texture2D textureBaseSwordBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBaseSwordBB = new Texture2D(graphics, textureBaseSword.Width, textureBaseSword.Height);
            Asset baseSwordAsset = new Asset(textureBaseSword, textureBaseSwordBB, 3, 4, 1.0f, "baseSword");
            SetSpriteAsset(baseSwordAsset, location);
        }
    }
}
