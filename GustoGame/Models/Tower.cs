using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class Tower : Sprite
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public int timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public int maxShotsMoving;
            
        Random randomGeneration;
        public List<CannonBall> Shots;

        public Tower(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            Shots = new List<CannonBall>();
            randomGeneration = new Random();
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // Only stop movement when colliding at the bottom of the tower
            int movePastTowerThresholdBehind = this.GetBoundingBox().Bottom - 40;
            int movePastTowerThresholdInfront = this.GetBoundingBox().Bottom + 40;
            if (collidedWith.bbKey.Equals("baseShip"))
            {
                if ((overlap.Bottom > movePastTowerThresholdBehind && collidedWith.GetBoundingBox().Bottom <= movePastTowerThresholdInfront))
                {
                    Trace.WriteLine("Collision at base of tower");
                    collidedWith.colliding = true;
                }
                else
                {
                    collidedWith.colliding = false;
                }
            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime)
        {
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            foreach (var shot in Shots)
                shot.Update(kstate, gameTime);

            if (timeSinceLastExpClean > millisecondsExplosionLasts)
            {
                // remove exploded shots
                for (int i = 0; i < Shots.Count; i++)
                {
                    if (Shots[i].exploded || Shots[i].outOfRange)
                        Shots.RemoveAt(i);
                }
                timeSinceLastExpClean -= millisecondsExplosionLasts;
            }

            if (timeSinceLastShot > millisecondsNewShot && Shots.Count < maxShotsMoving)
            {
                BaseCannonBall cannonShot = new BaseCannonBall(location, _content, _graphics);
                Tuple<int, int> shotDirection = BoundingBoxLocations.BoundingBoxLocationMap["baseShip"]; // TODO REMOVE HARDCODE AND SCAN BY TOWER RANGE
                cannonShot.SetFireAtDirection(shotDirection, RandomShotSpeed(), RandomAimOffset());
                cannonShot.moving = true;
                Shots.Add(cannonShot);
                timeSinceLastShot -= millisecondsNewShot;
            }
        }

        private int RandomAimOffset()
        {
            return randomGeneration.Next(-120, 120
                );
        }

        private int RandomShotSpeed()
        {
            return randomGeneration.Next(10, 25);
        }
    }
}
