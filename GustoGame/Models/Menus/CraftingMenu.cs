using Comora;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Menus
{
    public class CraftingMenu : Sprite, ICanUpdate, IMenuGUI
    {
        bool menuOpen;
        int maxInventorySlots;
        int selectedIndex;
        string itemMenuFunc;
        float timeRClicked;
        float timeLClicked;
        Dictionary<int, Rectangle> slotLocations;
        Dictionary<string, Rectangle> itemMenuButtonLocations;
        Dictionary<InventoryItem, float> saveItemSpriteScale;

        float itemDisplaySizePix;
        Vector2 itemDrawLocStart;
        Vector2 cursorPos;
        Vector2 itemMenuPos;
        Texture2D cursor;
        SpriteFont font;

        PlayerPirate inventoryOfPlayer;

        GraphicsDevice _graphics;
        ContentManager _content;

        public CraftingMenu(Vector2 location, ContentManager content, GraphicsDevice graphics, PlayerPirate invOfPlayer) : base(graphics)
        {
            _graphics = graphics;
            _content = content;
            font = _content.Load<SpriteFont>("helperFont");
            cursor = _content.Load<Texture2D>("pointer");

            inventoryOfPlayer = invOfPlayer;

            itemDisplaySizePix = 40;
            maxInventorySlots = inventoryOfPlayer.maxInventorySlots;

            itemMenuButtonLocations = new Dictionary<string, Rectangle>();
            saveItemSpriteScale = new Dictionary<InventoryItem, float>();

            Texture2D textureInventory = new Texture2D(graphics, 440, 400);
            Color[] data = new Color[440 * 400];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.DimGray;
            textureInventory.SetData(data);
            Asset invetoryAsset = new Asset(textureInventory, null, 1, 1, 1.0f, null, null);
            SetSpriteAsset(invetoryAsset, location);

            itemDrawLocStart = new Vector2(location.X - _texture.Width / 2 + 50, location.Y - _texture.Height / 2 + 100);
        }

        public void DrawInventory(SpriteBatch sb, List<InventoryItem> itemsPlayer)
        {
            Vector2 itemDrawLoc = itemDrawLocStart;
            menuOpen = true;

            List<InventoryItem> items = itemsPlayer;
            int showSlots = maxInventorySlots;

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            if (menuOpen)
            {

            }
            menuOpen = false;
        }
    }
}
