using Gusto.AnimatedSprite;
using Gusto.Bounds;
using Gusto.Models;
using Gusto.Utility;
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
        Tower tower;
        WindArrows windArrows;

        QuadTreeCollision quad = new QuadTreeCollision(0, new Rectangle(0, 0, 1400, 1000));
        GraphicsDeviceManager graphics;
        List<Sprite> DrawOrder;
        SpriteBatch spriteBatch;
        
        public GameGusto()
        {
            GameOptions.ShowBoundingBox = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1400;
            graphics.PreferredBackBufferHeight = 1200;
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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // PREPROCESSING
            Texture2D textureBaseShip = Content.Load<Texture2D>("BaseShip");
            LoadDynamicBoundingBoxPerFrame(8, 1, textureBaseShip, "baseShip", 0.6f);
            //GetSailMountCords(textureBaseShip, 8, 1, "baseShip");
            //textureBaseShip.Dispose();
            Texture2D textureBaseSail = Content.Load<Texture2D>("DecomposedBaseSail");
            LoadDynamicBoundingBoxPerFrame(8, 3, textureBaseSail, "baseSail", 0.6f);
            //GetSailMountCords(textureBaseSail, 8, 3, "baseSail");
            //textureBaseSail.Dispose();
            Texture2D textureTower = Content.Load<Texture2D>("tower");
            LoadDynamicBoundingBoxPerFrame(1, 1, textureTower, "tower", 0.5f);
            //textureTower.Dispose();



            // create models and initally place them
            baseShip = new BaseShip(new Vector2(1000, 800), Content, GraphicsDevice);
            tower = new Tower(new Vector2(600, 300), Content, GraphicsDevice);
            windArrows = new WindArrows(new Vector2(1250, 0), Content, GraphicsDevice);
            
            
            // fill draw order list
            DrawOrder = new List<Sprite>();
            DrawOrder.Add(baseShip);
            // TODO figure out how to get a ships sail in the draw order
            DrawOrder.Add(tower);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var kstate = Keyboard.GetState();

            // quadtree collision handling
            quad.Clear();
            foreach (var sprite in DrawOrder) // TODO: could use just a list of colliadable objects here (e.g. dont use wind arrows)
                quad.Insert(sprite);
            List<Sprite> collidable = new List<Sprite>();
            foreach (var spriteA in DrawOrder)
            {
                Rectangle bbA = spriteA.GetBoundingBox();
                quad.Retrieve(collidable, spriteA); //adds objects to collidable list if it is in quadrent of this sprite
                foreach (var spriteB in collidable)
                {
                    if (spriteB == spriteA)
                        continue;

                    // Reset any collision values on the sprites that need to be
                    spriteA.moving = true;
                    spriteB.moving = true;

                    Rectangle bbB = spriteB.GetBoundingBox();
                    if (bbA.Intersects(bbB))
                    {
                        Rectangle overlap = Rectangle.Intersect(bbA, bbB);
                        spriteA.HandleCollision(spriteB, overlap);
                        spriteB.HandleCollision(spriteA, overlap);
                    }
                }
            }

            // Wind
            windArrows.Update(kstate, gameTime);
            int windDirection = windArrows.getWindDirection();
            int windSpeed = windArrows.getWindSpeed();
            // Ship & Sail TEMPORARY -- hardcode one baseShip and baseSail to update
            baseShip.Update(kstate, gameTime, windDirection, windSpeed);
            baseShip.shipSail.Update(kstate, gameTime, windDirection, windSpeed);

            // Tower
            tower.Update(kstate, gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw sprites that don't move
            windArrows.Draw(spriteBatch);

            // sort sprites by y cord asc and draw
            DrawOrder.Sort((a, b) => a.GetYPosition().CompareTo(b.GetYPosition()));
            foreach (var sprite in DrawOrder)
            {
                // Draw a ships sail before a ship
                if(sprite.GetType().BaseType == typeof(Gusto.Models.Ship))
                {
                    Ship ship = (Ship) sprite;
                    sprite.Draw(spriteBatch);
                    ship.shipSail.Draw(spriteBatch);
                    continue;
                }
                sprite.Draw(spriteBatch);
            }
            base.Draw(gameTime);
        }
    }
}
