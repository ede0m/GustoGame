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
    public class Arrow : Ammo, IDirectionalAmmo
    {
        public Arrow(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(location, team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 2.0f;
            structureDamage = 1.0f;
            groundDamage = 6.0f;

            Texture2D texture = content.Load<Texture2D>("Arrow");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset basePistolShotAsset = new Asset(texture, textureBB, 2, 1, 1.0f, "arrow", region);
            SetSpriteAsset(basePistolShotAsset, location);
        }
    }
}
