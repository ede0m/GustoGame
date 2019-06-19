using Gusto.Models;
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
    public class PistolShotItem : InventoryItem
    {
        public PistolShotItem(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "pistolShot";

            Texture2D texturePistolShotItem = content.Load<Texture2D>("PistolShot");
            Texture2D texturePistolShotItemBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                texturePistolShotItemBB = new Texture2D(graphics, texturePistolShotItem.Width, texturePistolShotItem.Height);
            Asset basePistolShotItemAsset = new Asset(texturePistolShotItem, texturePistolShotItemBB, 2, 1, 1.0f, "pistolShotItem", region);
            SetSpriteAsset(basePistolShotItemAsset, location);
            stackable = true;
        }
    }
}
