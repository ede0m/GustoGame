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
    public class CrossBow : HandHeld, IRanged
    {
        public CrossBow(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.0f;
            shotRange = 100;
            millisecondsNewShot = 1000;
            millisecondsExplosionLasts = 400;
            itemKey = "crossBow";
            msCraftTime = 20000;
            ammoTypeKey = "arrow";
            ammoItemType = typeof(Gusto.AnimatedSprite.InventoryItems.ArrowItem);

            Texture2D texture = content.Load<Texture2D>("CrossBow");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset basePistolAsset = new Asset(texture, textureBB, 3, 4, 1.0f, "crossBow", region);
            SetSpriteAsset(basePistolAsset, location);

            stackable = false;
        }
    }
}