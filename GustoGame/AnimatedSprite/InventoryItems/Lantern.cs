using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class Lantern : HandHeld, ILight
    {
        public Lantern(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.35f;
            itemKey = "lantern";

            Texture2D textureLantern = content.Load<Texture2D>("Lantern");
            Texture2D textureLanternBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureLanternBB = new Texture2D(graphics, textureLantern.Width, textureLantern.Height);
            Asset baseLantern = new Asset(textureLantern, textureLanternBB, 3, 4, 1.0f, "lantern", region);

            // todo: random size?? Color?
            Light lanternLight = new Light(content, graphics, 1.0f, Color.LightGoldenrodYellow);
            emittingLight = lanternLight;
            
            SetSpriteAsset(baseLantern, location);
        }
    }
}
