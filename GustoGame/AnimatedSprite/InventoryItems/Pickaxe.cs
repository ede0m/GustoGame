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

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class Pickaxe : HandHeld
    {
        public Pickaxe(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            timeSinceLastFrame = 0;
            millisecondsPerFrame = 100;
            damage = 0.1f;
            itemKey = "pickaxe";
            msCraftTime = 10000;

            Texture2D texture = content.Load<Texture2D>("pickaxe");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset baseSwordAsset = new Asset(texture, textureBB, 3, 4, 1.0f, itemKey, region);
            SetSpriteAsset(baseSwordAsset, location);
        }
    }
}
