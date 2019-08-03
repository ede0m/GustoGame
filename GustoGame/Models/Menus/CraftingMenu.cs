﻿using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
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

        Dictionary<string, InventoryItem> IconTextures; 

        public CraftingMenu(Vector2 location, ContentManager content, GraphicsDevice graphics, PlayerPirate invOfPlayer) : base(graphics)
        {
            _graphics = graphics;
            _content = content;
            font = _content.Load<SpriteFont>("helperFont");
            cursor = _content.Load<Texture2D>("pointer");

            inventoryOfPlayer = invOfPlayer;

            itemDisplaySizePix = 60;

            slotLocations = new Dictionary<int, Rectangle>();
            itemMenuButtonLocations = new Dictionary<string, Rectangle>();
            saveItemSpriteScale = new Dictionary<InventoryItem, float>();

            Texture2D textureInventory = new Texture2D(graphics, 440, 400);
            Color[] data = new Color[440 * 400];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.DimGray;
            textureInventory.SetData(data);
            Asset invetoryAsset = new Asset(textureInventory, null, 1, 1, 1.0f, null, null);
            SetSpriteAsset(invetoryAsset, location);

            itemDrawLocStart = new Vector2(location.X - _texture.Width / 2 + 50, location.Y - _texture.Height / 2 + 100);

            // just used to display the crafting textures in the menu. Not used in game.
            IconTextures = new Dictionary<string, InventoryItem>
            {
                {"craftingAnvil", new AnvilItem(TeamType.Player, "GustoGame", Vector2.Zero, _content, _graphics) },
                {"baseSword", new BaseSword(TeamType.Player, "GustoGame", Vector2.Zero, _content, _graphics) },
            };
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


            List<InventoryItem> craftableItemsChecked = SearchCraftingRecipes(itemsPlayer);
            

            // TODO: When something is actually crafted - I will need to account for if it is placable or not (and create the placable object first)
            
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

            // draw items
            itemDrawLoc = itemDrawLocStart;
            for (int i = 0; i < craftableItemsChecked.Count; i++)
            {
                var item = craftableItemsChecked[i];
                Vector2 offsetLocation;
                Vector2 itemLoc = itemDrawLoc;
                // track sprite scale
                if (!saveItemSpriteScale.ContainsKey(item))
                    saveItemSpriteScale[item] = item.spriteScale;

                if (item is IHandHeld) // handhelds display action frames so scaling them will make them too tiny and offset
                {
                    item.spriteScale = 1.3f;
                    offsetLocation = new Vector2(itemLoc.X + textureHW / 3, itemLoc.Y + textureHW / 3);
                    item.currRowFrame = 0;
                }
                else
                {
                    item.spriteScale = (float)(itemDisplaySizePix / item.targetRectangle.Width);
                    offsetLocation = new Vector2(itemLoc.X + textureHW / 2, itemLoc.Y + textureHW / 1.7f); // move Y down a little to leave room for stack number display
                }

                item.location = offsetLocation;

                item.currColumnFrame = 0;
                item.currRowFrame = 0;
                item.Draw(sb, null);

                sb.Begin();
                //name display
                if (i == selectedIndex)
                    sb.DrawString(font, item.itemKey, itemStatLoc, Color.Black);
                sb.End();

                itemDrawLoc.X += 70;
                if (itemDrawLoc.X > location.X + GetWidth() / 2 - 50)
                {
                    itemDrawLoc.X = itemDrawLocStart.X;
                    itemDrawLoc.Y += 70;
                }
            }

            // draw cursor
            sb.Begin();
            sb.Draw(cursor, cursorPos, Color.White);
            sb.End();

            // reset itemDrawLoc
            itemDrawLoc = itemDrawLocStart;
        }


        // returns a list of craftabale items based on the invetory of the player
        private List<InventoryItem> SearchCraftingRecipes(List<InventoryItem> itemsPlayer)
        {
            List<InventoryItem> craftableItems = new List<InventoryItem>();

            // hash map the players available items
            Dictionary<string, int> playInvMap = new Dictionary<string, int>();
            foreach (var item in itemsPlayer)
            {
                if (item == null)
                    continue;

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
                    if (!(playInvMap.ContainsKey(ingredient.Key) && playInvMap[ingredient.Key] >= ingredient.Value))
                        break;
                    else
                    {
                        ingredientCount += 1;
                        if (ingredientCount >= craftingItem.Value.Count) // we have all the ingredients
                            craftableItems.Add(IconTextures[craftingItem.Key]);

                    }
                }
            }

            return craftableItems;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            if (menuOpen)
            {
                cursorPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                Rectangle cursorRect = new Rectangle((int)cursorPos.X, (int)cursorPos.Y, cursor.Width, cursor.Height);

                selectedIndex = -1;
                int i = 0;
                foreach (var slot in slotLocations.Values)
                {
                    if (slot.Intersects(cursorRect))
                    {
                        selectedIndex = i;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            // create item
                        }
                    }
                    i++;
                }


            }
            menuOpen = false;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

    }
}
