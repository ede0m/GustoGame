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
        int shipInventorySlots;
        int storageInvSlots;
        int selectedIndex;
        int itemMenuIndex;
        bool openItemMenu;
        bool openShipInventory;
        string itemMenuFunc;
        float timeRClicked;
        float timeLClicked;
        Dictionary<int, Rectangle> slotLocations;
        Dictionary<string, Rectangle> itemMenuButtonLocations;
        Dictionary<InventoryItem, float> saveItemSpriteScale;

        bool draggingItem;
        int selectDragIndex;
        int dropDragIndex;
        List<InventoryItem> tempInventory;

        float itemDisplaySizePix;
        Vector2 itemDrawLocStart;
        Vector2 storageItemDrawLoc;
        Vector2 storageItemDrawLocStart;
        Vector2 cursorPos;
        Vector2 itemMenuPos;
        Texture2D cursor;
        SpriteFont font;

        PlayerPirate inventoryOfPlayer;
        Storage storageContainer;

        GraphicsDevice _graphics;
        ContentManager _content;

        public Inventory(Vector2 location, ContentManager content, GraphicsDevice graphics, PlayerPirate invOfPlayer) : base(graphics)
        {

            _graphics = graphics;
            _content = content;
            font = _content.Load<SpriteFont>("helperFont");
            cursor = _content.Load<Texture2D>("pointer");

            inventoryOfPlayer = invOfPlayer;

            itemDisplaySizePix = 60;
            maxInventorySlots = inventoryOfPlayer.maxInventorySlots;
            shipInventorySlots = 0;
            storageInvSlots = 0;
            selectedIndex = 0;
            dropDragIndex = -1;
            selectDragIndex = -1;
            itemMenuIndex = -1;
            tempInventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots + shipInventorySlots).ToList();

            slotLocations = new Dictionary<int, Rectangle>();
            itemMenuButtonLocations = new Dictionary<string, Rectangle>();
            saveItemSpriteScale = new Dictionary<InventoryItem, float>();

            Texture2D textureInventory = new Texture2D(graphics, 440, 275);
            Color[] data = new Color[440 * 275];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.DimGray;
            textureInventory.SetData(data);
            Asset invetoryAsset = new Asset(textureInventory, null, 1, 1, 1.0f, null, null);
            SetSpriteAsset(invetoryAsset, location);

            itemDrawLocStart = new Vector2(location.X - _texture.Width/2 + 50, location.Y - _texture.Height/2 + 100);
        }

        public void DrawInventory(SpriteBatch sb, List<InventoryItem> itemsPlayer, List<InventoryItem> itemsShip, Storage storage)
        {
            storageContainer = storage;
            Vector2 itemDrawLoc = itemDrawLocStart;
            menuOpen = true;

            List<InventoryItem> items = itemsPlayer;
            int showSlots = maxInventorySlots;

            List<InventoryItem> itemsStorage = null;

            // draw item stat panel
            Texture2D textureItemStat = new Texture2D(_graphics, 140, 50);
            Color[] csdata = new Color[50 * 140];
            for (int j = 0; j < csdata.Length; ++j) csdata[j] = Color.Gray;
            textureItemStat.SetData(csdata);
            Vector2 itemStatLoc = new Vector2(itemDrawLoc.X + 100, itemDrawLoc.Y - 80);
            itemMenuButtonLocations["itemStat"] = new Rectangle((int)itemStatLoc.X, (int)itemStatLoc.Y, textureItemStat.Width, textureItemStat.Height);
            sb.Begin();
            sb.Draw(textureItemStat, itemStatLoc, Color.Gray);
            sb.End();

            // ship inv button
            if (itemsShip != null)
            {
                Texture2D textureShipButton = new Texture2D(_graphics, 90, 50);
                Color[] data = new Color[50 * 90];
                for (int j = 0; j < data.Length; ++j) data[j] = Color.Gray;
                textureShipButton.SetData(data);
                Vector2 shipButtonLoc = new Vector2(itemDrawLoc.X, itemDrawLoc.Y - 80);
                itemMenuButtonLocations["ship"] = new Rectangle((int)shipButtonLoc.X, (int)shipButtonLoc.Y, textureShipButton.Width, textureShipButton.Height);
                sb.Begin();
                sb.Draw(textureShipButton, shipButtonLoc , Color.Gray);
                sb.DrawString(font, "ship", shipButtonLoc, Color.Black);
                sb.End();

                if (openShipInventory)
                {
                    // ship inv addon
                    Texture2D textureInventoryShip = new Texture2D(_graphics, 440, 220);
                    Color[] cdata = new Color[440 * 220];
                    for (int i = 0; i < cdata.Length; ++i) cdata[i] = Color.DimGray;
                    textureInventoryShip.SetData(cdata);
                    sb.Begin();
                    sb.Draw(textureInventoryShip, new Vector2(location.X - _texture.Width/2, location.Y + _texture.Height/2), Color.White);
                    sb.End();

                    items.AddRange(itemsShip);
                    shipInventorySlots = inventoryOfPlayer.playerOnShip.maxInventorySlots;
                    showSlots = maxInventorySlots + shipInventorySlots;
                    tempInventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots + shipInventorySlots).ToList();
                }
            }

            // storage inv add on
            else if (storage != null)
            {
                itemsStorage = storage.inventory;
                Texture2D textureInvStorage = new Texture2D(_graphics, 150, 400);
                Color[] cdata = new Color[400 * 150];
                for (int i = 0; i < cdata.Length; ++i) cdata[i] = Color.DimGray;
                textureInvStorage.SetData(cdata);
                sb.Begin();
                sb.Draw(textureInvStorage, new Vector2(location.X + (_texture.Width / 2) + 50, location.Y - (_texture.Height / 2)), Color.White);
                sb.End();

                items.AddRange(itemsStorage);
                storageInvSlots = itemsStorage.Count;
                showSlots = maxInventorySlots + storageInvSlots; // BUG?
                storageItemDrawLocStart = new Vector2(location.X + (_texture.Width / 2) + 90, location.Y - (_texture.Height / 2) + 30);
                storageItemDrawLoc = storageItemDrawLocStart;
                tempInventory = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots + storageInvSlots).ToList();
            }

            int textureHW = 64;
            int inventorySeparation = 20;
            // draw slots
            for (int i = 0; i < showSlots; i++)
            {
                // some separation between ship/storage and player inventory
                if (i == maxInventorySlots && itemsShip != null)
                    itemDrawLoc.Y += inventorySeparation;

                // position storage items in storage
                if (i >= maxInventorySlots && itemsStorage != null)
                {
                    itemDrawLoc = storageItemDrawLoc;
                    storageItemDrawLoc.X += 70;
                    if (storageItemDrawLoc.X > location.X + (GetWidth() / 2) - 50)
                    {
                        storageItemDrawLoc.X = storageItemDrawLocStart.X;
                        storageItemDrawLoc.Y += 70;
                    }
                }

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

            // draw items in list and set new list order if needed
            List<InventoryItem> tempPlayerInv = Enumerable.Repeat<InventoryItem>(null, maxInventorySlots).ToList();
            List<InventoryItem> tempShipInv = Enumerable.Repeat<InventoryItem>(null, shipInventorySlots).ToList();
            List<InventoryItem> tempStorageInv = Enumerable.Repeat<InventoryItem>(null, storageInvSlots).ToList();

            List<InventoryItem> invMap = itemsPlayer;
            itemDrawLoc = itemDrawLocStart;
            storageItemDrawLoc = storageItemDrawLocStart;
            int emptyIndex = -1; // used to denote when an item is dragged into an empty slot
            for (int i = 0; i < showSlots; i++)
            {
                InventoryItem item = null;
                if (items.ElementAtOrDefault(i) != null)
                {
                    item = items[i];
                    Vector2 offsetLocation;
                    Vector2 itemLoc = itemDrawLoc;

                    // offset the item location by the ships inventory separation
                    if (i >= maxInventorySlots && itemsShip != null)
                        itemLoc.Y += inventorySeparation;
                    // position storage items in storage
                    if (i >= maxInventorySlots && itemsStorage != null)
                        itemLoc = storageItemDrawLoc;

                    // dragging
                    if (draggingItem && selectDragIndex == i)
                        itemLoc = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                    // dropping on drag
                    if (dropDragIndex >= 0 && selectDragIndex == i)
                    {
                        itemLoc = slotLocations[dropDragIndex].Location.ToVector2();
                        if (invMap[dropDragIndex] != null) // switch spots
                        {
                            if (invMap[dropDragIndex].bbKey.Equals(invMap[selectDragIndex].bbKey) && invMap[dropDragIndex].stackable)
                            {
                                // stack items
                                item.amountStacked += invMap[selectDragIndex].amountStacked;
                                tempInventory[selectDragIndex] = null;
                                emptyIndex = selectDragIndex;
                            }
                            else
                                tempInventory[selectDragIndex] = invMap[dropDragIndex];

                        }
                        else
                        {
                            tempInventory[selectDragIndex] = null;
                            emptyIndex = selectDragIndex;
                        }
                        tempInventory[dropDragIndex] = item;
                        dropDragIndex = -1;
                    }

                    // track sprite scale
                    if (!saveItemSpriteScale.ContainsKey(item))
                        saveItemSpriteScale[item] = item.spriteScale;

                    // draw the items TODO: could move it icon method like CraftingMenu
                    if (item is IHandHeld) // handhelds display action frames so scaling them will make them too tiny and offset
                    {
                        item.spriteScale = 1.3f;
                        offsetLocation = new Vector2(itemLoc.X + textureHW / 3, itemLoc.Y + textureHW / 3);
                        item.currRowFrame = 0;
                        item.currColumnFrame = 0;
                    }
                    else
                    {
                        item.spriteScale = (float)(itemDisplaySizePix / item.GetWidth());
                        offsetLocation = new Vector2(itemLoc.X + textureHW / 2, itemLoc.Y + textureHW / 1.7f); // move Y down a little to leave room for stack number display
                    }

                    item.location = offsetLocation;
                    item.Draw(sb, null);

                    sb.Begin();
                    //stack amount display
                    sb.DrawString(font, "x" + item.amountStacked.ToString(), itemLoc, Color.Black);
                    //name display
                    if (i == selectedIndex)
                        sb.DrawString(font, item.itemKey, itemStatLoc, Color.Black);
                    sb.End();
                }

                // advance inventory spot
                itemDrawLoc.X += 70;
                if (itemDrawLoc.X > location.X + GetWidth() / 2 - 50)
                {
                    itemDrawLoc.X = itemDrawLocStart.X;
                    itemDrawLoc.Y += 70;
                }
                // advance storage spot if we are in that range
                if (i >= maxInventorySlots && itemsStorage != null)
                {
                    storageItemDrawLoc.X += 70;
                    if (storageItemDrawLoc.X > location.X + (GetWidth() / 2) - 50)
                    {
                        storageItemDrawLoc.X = storageItemDrawLocStart.X;
                        storageItemDrawLoc.Y += 70;
                    }
                }

                // keep the items that were not dragged the same 
                if (tempInventory[i] == null && i != emptyIndex)
                    tempInventory[i] = item;

                // clear any zero stacked items
                if (tempInventory[i] != null && tempInventory[i].stackable && tempInventory[i].amountStacked <= 0)
                    tempInventory[i] = null;

            }

            // copy new inventory with any inventory movements
            for (int i = 0; i <  tempInventory.Count; i++)
            {
                if (i < maxInventorySlots)
                    tempPlayerInv[i] = tempInventory[i];
                else if (i >= maxInventorySlots && itemsShip != null && openShipInventory)
                    tempShipInv[i - maxInventorySlots] = tempInventory[i];
                else if (i >= maxInventorySlots && itemsStorage != null)
                    tempStorageInv[i - maxInventorySlots] = tempInventory[i];
            }

            // set any inventory movements
            inventoryOfPlayer.inventory = tempPlayerInv;
            if (openShipInventory && itemsShip != null)
                inventoryOfPlayer.playerOnShip.inventory = tempShipInv;
            // storage movements
            if (itemsStorage != null)
                storage.inventory = tempStorageInv;

            if (openItemMenu)
                DrawItemMenu(sb, itemsPlayer);

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

                selectedIndex = -1;
                if (itemMenuIndex != -1)  // don't switch slots when item menu open
                {
                    selectedIndex = itemMenuIndex;
                }
                else
                {
                    int i = 0;
                    foreach (var slot in slotLocations.Values)
                    {
                        if (slot.Intersects(cursorRect))
                        {
                            selectedIndex = i;
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !draggingItem)
                            {
                                draggingItem = true;
                                selectDragIndex = selectedIndex;
                            }

                            if (!(Mouse.GetState().LeftButton == ButtonState.Pressed) && draggingItem)
                            {
                                dropDragIndex = i;
                                draggingItem = false;
                            }

                        }
                        i++;
                    }
                }

                foreach (var entry in itemMenuButtonLocations)
                {
                    if (entry.Value.Intersects(cursorRect))
                    {
                        itemMenuFunc = entry.Key;

                        if (itemMenuFunc.Equals("ship") && (Mouse.GetState().LeftButton == ButtonState.Pressed) && !(timeLClicked < 200))
                        {
                            openShipInventory = !openShipInventory;
                            timeLClicked = 0;
                        }

                        // display item menu options
                        if (openItemMenu && Mouse.GetState().LeftButton == ButtonState.Pressed && !(timeLClicked < 200))
                        {
                            InventoryItem item = null;
                            List<InventoryItem> inv = null;
                            if (selectedIndex < maxInventorySlots)
                            {
                                item = inventoryOfPlayer.inventory[selectedIndex];
                                inv = inventoryOfPlayer.inventory;
                            }
                            else
                            {
                                if (openShipInventory)
                                {
                                    item = inventoryOfPlayer.playerOnShip.inventory[selectedIndex - maxInventorySlots];
                                    inv = inventoryOfPlayer.playerOnShip.inventory;
                                }

                                else
                                {
                                    item = storageContainer.inventory[selectedIndex - maxInventorySlots];
                                    inv = storageContainer.inventory;
                                }
                            }
                            item.spriteScale = saveItemSpriteScale[item];
                            saveItemSpriteScale.Remove(item);

                            if (itemMenuFunc.Equals("drop"))
                            {
                                // placable item?
                                if (item.placeableVersion != null)
                                {
                                    Sprite placeableItem = (Sprite)item.placeableVersion;
                                    placeableItem.remove = false;
                                    placeableItem.location.X = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().X + RandomEvents.rand.Next(-10, 10);
                                    placeableItem.location.Y = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().Y + RandomEvents.rand.Next(-10, 10);
                                    ItemUtility.ItemsToUpdate.Add(placeableItem);
                                }
                                else
                                {
                                    item.inInventory = false;
                                    item.onGround = true;
                                    item.remove = false;
                                    item.location.X = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().X + RandomEvents.rand.Next(-10, 10);
                                    item.location.Y = inventoryOfPlayer.GetBoundingBox().Location.ToVector2().Y + RandomEvents.rand.Next(-10, 10);
                                    ItemUtility.ItemsToUpdate.Add(item);
                                }

                                if (selectedIndex < maxInventorySlots)
                                    inv[selectedIndex] = null;
                                else
                                    inv[selectedIndex - maxInventorySlots] = null;
                                tempInventory[selectedIndex] = null;
                                timeLClicked = 0;
                            }
                            else if (itemMenuFunc.Equals("eq")) {
                                inventoryOfPlayer.inventory[selectedIndex] = inventoryOfPlayer.inHand;
                                tempInventory[selectedIndex] = inventoryOfPlayer.inHand;
                                inventoryOfPlayer.inHand = (HandHeld)item;
                                timeLClicked = 0;
                            }
                        }

                    }
                }

                bool toggleItemMenu = Mouse.GetState().RightButton == ButtonState.Pressed;
                if (toggleItemMenu && !(timeRClicked < 200))
                {
                    openItemMenu = !openItemMenu;
                    if (openItemMenu)
                    {
                        itemMenuIndex = selectedIndex;
                        if (selectedIndex != -1)
                            itemMenuPos = slotLocations[selectedIndex].Center.ToVector2();
                    }
                    else
                        itemMenuIndex = -1;
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
