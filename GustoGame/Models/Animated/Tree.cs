using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gusto.Models.Animated
{
    public class Tree : Sprite, ICanUpdate, IGroundObject
    {

        int timeSinceLastFrame;
        int respawnTimeCountMs;
        public int msRespawn;

        TeamType team;
        ContentManager _content;
        GraphicsDevice _graphics;

        bool objectHit;
        bool animate;
        bool animateLeft;
        bool animateRight;
        bool animateUp; // TODO
        bool animateDown;

        public List<InventoryItem> drops;
        public int nHitsToDestory;
        int nHits;

        Vector2 startingLoc;
        public Random rand;

        public Tree (TeamType t, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;

            drops = new List<InventoryItem>();
            rand = new Random();
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IHandHeld && !(collidedWith is IRanged))
            {
                objectHit = true;
                animateLeft = animateRight = false;

                if (collidedWith.GetBoundingBox().Left < GetBoundingBox().Left)  //left side collision
                    animateRight = true;
                else if (collidedWith.GetBoundingBox().Right > GetBoundingBox().Right) // right side collision
                    animateLeft = true;

                // one or the other
                animateLeft = !animateRight;

            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {

            // hitting a tree
            if (objectHit && !animate)
            {
                startingLoc = location;
                animate = true;
                nHits++;

                // drop items
                if (nHits == nHitsToDestory)
                {
                    foreach (var item in drops)
                    {
                        item.inInventory = false;
                        // scatter items
                        item.location.X = location.X + rand.Next(-10, 10);
                        item.location.Y = location.Y + rand.Next(-10, 10);
                        item.onGround = true;
                        ItemUtility.ItemsToUpdate.Add(item);
                    }
                    drops.Clear();
                    remove = true;
                }
            }

            if (animate) { 
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > 100)
                {
                    if (currColumnFrame < nColumns - 1)
                        currColumnFrame++;
                    else
                    {
                        currColumnFrame = 0;
                        animate = false;
                    }

                    // movement animation
                    if (currColumnFrame == 1)
                    {
                        if (animateRight)
                            location.X += 2;
                        else
                            location.X -= 2;

                    }
                    else
                        location = startingLoc;
                        

                    timeSinceLastFrame = 0;
                }
            }
            objectHit = false;
        }

        public void UpdateRespawn(GameTime gt)
        {
            respawnTimeCountMs += gt.ElapsedGameTime.Milliseconds;
            if (respawnTimeCountMs > msRespawn)
            {
                SetTileDesignRow(RandomEvents.RandomSelection(nRows, rand));
                location.X += RandomEvents.RandomSelectionRange(GameOptions.tileWidth, rand);
                location.Y += RandomEvents.RandomSelectionRange(GameOptions.tileHeight, rand);
                List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(bbKey, rand, 2);
                drops = ItemUtility.CreateNPInventory(itemDrops, team, regionKey, location, _content, _graphics);
                remove = false;
                respawnTimeCountMs = 0;
                nHits = 0;
            }
        }
    }
}
