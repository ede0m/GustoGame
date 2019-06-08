using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Bounds;
using Gusto.Models;
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
        BaseSword baseSword;

        TileGameMap map;
        JObject mapData;

        // static
        WindArrows windArrows;
        Texture2D anchorIcon;
 
        SpatialBounding collision;
        GraphicsDeviceManager graphics;
        List<Sprite> DrawOrder;
        List<Sprite> Collidable;
        List<Sprite> UpdateOrder;
        SpriteBatch spriteBatchView;
        SpriteBatch spriteBatchStatic;
        Camera camera;
        
        public GameGusto()
        {
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
            UpdateOrder = new List<Sprite>();
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
            mapData = JObject.Parse(File.ReadAllText(@"C:\Users\GMON\source\repos\GustoGame\GustoGame\Content\gamemap.json"));
            map.LoadMapData(mapData);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatchView = new SpriteBatch(GraphicsDevice);
            spriteBatchStatic = new SpriteBatch(GraphicsDevice);

            // PREPROCESSING
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseShip, "baseShip", 0.6f);
            Texture2D texturePlayerPirate = Content.Load<Texture2D>("Pirate1-combat");
            LoadDynamicBoundingBoxPerFrame(4, 11, texturePlayerPirate, "playerPirate", 1.0f);
            Texture2D textureBaseTribal = Content.Load<Texture2D>("Tribal1");
            LoadDynamicBoundingBoxPerFrame(4, 12, texturePlayerPirate, "baseTribal", 0.8f);
            Texture2D textureBaseSword = Content.Load<Texture2D>("BaseSword");
            LoadDynamicBoundingBoxPerFrame(4, 3, textureBaseSword, "baseSword", 1.0f);
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(8, 3, textureBaseSail, "baseSail", 0.6f);
            Texture2D textureTower = Content.Load<Texture2D>("tower");
            LoadDynamicBoundingBoxPerFrame(1, 1, textureTower, "tower", 0.5f);
            Texture2D textureCannonBall = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(1, 2, textureCannonBall, "baseCannonBall", 1.0f);
            Texture2D textureBaseCannon = Content.Load<Texture2D>("BaseCannon");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseCannon, "baseCannon", 1.0f);
            
            // Tile Pieces
            Texture2D textureOcean1 = Content.Load<Texture2D>("Ocean1");
            LoadDynamicBoundingBoxPerFrame(1, 4, textureOcean1, "oceanTile", 1.0f);
            Texture2D textureLand1 = Content.Load<Texture2D>("Land1");
            LoadDynamicBoundingBoxPerFrame(1, 4, textureLand1, "landTile", 1.0f);


            // Game Map
            map.SetGameMap(Content, GraphicsDevice);
            Dictionary<string, List<Sprite>> regionMap = map.GetRegionMap();

            //TEMPORARY 
            Random rnd = new Random();
            Sprite GiannaRegionTile = regionMap["Gianna"][rnd.Next(regionMap["Gianna"].Count)];


            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            // create Team models and initally place them
            baseShip = new BaseShip(TeamType.Player, new Vector2(300, -500), Content, GraphicsDevice);
            piratePlayer = new PiratePlayer(TeamType.Player, new Vector2(300, -300), Content, GraphicsDevice);
            baseTribal = new BaseTribal(TeamType.B, GiannaRegionTile.location, Content, GraphicsDevice);
            tower = new BaseTower(TeamType.A, new Vector2(200, 700), Content, GraphicsDevice);
            baseShipAI = new BaseShip(TeamType.A, new Vector2(470, 0), Content, GraphicsDevice);

            // static 
            windArrows = new WindArrows(new Vector2(1740, 50), Content, GraphicsDevice);
            anchorIcon = Content.Load<Texture2D>("anchor-shape");


            // fill draw order list
            DrawOrder.Add(baseShip);
            DrawOrder.Add(piratePlayer);
            DrawOrder.Add(baseTribal);
            DrawOrder.Add(baseShipAI);
            DrawOrder.Add(tower);

            // fill update order list
            UpdateOrder.Add(baseShip);
            UpdateOrder.Add(piratePlayer);
            UpdateOrder.Add(baseTribal);
            UpdateOrder.Add(baseShipAI);
            UpdateOrder.Add(tower);

            // fill collidable list
            Collidable.Add(baseShip);
            SpatialBounding.SetQuad(baseShip.GetBase());
            Collidable.Add(piratePlayer);
            SpatialBounding.SetQuad(piratePlayer);
            Collidable.Add(baseTribal);
            SpatialBounding.SetQuad(baseTribal);
            Collidable.Add(baseShipAI);
            SpatialBounding.SetQuad(baseShipAI.GetBase());
            Collidable.Add(tower);
            SpatialBounding.SetQuad(tower.GetBase());

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
            List<Sprite> toRemove = new List<Sprite>();

            // camera follows player
            if (!piratePlayer.onShip)
                this.camera.Position = piratePlayer.location;
            else
                this.camera.Position = piratePlayer.playerOnShip.location;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var kstate = Keyboard.GetState();


            // static update (Wind)
            windArrows.Update(kstate, gameTime);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();
            
            // main update for all non static objects
            foreach (var sp in UpdateOrder)
            {
                if (sp.remove)
                {
                    DrawOrder.Remove(sp);
                    Collidable.Remove(sp);
                    toRemove.Add(sp);
                }

                if (sp.GetType().BaseType == typeof(Gusto.Models.Ship))
                {
                    Ship ship = (Ship)sp;
                    ship.Update(kstate, gameTime, windDirection, windSpeed, this.camera);
                }
                else if (sp.GetType() == typeof(Gusto.AnimatedSprite.BaseTower))
                {
                    Tower ship = (Tower)sp;
                    tower.Update(kstate, gameTime);
                }
                else if (sp.GetType() == typeof(Gusto.AnimatedSprite.PiratePlayer))
                {
                    PlayerPirate pirate = (PlayerPirate)sp;
                    pirate.Update(kstate, gameTime, this.camera);
                }
            }


            // clear any "dead" objects from updating
            foreach (var r in toRemove)
                UpdateOrder.Remove(r);
            
            // reset collidable with "alive" objects and map pieces that are in view 
            Collidable.Clear();
            foreach (var sp in UpdateOrder)
            {
                Collidable.Add(sp);
                SpatialBounding.SetQuad(sp.GetBase());
            }

            // set any visible collidable map pieces for collision
            foreach (var tile in BoundingBoxLocations.LandTileLocationList)
                SpatialBounding.SetQuad(tile.GetBase());

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw map
            map.DrawMap(spriteBatchView);

            // draw sprites that don't move
            windArrows.Draw(spriteBatchStatic, null);

            // sort sprites by y cord asc and draw
            DrawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
            foreach (var sprite in DrawOrder)
            {
                // Draw a ships sail before a ship
                if (sprite.GetType().BaseType == typeof(Gusto.Models.Ship))
                {
                    Ship ship = (Ship) sprite;
                    ship.DrawAnchorMeter(spriteBatchStatic, new Vector2(1660, 30), anchorIcon);
                    ship.DrawHealthBar(spriteBatchView, camera);

                    if (ship.sinking)
                    {
                        ship.DrawSinking(spriteBatchView, this.camera);
                        ship.shipSail.DrawSinking(spriteBatchView, this.camera);
                    }
                    else
                    {
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
                    if (pirate.inCombat && pirate.currRowFrame == 3) // draw sword before pirate when moving up
                        pirate.playerSword.Draw(spriteBatchView, this.camera);
                    if (pirate.nearShip)
                        pirate.DrawEnterShip(spriteBatchView, this.camera);
                    else if (pirate.onShip)
                        pirate.DrawOnShip(spriteBatchView, this.camera);

                    if (pirate.swimming && !pirate.onShip)
                        pirate.DrawSwimming(spriteBatchView, this.camera);
                    else if (!pirate.onShip)
                        pirate.Draw(spriteBatchView, this.camera);

                    if (pirate.inCombat && pirate.currRowFrame != 3)
                        pirate.playerSword.Draw(spriteBatchView, this.camera);

                    continue;
                }

                else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.BaseTower))
                {
                    Tower tower = (Tower) sprite;
                    tower.DrawHealthBar(spriteBatchView, this.camera);
                    sprite.Draw(spriteBatchView, this.camera);
                    // draw any shots this tower has in motion
                    foreach (var shot in tower.Shots)
                        shot.Draw(spriteBatchView, this.camera);
                    continue;
                }
                sprite.Draw(spriteBatchView, this.camera);
            }
            base.Draw(gameTime);
        }

        private void SpatialCollision()
        {
            foreach (var team in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
                BoundingBoxLocations.BoundingBoxLocationMap[team].Clear();

            foreach (var spriteA in Collidable)
            {
                if (spriteA.GetType().BaseType == typeof(Gusto.Models.Ship))
                {
                    Ship ship = (Ship)spriteA;
                    BoundingBoxLocations.BoundingBoxLocationMap[ship.teamType].Add(new Tuple<int, int>(spriteA.GetBoundingBox().X, spriteA.GetBoundingBox().Y));
                }
                else if (spriteA.GetType().BaseType == typeof(Gusto.Models.Tower))
                {
                    Tower tower = (Tower)spriteA;
                    BoundingBoxLocations.BoundingBoxLocationMap[tower.teamType].Add(new Tuple<int, int>(spriteA.GetBoundingBox().X, spriteA.GetBoundingBox().Y));
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
