using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Utility;
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
    public class BaseBarrel : Barrel
    {
        public BaseBarrel(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, region, content, graphics)
        {
            nHitsToDestroy = 5;
            var objKey = "baseBarrel";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 4);
            drops = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D texture = content.Load<Texture2D>("Barrel");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 3, 2, 0.5f, objKey, region);

            SetSpriteAsset(asset, location);
        }
    }
}
