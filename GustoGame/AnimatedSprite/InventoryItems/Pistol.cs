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

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class Pistol : HandHeld, IRanged
    {
        public Pistol(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.0f;
            shotRange = 150;
            millisecondsNewShot = 2000;
            millisecondsExplosionLasts = 400;

            Texture2D texturePistol = content.Load<Texture2D>("pistol");
            Texture2D texturePistolBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                texturePistolBB = new Texture2D(graphics, texturePistol.Width, texturePistol.Height);
            Asset basePistolAsset = new Asset(texturePistol, texturePistolBB, 3, 4, 1.0f, "pistol", region);
            SetSpriteAsset(basePistolAsset, location);
        }
    }
}