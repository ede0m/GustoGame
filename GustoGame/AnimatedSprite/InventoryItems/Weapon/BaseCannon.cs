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
    public class BaseCannon : ShipMount
    {
        public BaseCannon(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.0f;
            shotRange = 100;
            millisecondsNewShot = 1000;
            millisecondsExplosionLasts = 400;
            itemKey = "baseCannon";
            msCraftTime = 40000;
            shotRange = 600;
            //ammoTypeKey = "baseCannonBall";

            ammoItemType = typeof(Gusto.AnimatedSprite.InventoryItems.CannonBallItem);

            Texture2D texture = content.Load<Texture2D>("BaseCannon");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 4, 1, 1.0f, "baseCannon", region);
            SetSpriteAsset(asset, location);

            stackable = false;
        }
    }
}