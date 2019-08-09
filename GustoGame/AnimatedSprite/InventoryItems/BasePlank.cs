using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Gusto.AnimatedSprite.InventoryItems
{
    public class BasePlank : InventoryItem, IPlank
    {
        public BasePlank(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            restorePoints = 5;

            itemKey = "basePlank";

            Texture2D textureBasePlank = content.Load<Texture2D>("plank");
            Texture2D textureBasePlankBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBasePlankBB = new Texture2D(graphics, textureBasePlank.Width, textureBasePlank.Height);
            Asset basePlankAsset = new Asset(textureBasePlank, textureBasePlankBB, 1, 1, 0.5f, itemKey, region);
            SetSpriteAsset(basePlankAsset, location);
            stackable = true;
        }
    }
}
