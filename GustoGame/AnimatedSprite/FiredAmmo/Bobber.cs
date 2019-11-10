using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.AnimatedSprite
{
    public class Bobber : Ammo, IDirectionalAmmo
    {

        public Bobber(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(location, team)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            baseMovementSpeed = 2.0f;
            structureDamage = 0.0f;
            groundDamage = 0.0f;
            arcShot = true;

            Texture2D texture = content.Load<Texture2D>("Bobber");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 3, 2, 1.0f, "bobber", region);
            SetSpriteAsset(asset, location);
        }
        
    }
}
