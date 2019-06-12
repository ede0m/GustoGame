using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Gusto.Models;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Gusto.Models.Animated;

namespace Gusto.AnimatedSprite
{
    public class BaseTower : Tower
    {
        public BaseTower(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            timeSinceLastShot = 0;
            timeSinceLastExpClean = 0;
            millisecondsNewShot = 2000;
            millisecondsExplosionLasts = 400;
            maxShotsMoving = 4;
            range = 600f;
            fullHealth = 100;
            health = fullHealth;

            Texture2D textureTower = content.Load<Texture2D>("tower");
            Texture2D textureTowerBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTowerBB = new Texture2D(graphics, textureTower.Width, textureTower.Height);
            Asset towerAsset = new Asset(textureTower, textureTowerBB, 1, 1, 0.5f, "tower", region);
            SetSpriteAsset(towerAsset, location);
        }

    }
}
