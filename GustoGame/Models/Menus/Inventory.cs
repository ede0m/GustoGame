using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models.Menus
{
    public class Inventory : Sprite, ICanUpdate, IMenuGUI
    {
        bool menuOpen;
        int maxInventorySlots;
        int selectedIndex;
        bool openItemMenu;
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
        Random rand;

        public Inventory(Vector2 location, ContentManager content, GraphicsDevice graphics, PlayerPirate invOfPlayer)
        {

            _graphics = graphics;
            _content = content;
            rand = new Random();
            font = _content.Load<SpriteFont>("helperFont");
            cursor = _content.Load<Texture2D>("pointer");

            inventoryOfPlayer = invOfPlayer;

            itemDisplaySizePix = 40;
            maxInventorySlots = 12;
            selectedIndex = 0;
            Texture2D textureInventory = new Texture2D(graphics, 500, 275);
            slotLocations = new Dictionary<int, Rectangle>();
            itemMenuButtonLocations = new Dictionary<string, Rectangle>();
            saveItemSpriteScale = new Dictionary<InventoryItem, float>();

            Color[] data = new Color[500 * 275];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.DimGray;
            textureInventory.SetData(data);
            Asset invetoryAsset = new Asset(textureInventory, null, 1, 1, 1.0f, null, null);
            SetSpriteAsset(invetoryAsset, location);

            itemDrawLocStart = new Vector2(location.X - _texture.Width/2 + 50, location.Y - _texture.Height/2 + 100);
        }

        public void DrawInventory(List<InventoryItem> items, SpriteBatch sb)
        {
            Vector2 itemDrawLoc = itemDrawLocStart;
            menuOpen = true;

            for (int i = 0; i < maxInventorySlots; i++)
            {
                Texture2D textureSlot = new Texture2D(_graphics, 64, 64);
                Color[] data = new Color[64 * 64];
                Color color = Color.DarkGray;
                if (i == selectedIndex)
                    color = Color.Crimson;
                for (int j = 0; j < data.Length; ++j) data[j] = color;
                textureSlot.SetData(data);
                Rectangle slotLoc = new Rectangle((int)itemDrawLoc.X, (int)itemDrawLoc.Y, 64, 64);
                slotLocations[i] = slotLoc;
                sb.Begin();
                sb.Draw(textureSlot, itemDrawLoc, Color.DarkGray);
                sb.End();

                if (items.ElementAtOrDefault(i) != null)
                {
                    var item = items[i];
                    Vector2 offsetLocation;

                    if (!saveItemSpriteScale.ContainsKey(item))
                        saveItemSpriteScale[item] = item.spriteScale;

                    if (item is IHandHeld) // handhelds display action frames so scaling them will make them to tiny and offset
                    {
                        item.spriteScale = 1.3f;
                        offsetLocation = new Vector2(itemDrawLoc.X + textureSlot.Width / 3, itemDrawLoc.Y + textureSlot.Height / 3);
                        item.currRowFrame = 0;
                    }
                    else
                    {
                        item.spriteScale = (float)(itemDisplaySizePix / item.targetRectangle.Width);
                        offsetLocation = new Vector2(itemDrawLoc.X + textureSlot.Width / 2, itemDrawLoc.Y + textureSlot.Height / 1.7f); // move Y down a little to leave room for stack number display
                    }
                    item.location = offsetLocation;
                    item.Draw(sb, null);

                    sb.Begin();
                    sb.DrawString(font, "x" + item.amountStacked.ToString(), itemDrawLoc, Color.Black);
                    sb.End();
                }

                itemDrawLoc.X += 70;
                if (itemDrawLoc.X > location.X + GetWidth() / 2 - 50)
                {
                    itemDrawLoc.X = itemDrawLocStart.X;
                    itemDrawLoc.Y += 70;
                }

            }

            if (openItemMenu)
            {
                DrawItemMenu(sb, items);
            }

            // draw cursor
            sb.Begin();
            sb.Draw(cursor, cursorPos, Color.White);
            sb.End();

            // reset itemDrawLoc
            itemDrawLoc = itemDrawLocStart;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera cam)
        {
            if (menuOpen)
            {
                timeRClicked += gameTime.ElapsedGameTime.Milliseconds;
                timeLClicked += gameTime.ElapsedGameTime.Milliseconds;

                cursorPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                Rectangle cursorRect = new Rectangle((int)cursorPos.X, (int)cursorPos.Y, cursor.Width, cursor.Height);

                int i = 0;
                foreach (var slot in slotLocations.Values)
                {
                    if (slot.Intersects(cursorRect))
                        selectedIndex = i;
                    i++;
                }

                foreach (var entry in itemMenuButtonLocations)
                {
                    if (entry.Value.Intersects(cursorRect))
                    {
                        itemMenuFunc = entry.Key;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed && !(timeLClicked < 200))
                        {
                            var item = inventoryOfPlayer.inventory[selectedIndex];
                            item.spriteScale = saveItemSpriteScale[item];
                            saveItemSpriteScale.Remove(item);

                            if (itemMenuFunc.Equals("drop"))
                            {
                                item.inInventory = false;
                                item.onGround = true;
                                item.remove = false;
                                item.location.X = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().X + rand.Next(-10, 10);
                                item.location.Y = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().Y + rand.Next(-10, 10);
                                ItemUtility.ItemsToUpdate.Add(item);
                                inventoryOfPlayer.inventory.Remove(item);
                                timeLClicked = 0;
                            }
                            else if (itemMenuFunc.Equals("eq")) {
                                inventoryOfPlayer.inventory.Add(inventoryOfPlayer.inHand);
                                inventoryOfPlayer.inHand = (HandHeld)item;
                                inventoryOfPlayer.inventory.Remove(item);
                                timeLClicked = 0;
                            }
                        }

                    }
                }

                bool toggleItemMenu = Mouse.GetState().RightButton == ButtonState.Pressed;
                if (toggleItemMenu && !(timeRClicked < 200))
                {
                    openItemMenu = !openItemMenu;
                    itemMenuPos = cursorPos;
                    timeRClicked = 0;
                }
            }
            menuOpen = false;

        }

        private void DrawItemMenu(SpriteBatch sb, List<InventoryItem> items)
        {
            Texture2D texItemMenu = new Texture2D(_graphics, 60, 80);
            Color[] data = new Color[60 * 80];
            Color color = Color.Gray;
            for (int j = 0; j < data.Length; ++j) data[j] = color;
            texItemMenu.SetData(data);

            itemMenuButtonLocations.Clear();

            sb.Begin();
            sb.Draw(texItemMenu, itemMenuPos, Color.DarkGray);

            if (items.ElementAtOrDefault(selectedIndex) != null)
            {
                Color colorSel = Color.LightGray;

                Texture2D texItemDrop = new Texture2D(_graphics, 50, 34);
                Color[] dataD = new Color[50 * 34];
                if (itemMenuFunc == "drop")
                    colorSel = Color.Thistle;
                for (int j = 0; j < dataD.Length; ++j) dataD[j] = colorSel;
                texItemDrop.SetData(dataD);
                Vector2 dropTextLoc = new Vector2(itemMenuPos.X + 5, itemMenuPos.Y + 3);
                itemMenuButtonLocations["drop"] = new Rectangle((int)dropTextLoc.X, (int)dropTextLoc.Y, texItemDrop.Width, texItemDrop.Height);
                sb.Draw(texItemDrop, dropTextLoc, Color.LightGray);
                sb.DrawString(font, "drop", dropTextLoc, Color.Black);

                colorSel = Color.LightGray;
                if (items[selectedIndex] is IHandHeld)
                {

                    Texture2D texItemEQ = new Texture2D(_graphics, 50, 34);
                    Color[] dataE = new Color[50 * 34];
                    if (itemMenuFunc == "eq")
                        colorSel = Color.Thistle;
                    for (int j = 0; j < dataE.Length; ++j) dataE[j] = colorSel;
                    texItemEQ.SetData(dataE);
                    Vector2 eqTextLoc = new Vector2(itemMenuPos.X + 5, itemMenuPos.Y + 40 + 3);
                    itemMenuButtonLocations["eq"] = new Rectangle((int)eqTextLoc.X, (int)eqTextLoc.Y, texItemEQ.Width, texItemEQ.Height);
                    sb.Draw(texItemEQ, eqTextLoc, Color.LightGray);
                    sb.DrawString(font, "equip", eqTextLoc, Color.Black);
                }
                itemMenuFunc = "";
            }

            sb.End();
        }
    }
}
