using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Bounds;
using Gusto.Models;
using Gusto.Models.Interfaces;
using Gusto.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Utility;
using Gusto.Models.Menus;
using Gusto.Models.Animated;
using Gusto.Models.Menus;
using Gusto.GameMap.lightning;

namespace Gusto
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameGusto : Game
    {

        // TEMPORARY -- expose the "players and enemies". 
        //BaseShip baseShip;
        //BaseShip baseShipAI;
        BaseTower tower;
        PiratePlayer piratePlayer;
        BaseTribal baseTribal;
        Pistol pistol;
        Shovel shovel;
        PistolShotItem pistolAmmo;
        CannonBallItem cannonAmmo;
        BasePlank basePlank;
        Pickaxe pickaxe;
        Lantern lantern;
        ClayFurnace furnace;
        CraftingAnvil craftingAnvil;
        BaseBarrel barrelLand;
        BaseBarrel barrelOcean;
        BaseChest chestLand;
        BaseChest chestOcean;

        TileGameMap map;
        JObject mapData;

        // static
        WindArrows windArrows;
        Texture2D anchorIcon;
        Texture2D repairIcon;
        Texture2D treasureXMark;
        Inventory inventoryMenu;
        CraftingMenu craftingMenu;
        OpenGameMenu startingMenu;

        GraphicsDeviceManager graphics;
        FrameCounter _frameCounter;
        RenderTarget2D gameScene;
        RenderTarget2D ambientLight;
        RenderTarget2D lightsTarget;

        DayLight dayLight;
        Weather weather;

        GameState gameState;
        SpatialBounding collision;
        List<Sprite> DrawOrder;
        List<Sprite> Collidable;
        HashSet<Sprite> UpdateOrder;
        SpriteFont font;
        SpriteBatch spriteBatchView;
        SpriteBatch spriteBatchStatic;
        Camera camera;
        
        public GameGusto()
        {
            _frameCounter = new FrameCounter();
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GameOptions.PrefferedBackBufferWidth;
            graphics.PreferredBackBufferHeight = GameOptions.PrefferedBackBufferHeight;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = new GameState(Content, GraphicsDevice);
            DrawOrder = new List<Sprite>();
            Collidable = new List<Sprite>();
            UpdateOrder = new HashSet<Sprite>();
            this.camera = new Camera(GraphicsDevice);
            collision = new SpatialBounding(new Rectangle(0, 0, GameOptions.PrefferedBackBufferWidth, GameOptions.PrefferedBackBufferHeight), this.camera);
            map = new TileGameMap(this.camera);

            dayLight = new DayLight(Content, GraphicsDevice);
            weather = new Weather(Content, GraphicsDevice);
            lightsTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            ambientLight = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            gameScene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            mapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\gamemap.json"));
            map.LoadMapData(mapData);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatchView = new SpriteBatch(GraphicsDevice);
            spriteBatchStatic = new SpriteBatch(GraphicsDevice);

            // PREPROCESSING Bounding Sprites
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(true, 8, 1, textureBaseShip, "baseShip", 0.6f, 1.0f);
            Texture2D texturePlayerPirate = Content.Load<Texture2D>("Pirate1-combat");
            LoadDynamicBoundingBoxPerFrame(false, 4, 11, texturePlayerPirate, "playerPirate", 1.0f, 1.0f);
            Texture2D textureBaseTribal = Content.Load<Texture2D>("Tribal1");
            LoadDynamicBoundingBoxPerFrame(false, 4, 12, textureBaseTribal, "baseTribal", 1.0f, 1.0f);
            Texture2D textureBaseSword = Content.Load<Texture2D>("BaseSword");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureBaseSword, "baseSword", 1.0f, 1.0f);
            Texture2D texturePistol = Content.Load<Texture2D>("pistol");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, texturePistol, "pistol", 1.0f, 1.0f);
            Texture2D texturePickaxe = Content.Load<Texture2D>("pickaxe");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, texturePickaxe, "pickaxe", 1.0f, 1.0f);
            Texture2D textureShortSword = Content.Load<Texture2D>("ShortSword");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureShortSword, "shortSword", 1.0f, 1.0f);
            Texture2D textureShovel = Content.Load<Texture2D>("Shovel");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureShovel, "shovel", 1.0f, 1.0f);
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(false, 8, 3, textureBaseSail, "baseSail", 0.6f, 1.0f);
            Texture2D textureTower = Content.Load<Texture2D>("tower");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureTower, "tower", 0.5f, 1.0f);
            Texture2D textureCannonBall = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureCannonBall, "baseCannonBall", 1.0f, 1.0f);
            Texture2D textureCannonBallItem = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureCannonBallItem, "cannonBallItem", 1.0f, 1.0f);
            Texture2D texturePistolShot = Content.Load<Texture2D>("PistolShot");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, texturePistolShot, "pistolShot", 1.0f, 1.0f);
            Texture2D texturePistolShotItem = Content.Load<Texture2D>("PistolShot");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, texturePistolShotItem, "pistolShotItem", 1.0f, 1.0f);
            Texture2D textureBaseCannon = Content.Load<Texture2D>("BaseCannon");
            LoadDynamicBoundingBoxPerFrame(false, 8, 1, textureBaseCannon, "baseCannon", 1.0f, 1.0f);
            Texture2D textureLantern = Content.Load<Texture2D>("Lantern");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureLantern, "lantern", 1.0f, 1.0f);
            Texture2D textureBarrel = Content.Load<Texture2D>("Barrel");
            LoadDynamicBoundingBoxPerFrame(false, 2, 3, textureBarrel, "baseBarrel", 0.5f, 1.0f);
            Texture2D textureChest = Content.Load<Texture2D>("Barrel");
            LoadDynamicBoundingBoxPerFrame(false, 2, 3, textureChest, "baseChest", 0.5f, 1.0f);

            Texture2D textureClayFurnace = Content.Load<Texture2D>("Furnace");
            LoadDynamicBoundingBoxPerFrame(false, 1, 6, textureClayFurnace, "clayFurnace", 0.5f, 1.0f);
            Texture2D textureCraftingAnvil = Content.Load<Texture2D>("Anvil");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureCraftingAnvil, "craftingAnvil", 0.5f, 1.0f);

            // Tile Pieces, Ground Objects and Invetory Items
            Texture2D textureOcean1 = Content.Load<Texture2D>("Ocean1");
            LoadDynamicBoundingBoxPerFrame(false, 4, 1, textureOcean1, "oceanTile", 1.0f, 1.0f);
            Texture2D textureLand1 = Content.Load<Texture2D>("Land1Holes");
            LoadDynamicBoundingBoxPerFrame(false, 4, 4, textureLand1, "landTile", 1.0f, 1.0f);
            Texture2D textureTree1 = Content.Load<Texture2D>("Tree1");
            LoadDynamicBoundingBoxPerFrame(true, 2, 6, textureTree1, "tree1", 0.4f, 1.0f);
            Texture2D textureSoftWood = Content.Load<Texture2D>("softwoodpile");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureSoftWood, "softWood", 0.5f, 1.0f);
            Texture2D textureGrass1 = Content.Load<Texture2D>("Grass1");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureGrass1, "grass1", 1.5f, 1.0f);
            Texture2D textureRock1 = Content.Load<Texture2D>("Rock1");
            LoadDynamicBoundingBoxPerFrame(false, 2, 4, textureRock1, "rock1", 0.3f, 1.0f);
            Texture2D textureIslandGrass = Content.Load<Texture2D>("islandGrass");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureIslandGrass, "islandGrass", 0.5f, 1.0f);
            Texture2D textureCoal = Content.Load<Texture2D>("coal");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureCoal, "coal", 1.0f, 1.0f);
            Texture2D textureNails = Content.Load<Texture2D>("Nails");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureNails, "nails", 0.4f, 1.0f);
            Texture2D textureIronOre = Content.Load<Texture2D>("IronOre");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureIronOre, "ironOre", 1.0f, 1.0f);
            Texture2D textureIronBar = Content.Load<Texture2D>("IronBar");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureIronBar, "ironBar", 1.0f, 1.0f);
            Texture2D textureTribalTokens = Content.Load<Texture2D>("TribalTokens");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureTribalTokens, "tribalTokens", 0.5f, 1.0f);
            Texture2D textureBasePlank = Content.Load<Texture2D>("plank");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureBasePlank, "basePlank", 0.5f, 1.0f);
            Texture2D textureClayFurnaceItem = Content.Load<Texture2D>("Furnace");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureClayFurnaceItem, "clayFurnaceItem", 1.0f, 1.0f);
            Texture2D textureAnvilItem = Content.Load<Texture2D>("Furnace");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureAnvilItem, "anvilItem", 1.0f, 1.0f);
            Texture2D textureBarrelItem = Content.Load<Texture2D>("Barrel");
            LoadDynamicBoundingBoxPerFrame(false, 2, 3, textureBarrelItem, "baseBarrelItem", 1.0f, 1.0f);
            Texture2D textureChestItem = Content.Load<Texture2D>("BaseChest");
            LoadDynamicBoundingBoxPerFrame(false, 2, 3, textureChestItem, "baseChestItem", 1.0f, 1.0f);
            Texture2D textureTreasureMap = Content.Load<Texture2D>("TreasureMap");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureTreasureMap, "treasureMapItem", 0.5f, 1.0f);

            // Game Map
            map.SetGameMap(Content, GraphicsDevice);
            BuildRegionTree();

            //TEMPORARY NEED TO CREATE SOME SORT OF GAME SETUP / REGION SETUP
            List<Sprite> giannaLandTiles = BoundingBoxLocations.RegionMap["Gianna"].RegionLandTiles;
            Sprite GiannaRegionTile = giannaLandTiles[RandomEvents.rand.Next(giannaLandTiles.Count)];

            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            // static load
            anchorIcon = Content.Load<Texture2D>("anchor-shape");
            repairIcon = Content.Load<Texture2D>("work-hammer-");
            treasureXMark = Content.Load<Texture2D>("XSpot");
            font = Content.Load<SpriteFont>("helperFont");

            windArrows = new WindArrows(new Vector2(1740, 50), Content, GraphicsDevice);
            WeatherState.wind = windArrows;

            // TEMPORARY create Team models and initally place them - this will eventually be set in game config menu

            //baseShip = new BaseShip(TeamType.Player, "GustoMap", new Vector2(-100, -500), windArrows, Content, GraphicsDevice);

            piratePlayer = new PiratePlayer(TeamType.Player, "GustoMap", new Vector2(0, -300), Content, GraphicsDevice);
            baseTribal = new BaseTribal(TeamType.B, "Gianna", GiannaRegionTile.location, Content, GraphicsDevice);
            tower = new BaseTower(TeamType.A, "GustoMap", new Vector2(200, 700), Content, GraphicsDevice);

            //baseShipAI = new BaseShip(TeamType.A, "GustoMap", new Vector2(470, 0), windArrows, Content, GraphicsDevice);

            furnace = new ClayFurnace(TeamType.Player, "GustoMap", new Vector2(180, 140), Content, GraphicsDevice);
            craftingAnvil = new CraftingAnvil(TeamType.Player, "GustoMap", new Vector2(120, 40), Content, GraphicsDevice);
            barrelLand = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(-20, -160), Content, GraphicsDevice);
            barrelOcean = new BaseBarrel(TeamType.A, "GustoMap", new Vector2(380, -60), Content, GraphicsDevice);
            chestLand = new BaseChest(TeamType.A, "GustoMap", new Vector2(100, -120), Content, GraphicsDevice);
            chestOcean = new BaseChest(TeamType.A, "GustoMap", new Vector2(350, 0), Content, GraphicsDevice);

            shovel = new Shovel(TeamType.A, "GustoMap", new Vector2(200, -330), Content, GraphicsDevice);
            shovel.onGround = true;
            pickaxe = new Pickaxe(TeamType.Player, "GustoMap", new Vector2(130, -430), Content, GraphicsDevice);
            pickaxe.onGround = true;
            pistol = new Pistol(TeamType.A, "GustoMap", new Vector2(250, -300), Content, GraphicsDevice);
            pistol.amountStacked = 1;
            pistol.onGround = true;
            pistolAmmo = new PistolShotItem(TeamType.A, "GustoMap", new Vector2(220, -300), Content, GraphicsDevice);
            pistolAmmo.amountStacked = 14;
            pistolAmmo.onGround = true;
            cannonAmmo = new CannonBallItem(TeamType.A, "GustoMap", new Vector2(200, -300), Content, GraphicsDevice);
            cannonAmmo.amountStacked = 10;
            cannonAmmo.onGround = true;
            lantern = new Lantern(TeamType.A, "GustoMap", new Vector2(180, -300), Content, GraphicsDevice);
            lantern.onGround = true;
            basePlank = new BasePlank(TeamType.A, "GustoMap", new Vector2(150, -300), Content, GraphicsDevice);
            basePlank.onGround = true;
            basePlank.amountStacked = 10;

            // static init (MENUS)
            inventoryMenu = new Inventory(screenCenter, Content, GraphicsDevice, piratePlayer);
            craftingMenu = new CraftingMenu(screenCenter, Content, GraphicsDevice, piratePlayer);
            startingMenu = new OpenGameMenu(Content, GraphicsDevice);

            // fill update order list
            //UpdateOrder.Add(baseShip);
            UpdateOrder.Add(piratePlayer);
            UpdateOrder.Add(baseTribal);
            //UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(tower);
            UpdateOrder.Add(pistol);
            UpdateOrder.Add(pickaxe);
            UpdateOrder.Add(pistolAmmo);
            UpdateOrder.Add(cannonAmmo);
            UpdateOrder.Add(basePlank);
            UpdateOrder.Add(inventoryMenu);
            UpdateOrder.Add(craftingMenu);
            UpdateOrder.Add(lantern);
            UpdateOrder.Add(furnace);
            UpdateOrder.Add(craftingAnvil);
            UpdateOrder.Add(barrelLand);
            UpdateOrder.Add(barrelOcean);
            UpdateOrder.Add(chestLand);
            UpdateOrder.Add(chestOcean);
            UpdateOrder.Add(shovel);

        }

        // creates dynamic bounding boxes for each sprite frame
        private void LoadDynamicBoundingBoxPerFrame(bool polygon, int rows, int columns, Texture2D source, string key, float scale, float scaleBoundingBox)
        {
            int width = source.Width / columns;
            int height = source.Height / rows;
            BoundingBoxTextures.DynamicBoundingBoxTextures.Add(key, new Dictionary<string, Rectangle>());
            if (polygon)
                BoundingBoxTextures.DynamicBoundingPolygons.Add(key, new Dictionary<string, Polygon>());
            // create custom bounding box for each ship frame (8 rows, 3 columns)
            for (int i = 0; i < columns; i++) // columns
            {
                for (int j = 0; j < rows; j++) // rows
                {
                    Rectangle sourceRectangle = new Rectangle(width * i, height * j, width, height); // x and y here are cords of the texture
                    Texture2D cropTexture = new Texture2D(GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
                    Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
                    source.GetData(0, sourceRectangle, data, 0, data.Length);
                    cropTexture.SetData(data);

                    Polygon poly = null;
                    if (polygon)
                    {
                        poly = new Polygon();
                        poly.Verts = CalculateTextureBoundingBox.CropTextureToPolygon(cropTexture, scale);
                        poly.UpperLeftPoint = Vector2.Zero;
                        BoundingBoxTextures.DynamicBoundingPolygons[key].Add(i.ToString() + j.ToString(), poly);
                    }

                    // store the BB texture 
                    Rectangle bb = CalculateTextureBoundingBox.GetSmallestRectangleFromTexture(cropTexture, scale, scaleBoundingBox);
                    BoundingBoxTextures.DynamicBoundingBoxTextures[key].Add(i.ToString() + j.ToString(), bb);
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary> ESSENTIALLY THE USER INPUT AND PHYSICS DETAILS 
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            List<Sprite> toRemove = new List<Sprite>();
            HashSet<Sprite> tempUpdateOrder = new HashSet<Sprite>();

            // game menu before everything
            if (startingMenu.showMenu)
            {
                startingMenu.Update(kstate, gameTime);
                return;
            }
            else if (!gameState.ready)
            {
                gameState.CreateNewGame();
                return;
            }


            // weather
            weather.Update(kstate, gameTime);

            // daylight shader 
            dayLight.Update(kstate, gameTime);

            // camera follows player
            if (!piratePlayer.onShip)
                this.camera.Position = piratePlayer.location;
            else
                this.camera.Position = piratePlayer.playerOnShip.location;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F4))
                Exit();

            // static update (Wind)
            windArrows.Update(kstate, gameTime, null);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();

            // add any dropped items (and placable items)
            foreach (var item in ItemUtility.ItemsToUpdate)
                UpdateOrder.Add(item);

            // TEMPORARY@#@!$#@
            HashSet<Sprite> TEMPTESTUPDATEORDER = gameState.Update(kstate, gameTime, camera);

            foreach (var s in TEMPTESTUPDATEORDER)
                UpdateOrder.Add(s);

            // main update for all non static objects
            foreach (var sp in UpdateOrder)
            {
                if (sp.remove)
                    toRemove.Add(sp);
                // ICanUpdate is the update for main sprites. Any sub-sprites (items, weapons, sails, etc) that belong to the main sprite are updated within the sprite's personal update method. 
                ICanUpdate updateSp = (ICanUpdate)sp; 
                updateSp.Update(kstate, gameTime, this.camera);
            }

            // clear any "dead" objects from updating
            foreach (var r in toRemove)
                UpdateOrder.Remove(r);

            // reset collidable and drawOrder with "alive" objects and map pieces that are in view
            Collidable.Clear();
            DrawOrder.Clear();
            foreach (var sp in UpdateOrder)
            {
                if (!(sp is IMenuGUI))
                {
                    Collidable.Add(sp);
                    SpatialBounding.SetQuad(sp.GetBase());
                }
                DrawOrder.Add(sp);
            }

            // set any visible collidable map pieces for collision
            foreach (var tile in BoundingBoxLocations.LandTileLocationList)
                SpatialBounding.SetQuad(tile.GetBase());

            // handle collision
            collision.Update(this.camera.Position);
            SpatialCollision();

            // add ground objects to update order
            tempUpdateOrder = UpdateOrder;
            foreach (var obj in BoundingBoxLocations.GroundObjectLocationList)
                tempUpdateOrder.Add(obj);
            UpdateOrder = tempUpdateOrder;

            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // game menu before everything
            if (startingMenu.showMenu)
            {
                startingMenu.DrawInventory(spriteBatchStatic);
                return;
            }


            // setup lightTarget for spot lights
            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            foreach (var light in BoundingBoxLocations.LightLocationList)
                light.Draw(spriteBatchView, this.camera);
            BoundingBoxLocations.LightLocationList.Clear(); // clear after we have drawn the light mask

            // set up gamescene draw
            GraphicsDevice.SetRenderTarget(gameScene);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw map
            map.DrawMap(spriteBatchView, gameTime);

            // draw treasure locations if any
            spriteBatchView.Begin(this.camera);
            foreach (var map in BoundingBoxLocations.treasureLocationsList)
            {
                spriteBatchView.Draw(treasureXMark, map.digTile.location, Color.White);
            }
            spriteBatchView.End();

            // trackers for statically drawn sprites as we move through draw order
            bool showInventoryMenu = false;
            bool showCraftingMenu = false;
            bool showStorageMenu = false;
            bool playerOnShip = false;
            Ship playerShip = null;
            List<InventoryItem> invItemsPlayer = null;
            List<InventoryItem> invItemsShip = null;
            Storage invStorage = null;

            // draw shadows
            foreach (var sprite in DrawOrder)
            {
                if (sprite is IShadowCaster)
                {
                    sprite.DrawShadow(spriteBatchView, camera, dayLight.sunAngleX, dayLight.shadowTransparency);
                    if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                    {
                        Ship ship = (Ship)sprite;
                        ship.shipSail.DrawShadow(spriteBatchView, this.camera, dayLight.sunAngleX, dayLight.shadowTransparency);
                    }
                }
            }

            // sort sprites by y cord asc and draw
            DrawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
            int i = 0;
            foreach (var sprite in DrawOrder)
            {

                if (sprite is IVulnerable)
                {
                    IVulnerable v = (IVulnerable) sprite;
                    v.DrawHealthBar(spriteBatchView, camera);
                }

                if (sprite is IInventoryItem)
                {
                    InventoryItem item = (InventoryItem)sprite;
                    if (!item.inInventory)
                        item.DrawPickUp(spriteBatchView, camera);
                }

                if (sprite is ICraftingObject)
                {
                    ICraftingObject craftObj = (ICraftingObject)sprite;
                    craftObj.DrawCanCraft(spriteBatchView, camera);
                }

                if (sprite is IPlaceable)
                {
                    IPlaceable placeObj = (IPlaceable)sprite;
                    placeObj.DrawCanPickUp(spriteBatchView, camera);
                }

                if (sprite is IStorage)
                {
                    Storage storage = (Storage)sprite;
                    storage.DrawOpenStorage(spriteBatchView, camera);
                    if (storage.storageOpen)
                    {
                        showStorageMenu = true;
                        invStorage = storage;
                    }
                }

                if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship ship = (Ship) sprite;

                    if (ship.sinking)
                    {
                        ship.DrawSinking(spriteBatchView, this.camera);
                        ship.shipSail.DrawSinking(spriteBatchView, this.camera);
                    }
                    else
                    {
                        // Draw a ships sail before a ship
                        ship.Draw(spriteBatchView, this.camera);
                        ship.shipSail.Draw(spriteBatchView, this.camera);
                        foreach (var shot in ship.Shots)
                            shot.Draw(spriteBatchView, this.camera);
                        if (ship.aiming)
                            ship.DrawAimLine(spriteBatchView, this.camera);
                    }
                    continue;
                }

                else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.PiratePlayer))
                {
                    PlayerPirate pirate = (PlayerPirate)sprite;
                    invItemsPlayer = pirate.inventory;

                    if (pirate.showInventory)
                    {
                        showInventoryMenu = true;
                        if (pirate.onShip)
                            invItemsShip = pirate.playerOnShip.inventory;
                    }

                    if (pirate.inCombat && pirate.currRowFrame == 3) // draw sword before pirate when moving up
                        pirate.inHand.Draw(spriteBatchView, this.camera);
                    if (pirate.nearShip)
                        pirate.DrawEnterShip(spriteBatchView, this.camera);
                    else if (pirate.onShip)
                    {
                        pirate.DrawOnShip(spriteBatchView, this.camera);
                        playerShip = pirate.playerOnShip;
                        playerOnShip = true;
                    }

                    if (pirate.swimming && !pirate.onShip)
                        pirate.DrawSwimming(spriteBatchView, this.camera);
                    else if (!pirate.onShip)
                        pirate.Draw(spriteBatchView, this.camera);

                    if (pirate.canBury)
                        pirate.DrawCanBury(spriteBatchView, this.camera);

                    if (pirate.inCombat && pirate.currRowFrame != 3)
                        pirate.inHand.Draw(spriteBatchView, this.camera);

                    foreach (var shot in pirate.inHand.Shots)
                        shot.Draw(spriteBatchView, this.camera);

                    continue;
                }

                else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Anvil))
                {
                    Anvil anvil = (Anvil)sprite;
                    if (anvil.drawCraftingMenu)
                        showCraftingMenu = true;
                }

                else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.GroundEnemy))
                {
                    GroundEnemy enemy = (GroundEnemy)sprite;

                    if (enemy.swimming && !enemy.onShip)
                        enemy.DrawSwimming(spriteBatchView, this.camera);
                    else if (!enemy.onShip)
                        enemy.Draw(spriteBatchView, this.camera);

                    if (enemy.dying)
                        enemy.DrawDying(spriteBatchView, this.camera);
                    continue;
                }

                else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.BaseTower))
                {
                    Tower tower = (Tower) sprite;
                    sprite.Draw(spriteBatchView, this.camera);
                    // draw any shots this tower has in motion
                    foreach (var shot in tower.Shots)
                        shot.Draw(spriteBatchView, this.camera);
                    continue;
                }

                if (sprite.GetType() == typeof(Gusto.Models.Menus.Inventory) || sprite.GetType() == typeof(Gusto.Models.Menus.CraftingMenu))
                    continue; // we handle this after everthing else

                sprite.Draw(spriteBatchView, this.camera);
            }

            // draw weather
            weather.DrawWeather(spriteBatchStatic);

            // lighting shader - for ambient day/night light and lanterns
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);
            dayLight.Draw(spriteBatchStatic, gameScene, lightsTarget);

            // lightning is drawn after ambient light
            if (WeatherState.lightning)
                weather.DrawLightning(spriteBatchStatic);

            // draw static and menu sprites
            windArrows.Draw(spriteBatchStatic, null);
            if (showInventoryMenu)
            {
                inventoryMenu.Draw(spriteBatchStatic, null);
                inventoryMenu.DrawInventory(spriteBatchStatic, invItemsPlayer, invItemsShip, null);
            }
            else if (showCraftingMenu)
            {
                craftingMenu.Draw(spriteBatchStatic, null);
                craftingMenu.DrawInventory(spriteBatchStatic, invItemsPlayer);
            }
            else if (showStorageMenu)
            {
                inventoryMenu.Draw(spriteBatchStatic, null);
                inventoryMenu.DrawInventory(spriteBatchStatic, invItemsPlayer, invItemsShip, invStorage);
            }
            
            if (playerOnShip)
            {
                playerShip.DrawAnchorMeter(spriteBatchStatic, new Vector2(1660, 30), anchorIcon);
                playerShip.DrawRepairHammer(spriteBatchStatic, new Vector2(1600, 30), repairIcon);
            }

            // fps
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
            spriteBatchStatic.Begin();
            spriteBatchStatic.DrawString(font, fps, new Vector2(10, 10), Color.Green);
            spriteBatchStatic.End();
            base.Draw(gameTime);
        }

        // preprocessing to build the region tree
        private void BuildRegionTree()
        {
            // compute the bounds of each region
            List<Tuple<string, Rectangle>> regionBounds = new List<Tuple<string, Rectangle>>();
            foreach (var r in BoundingBoxLocations.RegionMap)
            {
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                List<Sprite> regionTiles = new List<Sprite>();
                regionTiles.AddRange(r.Value.RegionLandTiles);
                regionTiles.AddRange(r.Value.RegionOceanTiles);
                foreach(var l in regionTiles)
                {
                    if (l.location.Y > maxY)
                        maxY = l.location.Y;
                    if (l.location.X > maxX)
                        maxX = l.location.X;
                    if (l.location.Y < minY)
                        minY = l.location.Y;
                    if (l.location.X < minX)
                        minX = l.location.X;
                }
                Rectangle bounds = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
                r.Value.Bounds = bounds; // set the bounds on the region
                // expand the bounds so there is overlap with neighboring regions
                int expandBy = 100;
                bounds.X = bounds.X - expandBy;
                bounds.Y = bounds.Y - expandBy;
                bounds.Width = bounds.Width + expandBy;
                bounds.Height = bounds.Height + expandBy;
                regionBounds.Add(new Tuple<string, Rectangle>(r.Key, bounds));
            }

            // check neighbors
            foreach(var r in regionBounds)
            {
                string name = r.Item1;
                Rectangle bounds = r.Item2;
                foreach(var neighbor in regionBounds)
                {
                    if (neighbor == r)
                        continue;
                    if (neighbor.Item2.Intersects(bounds))
                        BoundingBoxLocations.RegionMap[neighbor.Item1].Neighbors.Add(BoundingBoxLocations.RegionMap[name]);
                }
            }
        }


        // collision handling beef
        private void SpatialCollision()
        {

            foreach (var team in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
                BoundingBoxLocations.BoundingBoxLocationMap[team].Clear();

            foreach (var spriteA in Collidable)
            {
                // set the current region
                foreach (var region in BoundingBoxLocations.RegionMap)
                {
                    if (spriteA.regionKey != region.Key && region.Value.Bounds.Intersects(spriteA.GetBoundingBox()))
                        spriteA.regionKey = region.Key;
                }

                Polygon polyA = null;

                // BoundBoxLocationMap update - this structure is used for AI locating targets
                if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship ship = (Ship)spriteA;
                    BoundingBoxLocations.BoundingBoxLocationMap[ship.teamType].Add(new Tuple<int, int>(spriteA.GetBoundingBox().X, spriteA.GetBoundingBox().Y));
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.Tower))
                {
                    Tower tower = (Tower)spriteA;
                    BoundingBoxLocations.BoundingBoxLocationMap[tower.teamType].Add(new Tuple<int, int>(spriteA.GetBoundingBox().X, spriteA.GetBoundingBox().Y));
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.PlayerPirate))
                {
                    PlayerPirate player = (PlayerPirate)spriteA;
                    BoundingBoxLocations.BoundingBoxLocationMap[player.teamType].Add(new Tuple<int, int>(spriteA.GetBoundingBox().X, spriteA.GetBoundingBox().Y));
                }

                Rectangle bbA = spriteA.GetBoundingBox();
                if (BoundingBoxTextures.DynamicBoundingPolygons.ContainsKey(spriteA.bbKey))
                    polyA = spriteA.GetBoundingPolygon();

                HashSet<string> quadKeys = collision.GetQuadKey(bbA);
                HashSet<Sprite> possible = new HashSet<Sprite>();
                foreach (var key in quadKeys)
                    possible.UnionWith(collision.GetSpatialBoundingMap()[key]);

                foreach (var spriteB in possible)
                {
                    var bbB = spriteB.GetBoundingBox();

                    if (Object.ReferenceEquals(spriteA, spriteB))
                        continue;

                    Polygon polyB = null;
                    if (BoundingBoxTextures.DynamicBoundingPolygons.ContainsKey(spriteB.bbKey))
                        polyB = spriteB.GetBoundingPolygon();

                    // pass collision game logic - this filters out some collisions that are colliding but we don't want to handle a collision (trees colliding with other trees)
                    if (CollisionGameLogic.CheckCollidable(spriteA, spriteB) && CollisionGameLogic.CheckCollidable(spriteB, spriteA))
                    {
                        // spatial intersection for rect, poly
                        if (polyA != null || polyB != null)
                        {
                            // poly vs rect
                            if (polyA != null && polyB == null)
                            {
                                if (polyA.IntersectsRect(bbB))
                                    MarkCollision(spriteA, spriteB);
                            }
                            else if (polyB != null && polyA == null)
                            {
                                if (polyB.IntersectsRect(bbA))
                                    MarkCollision(spriteA, spriteB);
                            }
                            else
                            {
                                // poly vs poly
                                if (polyA.IntersectsPoly(polyB))
                                    MarkCollision(spriteA, spriteB);
                            }
                        }
                        else
                        {
                            // rect vs rects
                            if (bbA.Intersects(bbB))
                                MarkCollision(spriteA, spriteB);
                        }
                    }
                }
            }
            collision.Clear();
        }

        private void MarkCollision(Sprite spriteA, Sprite spriteB)
        {
            spriteA.colliding = true;
            spriteB.colliding = true;
            Rectangle overlap = Rectangle.Intersect(spriteA.GetBoundingBox(), spriteB.GetBoundingBox());
            spriteA.HandleCollision(spriteB, overlap);
            spriteB.HandleCollision(spriteA, overlap);
        }
    }
}
