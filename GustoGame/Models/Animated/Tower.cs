using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Mappings;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gusto.Models.Animated
{
    public class Tower : Sprite, ICanUpdate
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public int timeSinceLastShot;
        public int timeSinceLastExpClean;
        public int timeShowingHealthBar;
        public int millisecondsNewShot;
        public int millisecondsExplosionLasts;
        public int millisecondsToShowHealthBar;
        public int maxShotsMoving;
        public float range;
        public float health;
        public float fullHealth;
        private bool showHealthBar;
            
        public TeamType teamType;
        public List<Ammo> Shots;

        public Tower(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            Shots = new List<Ammo>();

            millisecondsToShowHealthBar = 6000;
            timeShowingHealthBar = 0;
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
                    showHealthBar = true;
                    Trace.WriteLine("Collision at base of tower");
                    collidedWith.colliding = true;
                }
                else
                {
                    collidedWith.colliding = false;
                }
            } 
            else if (collidedWith.bbKey.Equals("baseCannonBall"))
            {
                showHealthBar = true;
                Ammo ball = (Ammo)collidedWith;
                if (!ball.exploded)
                    health -= 5;
            }

            collidedWith.colliding = false;
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastExpClean += gameTime.ElapsedGameTime.Milliseconds;

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

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

            if (timeSinceLastShot > millisecondsNewShot && Shots.Count < maxShotsMoving && health > 0)
            {
                Vector2? shotDirection = AIUtility.ChooseTarget(teamType, range, GetBoundingBox(), inInteriorId);
                if (shotDirection != null)
                {
                    BaseCannonBall cannonShot = new BaseCannonBall(teamType, regionKey, location, _content, _graphics);
                    cannonShot.SetFireAtDirection(shotDirection.Value, RandomEvents.rand.Next(10, 25), RandomEvents.rand.Next(-100, 100)); // 3rd param is aim offset
                    cannonShot.moving = true;
                    Shots.Add(cannonShot);
                }
                timeSinceLastShot = 0;
            }

            SpatialBounding.SetQuad(GetBase());
        }

        public void DrawHealthBar(SpriteBatch sb, Camera camera)
        {
            if (showHealthBar)
            {
                Texture2D meterAlive = new Texture2D(_graphics, 1, 1);
                Texture2D meterDead = new Texture2D(_graphics, 1, 1);
                meterAlive.SetData<Color>(new Color[] { Color.DarkKhaki });
                meterDead.SetData<Color>(new Color[] { Color.IndianRed });
                float healthLeft = (1f - (1f - (health / fullHealth))) * 60f;
                Rectangle dead = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 150, 60, 7);
                Rectangle alive = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 150, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
        }
    }
}
