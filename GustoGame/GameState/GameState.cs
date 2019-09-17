using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.GameMap;
using Gusto.Models;
using Gusto.Models.Animated;
using Gusto.Models.Animated.Weather;
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

            player = new PiratePlayer(TeamType.Player, "GustoMap", new Vector2(-120, -550), _content, _graphics); // This is a default location (for new game) if there is a load it will be overwritten
        }

        // Creates the initial game state - this will probably be a huge method at the end of it.. TODO: find way to dynamically create items/npc/etc and place them in appropriate region
        public void CreateNewGame()
        {

            //TEMPORARY NEED TO CREATE SOME SORT OF GAME SETUP / REGION SETUP that is easily scalable
            List<Sprite> giannaLandTiles = BoundingBoxLocations.RegionMap["Gianna"].RegionLandTiles;
            List<Sprite> scLandTiles = BoundingBoxLocations.RegionMap["SnooCat"].RegionLandTiles;
            List<Sprite> usoppLandTiles = BoundingBoxLocations.RegionMap["Usopp"].RegionLandTiles;
            var screenCenter = new Vector2(_graphics.Viewport.Bounds.Width / 2, _graphics.Viewport.Bounds.Height / 2);

            ShortShip shortShip = new ShortShip(TeamType.Player, "GustoMap", new Vector2(-100, -600), _content, _graphics);
            shortShip.shipInterior.interiorId = Guid.NewGuid();
            BoundingBoxLocations.interiorMap.Add(shortShip.shipInterior.interiorId, shortShip.shipInterior);

            BaseShip baseShip = new BaseShip(TeamType.Player, "GustoMap", new Vector2(-300, -600), _content, _graphics);
            baseShip.shipInterior.interiorId = Guid.NewGuid();
            BoundingBoxLocations.interiorMap.Add(baseShip.shipInterior.interiorId, baseShip.shipInterior);

            BaseShip baseShipAI = new BaseShip(TeamType.A, "Usopp", new Vector2(500, -140), _content, _graphics);
            baseShipAI.shipInterior.interiorId = Guid.NewGuid();
            BoundingBoxLocations.interiorMap.Add(baseShipAI.shipInterior.interiorId, baseShipAI.shipInterior);

            TeePee teePee = new TeePee(TeamType.A, "Gianna", new Vector2(340, -850), _content, _graphics);
            teePee.structureInterior.interiorId = Guid.NewGuid();
            BoundingBoxLocations.interiorMap.Add(teePee.structureInterior.interiorId, teePee.structureInterior);

            BaseTribal baseTribalLand = new BaseTribal(TeamType.A, "Gianna", giannaLandTiles[RandomEvents.rand.Next(giannaLandTiles.Count)].location, _content, _graphics);
            BaseCat baseCatLand = new BaseCat(TeamType.B, "SnooCat", scLandTiles[RandomEvents.rand.Next(scLandTiles.Count)].location, _content, _graphics);
            Chicken chickenLand = new Chicken(TeamType.PassiveGround, "Gianna", giannaLandTiles[RandomEvents.rand.Next(giannaLandTiles.Count)].location, _content, _graphics);
            Snake snakeLand = new Snake(TeamType.DefenseGround, "Usopp", usoppLandTiles[RandomEvents.rand.Next(usoppLandTiles.Count)].location, _content, _graphics);
            BlueBird blueBird = new BlueBird(TeamType.PassiveAir, "Usopp", usoppLandTiles[RandomEvents.rand.Next(usoppLandTiles.Count)].location, _content, _graphics);
            Tower tower = new BaseTower(TeamType.B, "GustoMap", new Vector2(-1600, -1500), _content, _graphics);
            ClayFurnace furnace = new ClayFurnace(TeamType.Player, "GustoMap", new Vector2(180, 140), _content, _graphics);
            CraftingAnvil craftingAnvil = new CraftingAnvil(TeamType.Player, "GustoMap", new Vector2(120, 40), _content, _graphics);
            BaseBarrel barrelLand = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(-20, -160), _content, _graphics);
            BaseBarrel barrelOcean = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(380, -90), _content, _graphics);
            BaseChest chestLand = new BaseChest(TeamType.A, "GustoMap", new Vector2(100, -120), _content, _graphics);
            CampFire campfire = new CampFire(TeamType.A, "GustoMap", new Vector2(70, -350), _content, _graphics);

            Shovel shovel = new Shovel(TeamType.A, "GustoMap", new Vector2(200, -330), _content, _graphics);
            shovel.onGround = true;
            Pickaxe pickaxe = new Pickaxe(TeamType.Player, "GustoMap", new Vector2(130, -430), _content, _graphics);
            pickaxe.onGround = true;
            //Pistol pistol = new Pistol(TeamType.A, "GustoMap", new Vector2(250, -300), _content, _graphics);
            //pistol.amountStacked = 1;
            //pistol.onGround = true;
            BaseCannon cannon = new BaseCannon(TeamType.A, "GustoMap", new Vector2(0, -450), _content, _graphics);
            cannon.onGround = true;
            Ballista ballista = new Ballista(TeamType.A, "GustoMap", new Vector2(200, -300), _content, _graphics);
            ballista.onGround = true;
            CrossBow crossBow = new CrossBow(TeamType.A, "GustoMap", new Vector2(220, -350), _content, _graphics);
            crossBow.amountStacked = 1;
            crossBow.onGround = true;
            ArrowItem arrows = new ArrowItem(TeamType.A, "GustoMap", new Vector2(210, -340), _content, _graphics);
            arrows.onGround = true;
            arrows.amountStacked = 10;
            //PistolShotItem pistolAmmo = new PistolShotItem(TeamType.A, "GustoMap", new Vector2(220, -300), _content, _graphics);
            //pistolAmmo.amountStacked = 14;
            //pistolAmmo.onGround = true;
            CannonBallItem cannonAmmo = new CannonBallItem(TeamType.A, "GustoMap", new Vector2(200, -300), _content, _graphics);
            cannonAmmo.amountStacked = 10;
            cannonAmmo.onGround = true;
            RustyHarpoonItem harpoonAmmo = new RustyHarpoonItem(TeamType.A, "GustoMap", new Vector2(190, -330), _content, _graphics);
            harpoonAmmo.amountStacked = 10;
            harpoonAmmo.onGround = true;
            Lantern lantern = new Lantern(TeamType.A, "GustoMap", new Vector2(180, -300), _content, _graphics);
            lantern.onGround = true;
            BasePlank basePlank = new BasePlank(TeamType.A, "GustoMap", new Vector2(150, -300), _content, _graphics);
            basePlank.onGround = true;
            basePlank.amountStacked = 10;

            // Item utility is global and is accessed in main update, all items that are "dropped" or placed on the world view ground exist in this list (placable, invetory, etc) 
            ItemUtility.ItemsToUpdate.Add(lantern);
            ItemUtility.ItemsToUpdate.Add(furnace);
            ItemUtility.ItemsToUpdate.Add(craftingAnvil);
            ItemUtility.ItemsToUpdate.Add(barrelLand);
            ItemUtility.ItemsToUpdate.Add(barrelOcean);
            ItemUtility.ItemsToUpdate.Add(chestLand);
            ItemUtility.ItemsToUpdate.Add(shovel);
            //ItemUtility.ItemsToUpdate.Add(pistol);
            ItemUtility.ItemsToUpdate.Add(cannon);
            ItemUtility.ItemsToUpdate.Add(ballista);
            ItemUtility.ItemsToUpdate.Add(crossBow);
            ItemUtility.ItemsToUpdate.Add(arrows);
            ItemUtility.ItemsToUpdate.Add(pickaxe);
            //ItemUtility.ItemsToUpdate.Add(pistolAmmo);
            ItemUtility.ItemsToUpdate.Add(cannonAmmo);
            ItemUtility.ItemsToUpdate.Add(harpoonAmmo);
            ItemUtility.ItemsToUpdate.Add(basePlank);
            ItemUtility.ItemsToUpdate.Add(campfire);

            UpdateOrder.Add(shortShip);
            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(player);
            UpdateOrder.Add(baseTribalLand);
            UpdateOrder.Add(baseCatLand);
            UpdateOrder.Add(chickenLand);
            UpdateOrder.Add(blueBird);
            UpdateOrder.Add(snakeLand);
            UpdateOrder.Add(tower);
            UpdateOrder.Add(teePee);

            // interior set
            BaseTribal baseTribalInShip = new BaseTribal(TeamType.A, "GustoMap", Vector2.Zero, _content, _graphics);
            baseTribalInShip.npcInInterior = baseShipAI.shipInterior;
            baseShipAI.shipInterior.interiorObjects.Add(baseTribalInShip);

            ready = true;
        }

        // Runs the in game update for objets that need to save state
        public HashSet<Sprite> Update (KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            List<Sprite> toRemove = new List<Sprite>();
            HashSet<Sprite> droppedItemObjectUpdateOrder = new HashSet<Sprite>();

            // camera follows player
            if (!player.onShip || player.playerInInterior != null)
                camera.Position = player.location;
            else
                camera.Position = player.playerOnShip.location;

            // add any dropped/onGround items in the world (and placable items)
            foreach (var item in ItemUtility.ItemsToUpdate)
                droppedItemObjectUpdateOrder.Add(item);

            HashSet<Sprite> fullOrder = UpdateOrder;
            fullOrder.UnionWith(droppedItemObjectUpdateOrder);

            foreach (Sprite sp in fullOrder)
            {
                if (sp.remove)
                {
                    toRemove.Add(sp);
                    ItemUtility.ItemsToUpdate.Remove(sp);
                }

                // ICanUpdate is the update for main sprites. Any sub-sprites (items, weapons, sails, etc) that belong to the main sprite are updated within the sprite's personal update method. 
                ICanUpdate updateSp = (ICanUpdate)sp;
                updateSp.Update(kstate, gameTime, camera);
            }

            // keep updates running for anything(non player) in an interior 
            foreach (Interior inside in BoundingBoxLocations.interiorMap.Values)
            {
                inside.Update(kstate, gameTime, camera);
            }

            // clear any "dead" or picked up objects from updating
            foreach (var r in toRemove)
                fullOrder.Remove(r);

            return fullOrder;
        }

        public void SaveGameState()
        {
            // Create the save state
            List<ISaveState> SaveState = new List<ISaveState>();
            Dictionary<Interior, Guid> interiorMap = new Dictionary<Interior, Guid>();

            // save all interior states
            foreach (var interior in BoundingBoxLocations.interiorMap)
            {
                InteriorState its = SerializeInterior(interior.Value);
                interiorMap.Add(interior.Value, interior.Key);
                SaveState.Add(its);
            }

            // save the world state
            foreach (Sprite sp in UpdateOrder)
            {

                // PLAYER
                if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.PlayerPirate))
                {
                    PlayerState state = new PlayerState();
                    state.team = player.teamType;
                    state.location = player.location;
                    state.interiorEntranceLocation = player.entranceLoc;
                    state.region = player.regionKey;
                    state.inventory = SerializeInventory(player.inventory);
                    state.onShip = player.onShip;
                    // set the player in their interior if they save while on ship
                    if (player.onShip)
                        player.playerInInterior = player.playerOnShip.shipInterior;

                    if (player.playerInInterior != null)
                    {
                        state.playerInInteriorId = interiorMap[player.playerInInterior]; // interiorMap should be full since we ran interiors before this
                    }
                    else
                        state.playerInInteriorId = Guid.Empty;

                    state.inHandItemKey = player.inHand.itemKey;
                    state.health = player.health;
                    SaveState.Add(state);
                }

                else if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.Npc))
                {
                    Npc npc = (Npc)sp;
                    NpcState state = new NpcState();
                    state.team = npc.teamType;
                    state.location = npc.location;
                    state.objKey = npc.bbKey;
                    state.region = npc.regionKey;
                    state.inventory = SerializeInventory(npc.inventory);
                    state.onShip = npc.onShip;

                    state.npcInInteriorId = Guid.Empty;
                    state.health = npc.health;
                    SaveState.Add(state);
                }

                else if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship sh = (Ship)sp;
                    ShipState state = new ShipState();
                    state.team = sh.teamType;
                    state.location = sh.location;
                    state.region = sh.regionKey;
                    state.objKey = sh.bbKey;
                    state.actionInventory = SerializeInventory(sh.actionInventory);
                    state.playerAboard = sh.playerAboard;
                    state.anchored = sh.anchored;
                    state.health = sh.health;
                    state.shipId = sh.GetInteriorForId();
                    SaveState.Add(state);
                }

                else if (sp.GetType().BaseType == typeof(Gusto.Models.Animated.Structure))
                {
                    Structure st = (Structure)sp;
                    StructureState state = new StructureState();
                    state.team = st.teamType;
                    state.location = st.location;
                    state.region = st.regionKey;
                    state.objKey = st.bbKey;
                    state.structureId = st.GetInteriorForId();
                    SaveState.Add(state);
                }
            }

            // All objs on ground
            foreach (var item in ItemUtility.ItemsToUpdate)
            {
                OnGroundState ogs = new OnGroundState();
                ogs.objKey = item.bbKey;
                ogs.region = item.regionKey;
                ogs.team = TeamType.Gusto;
                ogs.location = item.location;
                if (item is IInventoryItem)
                {
                    ogs.inventoryItem = true;
                    InventoryItem ii = (InventoryItem)item;
                    ogs.amountStacked = ii.amountStacked;
                }
                else
                    ogs.inventoryItem = false;
                if (item is IStorage)
                {
                    Storage storage = (Storage)item;
                    ogs.inventory = SerializeInventory(storage.inventory);
                }
                else if (item is IContainer)
                {
                    Container cont = (Container)item;
                    ogs.inventory = SerializeInventory(cont.drops);
                }

                SaveState.Add(ogs);
            }

            // Can do the weather state separately becasue it is not in the updateOrder
            WeatherSaveState wss = new WeatherSaveState();
            wss.currentMsOfDay = WeatherState.currentMsOfDay;
            wss.currentLightIntensity = WeatherState.currentLightIntensity;
            wss.sunAngleX = WeatherState.sunAngleX;
            wss.shadowTransparency = WeatherState.shadowTransparency;
            wss.nDays = WeatherState.totalDays;
            wss.weatherDuration = WeatherState.weatherDuration;
            wss.msThroughWeather = WeatherState.msThroughWeather;
            wss.rainState = WeatherState.rainState;
            wss.rainIntensity = WeatherState.rainIntensity;
            wss.lightning = WeatherState.lightning;
            SaveState.Add(wss);



            // serialize save to file system
            DataContractSerializer s = new DataContractSerializer(typeof(List<ISaveState>));
            using (FileStream fs = new FileStream(savePath + "GustoGame_" + gameName, FileMode.Create))
            {
                s.WriteObject(fs, SaveState);
            }
        }

        public void LoadGameState()
        {
            Type[] deserializeTypes = new Type[] { typeof(ShipState), typeof(PlayerState), typeof(WeatherSaveState), typeof(NpcState), typeof(OnGroundState), typeof(InteriorState), typeof(StructureState)  };
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
            Dictionary<Guid, Interior> interiorForObjMap = new Dictionary<Guid, Interior>(); // key is the interiorObjFor and val is the interior that belongs to this obj

            foreach (ISaveState objState in LoadFromState)
            {
                // Interiors are deserialized first since they were serialized first
                if (objState.GetType() == typeof(InteriorState))
                {
                    InteriorState its = (InteriorState)objState;
                    Interior i = DeserializeInterior(its);
                    interiorForObjMap.Add(its.interiorForId, i);
                    BoundingBoxLocations.interiorMap.Add(i.interiorId, i);
                }

                if (objState.GetType() == typeof(PlayerState))
                {
                    PlayerState ps = (PlayerState)objState;
                    player.location = ps.location;
                    player.inventory = DeserializeInventory(ps.inventory);
                    player.inHand = (HandHeld)DeserializeInventoryItem(ps.inHandItemKey);

                    if (ps.playerInInteriorId == Guid.Empty)
                    {
                        player.playerOnShip = null;
                        player.playerInInterior = null;
                    }
                    else
                    {
                        player.entranceLoc = ps.interiorEntranceLocation;
                        // check if the interior already exists
                        foreach (KeyValuePair<Guid, Interior> interior in BoundingBoxLocations.interiorMap)
                        {
                            if (ps.playerInInteriorId == interior.Key)
                            {
                                player.playerInInterior = interior.Value;
                                break;
                            }
                        }
                    }

                    player.onShip = ps.onShip;
                    player.regionKey = ps.region;
                    player.health = ps.health;
                    UpdateOrder.Add(player);
                }

                else if (objState.GetType() == typeof(NpcState))
                {
                    NpcState npcs = (NpcState)objState;
                    Npc npc = (Npc)DeserializeModel(npcs.objKey, npcs);
                    npc.location = npcs.location;
                    npc.inventory = DeserializeInventory(npcs.inventory);

                    if (npcs.npcInInteriorId == Guid.Empty)
                        npc.npcInInterior = null;
                    else
                    {
                        // THIS SHOULD NEVER RUN BECAUSE NPCS THAT ARE IN INTERIORS ARE DESERIALIZED ELSEWHERE

                        // check if the interior already exists
                        foreach (KeyValuePair<Guid, Interior> interior in BoundingBoxLocations.interiorMap)
                        {
                            if (npcs.npcInInteriorId == interior.Key)
                            {
                                npc.npcInInterior = interior.Value;
                                break;
                            }
                        }
                    }

                    npc.onShip = npcs.onShip;
                    npc.regionKey = npcs.region;
                    npc.health = npcs.health;
                    UpdateOrder.Add(npc);
                }

                else if (objState.GetType() == typeof(ShipState))
                {
                    ShipState ss = (ShipState)objState;
                    Ship s = (Ship)DeserializeModel(ss.objKey, ss);
                    UpdateOrder.Add(s);
                }

                else if (objState.GetType() == typeof(StructureState))
                {
                    StructureState ss = (StructureState)objState;
                    Structure s = (Structure)DeserializeModel(ss.objKey, ss);
                    UpdateOrder.Add(s);
                }

                else if (objState.GetType() == typeof(WeatherSaveState))
                {
                    WeatherSaveState wss = (WeatherSaveState)objState;
                    WeatherState.currentLightIntensity = wss.currentLightIntensity;
                    WeatherState.currentMsOfDay = wss.currentMsOfDay;
                    WeatherState.sunAngleX = wss.sunAngleX;
                    WeatherState.shadowTransparency = wss.shadowTransparency;
                    WeatherState.totalDays = wss.nDays;
                    WeatherState.weatherDuration = wss.weatherDuration;
                    WeatherState.msThroughWeather = wss.msThroughWeather;
                    WeatherState.rainState = wss.rainState;
                    WeatherState.rainIntensity = wss.rainIntensity;
                    WeatherState.lightning = wss.lightning;

                    // set the rain
                    for(int i = 0; i < WeatherState.rainIntensity; i++)
                        WeatherState.rain.Add(new RainDrop(_content, _graphics));
                }

                else if (objState.GetType() == typeof(OnGroundState))
                {
                    OnGroundState ogs = (OnGroundState)objState;
                    Sprite sp = null;
                    if (ogs.inventoryItem)
                    {
                        InventoryItem ii = DeserializeInventoryItem(ogs.objKey);
                        ii.amountStacked = ogs.amountStacked;
                        ii.onGround = true;
                        sp = ii;
                    }
                    else
                    {
                        sp = DeserializeModel(ogs.objKey, objState);
                    }
                    sp.location = ogs.location;
                    sp.regionKey = ogs.region;

                    ItemUtility.ItemsToUpdate.Add(sp);
                }
            }

            // go through newly created world state and set any interiors for the object that owns that interior
            foreach(Sprite sp in UpdateOrder)
            {
                if (sp is IHasInterior)
                {
                    IHasInterior hasInterior = (IHasInterior)sp;
                    Guid interiorForId = hasInterior.GetInteriorForId();

                    if (sp is IShip)
                    {
                        Ship sh = (Ship)sp;
                        sh.shipInterior = interiorForObjMap[interiorForId];
                    }
                    else if (sp is IStructure)
                    {
                        Structure st = (Structure)sp;
                        st.structureInterior = interiorForObjMap[interiorForId];
                    }

                    interiorForObjMap[interiorForId].interiorForObj = sp;
                }
            }

            // set player's current ship, the interior will be the ships interior
            if (player.onShip)
                player.playerOnShip = (Ship)BoundingBoxLocations.interiorMap[player.playerInInterior.interiorId].interiorForObj;

        }

        private Sprite DeserializeModel(string objKey, ISaveState objSave)
        {
            // Add base sprite models here, (AnimatedSprite namespace, not Model namespace)
            Sprite sp = null;
            OnGroundState ogs;
            ShipState ss;
            NpcState npcs;
            StructureState sts;
            switch (objKey)
            {
                // TODO: Should I save fired ammo state?
                
                case "baseShip":
                    ss = (ShipState)objSave;
                    BaseShip bs = new BaseShip(ss.team, ss.region, ss.location, _content, _graphics);
                    bs.health = ss.health;
                    bs.actionInventory = DeserializeInventory(ss.actionInventory);
                    //bs.shipInterior = DeserializeInterior(ss.interiorState); we do this after models are created
                    bs.playerAboard = ss.playerAboard;
                    bs.SetInteriorForId(ss.shipId);
                    if (bs.playerAboard)
                        bs.shipSail.playerAboard = true;
                    return bs;

                case "baseTribal":
                    npcs = (NpcState)objSave;
                    BaseTribal bt = new BaseTribal(npcs.team, npcs.region, npcs.location, _content, _graphics);
                    bt.health = npcs.health;
                    bt.inventory = DeserializeInventory(npcs.inventory);
                    return bt;

                case "baseCat":
                    npcs = (NpcState)objSave;
                    BaseCat bct = new BaseCat(npcs.team, npcs.region, npcs.location, _content, _graphics);
                    bct.health = npcs.health;
                    bct.inventory = DeserializeInventory(npcs.inventory);
                    return bct;

                case "chicken":
                    npcs = (NpcState)objSave;
                    Chicken chk = new Chicken(npcs.team, npcs.region, npcs.location, _content, _graphics);
                    chk.health = npcs.health;
                    chk.inventory = DeserializeInventory(npcs.inventory);
                    return chk;

                case "blueBird":
                    npcs = (NpcState)objSave;
                    BlueBird bbr = new BlueBird(npcs.team, npcs.region, npcs.location, _content, _graphics);
                    bbr.health = npcs.health;
                    bbr.inventory = DeserializeInventory(npcs.inventory);
                    return bbr;

                case "snake":
                    npcs = (NpcState)objSave;
                    Snake snk = new Snake(npcs.team, npcs.region, npcs.location, _content, _graphics);
                    snk.health = npcs.health;
                    snk.inventory = DeserializeInventory(npcs.inventory);
                    return snk;

                case "teePee":
                    sts = (StructureState)objSave;
                    TeePee tp = new TeePee(sts.team, sts.region, sts.location, _content, _graphics);
                    return tp;

                case "baseBarrel":
                    ogs = (OnGroundState)objSave;
                    BaseBarrel bb = new BaseBarrel(ogs.team, ogs.region, ogs.location, _content, _graphics);
                    bb.drops = DeserializeInventory(ogs.inventory);
                    return bb;

                case "baseChest":
                    ogs = (OnGroundState)objSave;
                    BaseChest bc = new BaseChest(ogs.team, ogs.region, ogs.location, _content, _graphics);
                    bc.inventory = DeserializeInventory(ogs.inventory);
                    return bc;

                case "clayFurnace":
                    ogs = (OnGroundState)objSave;
                    ClayFurnace cf = new ClayFurnace(ogs.team, ogs.region, ogs.location, _content, _graphics);
                    return cf;

                case "campFire":
                    ogs = (OnGroundState)objSave;
                    CampFire caf = new CampFire(ogs.team, ogs.region, ogs.location, _content, _graphics);
                    return caf;

                case "craftingAnvil":
                    ogs = (OnGroundState)objSave;
                    CraftingAnvil ca = new CraftingAnvil(ogs.team, ogs.region, ogs.location, _content, _graphics);
                    return ca;

            }
            return sp;
        }

        private InventoryItem DeserializeInventoryItem(string itemKey)
        {
            InventoryItem item = null;
            switch (itemKey)
            {
                // TODO: ALL THE ITEMS :( 
                case "scales":
                    return new Scales(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "feather":
                    return new Feather(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "fishOil":
                    return new FishOil(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "goldCoins":
                    return new GoldCoins(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "chiliFish":
                    return new ChiliFish(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "chiliPepper":
                    return new ChiliPepper(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "cookedFish":
                    return new CookedFish(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "cookedMeat":
                    return new CookedMeat(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "rawFish":
                    return new RawFish(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "rawMeat":
                    return new RawMeat(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "spoiledFish":
                    return new SpoiledFish(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
                case "spoiledMeat":
                    return new SpoiledMeat(TeamType.GroundObject, "GustoMap", Vector2.Zero, _content, _graphics);
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
                case "cannonBallItem":
                    return new CannonBallItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "arrowItem":
                    return new ArrowItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "rustyHarpoonItem":
                    return new RustyHarpoonItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "coal":
                    return new Coal(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "ironBar":
                    return new IronBar(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "tribalTokens":
                    return new TribalTokens(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "ironOre":
                    return new IronOre(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "nails":
                    return new Nails(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pickaxe":
                    return new Pickaxe(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pistol":
                    return new Pistol(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "baseCannon":
                    return new BaseCannon(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "ballista":
                    return new Ballista(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "crossBow":
                    return new CrossBow(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "pistolShotItem":
                    return new PistolShotItem(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "shortSword":
                    return new ShortSword(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                case "shovel":
                    return new Shovel(TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
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

        private Interior DeserializeInterior(InteriorState interiorSaveState)
        {
            // deserialize the entire interior and its objects
            Interior i = new Interior(interiorSaveState.interiorTypeKey, null, _content, _graphics); // interiorForObj will be set after everything else has been deserialized
            i.interiorId = interiorSaveState.interiorId;
            i.startDrawPoint = interiorSaveState.location;
            i.interiorObjects = DeserializeInteriorObjects(interiorSaveState.interiorObjs, i);
            i.interiorWasLoaded = true;
            return i;
        }


        private HashSet<Sprite> DeserializeInteriorObjects(List<InteriorObjectSerialized> interiorObjsSaved, Interior interior)
        {
            HashSet<Sprite> ret = new HashSet<Sprite>();
            foreach(InteriorObjectSerialized objSave in interiorObjsSaved)
            {
                Sprite sp = null;
                if (objSave.groundObj)
                {
                    OnGroundState ogs = (OnGroundState)objSave.saveState;
                    sp = DeserializeModel(ogs.objKey, objSave.saveState);
                    sp.location = ogs.location;
                    sp.regionKey = ogs.region;
                }
                else if (objSave.npcObj)
                {
                    NpcState npcs = (NpcState)objSave.saveState;
                    Npc npc = (Npc)DeserializeModel(npcs.objKey, npcs);
                    npc.location = npcs.location;
                    npc.inventory = DeserializeInventory(npcs.inventory);
                    npc.regionKey = npcs.region;
                    npc.onShip = npcs.onShip;
                    npc.health = npcs.health;
                    npc.npcInInterior = interior;
                    sp = npc;
                }
                ret.Add(sp);
            }
            return ret;
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

                // special cases
                if (item.treasureMaps != null)
                {
                    Storage reward = null;
                    if (item.treasureMaps[index].reward != null)
                    {
                        reward = (Storage)DeserializeInventoryItem(item.treasureMaps[index].storageType).placeableVersion;
                        reward.inventory = DeserializeInventory(item.treasureMaps[index].reward);
                    }
                    TreasureMapItem tm = new TreasureMapItem(reward, TeamType.Gusto, "GustoMap", Vector2.Zero, _content, _graphics);
                    tm.digTileLoc = item.treasureMaps[index].digLocation;
                    tm.treasureInRegion = item.treasureMaps[index].treasureInRegion;
                    ret[index] = (InventoryItem)tm;
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
                    Storage store = (Storage)ii.placeableVersion;
                    store.inventory = DeserializeInventory(item.storageItems[index]);
                }

                ret[index] = ii;
                index++;
            }
            return ret;
        }


        private InteriorState SerializeInterior(Interior interior)
        {
            InteriorState ins = new InteriorState();
            ins.interiorId = interior.interiorId;
            ins.interiorForId = interior.GetInteriorForId();
            ins.location = interior.startDrawPoint;
            ins.interiorTypeKey = interior.interiorTypeKey;
            ins.interiorObjs = SerializeInteriorObjects(interior.interiorObjects.ToList(), interior.interiorId);
            return ins;
        }

        private List<InteriorObjectSerialized> SerializeInteriorObjects(List<Sprite> interiorObjs, Guid interiorId)
        {
            List<InteriorObjectSerialized> ret = new List<InteriorObjectSerialized>();
            foreach (Sprite obj in interiorObjs)
            {
                InteriorObjectSerialized ios = new InteriorObjectSerialized();
                if (obj is IPlayer)
                    continue; // don't save player as part of interior state, player is done seperatly

                if (obj is INPC)
                {
                    NpcState state = new NpcState();
                    Npc npc = (Npc)obj;
                    state.team = npc.teamType;
                    state.location = npc.location;
                    state.objKey = npc.bbKey;
                    state.region = npc.regionKey;
                    state.inventory = SerializeInventory(npc.inventory);
                    state.onShip = npc.onShip;
                    state.health = npc.health;
                    state.npcInInteriorId = interiorId;

                    ios.npcObj = true;
                    ios.saveState = state;
                    ret.Add(ios);
                }
                else
                {
                    OnGroundState ogs = new OnGroundState();
                    ogs.objKey = obj.bbKey;
                    ogs.region = obj.regionKey;
                    ogs.team = TeamType.Gusto;
                    ogs.location = obj.location;

                    if (obj is IStorage)
                    {
                        Storage storage = (Storage)obj;
                        ogs.inventory = SerializeInventory(storage.inventory);
                    }
                    else if (obj is IContainer)
                    {
                        Container cont = (Container)obj;
                        ogs.inventory = SerializeInventory(cont.drops);
                    }
                    ios.groundObj = true;
                    ios.saveState = ogs;
                    ret.Add(ios);
                }
            }
            return ret;
        }

        private List<InventoryItemSerialized> SerializeInventory(List<InventoryItem> inv)
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
                    TreasureMapItemSerialized tms = new TreasureMapItemSerialized();
                    TreasureMap m = (TreasureMap)item;
                    tms.digLocation = m.digTileLoc;
                    tms.treasureInRegion = m.treasureInRegion;
                    if (m.rewarded != null)
                    {
                        tms.storageType = m.storageTierType;
                        tms.reward = SerializeInventory(m.rewarded.inventory); // risk of infite loop recusion, but only if you bury treasure maps in storage that you have a map for. It shouuuuld reach a base case.. lol
                    }
                         
                    iszd.treasureMaps.Add(index, tms);
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
                            storageInventorySzd[indexStorage] = null;
                            indexStorage++;
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
                            tm.digLocation = m.digTileLoc;
                            tm.treasureInRegion = m.treasureInRegion;
                            if (tm.reward != null)
                                tm.reward = SerializeInventory(m.rewarded.inventory); // risk of infite loop recusion, but only if you bury treasure maps in storage that you have a map for. It shouuuuld reach a base case.. lol
                            stItemSzd.treasureMaps.Add(indexStorage, tm);
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
