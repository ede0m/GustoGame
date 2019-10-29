using Comora;
using Gusto.Bounding;
using Gusto.Bounds;
using Gusto.Models;
using Gusto.Models.Interfaces;
using Gusto.GameMap;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Gusto.Utility;
using Gusto.Models.Menus;
using Gusto.Models.Animated;
using System.Linq;
using Gusto.Models.Types;

namespace Gusto
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameGusto : Game
    {

        TileGameMap map;
        JObject mapData;

        // static
        WindArrows windArrows;
        Texture2D anchorIcon;
        Texture2D repairIcon;
        Texture2D boardingIcon;
        Texture2D treasureXMark;
        Inventory inventoryMenu;
        CraftingMenu craftingMenu;
        OpenGameMenu startingMenu;

        GraphicsDeviceManager graphics;
        FrameCounter _frameCounter;
        RenderTarget2D worldScene;
        RenderTarget2D interiorScene;
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
        Vector2 camMove;
        Vector2 startCamPos;
        
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
            worldScene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            interiorScene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatchView = new SpriteBatch(GraphicsDevice);
            spriteBatchStatic = new SpriteBatch(GraphicsDevice);

            // PREPROCESSING Bounding Sprites
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(true, 8, 1, textureBaseShip, "baseShip", 0.6f, 1.0f);
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(false, 8, 3, textureBaseSail, "baseSail", 0.6f, 1.0f);
            Texture2D textureShortShip = Content.Load<Texture2D>("ShortShipHullRedMark");
            LoadDynamicBoundingBoxPerFrame(true, 8, 1, textureShortShip, "shortShip", 1.1f, 1.0f);
            Texture2D textureShortSail = Content.Load<Texture2D>("ShortSail");
            LoadDynamicBoundingBoxPerFrame(false, 8, 3, textureShortSail, "shortSail", 1.1f, 1.0f);
            Texture2D textureTeePee = Content.Load<Texture2D>("TeePee");
            LoadDynamicBoundingBoxPerFrame(true, 1, 4, textureTeePee, "teePee", 0.5f, 1.0f);
            Texture2D texturePlayerPirate = Content.Load<Texture2D>("Pirate1-combat");
            LoadDynamicBoundingBoxPerFrame(false, 4, 11, texturePlayerPirate, "playerPirate", 1.0f, 1.0f);
            Texture2D textureBaseTribal = Content.Load<Texture2D>("Tribal1");
            LoadDynamicBoundingBoxPerFrame(false, 4, 12, textureBaseTribal, "baseTribal", 1.0f, 1.0f);
            Texture2D textureBaseCat = Content.Load<Texture2D>("Cat1");
            LoadDynamicBoundingBoxPerFrame(false, 4, 12, textureBaseCat, "baseCat", 0.7f, 1.0f);
            Texture2D textureChicken = Content.Load<Texture2D>("Chicken");
            LoadDynamicBoundingBoxPerFrame(false, 4, 10, textureChicken, "chicken", 0.7f, 1.0f);
            Texture2D textureSnake = Content.Load<Texture2D>("Snake1");
            LoadDynamicBoundingBoxPerFrame(false, 5, 9, textureSnake, "snake", 0.7f, 1.0f);
            Texture2D textblueBird = Content.Load<Texture2D>("BlueBird");
            LoadDynamicBoundingBoxPerFrame(false, 5, 4, textblueBird, "blueBird", 0.4f, 1.0f);
            Texture2D textureBaseSword = Content.Load<Texture2D>("BaseSword");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureBaseSword, "baseSword", 1.0f, 1.0f);
            Texture2D texturePistol = Content.Load<Texture2D>("pistol");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, texturePistol, "pistol", 1.0f, 1.0f);
            Texture2D textureCrossBow = Content.Load<Texture2D>("crossBow");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureCrossBow, "crossBow", 1.0f, 1.0f);
            Texture2D texturePickaxe = Content.Load<Texture2D>("pickaxe");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, texturePickaxe, "pickaxe", 1.0f, 1.0f);
            Texture2D textureShortSword = Content.Load<Texture2D>("ShortSword");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureShortSword, "shortSword", 1.0f, 1.0f);
            Texture2D textureShovel = Content.Load<Texture2D>("Shovel");
            LoadDynamicBoundingBoxPerFrame(false, 4, 3, textureShovel, "shovel", 1.0f, 1.0f);
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
            Texture2D textureArrowItem = Content.Load<Texture2D>("Arrow");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureArrowItem, "arrowItem", 1.0f, 1.0f);
            Texture2D textureArrow = Content.Load<Texture2D>("Arrow");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureArrow, "arrow", 1.0f, 0.5f); // SCALING BOUNDING BOX FOR DIRECTIONAL AMMO
            Texture2D textureRustyHarpoonItem = Content.Load<Texture2D>("RustyHarpoon");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureRustyHarpoonItem, "rustyHarpoonItem", 1.0f, 0.5f); 
            Texture2D textureRustyHarpoon = Content.Load<Texture2D>("RustyHarpoon");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureRustyHarpoon, "rustyHarpoon", 1.0f, 0.5f); // SCALING BOUNDING BOX FOR DIRECTIONAL AMMO

            Texture2D textureBaseCannon = Content.Load<Texture2D>("BaseCannon");
            LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureBaseCannon, "baseCannon", 1.0f, 1.0f);
            Texture2D textureBallista = Content.Load<Texture2D>("Ballista");
            LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureBallista, "ballista", 1.0f, 1.0f);
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
            Texture2D textureCampFire = Content.Load<Texture2D>("CampFire");
            LoadDynamicBoundingBoxPerFrame(false, 1, 6, textureCampFire, "campFire", 0.3f, 1.0f);

            // Tile Pieces, Ground Objects and Invetory Items
            Texture2D textureOcean1 = Content.Load<Texture2D>("Ocean1v3");
            LoadDynamicBoundingBoxPerFrame(false, 4, 1, textureOcean1, "oceanTile", 1.0f, 1.0f);
            Texture2D textureLand1 = Content.Load<Texture2D>("Land1HolesShore");
            LoadDynamicBoundingBoxPerFrame(false, 9, 4, textureLand1, "landTile", 1.0f, 1.0f);
            //Texture2D textureShipDeck1 = Content.Load<Texture2D>("ShipDeck");
            //LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureShipDeck1, "interiorTile", 1.0f, 1.0f);
            //Texture2D textureShipDeck1Wall = Content.Load<Texture2D>("ShipDeckWall");
            //LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureShipDeck1Wall, "interiorTileWall", 1.0f, 1.0f);
            Texture2D textureShipInterior1 = Content.Load<Texture2D>("ShipInterior");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureShipInterior1, "interiorTile", 1.0f, 1.0f);
            Texture2D textureShipInterior1Wall = Content.Load<Texture2D>("ShipInteriorWall");
            LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureShipInterior1Wall, "interiorTileWall", 1.0f, 1.0f);
            Texture2D textureTree1 = Content.Load<Texture2D>("Tree1");
            LoadDynamicBoundingBoxPerFrame(true, 2, 6, textureTree1, "tree1", 0.6f, 1.0f);
            Texture2D textureTree2 = Content.Load<Texture2D>("Tree2");
            LoadDynamicBoundingBoxPerFrame(true, 2, 4, textureTree2, "tree2", 0.6f, 1.0f);
            Texture2D textureTree3 = Content.Load<Texture2D>("Tree3");
            LoadDynamicBoundingBoxPerFrame(true, 2, 4, textureTree3, "tree3", 0.6f, 1.0f);
            Texture2D textureSoftWood = Content.Load<Texture2D>("softwoodpile");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureSoftWood, "softWood", 0.5f, 1.0f);
            Texture2D textureGrass1 = Content.Load<Texture2D>("RevisedGrass1");
            LoadDynamicBoundingBoxPerFrame(false, 1, 2, textureGrass1, "grass1", 0.6f, 1.0f);
            Texture2D textureRock1 = Content.Load<Texture2D>("Rock1");
            LoadDynamicBoundingBoxPerFrame(false, 2, 4, textureRock1, "rock1", 0.3f, 1.0f);
            Texture2D textureIslandGrass = Content.Load<Texture2D>("islandGrass");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureIslandGrass, "islandGrass", 0.5f, 1.0f);
            Texture2D textureChiliFish = Content.Load<Texture2D>("ChiliFish");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureChiliFish, "chiliFish", 0.5f, 1.0f);
            Texture2D textureChiliPepper = Content.Load<Texture2D>("ChiliPepper");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureChiliPepper, "chiliPepper", 0.5f, 1.0f);
            Texture2D textureCookedFish = Content.Load<Texture2D>("CookedFish");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureCookedFish, "cookedFish", 0.5f, 1.0f);
            Texture2D textureCookedMeat = Content.Load<Texture2D>("CookedMeat");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureCookedMeat, "cookedMeat", 0.5f, 1.0f);
            Texture2D textureFeather = Content.Load<Texture2D>("Feather");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureFeather, "feather", 0.5f, 1.0f);
            Texture2D textureFishOil = Content.Load<Texture2D>("FishOil");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureFishOil, "fishOil", 0.5f, 1.0f);
            Texture2D textureGoldCoins = Content.Load<Texture2D>("GoldCoins");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureGoldCoins, "goldCoins", 0.5f, 1.0f);
            Texture2D textureRawFish = Content.Load<Texture2D>("RawFish");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureRawFish, "rawFish", 0.5f, 1.0f);
            Texture2D textureRawMeat = Content.Load<Texture2D>("RawMeat");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureRawMeat, "rawMeat", 0.5f, 1.0f);
            Texture2D textureScales = Content.Load<Texture2D>("Scales");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureScales, "scales", 0.5f, 1.0f);
            Texture2D textureSpoiledFish = Content.Load<Texture2D>("SpoiledFish");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureSpoiledFish, "spoiledFish", 0.5f, 1.0f);
            Texture2D textureSpoiledMeat = Content.Load<Texture2D>("SpoiledMeat");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureSpoiledMeat, "spoiledMeat", 0.5f, 1.0f);
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

            gameState = new GameState(Content, GraphicsDevice);

            // Game Map
            mapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\gamemap.json"));
            map.SetGameMap(Content, GraphicsDevice, spriteBatchView, mapData);
            BuildRegionTree();

            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            // static load
            anchorIcon = Content.Load<Texture2D>("anchor-shape");
            repairIcon = Content.Load<Texture2D>("work-hammer-");
            boardingIcon = Content.Load<Texture2D>("icons8-sword-52");
            treasureXMark = Content.Load<Texture2D>("XSpot");
            font = Content.Load<SpriteFont>("helperFont");

            windArrows = new WindArrows(new Vector2(1740, 50), Content, GraphicsDevice);
            WeatherState.wind = windArrows;

            // static init (MENUS)
            inventoryMenu = new Inventory(screenCenter, Content, GraphicsDevice, gameState.player);
            craftingMenu = new CraftingMenu(screenCenter, Content, GraphicsDevice, gameState.player);
            startingMenu = new OpenGameMenu(Content, GraphicsDevice);

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
            HashSet<Sprite> groundObjectUpdateOrder = new HashSet<Sprite>();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F4))
                Exit();
            if (kstate.IsKeyDown(Keys.F12))
            {
                gameState.SaveGameState();
                Exit();
            }

            // game menu before everything
            if (startingMenu.showMenu)
            {
                startingMenu.Update(kstate, gameTime);
                return;
            }
            else if (startingMenu.selected == "new" && !gameState.ready)
            {
                gameState.CreateNewGame();
                startCamPos = gameState.player.location;
                return;
            }
            else if (startingMenu.selected == "load" && !gameState.ready)
            {
                gameState.LoadGameState();
                startCamPos = gameState.player.location;
                return;
            }


            // weather
            weather.Update(kstate, gameTime);
            // daylight shader 
            dayLight.Update(kstate, gameTime);

            // static update (Menus)
            windArrows.Update(kstate, gameTime, null);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();

            inventoryMenu.Update(kstate, gameTime, this.camera);
            craftingMenu.Update(kstate, gameTime, this.camera);

            Collidable.Clear();
            DrawOrder.Clear();

            // set any viewport visible(and not visible when in interior) collidable map pieces for collision - update LandTileLocList and GroundObjLocList
            BoundingBoxLocations.LandTileLocationList.Clear();
            BoundingBoxLocations.OceanTileLocationList.Clear();
            BoundingBoxLocations.GroundObjectLocationList.Clear();
            BoundingBoxLocations.TilesInView.Clear();
            Vector2 minCorner = new Vector2(camera.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), camera.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(camera.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), camera.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));

            foreach (var tp in GameMapTiles.map)
            {
                if ((tp.location.X >= (minCorner.X - GameOptions.tileWidth) && tp.location.X <= (maxCorner.X + GameOptions.tileWidth)) && 
                    (tp.location.Y >= (minCorner.Y - GameOptions.tileHeight) && tp.location.Y <= (maxCorner.Y + GameOptions.tileHeight)))
                {
                    BoundingBoxLocations.TilesInView.Add(tp);

                    if (tp.bbKey.Equals("landTile"))
                    {
                        BoundingBoxLocations.LandTileLocationList.Add(tp);
                        SpatialBounding.SetQuad(tp.GetBase());
                    }
                    else
                        BoundingBoxLocations.OceanTileLocationList.Add(tp);

                    if (tp.groundObjects != null)
                    {
                        foreach (var groundObject in tp.groundObjects)
                        {
                            if (!groundObject.remove)
                                BoundingBoxLocations.GroundObjectLocationList.Add(groundObject);
                            else
                            {
                                if (groundObject is IGroundObject)
                                {
                                    IGroundObject go = (IGroundObject)groundObject;
                                    go.UpdateRespawn(gameTime);
                                }
                            }
                        }
                    }
                }
            }

            BoundingBoxLocations.InteriorTileList.Clear();
            // set interior for collision for the interior that the player is in
            if (gameState.player.playerInInterior != null)
            {
                // interior tiles for collision
                foreach(var tile in gameState.player.playerInInterior.interiorTiles)
                {
                    BoundingBoxLocations.InteriorTileList.Add(tile);
                    SpatialBounding.SetQuad(tile.GetBase());
                }

                // TODO: update the interior objects for collision (only do this when player is in there)
                foreach (var obj in gameState.player.playerInInterior.interiorObjects)
                {
                    SpatialBounding.SetQuad(obj.GetBase());
                    Collidable.Add(obj);
                }
            }

            Vector2 lastCamPos = camera.Position;

            // update any gameObjects that need to track state (will set camera pos to player)
            HashSet<Sprite> GameStateObjectUpdateOrder = gameState.Update(kstate, gameTime, camera);

            // use this to offset water noise
            Vector2 currCamPos = camera.Position;
            Vector2 camDistance = currCamPos - startCamPos;

            //camMove = currCamPos - lastCamPos;
            //camMove.X = camMove.X / GameOptions.PrefferedBackBufferWidth;
            //camMove.Y = camMove.Y / GameOptions.PrefferedBackBufferHeight;

            //camMove.X = (camDistance.X / (GameOptions.PrefferedBackBufferWidth / 2));
            //camMove.Y = (camDistance.Y / (GameOptions.PrefferedBackBufferHeight / 2));

            //camMove.X = ((camDistance.X / (GameOptions.PrefferedBackBufferWidth * GameOptions.GameMapWidthMult)));
            //camMove.Y = ((camDistance.Y / (GameOptions.PrefferedBackBufferHeight * GameOptions.GameMapHeightMult)));


            // update ground objects (they do not track their state since they are encoded in the map)
            foreach (var sp in BoundingBoxLocations.GroundObjectLocationList)
            {
                ICanUpdate updateSp = (ICanUpdate)sp;
                updateSp.Update(kstate, gameTime, this.camera);
                groundObjectUpdateOrder.Add(sp);
            }

            // merge update orders
            HashSet<Sprite> fullUpdateOrder = GameStateObjectUpdateOrder;
            fullUpdateOrder.UnionWith(groundObjectUpdateOrder);

            // Set draw order and collision from the full update order list
            foreach (var sp in fullUpdateOrder)
            {
                if (gameState.player.playerInInterior != null)
                {
                    // only add ships and land tiles when to collision when interior is being viewed
                    if (sp is IShip || sp is ITilePiece || sp is IPlayer)
                    {
                        sp.SetBoundingBox();
                        Collidable.Add(sp);
                        SpatialBounding.SetQuad(sp.GetBase());
                    }
                }
                else
                {
                    Collidable.Add(sp);
                    SpatialBounding.SetQuad(sp.GetBase());
                    DrawOrder.Add(sp);
                }

            }

            // handle collision
            collision.Update(this.camera.Position);
            SpatialCollision();

            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // trackers for statically drawn sprites as we move through draw order
            bool showCraftingMenu = false;
            bool showStorageMenu = false;
            ICraftingObject craftObj = null;
            Storage invStorage = null;

            Ship playerShip = gameState.player.playerOnShip;
            List<InventoryItem> invItemsPlayer = gameState.player.inventory;
            List<InventoryItem> invItemsShip = (gameState.player.playerOnShip == null) ? null : gameState.player.playerOnShip.actionInventory;
            if (gameState.player.onShip)
                invItemsShip = gameState.player.playerOnShip.actionInventory;


            // game menu before everything
            if (startingMenu.showMenu)
            {
                startingMenu.DrawInventory(spriteBatchStatic);
                return;
            }

            // draw the interior view if in interior
            if (gameState.player.playerInInterior != null)
            {
                gameState.player.playerInInterior.interiorObjects.Add(gameState.player);

                GraphicsDevice.SetRenderTarget(lightsTarget);
                GraphicsDevice.Clear(Color.Black);
                DrawUtility.DrawSpotLighting(spriteBatchView, this.camera, lightsTarget, gameState.player.playerInInterior.interiorObjects.ToList());

                gameState.player.playerInInterior.Draw(spriteBatchView, this.camera, interiorScene);

                // lighting shader - for ambient day/night light and lanterns
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.White);
                dayLight.Draw(spriteBatchStatic, interiorScene, lightsTarget);

                showCraftingMenu = gameState.player.playerInInterior.showCraftingMenu;
                showStorageMenu = gameState.player.playerInInterior.showStorageMenu;
                craftObj = gameState.player.playerInInterior.craftObj;
                invStorage = gameState.player.playerInInterior.invStorage;
            }
            // not in interior so draw the game scene
            else
            {
                // setup lightTarget for spot lights
                GraphicsDevice.SetRenderTarget(lightsTarget);
                GraphicsDevice.Clear(Color.Black);
                DrawUtility.DrawSpotLighting(spriteBatchView, this.camera, lightsTarget, DrawOrder);

                // draw map
                map.DrawMap(spriteBatchView, spriteBatchStatic, worldScene, gameTime, camMove);

                // draw treasure locations if any
                spriteBatchView.Begin(this.camera);
                foreach (var map in BoundingBoxLocations.treasureLocationsList)
                {
                    spriteBatchView.Draw(treasureXMark, map.digTileLoc, Color.White);
                }
                spriteBatchView.End();

                // draw shadows
                foreach (var sprite in DrawOrder)
                {
                    if (sprite is IShadowCaster)
                    {
                        sprite.DrawShadow(spriteBatchView, camera, WeatherState.sunAngleX, WeatherState.shadowTransparency);
                        if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                        {
                            Ship ship = (Ship)sprite;
                            ship.shipSail.DrawShadow(spriteBatchView, this.camera, WeatherState.sunAngleX, WeatherState.shadowTransparency);
                        }
                    }
                }

                List<Sprite> fliers = new List<Sprite>(); // draw any flyers on top of everything
                // sort sprites by y cord asc and draw
                DrawOrder.Sort((a, b) => a.GetBoundingBox().Bottom.CompareTo(b.GetBoundingBox().Bottom));
                int i = 0;
                foreach (var sprite in DrawOrder)
                {

                    if (sprite is IVulnerable)
                    {
                        IVulnerable v = (IVulnerable)sprite;
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
                        ICraftingObject tcraftObj = (ICraftingObject)sprite;
                        tcraftObj.DrawCanCraft(spriteBatchView, camera);
                        if (tcraftObj.GetShowMenu())
                        {
                            showCraftingMenu = true;
                            craftObj = tcraftObj;
                        }

                    }

                    if (sprite is IPlaceable)
                    {
                        IPlaceable placeObj = (IPlaceable)sprite;
                        placeObj.DrawCanPickUp(spriteBatchView, camera);
                    }

                    if (sprite is IStructure)
                    {
                        IStructure structure = (IStructure)sprite;
                        structure.DrawNearInterior(spriteBatchView, camera);
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
                        Ship ship = (Ship)sprite;

                        if (ship.sinking)
                        {
                            ship.DrawSinking(spriteBatchView, this.camera);
                            ship.shipSail.DrawSinking(spriteBatchView, this.camera);
                        }
                        else
                        {
                            // Draw a ship before its sail and mount
                            ship.Draw(spriteBatchView, this.camera);
                            if (ship.mountedOnShip != null)
                            {
                                foreach (var shot in ship.mountedOnShip.Shots)
                                    shot.Draw(spriteBatchView, this.camera);
                                if (ship.mountedOnShip.aiming)
                                {
                                    ship.mountedOnShip.Draw(spriteBatchView, this.camera);
                                    if (ship.teamType == TeamType.Player)
                                        ship.mountedOnShip.DrawAimLine(spriteBatchView, this.camera);
                                }
                            }
                            ship.shipSail.Draw(spriteBatchView, this.camera);
                        }
                        continue;
                    }

                    else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.PiratePlayer))
                    {
                        DrawUtility.DrawPlayer(spriteBatchView, this.camera, gameState.player);
                        continue;
                    }

                    else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Npc))
                    {
                        Npc npc = (Npc)sprite;

                        // flying drawn after everything else
                        if (npc.flying)
                        {
                            fliers.Add(npc);
                            continue;
                        }

                        if (npc.swimming && !npc.onShip)
                            npc.DrawSwimming(spriteBatchView, this.camera);
                        else if (!npc.onShip)
                            npc.Draw(spriteBatchView, this.camera);
                        if (npc.dying)
                            npc.DrawDying(spriteBatchView, this.camera);
                        continue;
                    }

                    else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.BaseTower))
                    {
                        Tower tower = (Tower)sprite;
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

                // draw fliers
                foreach (var flier in fliers)
                    flier.Draw(spriteBatchView, this.camera);

                // draw weather
                weather.DrawWeather(spriteBatchStatic);

                // lighting shader - for ambient day/night light and lanterns
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.White);
                dayLight.Draw(spriteBatchStatic, worldScene, lightsTarget);
            }

            // lightning is drawn after ambient light
            if (WeatherState.lightning)
                weather.DrawLightning(spriteBatchStatic);

            // draw static and menu sprites
            windArrows.Draw(spriteBatchStatic, null);

            if (gameState.player.onShip)
            {
                playerShip.DrawAnchorMeter(spriteBatchStatic, new Vector2(1660, 30), anchorIcon);
                playerShip.DrawRepairHammer(spriteBatchStatic, new Vector2(1600, 30), repairIcon);

                if (playerShip.msBoarding > 0)
                    playerShip.DrawBeingBoarded(spriteBatchStatic, new Vector2(1540, 30), boardingIcon);

                if (gameState.player.playerInInterior != null)
                    invItemsShip = null;
            }
            else
                invItemsShip = null;

            if (gameState.player.showInventory)
            {
                inventoryMenu.Draw(spriteBatchStatic, null);
                inventoryMenu.DrawInventory(spriteBatchStatic, invItemsPlayer, invItemsShip, null);
            }
            else if (showCraftingMenu)
            {
                craftingMenu.Draw(spriteBatchStatic, null);
                craftingMenu.DrawInventory(spriteBatchStatic, invItemsPlayer, craftObj);
            }
            else if (showStorageMenu)
            {
                inventoryMenu.Draw(spriteBatchStatic, null);
                inventoryMenu.DrawInventory(spriteBatchStatic, invItemsPlayer, invItemsShip, invStorage);
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

                Target targetEntry = new Target(); 
                targetEntry.interiorId = spriteA.inInteriorId;
                targetEntry.targetLoc = spriteA.GetBoundingBox().Center.ToVector2();
                if (spriteA.mapCordPoint != null)
                    targetEntry.mapCordPoint = spriteA.mapCordPoint.Value;
                // BoundBoxLocationMap update - this structure is used for AI locating targets. Set all needed target values
                // pathType denotes what pathType ai will move to. ( i.e. Ships won't move to targets with Land pathType because it wouldn't find a path there and would waste time)
                if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship ship = (Ship)spriteA;
                    targetEntry.pathType = PathType.Ocean;
                    BoundingBoxLocations.BoundingBoxLocationMap[ship.teamType].Add(targetEntry);
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.Tower))
                {
                    Tower tower = (Tower)spriteA;
                    targetEntry.pathType = PathType.Land;
                    BoundingBoxLocations.BoundingBoxLocationMap[tower.teamType].Add(targetEntry);
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.PlayerPirate))
                {
                    PlayerPirate player = (PlayerPirate)spriteA;
                    targetEntry.pathType = PathType.Land;
                    BoundingBoxLocations.BoundingBoxLocationMap[player.teamType].Add(targetEntry);
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Animated.Npc))
                {
                    Npc npc = (Npc)spriteA;
                    targetEntry.pathType = PathType.Land;
                    BoundingBoxLocations.BoundingBoxLocationMap[npc.teamType].Add(targetEntry);
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
