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

namespace Gusto
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameGusto : Game
    {

        // TEMPORARY -- expose the "players and enemies". 
        BaseShip baseShip;
        BaseShip baseShipAI;
        BaseTower tower;
        PiratePlayer piratePlayer;
        BaseTribal baseTribal;
        Pistol pistol;
        PistolShotItem pistolAmmo;
        CannonBallItem cannonAmmo;

        TileGameMap map;
        JObject mapData;

        // static
        LightArea sun;
        Vector2 sunPos;
        WindArrows windArrows;
        Texture2D anchorIcon;
        Inventory inventory;

        RenderTarget2D screenShadows;
        ShadowMapResolver shadowMapResolver;
        QuadRenderComponent quadRender;
        SpatialBounding collision;
        GraphicsDeviceManager graphics;
        List<Sprite> DrawOrder;
        List<Sprite> Collidable;
        HashSet<Sprite> UpdateOrder;
        SpriteBatch spriteBatchView;
        SpriteBatch spriteBatchStatic;
        Camera camera;
        
        public GameGusto()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = GameOptions.PrefferedBackBufferWidth;
            graphics.PreferredBackBufferHeight = GameOptions.PrefferedBackBufferHeight;
            quadRender = new QuadRenderComponent(this);
            this.Components.Add(quadRender);
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            mapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\gamemap.json"));
            map.LoadMapData(mapData);

            shadowMapResolver = new ShadowMapResolver(GraphicsDevice, quadRender, ShadowMapSize.Size2048, ShadowMapSize.Size10192);
            shadowMapResolver.LoadContent(Content);
            sun = new LightArea(GraphicsDevice, ShadowMapSize.Size4096);
            sunPos = new Vector2(0, 200);
            screenShadows = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatchView = new SpriteBatch(GraphicsDevice);
            spriteBatchStatic = new SpriteBatch(GraphicsDevice);

            // PREPROCESSING
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseShip, "baseShip", 0.6f);
            Texture2D texturePlayerPirate = Content.Load<Texture2D>("Pirate1-combat");
            LoadDynamicBoundingBoxPerFrame(4, 11, texturePlayerPirate, "playerPirate", 1.0f);
            Texture2D textureBaseTribal = Content.Load<Texture2D>("Tribal1");
            LoadDynamicBoundingBoxPerFrame(4, 12, textureBaseTribal, "baseTribal", 1.0f);
            Texture2D textureTribalTokens = Content.Load<Texture2D>("TribalTokens");
            LoadDynamicBoundingBoxPerFrame(1, 1, textureTribalTokens, "tribalTokens", 0.5f);
            Texture2D textureBaseSword = Content.Load<Texture2D>("BaseSword");
            LoadDynamicBoundingBoxPerFrame(4, 3, textureBaseSword, "baseSword", 1.0f);
            Texture2D texturePistol = Content.Load<Texture2D>("pistol");
            LoadDynamicBoundingBoxPerFrame(4, 3, texturePistol, "pistol", 1.0f);
            Texture2D textureShortSword = Content.Load<Texture2D>("ShortSword");
            LoadDynamicBoundingBoxPerFrame(4, 3, textureShortSword, "shortSword", 1.0f);
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(8, 3, textureBaseSail, "baseSail", 0.6f);
            Texture2D textureTower = Content.Load<Texture2D>("tower");
            LoadDynamicBoundingBoxPerFrame(1, 1, textureTower, "tower", 0.5f);
            Texture2D textureCannonBall = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(1, 2, textureCannonBall, "baseCannonBall", 1.0f);
            Texture2D textureCannonBallItem = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(1, 2, textureCannonBallItem, "cannonBallItem", 1.0f);
            Texture2D texturePistolShot = Content.Load<Texture2D>("PistolShot");
            LoadDynamicBoundingBoxPerFrame(1, 2, texturePistolShot, "pistolShot", 1.0f);
            Texture2D texturePistolShotItem = Content.Load<Texture2D>("PistolShot");
            LoadDynamicBoundingBoxPerFrame(1, 2, texturePistolShotItem, "pistolShotItem", 1.0f);
            Texture2D textureBaseCannon = Content.Load<Texture2D>("BaseCannon");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseCannon, "baseCannon", 1.0f);
            
            // Tile Pieces
            Texture2D textureOcean1 = Content.Load<Texture2D>("Ocean1");
            LoadDynamicBoundingBoxPerFrame(1, 4, textureOcean1, "oceanTile", 1.0f);
            Texture2D textureLand1 = Content.Load<Texture2D>("Land1");
            LoadDynamicBoundingBoxPerFrame(1, 4, textureLand1, "landTile", 1.0f);
            Texture2D textureTree1 = Content.Load<Texture2D>("Tree1");
            LoadDynamicBoundingBoxPerFrame(2, 6, textureTree1, "tree1", 0.4f);

            // Game Map
            map.SetGameMap(Content, GraphicsDevice);
            List<Sprite> giannaRegionMap = BoundingBoxLocations.RegionMap["Gianna"];

            //TEMPORARY NEED TO CREATE SOME SORT OF GAME SETUP / REGION SETUP
            Random rnd = new Random();
            Sprite GiannaRegionTile = giannaRegionMap[rnd.Next(giannaRegionMap.Count)];

            // static 
            windArrows = new WindArrows(new Vector2(1740, 50), Content, GraphicsDevice);
            anchorIcon = Content.Load<Texture2D>("anchor-shape");
   
            
            // TEMPORARY create Team models and initally place them - this will eventually be set in game config menu
            baseShip = new BaseShip(TeamType.Player, "GustoGame", new Vector2(300, -500), windArrows, Content, GraphicsDevice);
            piratePlayer = new PiratePlayer(TeamType.Player, "GustoGame", new Vector2(300, -300), Content, GraphicsDevice);
            inventory = new Inventory(screenCenter, Content, GraphicsDevice, piratePlayer);

            baseTribal = new BaseTribal(TeamType.B, "Gianna", GiannaRegionTile.location, Content, GraphicsDevice);
            tower = new BaseTower(TeamType.A, "GustoGame", new Vector2(200, 700), Content, GraphicsDevice);
            baseShipAI = new BaseShip(TeamType.A, "GustoGame", new Vector2(470, 0), windArrows, Content, GraphicsDevice);
            pistol = new Pistol(TeamType.A, "GustoGame", new Vector2(250, -300), Content, GraphicsDevice);
            pistol.amountStacked = 1;
            pistol.onGround = true;
            pistolAmmo = new PistolShotItem(TeamType.A, "GustoGame", new Vector2(220, -300), Content, GraphicsDevice);
            pistolAmmo.amountStacked = 4;
            pistolAmmo.onGround = true;
            cannonAmmo = new CannonBallItem(TeamType.A, "GustoGame", new Vector2(200, -300), Content, GraphicsDevice);
            cannonAmmo.amountStacked = 10;
            cannonAmmo.onGround = true;

            // fill update order list
            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(piratePlayer);
            UpdateOrder.Add(baseTribal);
            UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(tower);
            UpdateOrder.Add(pistol);
            UpdateOrder.Add(pistolAmmo);
            UpdateOrder.Add(cannonAmmo);
            UpdateOrder.Add(inventory);

        }

        // creates dynamic bounding boxes for each sprite frame
        private void LoadDynamicBoundingBoxPerFrame(int rows, int columns, Texture2D source, string key, float scale)
        {
            int width = source.Width / columns;
            int height = source.Height / rows;
            BoundingBoxTextures.DynamicBoundingBoxTextures.Add(key, new Dictionary<string, Rectangle>());
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
                    // store the BB texture 
                    Rectangle bb = CalculateTextureBoundingBox.GetSmallestRectangleFromTexture(cropTexture, scale);
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

            sunPos.X += 1;

            List<Sprite> toRemove = new List<Sprite>();
            HashSet<Sprite> tempUpdateOrder = new HashSet<Sprite>();

            // camera follows player
            if (!piratePlayer.onShip)
                this.camera.Position = piratePlayer.location;
            else
                this.camera.Position = piratePlayer.playerOnShip.location;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var kstate = Keyboard.GetState();

            // static update (Wind)
            windArrows.Update(kstate, gameTime, null);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();

            // add any dropped items
            foreach (var item in ItemUtility.ItemsToUpdate)
                UpdateOrder.Add(item);

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

            //ItemUtility.ItemsToUpdate.Clear();

            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        private void DrawCasters(LightArea light)
        {
            piratePlayer.DrawShadow(spriteBatchView, this.camera, light.ToRelativePosition(piratePlayer.GetBoundingBox().Center.ToVector2()));
            baseShipAI.DrawShadow(spriteBatchView, this.camera, light.ToRelativePosition(baseShipAI.GetBoundingBox().Center.ToVector2()));
            baseShip.DrawShadow(spriteBatchView, this.camera, light.ToRelativePosition(baseShip.GetBoundingBox().Center.ToVector2()));
            baseTribal.DrawShadow(spriteBatchView, this.camera, light.ToRelativePosition(baseTribal.GetBoundingBox().Center.ToVector2()));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sun.LightPosition = sunPos;
            sun.BeginDrawingShadowCasters();
            DrawCasters(sun);
            sun.EndDrawingShadowCasters();
            shadowMapResolver.ResolveShadows(sun.RenderTarget, sun.RenderTarget, sunPos);

            GraphicsDevice.SetRenderTarget(screenShadows);
            //GraphicsDevice.Clear(Color.Black);
            spriteBatchView.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatchView.Draw(sun.RenderTarget, sun.LightPosition - sun.LightAreaSize * 0.5f, Color.White);
            spriteBatchView.End();
            GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(Color.Black);

            // draw map
            map.DrawMap(spriteBatchView);

            BlendState blendState = new BlendState();
            blendState.ColorSourceBlend = Blend.DestinationColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;

            spriteBatchView.Begin(SpriteSortMode.Immediate, blendState);
            spriteBatchView.Draw(screenShadows, Vector2.Zero, Color.White);
            spriteBatchView.End();

            // draw/set sprites that don't move
            windArrows.Draw(spriteBatchStatic, null);
            bool showInventoryMenu = false;
            List<InventoryItem> invItemsPlayer = null;
            List<InventoryItem> invItemsShip = null;

            // sort sprites by y cord asc and draw
            DrawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
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

                if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.Ship))
                {
                    Ship ship = (Ship) sprite;
                    ship.DrawAnchorMeter(spriteBatchStatic, new Vector2(1660, 30), anchorIcon);

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

                    if (pirate.showInventory)
                    {
                        showInventoryMenu = true;
                        invItemsPlayer = pirate.inventory;
                        if (pirate.onShip)
                            invItemsShip = pirate.playerOnShip.inventory;
                    }

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

                    if (pirate.inCombat && pirate.currRowFrame != 3)
                        pirate.inHand.Draw(spriteBatchView, this.camera);

                    foreach (var shot in pirate.inHand.Shots)
                        shot.Draw(spriteBatchView, this.camera);

                    continue;
                }

                else if (sprite.GetType().BaseType == typeof(Gusto.Models.Animated.GroundEnemy))
                {
                    GroundEnemy enemy = (GroundEnemy)sprite;
                    if (enemy.dying)
                    {
                        enemy.DrawDying(spriteBatchView, this.camera);
                        continue;
                    } 
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

                sprite.Draw(spriteBatchView, this.camera);
            }

            if (showInventoryMenu)
            {
                inventory.Draw(spriteBatchStatic, null);
                inventory.DrawInventory(spriteBatchStatic, invItemsPlayer, invItemsShip);
            }

            base.Draw(gameTime);
        }

        private void SpatialCollision()
        {
            foreach (var team in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
                BoundingBoxLocations.BoundingBoxLocationMap[team].Clear();

            foreach (var spriteA in Collidable)
            {

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
                HashSet<string> quadKeys = collision.GetQuadKey(bbA);
                HashSet<Sprite> possible = new HashSet<Sprite>();
                foreach (var key in quadKeys)
                    possible.UnionWith(collision.GetSpatialBoundingMap()[key]);

                foreach (var spriteB in possible)
                {
                    if (spriteB == spriteA)
                        continue;

                    var bbB = spriteB.GetBoundingBox();

                    if (bbA.Intersects(bbB) && CollisionGameLogic.CheckCollidable(spriteA, spriteB) && CollisionGameLogic.CheckCollidable(spriteB, spriteA))
                    {
                        spriteA.colliding = true;
                        spriteB.colliding = true;
                        Rectangle overlap = Rectangle.Intersect(bbA, bbB);
                        spriteA.HandleCollision(spriteB, overlap);
                        spriteB.HandleCollision(spriteA, overlap);
                    }
                }
            }
            collision.Clear();
        }
    }
}
