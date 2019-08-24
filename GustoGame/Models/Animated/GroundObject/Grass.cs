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
    public class Grass : Sprite, ICanUpdate, IGroundObject
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
        float msAnimate;
        float msNow;

        public List<InventoryItem> drops;


        public Grass(TeamType t, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            team = t;
            _content = content;
            _graphics = graphics;
            msAnimate = 1000;
            msNow = msAnimate;

            drops = new List<InventoryItem>();

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // grass walk through animation
            if (collidedWith is IWalks && collidedWith.moving && !animateWalkThrough)
                animateWalkThrough = true;
            if (collidedWith is IHandHeld)
            {
                HandHeld hh = (HandHeld)collidedWith;
                if (hh.inCombat)
                    harvesting = true;
            }

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            // animate walkthrough
            if (animateWalkThrough && msNow > 0)
            {
                if (msNow > msAnimate - (msAnimate / 4))
                    rotation += 0.01f;
                else if (msNow > msAnimate - ((msAnimate / 4) * 3))
                    rotation -= 0.01f;
                else
                    rotation += 0.01f;

                msNow -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                animateWalkThrough = false;
                msNow = msAnimate;
                rotation = 0;
            }

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
                        item.location.X = location.X + RandomEvents.rand.Next(-10, 10);
                        item.location.Y = location.Y + RandomEvents.rand.Next(-10, 10);
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
                if (timeSinceLastFrame > 200) // frame time
                {
                    animateHarvest = false;
                    timeSinceLastFrame = 0;
                }
            }

            harvesting = false;
                

        }

        public void UpdateRespawn(GameTime gt)
        {
            respawnTimeCountMs += gt.ElapsedGameTime.Milliseconds;
            if (respawnTimeCountMs > msRespawn)
            {
                SetTileDesignRow(RandomEvents.rand.Next(0, nRows));
                location.X += RandomEvents.rand.Next(-GameOptions.tileWidth, GameOptions.tileWidth);
                location.Y += RandomEvents.rand.Next(-GameOptions.tileHeight, GameOptions.tileHeight);
                List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(bbKey, 2);
                drops = ItemUtility.CreateNPInventory(itemDrops, team, regionKey, location, _content, _graphics);
                remove = false;
                respawnTimeCountMs = 0;
                nHits = 0;
            }
        }
    }
}
