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
    public class Rock : Sprite, ICanUpdate, IGroundObject
    {

        int timeSinceLastFrame;
        int respawnTimeCountMs;
        public int msRespawn;

        public int nHitsToDestory;
        int nHits;

        TeamType team;
        ContentManager _content;
        GraphicsDevice _graphics;

        Vector2 startingLoc;

        bool animateHarvest;
        bool animateWalkThrough;
        bool harvesting;
        bool animateLeft;
        bool animateRight;
        float msAnimate;
        float msNow;

        public List<InventoryItem> drops;
        public Random rand;


        public Rock(TeamType t, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;
            msAnimate = 100;
            msNow = msAnimate;

            drops = new List<InventoryItem>();
            rand = new Random();

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith is IHandHeld)
            {
                HandHeld hh = (HandHeld)collidedWith;
                if (hh.inCombat)
                    harvesting = true;

                // hit from what direction
                if (!animateHarvest)
                {
                    animateLeft = animateRight = false;

                    if (collidedWith.GetBoundingBox().Left < GetBoundingBox().Left)  //left side collision
                        animateRight = true;
                    else if (collidedWith.GetBoundingBox().Right > GetBoundingBox().Right) // right side collision
                        animateLeft = true;

                    // one or the other
                    animateLeft = !animateRight;
                }

            }

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {


            if (harvesting && !animateHarvest)
            {
                animateHarvest = true;
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

            if (animateHarvest)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                // debris animation
                if (timeSinceLastFrame > 80)
                {
                    currColumnFrame++;
                    if (currColumnFrame >= nColumns)
                        currColumnFrame = 0;
                    timeSinceLastFrame = 0;
                }

                if (msNow > 0)
                {
                    float step = 0.6f;
                    int sign = 0;
                    if (animateRight)
                        sign = 1;
                    else //left
                        sign = -1;
                    step = step * sign;

                    if (msNow > msAnimate - (msAnimate / 2))
                        location.X += step;
                    else
                        location.X -= step;

                    msNow -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    msNow = msAnimate;
                    animateHarvest = false;
                }
            }
            else
                currColumnFrame = 0;

            harvesting = false;
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
