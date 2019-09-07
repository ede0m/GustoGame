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
    public class PistolShot : Ammo
    {
        public PistolShot(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(location, team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 3.0f;
            structureDamage = 2.5f;
            groundDamage = 9.0f;

            Texture2D texturePistolShot = content.Load<Texture2D>("PistolShot");
            Texture2D texturePistolShotBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                texturePistolShotBB = new Texture2D(graphics, texturePistolShot.Width, texturePistolShot.Height);
            Asset basePistolShotAsset = new Asset(texturePistolShot, texturePistolShotBB, 2, 1, 1.0f, "pistolShot", region);
            SetSpriteAsset(basePistolShotAsset, location);
        }
    }
}
