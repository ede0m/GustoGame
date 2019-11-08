using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class FishingPole : HandHeld, IRanged
    {
        public FishingPole(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.0f;
            shotRange = 150;
            millisecondsNewShot = 1000;
            millisecondsExplosionLasts = 400;
            itemKey = "fishingPole";
            msCraftTime = 20000;

            ammoTypeKey = "bobber";
            //ammoItemType = typeof(Gusto.AnimatedSprite.InventoryItems.PistolShotItem);

            Texture2D texturePistol = content.Load<Texture2D>("FishingPole");
            Texture2D texturePistolBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                texturePistolBB = new Texture2D(graphics, texturePistol.Width, texturePistol.Height);
            Asset basePistolAsset = new Asset(texturePistol, texturePistolBB, 3, 4, 1.0f, "fishingPole", region);
            SetSpriteAsset(basePistolAsset, location);

            stackable = false;
        }
    }
}