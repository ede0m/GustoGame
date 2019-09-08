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
    public class RustyHarpoon : Ammo, IDirectionalAmmo
    {
        public RustyHarpoon(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(location, team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 2.0f;
            structureDamage = 3.0f;
            groundDamage = 7.0f;

            Texture2D texture = content.Load<Texture2D>("RustyHarpoon");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset basePistolShotAsset = new Asset(texture, textureBB, 2, 1, 1.0f, "rustyHarpoon", region);
            SetSpriteAsset(basePistolShotAsset, location);
        }
    }
}
