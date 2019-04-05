using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public float range;
            
        Random rand;
        public TeamType teamType;
        public List<CannonBall> Shots;

        public Tower(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            Shots = new List<CannonBall>();
            rand = new Random();
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
                timeSinceLastExpClean = 0;
            }

            if (timeSinceLastShot > millisecondsNewShot && Shots.Count < maxShotsMoving )
            {
                Tuple<int, int> shotDirection = ChooseTarget();
                if (shotDirection != null)
                {
                    BaseCannonBall cannonShot = new BaseCannonBall(location, _content, _graphics);
                    cannonShot.SetFireAtDirection(shotDirection, RandomEvents.RandomShotSpeed(rand), RandomEvents.RandomAimOffset(rand));
                    cannonShot.moving = true;
                    Shots.Add(cannonShot);
                }
                timeSinceLastShot = 0;
            }
        }

        private Tuple<int, int> ChooseTarget()
        {
            foreach (var otherTeam in BoundingBoxLocations.BoundingBoxLocationMap.Keys)
            {
                if (AttackMapping.AttackMappings[teamType][otherTeam])
                {
                    Tuple<int, int> shotCords = BoundingBoxLocations.BoundingBoxLocationMap[otherTeam][0];// TODO REMOVE HARDCODED random target (pick team member with lowest health)
                    float vmag = PhysicsUtility.VectorMagnitude(shotCords.Item1, GetBoundingBox().X, shotCords.Item2, GetBoundingBox().Y);
                    if (vmag <= range)
                        return shotCords;
                }
            }
            return null;
        }
    }
}
