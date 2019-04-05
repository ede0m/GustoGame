using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Gusto.Models;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Gusto.Bounding;

namespace Gusto.AnimatedSprite
{
    public class BaseTower : Tower
    {
        public BaseTower(TeamType team, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            timeSinceLastShot = 0;
            timeSinceLastExpClean = 0;
            millisecondsNewShot = 2000;
            millisecondsExplosionLasts = 400;
            maxShotsMoving = 5;
            range = 600f;

            Texture2D textureTower = content.Load<Texture2D>("tower");
            Texture2D textureTowerBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTowerBB = new Texture2D(graphics, textureTower.Width, textureTower.Height);
            Asset towerAsset = new Asset(textureTower, textureTowerBB, 1, 1, 0.5f, "tower");
            SetSpriteAsset(towerAsset, location);
        }

    }
}
