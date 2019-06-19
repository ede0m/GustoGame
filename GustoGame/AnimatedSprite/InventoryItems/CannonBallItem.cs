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
    public class CannonBallItem : InventoryItem
    {
        public CannonBallItem(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "cannonBall";

            Texture2D textureCannonBallItem = content.Load<Texture2D>("CannonBall");
            Texture2D textureCannonBallItemBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureCannonBallItemBB = new Texture2D(graphics, textureCannonBallItem.Width, textureCannonBallItem.Height);
            Asset baseCannonBallItemAsset = new Asset(textureCannonBallItem, textureCannonBallItemBB, 2, 1, 1.0f, "cannonBallItem", region);
            SetSpriteAsset(baseCannonBallItemAsset, location);
            stackable = true;
        }
    }
}
