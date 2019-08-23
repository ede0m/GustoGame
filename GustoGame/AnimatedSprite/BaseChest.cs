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

namespace Gusto.AnimatedSprite
{
    public class BaseChest : Storage
    {
        public BaseChest(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, region, content, graphics)
        {
            nInventorySlots = 5;
            var objKey = "baseChest";
            storageTierType = "baseChestItem"; // needed for deserialization of burying your own treasure

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 4);
            List<InventoryItem> items = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);
            inventory = Enumerable.Repeat<InventoryItem>(null, nInventorySlots).ToList();

            Texture2D texture = content.Load<Texture2D>("BaseChest");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 3, 2, 0.5f, objKey, region);

            SetSpriteAsset(asset, location);

            foreach (var i in items)
            {
                if (AddInventoryItem(i))
                    i.inInventory = true;
            }

        }
    }
}
