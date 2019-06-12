using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models.Menus
{
    public class Inventory : Sprite, ICanUpdate, IMenuGUI
    {
        Vector2 itemDrawLoc;

        public Inventory(Vector2 location, ContentManager content, GraphicsDevice graphics)
        {
            Texture2D textureInventory = new Texture2D(graphics, 800, 600);
            Color[] data = new Color[600 * 800];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.DimGray;
            textureInventory.SetData(data);
            Asset invetoryAsset = new Asset(textureInventory, null, 1, 1, 1.0f, null, null);
            SetSpriteAsset(invetoryAsset, location);

            itemDrawLoc = new Vector2(location.X - _texture.Width/2 + 50, location.Y - _texture.Height/2 + 50);
        }

        public void DrawInventory(List<InventoryItem> items, SpriteBatch sb)
        { 
            // TODO: scale each item to the same size
            // TODO: show cursor, slots, draggable items

            foreach (var item in items)
            {
                item.location = itemDrawLoc;
                item.Draw(sb, null);
                itemDrawLoc.X += 50;
            }

            // reset itemDrawLoc
            itemDrawLoc = new Vector2(location.X - _texture.Width / 2 + 50, location.Y - _texture.Height / 2 + 50);
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            
        }
    }
}
