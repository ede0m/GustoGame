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


        GameMapProcedural map;

        // static
        WindArrows windArrows;
        Texture2D anchorIcon;
 
        SpatialBounding collision;
        GraphicsDeviceManager graphics;
        List<Sprite> DrawOrder;
        List<Sprite> Collidable;
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
            this.camera = new Camera(GraphicsDevice);
            collision = new SpatialBounding(new Rectangle(0, 0, GameOptions.PrefferedBackBufferWidth, GameOptions.PrefferedBackBufferHeight), this.camera);
            map = new GameMapProcedural(this.camera);
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

            // PREPROCESSING
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseShip, "baseShip", 0.6f);
            //textureBaseShip.Dispose();
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(8, 3, textureBaseSail, "baseSail", 0.6f);
            //textureBaseSail.Dispose();
            Texture2D textureTower = Content.Load<Texture2D>("tower");
            LoadDynamicBoundingBoxPerFrame(1, 1, textureTower, "tower", 0.5f);
            //textureTower.Dispose();
            Texture2D textureCannonBall = Content.Load<Texture2D>("CannonBall");
            LoadDynamicBoundingBoxPerFrame(1, 2, textureCannonBall, "baseCannonBall", 1.0f);
            //
            Texture2D textureBaseCannon = Content.Load<Texture2D>("BaseCannon");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseCannon, "baseCannon", 1.0f);
            // 
            Texture2D textureOcean1 = Content.Load<Texture2D>("Ocean1");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureOcean1, "oceanTile", 1.0f);

            var screenCenter = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2);

            // create Team models and initally place them
            baseShip = new BaseShip(TeamType.Player, screenCenter, Content, GraphicsDevice);
            tower = new BaseTower(TeamType.A, new Vector2(200, 700), Content, GraphicsDevice);
            baseShipAI = new BaseShip(TeamType.A, new Vector2(200, 300), Content, GraphicsDevice);

            // static 
            windArrows = new WindArrows(new Vector2(1740, 50), Content, GraphicsDevice);
            anchorIcon = Content.Load<Texture2D>("anchor-shape");

            // Game Map
            map.SetGameMap(Content, GraphicsDevice);

            // fill draw order list
            DrawOrder.Add(baseShip);
            DrawOrder.Add(baseShipAI);
            DrawOrder.Add(tower);
            // fill collidable list
            Collidable.Add(baseShip);
            SpatialBounding.SetQuad(baseShip.GetBase());
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
            // camera follows player
            this.camera.Position = baseShip.location;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var kstate = Keyboard.GetState();

            //QuadTreeCollision(DrawOrder, gameTime);

            // Wind
            windArrows.Update(kstate, gameTime);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();
            // Tower
            tower.Update(kstate, gameTime);
            // ship AI
            baseShipAI.Update(kstate, gameTime, windDirection, windSpeed, this.camera);
            //baseShipAI.shipSail.Update(kstate, gameTime, windDirection, windSpeed);

            // Ship & Sail TEMPORARY -- hardcode one baseShip and baseSail to update
            baseShip.Update(kstate, gameTime, windDirection, windSpeed, this.camera);
            //baseShip.shipSail.Update(kstate, gameTime, windDirection, windSpeed);

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
                } else if (sprite.GetType() == typeof(Gusto.AnimatedSprite.BaseTower))
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
                List<string> quadKeys = collision.GetQuadKey(bbA);
                List<Sprite> possible = new List<Sprite>();
                foreach (var key in quadKeys)
                {
                    possible.AddRange(collision.GetSpatialBoundingMap()[key]);
                }
                
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
