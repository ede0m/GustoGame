using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Models.Menus;
using Gusto.SaveState;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gusto
{
    public class GameState
    {

        public bool ready;
        string savePath;

        ContentManager _content;
        GraphicsDevice _graphics;

        public string gameName;
        public HashSet<Sprite> UpdateOrder;

        public PiratePlayer player; // only track the player because there are some special focuses around it.. obviously

        public GameState(ContentManager c, GraphicsDevice g)
        {
            savePath = @"C:\Users\GMON\Desktop\";
            gameName = "game1";

            _content = c;
            _graphics = g;
            UpdateOrder = new HashSet<Sprite>();

            player = new PiratePlayer(TeamType.Player, "GustoMap", new Vector2(0, -300), _content, _graphics); // This is a default location (for new game) if there is a load it will be overwritten
        }

        // Creates the initial game state - this will probably be a huge method at the end of it.. TODO: find way to dynamically create items/npc/etc and place them in appropriate region
        public void CreateNewGame()
        {

            //TEMPORARY NEED TO CREATE SOME SORT OF GAME SETUP / REGION SETUP
            List<Sprite> giannaLandTiles = BoundingBoxLocations.RegionMap["Gianna"].RegionLandTiles;
            Sprite GiannaRegionTile = giannaLandTiles[RandomEvents.rand.Next(giannaLandTiles.Count)];
            var screenCenter = new Vector2(_graphics.Viewport.Bounds.Width / 2, _graphics.Viewport.Bounds.Height / 2);

            BaseShip baseShip = new BaseShip(TeamType.Player, "GustoMap", new Vector2(-100, -500), _content, _graphics);
            BaseShip baseShipAI = new BaseShip(TeamType.A, "GustoMap", new Vector2(470, 0), _content, _graphics);
            BaseTribal baseTribal = new BaseTribal(TeamType.B, "Gianna", GiannaRegionTile.location, _content, _graphics);
            Tower tower = new BaseTower(TeamType.A, "GustoMap", new Vector2(200, 700), _content, _graphics);
            ClayFurnace furnace = new ClayFurnace(TeamType.Player, "GustoMap", new Vector2(180, 140), _content, _graphics);
            CraftingAnvil craftingAnvil = new CraftingAnvil(TeamType.Player, "GustoMap", new Vector2(120, 40), _content, _graphics);
            BaseBarrel barrelLand = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(-20, -160), _content, _graphics);
            BaseBarrel barrelOcean = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(380, -60), _content, _graphics);
            BaseChest chestLand = new BaseChest(TeamType.A, "GustoMap", new Vector2(100, -120), _content, _graphics);
            BaseChest chestOcean = new BaseChest(TeamType.A, "GustoMap", new Vector2(350, 0), _content, _graphics);

            Shovel shovel = new Shovel(TeamType.A, "GustoMap", new Vector2(200, -330), _content, _graphics);
            shovel.onGround = true;
            Pickaxe pickaxe = new Pickaxe(TeamType.Player, "GustoMap", new Vector2(130, -430), _content, _graphics);
            pickaxe.onGround = true;
            Pistol pistol = new Pistol(TeamType.A, "GustoMap", new Vector2(250, -300), _content, _graphics);
            pistol.amountStacked = 1;
            pistol.onGround = true;
            PistolShotItem pistolAmmo = new PistolShotItem(TeamType.A, "GustoMap", new Vector2(220, -300), _content, _graphics);
            pistolAmmo.amountStacked = 14;
            pistolAmmo.onGround = true;
            CannonBallItem cannonAmmo = new CannonBallItem(TeamType.A, "GustoMap", new Vector2(200, -300), _content, _graphics);
            cannonAmmo.amountStacked = 10;
            cannonAmmo.onGround = true;
            Lantern lantern = new Lantern(TeamType.A, "GustoMap", new Vector2(180, -300), _content, _graphics);
            lantern.onGround = true;
            BasePlank basePlank = new BasePlank(TeamType.A, "GustoMap", new Vector2(150, -300), _content, _graphics);
            basePlank.onGround = true;
            basePlank.amountStacked = 10;


            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(player);
            UpdateOrder.Add(baseTribal);
            UpdateOrder.Add(tower);
            UpdateOrder.Add(lantern);
            UpdateOrder.Add(furnace);
            UpdateOrder.Add(craftingAnvil);
            UpdateOrder.Add(barrelLand);
            UpdateOrder.Add(barrelOcean);
            UpdateOrder.Add(chestLand);
            UpdateOrder.Add(chestOcean);
            UpdateOrder.Add(shovel);
            UpdateOrder.Add(pistol);
            UpdateOrder.Add(pickaxe);
            UpdateOrder.Add(pistolAmmo);
            UpdateOrder.Add(cannonAmmo);
            UpdateOrder.Add(basePlank);

            ready = true;
        }

        // Runs the in game update for objets that need to save state
        public HashSet<Sprite> Update (KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            List<Sprite> toRemove = new List<Sprite>();

            // camera follows player
            if (!player.onShip)
                camera.Position = player.location;
            else
                camera.Position = player.playerOnShip.location;

            foreach (Sprite sp in UpdateOrder)
            {
                // ICanUpdate is the update for main sprites. Any sub-sprites (items, weapons, sails, etc) that belong to the main sprite are updated within the sprite's personal update method. 
                ICanUpdate updateSp = (ICanUpdate)sp;
                updateSp.Update(kstate, gameTime, camera);
            }


            return UpdateOrder;
        }

        public void SaveGameState()
        {
            // Create the save state
            List<ISaveState> SaveState = new List<ISaveState>();
            foreach (Sprite sp in UpdateOrder)
            {
                if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship sh = (Ship)sp;
                    ShipState state = new ShipState();
                    state.team = sh.teamType;
                    state.location = sh.location;
                    state.region = sh.regionKey;
                    state.objKey = sh.bbKey;
                    state.inventory = CreateSerializableInventory(sh.inventory);
                    state.playerAboard = sh.playerAboard;
                    state.anchored = sh.anchored;
                    state.health = sh.health;
                    SaveState.Add(state);
                }
            }

            // serialize save to file system
            DataContractSerializer s = new DataContractSerializer(typeof(List<ISaveState>));
            using (FileStream fs = new FileStream(savePath + "GustoGame_" + gameName, FileMode.Create))
            {
                s.WriteObject(fs, SaveState);
            }
        }

        public void LoadGameState()
        {
            
            DataContractSerializer s = new DataContractSerializer(typeof(List<ISaveState>), new Type[] { typeof(ShipState)});
            FileStream fs = new FileStream(savePath + "GustoGame_" + gameName, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            List<ISaveState> LoadFromState = (List<ISaveState>)s.ReadObject(reader);
            reader.Close();
            fs.Close();
            InitializeLoadState(LoadFromState);
            ready = true;
        }

        private void InitializeLoadState(List<ISaveState> LoadFromState)
        {
            foreach (ISaveState objState in LoadFromState)
            {
                if (objState.GetType() == typeof(ShipState))
                {
                    ShipState ss = (ShipState)objState;
                    if (ss.objKey.Equals("baseShip"))
                    {
                        BaseShip baseShip = new BaseShip(ss.team, ss.region, ss.location, _content, _graphics);
                        baseShip.health = ss.health;
                        baseShip.inventory = DeserializeInventory(ss.inventory);
                        baseShip.playerAboard = ss.playerAboard;
                        UpdateOrder.Add(baseShip);
                    }
                }
            }

        }

        private List<InventoryItem> DeserializeInventory(List<InventoryItemSerialized> inv)
        {
            // todo!
            return null;
        }

        private List<InventoryItemSerialized> CreateSerializableInventory(List<InventoryItem> inv)
        {
            int index = 0;
            List<InventoryItemSerialized> ret = Enumerable.Repeat<InventoryItemSerialized>(null, inv.Count()).ToList();
            foreach (InventoryItem item in inv)
            {
                if (item == null)
                {
                    ret[index] = null;
                    continue;
                }

                InventoryItemSerialized iszd = new InventoryItemSerialized();
                iszd.itemKey = item.itemKey;
                iszd.stackedAmount = item.amountStacked;

                // Special cases
                if (item.itemKey.Equals("treasureMapItem"))
                {
                    iszd.treasureMaps = new Dictionary<int, TreasureMapItemSerialized>();
                    TreasureMapItemSerialized tm = new TreasureMapItemSerialized();
                    TreasureMap m = (TreasureMap)item;
                    tm.digLocation = m.digTile.location;
                    tm.region = m.treasureInRegion;
                    if (tm.reward != null)
                        tm.reward = CreateSerializableInventory(m.rewarded.inventory); // risk of infite loop recusion, but only if you bury treasure maps in storage that you have a map for. It shouuuuld reach a base case.. lol
                    iszd.treasureMaps.Add(index, tm);
                }
                else if (item is IStorageItem)
                {
                    iszd.storageItems = new Dictionary<int, List<InventoryItemSerialized>>();
                    Storage st = (Storage)item.placeableVersion;
                    int indexStorage = 0;
                    List<InventoryItemSerialized> storageInventorySzd = Enumerable.Repeat<InventoryItemSerialized>(null, st.inventory.Count()).ToList();
                    foreach (InventoryItem stItem in st.inventory)
                    {
                        if (stItem == null)
                        {
                            storageInventorySzd[index] = null;
                            continue;
                        }

                        InventoryItemSerialized stItemSzd = new InventoryItemSerialized();
                        stItemSzd.itemKey = stItem.itemKey;
                        stItemSzd.stackedAmount = stItem.amountStacked;
                        if (stItem.itemKey.Equals("treasureMapItem"))
                        {
                            stItemSzd.treasureMaps = new Dictionary<int, TreasureMapItemSerialized>();
                            TreasureMapItemSerialized tm = new TreasureMapItemSerialized();
                            TreasureMap m = (TreasureMap)stItem;
                            tm.digLocation = m.digTile.location;
                            tm.region = m.treasureInRegion;
                            if (tm.reward != null)
                                tm.reward = CreateSerializableInventory(m.rewarded.inventory); // risk of infite loop recusion, but only if you bury treasure maps in storage that you have a map for. It shouuuuld reach a base case.. lol
                            stItemSzd.treasureMaps.Add(index, tm);
                        }
                        // do not need to check for more storage here because I have prevented storage from being placed within storage.. TODO

                        storageInventorySzd[indexStorage] = stItemSzd;
                        indexStorage++;
                    }
                    iszd.storageItems.Add(index, storageInventorySzd);
                }

                ret[index] = iszd;
                index++;
            }

            return ret;
        }

    }
}
