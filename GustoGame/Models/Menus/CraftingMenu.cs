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

            // draw item stat panel
            Texture2D textureItemStat = new Texture2D(_graphics, 140, 50);
            Color[] csdata = new Color[50 * 140];
            for (int j = 0; j < csdata.Length; ++j) csdata[j] = Color.Gray;
            textureItemStat.SetData(csdata);
            Vector2 itemStatLoc = new Vector2(itemDrawLoc.X, itemDrawLoc.Y - 80);
            itemMenuButtonLocations["itemStat"] = new Rectangle((int)itemStatLoc.X, (int)itemStatLoc.Y, textureItemStat.Width, textureItemStat.Height);
            sb.Begin();
            sb.Draw(textureItemStat, itemStatLoc, Color.Gray);
            sb.End();


            List<string> craftableItemsChecked = SearchCraftingRecipes(itemsPlayer);
            
            
            int textureHW = 64;
            // draw slots
            for (int i = 0; i < craftableItemsChecked.Count; i++)
            {

                // draw slots
                Texture2D textureSlot = new Texture2D(_graphics, textureHW, textureHW);
                Color[] data = new Color[64 * 64];
                Color color = Color.DarkGray;
                if (i == selectedIndex)
                    color = Color.Crimson;
                for (int j = 0; j < data.Length; ++j) data[j] = color;
                textureSlot.SetData(data);

                Rectangle slotLoc = new Rectangle((int)itemDrawLoc.X, (int)itemDrawLoc.Y, textureHW, textureHW);
                slotLocations[i] = slotLoc;
                sb.Begin();
                sb.Draw(textureSlot, itemDrawLoc, Color.DarkGray);
                sb.End();

                itemDrawLoc.X += 70;
                if (itemDrawLoc.X > location.X + GetWidth() / 2 - 50)
                {
                    itemDrawLoc.X = itemDrawLocStart.X;
                    itemDrawLoc.Y += 70;
                }

            }

        }

        // returns a list of craftabale items based on the invetory of the player
        private List<string> SearchCraftingRecipes(List<InventoryItem> itemsPlayer)
        {
            List<string> craftableItems = new List<string>();

            // hash map the players available items
            Dictionary<string, int> playInvMap = new Dictionary<string, int>();
            foreach (var item in itemsPlayer)
            {
                if (playInvMap.ContainsKey(item.bbKey))
                    playInvMap[item.bbKey] += item.amountStacked;
                else
                    playInvMap.Add(item.bbKey, item.amountStacked);
            }
            // now check our available items against the crafting recipes 
            foreach (KeyValuePair<string, Dictionary<string, int>> craftingItem in Mappings.ItemMappings.CraftingRecipes)
            {
                int ingredientCount = 0;
                foreach (KeyValuePair<string, int> ingredient in craftingItem.Value)
                {
                    // if we have the item and enough of the item
                    if (!(playInvMap.ContainsKey(ingredient.Key) && playInvMap[ingredient.Key] > ingredient.Value))
                        break;
                    else
                    {
                        ingredientCount += 1;
                        if (ingredientCount >= craftingItem.Value.Count) // we have all the ingredients
                            craftableItems.Add(craftingItem.Key);

                    }
                }
            }

            return craftableItems;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            if (menuOpen)
            {

            }
            menuOpen = false;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }
    }
}
