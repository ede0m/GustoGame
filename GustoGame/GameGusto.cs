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
            //Texture2D textureShipDeck1 = Content.Load<Texture2D>("ShipDeck");
            //LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureShipDeck1, "interiorTile", 1.0f, 1.0f);
            //Texture2D textureShipDeck1Wall = Content.Load<Texture2D>("ShipDeckWall");
            //LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureShipDeck1Wall, "interiorTileWall", 1.0f, 1.0f);
            Texture2D textureShipInterior1 = Content.Load<Texture2D>("ShipInterior");
            LoadDynamicBoundingBoxPerFrame(false, 1, 1, textureShipInterior1, "interiorTile", 1.0f, 1.0f);
            Texture2D textureShipInterior1Wall = Content.Load<Texture2D>("ShipInteriorWall");
            LoadDynamicBoundingBoxPerFrame(false, 1, 4, textureShipInterior1Wall, "interiorTileWall", 1.0f, 1.0f);
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

            gameState = new GameState(Content, GraphicsDevice);

            // Game Map
            map.SetGameMap(Content, GraphicsDevice);
            BuildRegionTree();

            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            // static load
            anchorIcon = Content.Load<Texture2D>("anchor-shape");
            repairIcon = Content.Load<Texture2D>("work-hammer-");
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
                return;
            }
            else if (startingMenu.selected == "load" && !gameState.ready)
            {
                gameState.LoadGameState();
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
            BoundingBoxLocations.GroundObjectLocationList.Clear();
            Vector2 minCorner = new Vector2(camera.Position.X - (GameOptions.PrefferedBackBufferWidth / 2), camera.Position.Y - (GameOptions.PrefferedBackBufferHeight / 2));
            Vector2 maxCorner = new Vector2(camera.Position.X + (GameOptions.PrefferedBackBufferWidth / 2), camera.Position.Y + (GameOptions.PrefferedBackBufferHeight / 2));
            foreach (var tile in map.GetMap())
            {
                TilePiece tp = (TilePiece)tile;
                var loc = tp.location;
                if ((loc.X >= minCorner.X && loc.X <= maxCorner.X) && (loc.Y >= minCorner.Y && loc.Y <= maxCorner.Y))
                {
                    
                    if (tp.bbKey.Equals("landTile"))
                    {
                        BoundingBoxLocations.LandTileLocationList.Add(tile);
                        SpatialBounding.SetQuad(tile.GetBase());
                    }

                    // handle ground objects (and respawn)
                    if (tp.groundObject != null)
                    {
                        if (!tp.groundObject.remove)
                            BoundingBoxLocations.GroundObjectLocationList.Add(tp.groundObject);
                        else
                        {
                            if (tp.groundObject is IGroundObject)
                            {
                                IGroundObject go = (IGroundObject)tp.groundObject;
                                go.UpdateRespawn(gameTime);
                            }
                        }
                    }
                }
            }

            BoundingBoxLocations.InteriorTileList.Clear();
            // set interior for collision for the interior that the player is in
            if (gameState.player.playerInInterior != null)
            {
                // tiles
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

            // update any gameObjects that need to track state
            HashSet<Sprite> GameStateObjectUpdateOrder = gameState.Update(kstate, gameTime, camera);

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
                Collidable.Add(sp);
                SpatialBounding.SetQuad(sp.GetBase());
                DrawOrder.Add(sp);
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

            // trackers for statically drawn sprites as we move through draw order
            bool showCraftingMenu = false;
            bool showStorageMenu = false;
            Ship playerShip = gameState.player.playerOnShip;
            List<InventoryItem> invItemsPlayer = gameState.player.inventory;
            List<InventoryItem> invItemsShip = (gameState.player.playerOnShip == null) ? null : gameState.player.playerOnShip.actionInventory;
            if (gameState.player.onShip)
                invItemsShip = gameState.player.playerOnShip.actionInventory;
            Storage invStorage = null;

            // draw the interior view if in interior
            if (gameState.player.playerInInterior != null)
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);
                gameState.player.playerInInterior.Draw(spriteBatchView, this.camera);
                showCraftingMenu = gameState.player.playerInInterior.showCraftingMenu;
                showStorageMenu = gameState.player.playerInInterior.showStorageMenu;
                invStorage = gameState.player.playerInInterior.invStorage;
                DrawPlayer();
            }
            // not in interior so draw the game scene
            else
            {
                // set up gamescene draw
                GraphicsDevice.SetRenderTarget(gameScene);
                GraphicsDevice.Clear(Color.CornflowerBlue);

                // draw map
                map.DrawMap(spriteBatchView, gameTime);

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

                // sort sprites by y cord asc and draw
                DrawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
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
                        Ship ship = (Ship)sprite;

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
                        DrawPlayer();
                        continue;
                    }

                    else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Anvil))
                    {
                        Anvil anvil = (Anvil)sprite;
                        if (anvil.drawCraftingMenu)
                            showCraftingMenu = true;
                    }

                    else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Npc))
                    {
                        Npc enemy = (Npc)sprite;

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

                // draw weather
                weather.DrawWeather(spriteBatchStatic);

                // lighting shader - for ambient day/night light and lanterns
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.White);
                dayLight.Draw(spriteBatchStatic, gameScene, lightsTarget);
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

                if (gameState.player.playerInInterior != null)
                    invItemsShip = null;
            }

            if (gameState.player.showInventory)
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
           
            // fps
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);
            spriteBatchStatic.Begin();
            spriteBatchStatic.DrawString(font, fps, new Vector2(10, 10), Color.Green);
            spriteBatchStatic.End();
            base.Draw(gameTime);
        }

        public void DrawPlayer()
        {
            PiratePlayer pirate = gameState.player;

            if (pirate.inCombat && pirate.currRowFrame == 3) // draw sword before pirate when moving up
                pirate.inHand.Draw(spriteBatchView, this.camera);
            if (pirate.nearShip)
                pirate.DrawEnterShip(spriteBatchView, this.camera);
            else if (pirate.onShip)
                pirate.DrawOnShip(spriteBatchView, this.camera);


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
