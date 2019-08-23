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
            Dictionary<Ship, Guid> shipMap = new Dictionary<Ship, Guid>();

            foreach (Sprite sp in UpdateOrder)
            {
                if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.PlayerPirate))
                {
                    PlayerState state = new PlayerState();
                    state.team = player.teamType;
                    state.location = player.location;
                    state.region = player.regionKey;
                    state.inventory = CreateSerializableInventory(player.inventory);
                    state.onShip = player.onShip;
                    if (player.playerOnShip != null)
                    {
                        if (shipMap.ContainsKey(player.playerOnShip))
                            state.playerOnShipId = shipMap[player.playerOnShip];
                        else
                        {
                            // save the ship that the player was on. 
                            Ship sh = (Ship)player.playerOnShip;
                            ShipState shipState = new ShipState();
                            shipState.team = sh.teamType;
                            shipState.location = sh.location;
                            shipState.region = sh.regionKey;
                            shipState.objKey = sh.bbKey;
                            shipState.inventory = CreateSerializableInventory(sh.inventory);
                            shipState.playerAboard = sh.playerAboard;
                            shipState.anchored = sh.anchored;
                            shipState.health = sh.health;
                            // create and save guid for player and ship, track it
                            Guid shipId = Guid.NewGuid();
                            state.playerOnShipId = shipId;
                            shipState.shipId = shipId;
                            shipMap.Add(sh, shipId);
                            SaveState.Add(state);
                        }
                    }
                    else
                        state.playerOnShipId = Guid.Empty;

                    state.inHandItemKey = player.inHand.itemKey;
                    state.health = player.health;
                    SaveState.Add(state);
                }

                else if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship sh = (Ship)sp;
                    if (!shipMap.ContainsKey(sh))
                    {
                        ShipState state = new ShipState();
                        state.team = sh.teamType;
                        state.location = sh.location;
                        state.region = sh.regionKey;
                        state.objKey = sh.bbKey;
                        state.inventory = CreateSerializableInventory(sh.inventory);
                        state.playerAboard = sh.playerAboard;
                        state.anchored = sh.anchored;
                        state.health = sh.health;
                        state.shipId = Guid.NewGuid();
                        shipMap.Add(sh, state.shipId);
                        SaveState.Add(state);
                    }
                }

                // TODO: OTHER STATES (WEATHER, ETC)
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
            Type[] deserializeTypes = new Type[] { typeof(ShipState), typeof(PlayerState) };
            DataContractSerializer s = new DataContractSerializer(typeof(List<ISaveState>), deserializeTypes);
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
            // TODO: deserialize the ps.onShipId (create if id not tracked yet, otherwise set to ship in tracked dict) kinda the opposite of how it is done in save
            Dictionary<Ship, Guid> shipMap = new Dictionary<Ship, Guid>();

            foreach (ISaveState objState in LoadFromState)
            {
                if (objState.GetType() == typeof(PlayerState))
                {
                    PlayerState ps = (PlayerState)objState;
                    player.location = ps.location;
                    player.inventory = DeserializeInventory(ps.inventory);
                    player.inHand = (HandHeld)DeserializeInventoryItem(ps.inHandItemKey);

                    if (ps.playerOnShipId == Guid.Empty)
                        player.playerOnShip = null;
                    else
                    {
                        // check if the ship already exists
                        foreach (KeyValuePair<Ship, Guid> ship in shipMap)
                        {
                            if (ps.playerOnShipId == ship.Value)
                            {
                                player.playerOnShip = ship.Key;
                                break;
                            }
                        }
                        // ship did not already exist so we have to create here
                        if (player.playerOnShip == null)
                        {
                            // we have to loop here again to find the save state.. eh
                            ISaveState shipSaveStateToFind = null;
                            foreach (ISaveState ss in LoadFromState)
                            {
                                if (objState.GetType() == typeof(ShipState))
                                {
                                    ShipState shipSave = (ShipState)objState;
                                    if (shipSave.shipId == ps.playerOnShipId)
                                    {
                                        shipSaveStateToFind = shipSave;
                                        break;
                                    }
                                }

                            }
                            Ship s = (Ship)DeserializeModel(ps.objKey, shipSaveStateToFind);
                            shipMap.Add(s, ps.playerOnShipId);
                            player.playerOnShip = s;
                            UpdateOrder.Add(s);
                            //UpdateOrder.Add(s.shipSail);
                        }
                    }

                    player.onShip = ps.onShip;
                    player.regionKey = ps.region;
                    player.health = ps.health;
                    UpdateOrder.Add(player);
                }

                else if (objState.GetType() == typeof(ShipState))
                {
                    ShipState ss = (ShipState)objState;
                    bool shipCreated = false;
                    // check if the ship already exists
                    foreach (KeyValuePair<Ship, Guid> ship in shipMap)
                    {
                        if (ss.shipId == ship.Value)
                        {
                            shipCreated = true;
                            break;
                        }
                    }
                    if (!shipCreated)
                    {
                        Ship s = (Ship)DeserializeModel(ss.objKey, ss);
                        shipMap.Add(s, ss.shipId);
                        UpdateOrder.Add(s);
                        //UpdateOrder.Add(s.shipSail);
                    }
                    
                }

            }

        }

        private List<InventoryItem> DeserializeInventory(List<InventoryItemSerialized> inv)
        {
            int index = 0;
            List<InventoryItem> ret = Enumerable.Repeat<InventoryItem>(null, inv.Count()).ToList();
            foreach (InventoryItemSerialized item in inv)
            {
                if (item == null)
                {
                    ret[index] = null;
                    index++;
                    continue;
                }

                InventoryItem ii = DeserializeInventoryItem(item.itemKey);
                ii.amountStacked = item.stackedAmount;
                ii.regionKey = "GustoMap";
                ii.inInventory = true;
                ii.remove = true;

                if (item.storageItems != null)
                {
                    // TODO: ii will already be a StorageItem, we just need to set the placable versions Storage values
                }
                
                if (item.treasureMaps != null)
                {
                    // TODO: deseraialize treasureMapSerialized and init treasureMap here (ii = new TreasureMapItem ...)
                }

                ret[index] = ii;
                index++;
            }
            return ret;
        }

        private Sprite DeserializeModel(string objKey, ISaveState objSave)
        {
            Sprite sp = null;
            switch (objKey)
            {
                // TODO: ALL THE Models :( 
                case "baseShip":
                    ShipState ss = (ShipState)objSave;
                    BaseShip bs = new BaseShip(ss.team, ss.region, ss.location, _content, _graphics);
                    bs.health = ss.health;
                    bs.inventory = DeserializeInventory(ss.inventory);
                    bs.playerAboard = ss.playerAboard;
                    if (bs.playerAboard)
                        bs.shipSail.playerAboard = true;
                    return bs;

            }
            return sp;
        }

        private InventoryItem DeserializeInventoryItem(string itemKey)
        {
            InventoryItem item = null;
            switch (itemKey)
            {
                // TODO: ALL THE ITEMS :( 
                case "islandGrass":
                    return new IslandGrass(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "baseSword":
                    return new BaseSword(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "lantern":
                    return new Lantern(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "baseBarrelItem":
                    return new BaseBarrelItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "basePlank":
                    return new BasePlank(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "cannonBall":
                    return new CannonBallItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "coal":
                    return new Coal(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "ironBar":
                    return new IronBar(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "ironOre":
                    return new IronOre(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "nails":
                    return new Nails(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pickaxe":
                    return new Pickaxe(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pistol":
                    return new Pistol(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pistolShot":
                    return new PistolShotItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "shortSword":
                    return new ShortSword(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "softWood":
                    return new SoftWood(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "baseChestItem":
                    BaseChestItem bci = new BaseChestItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    bci.placeableVersion = new BaseChest(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    return bci;
                case "anvilItem":
                    AnvilItem ai = new AnvilItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    ai.placeableVersion = new CraftingAnvil(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    return ai;
                case "clayFurnaceItem":
                    ClayFurnaceItem cfi = new ClayFurnaceItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    cfi.placeableVersion = new ClayFurnace(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    return cfi;
                case "treasureMapItem":
                    return null; // This is handled in calling method to deserialize some speical detials

            }
            return item;

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
                    index++;
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
