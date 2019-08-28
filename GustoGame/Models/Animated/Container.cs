using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models.Animated
{
    public class Container : Sprite, ICanUpdate, IDrops
    {
        public bool inWater;
        public int nHitsToDestroy;
        int nHits;
        bool harvesting;
        bool animateHarvest;
        bool animateLeft;
        bool animateRight;
        float msPerFrame;
        float msThisFrame;
        float msNow;
        float msAnimate;

        public Guid inInteriorId;

        public List<InventoryItem> drops;

        public Container(TeamType type, string region, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            msPerFrame = 250;
            msAnimate = 200;

            inInteriorId = Guid.Empty;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("landTile") || collidedWith.bbKey.Equals("interiorTile"))
            {
                inWater = false;
            }

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
                if (nHits == nHitsToDestroy)
                {
                    foreach (var item in drops)
                    {
                        item.inInventory = false;
                        // scatter items
                        item.location.X = location.X + RandomEvents.rand.Next(-10, 10);
                        item.location.Y = location.Y + RandomEvents.rand.Next(-10, 10);
                        item.onGround = true;

                        if (inInteriorId != Guid.Empty) // add drops to interior
                            BoundingBoxLocations.interiorMap[inInteriorId].interiorObjectsToAdd.Add(item);
                        else // add drops to world
                            ItemUtility.ItemsToUpdate.Add(item);
                    }
                    drops.Clear();
                    remove = true;
                }
            }

            if (animateHarvest)
            {
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

            if (inWater)
            {
                currRowFrame = 1;
                msThisFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msThisFrame > msPerFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame >= nColumns)
                        currColumnFrame = 0;
                    msThisFrame = 0;
                }
            }
            else
                currRowFrame = 0;
            inWater = true;
     
        }
    }
}
