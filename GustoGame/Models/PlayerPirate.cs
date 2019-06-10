using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
using GustoGame.Mappings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class PlayerPirate : Sprite, IWalks, IVulnerable, ICanUpdate
    {
        public float timeSinceLastTurnFrame;
        public float timeSinceLastWalkFrame;
        public float timeSinceSwordSwing;
        public float timeSinceExitShipStart;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        public float millisecondsCombatSwing;

        public float health;
        public float fullHealth;
        private bool showHealthBar;
        private int timeShowingHealthBar;

        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public Ship playerOnShip;
        public Sword playerSword;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;

        public PlayerPirate(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;

            timeShowingHealthBar = 0;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("landTile"))
            {
                colliding = false;
                swimming = false;
            }
            else if (collidedWith is IShip)
            {
                if (!onShip)
                {
                    nearShip = true;
                    playerOnShip = (Ship)collidedWith;
                }
            }
            else if (collidedWith.GetType().BaseType == typeof(Gusto.Models.GroundEnemy))
            {
                GroundEnemy enemy = (GroundEnemy)collidedWith;
                colliding = false;
                if (enemy.inCombat)
                {
                    showHealthBar = true;
                    health -= enemy.damage;
                }
            }
            else if (collidedWith is IWalks)
            {
                colliding = false;
            }
            else if (collidedWith.bbKey.Equals("baseCannonBall"))
            {
                showHealthBar = true;
                CannonBall ball = (CannonBall)collidedWith;
                if (!ball.exploded)
                    health -= 15;
                return;
            }

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurnFrame += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastWalkFrame += gameTime.ElapsedGameTime.Milliseconds;

            if (showHealthBar)
                timeShowingHealthBar += gameTime.ElapsedGameTime.Milliseconds;
            if (timeShowingHealthBar > GameOptions.millisecondsToShowHealthBar)
            {
                showHealthBar = false;
                timeShowingHealthBar = 0;
            }

            if (colliding)
                moving = false;

            colliding = false;
            swimming = true;

            if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
            {
                if (!onShip)
                {
                    moving = true;
                    // player direction
                    if (kstate.IsKeyDown(Keys.W))
                    {
                        currRowFrame = 3;
                        directionalFrame = 0;
                        if (kstate.IsKeyDown(Keys.A))
                            directionalFrame = 1;
                        else if (kstate.IsKeyDown(Keys.D))
                            directionalFrame = 7;
                    }
                    else if (kstate.IsKeyDown(Keys.S))
                    {
                        currRowFrame = 0;
                        directionalFrame = 4;
                        if (kstate.IsKeyDown(Keys.A))
                            directionalFrame = 3;
                        else if (kstate.IsKeyDown(Keys.D))
                            directionalFrame = 5;
                    }
                    else if (kstate.IsKeyDown(Keys.A))
                    {
                        currRowFrame = 2;
                        directionalFrame = 2;
                    }
                    else if (kstate.IsKeyDown(Keys.D))
                    {
                        currRowFrame = 1;
                        directionalFrame = 6;
                    }
                    else
                    {
                        moving = false;
                    }
                    playerSword.currRowFrame = currRowFrame;
                } else
                {
                    moving = false;
                }

                timeSinceLastTurnFrame -= millisecondsPerTurnFrame;
            }

            // combat 
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                inCombat = true;
                currColumnFrame = 8;
                playerSword.currColumnFrame = 0;
                playerSword.location = location;
            }
            else if (inCombat)
            {
                if (timeSinceSwordSwing > millisecondsCombatSwing)
                {

                    currColumnFrame++;
                    playerSword.location = location;
                    playerSword.Update(kstate, gameTime, currRowFrame);
                    if (currColumnFrame == nColumns)
                    {
                        inCombat = false;
                        currColumnFrame = 0;
                        playerSword.currColumnFrame = 0;
                    }
                    timeSinceSwordSwing = 0;
                }
                timeSinceSwordSwing += gameTime.ElapsedGameTime.Milliseconds;
            }

            // hop on ship
            if (nearShip && kstate.IsKeyDown(Keys.X) && !onShip && timeSinceExitShipStart < 2000)
            {
                location = playerOnShip.GetBoundingBox().Center.ToVector2();
                onShip = true;
                playerOnShip.playerAboard = true;
                playerOnShip.shipSail.playerAboard = true;
            }
            // exit ship
            else if (kstate.IsKeyDown(Keys.X) && onShip)
            {
                timeSinceExitShipStart += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceExitShipStart > 2000)
                {
                    onShip = false;
                    playerOnShip.playerAboard = false;
                    playerOnShip.shipSail.playerAboard = false;
                    location.X = playerOnShip.GetBoundingBox().Center.ToVector2().X - 70;
                    location.Y = playerOnShip.GetBoundingBox().Center.ToVector2().Y;
                    playerOnShip = null;
                }
            }
            else
            {
                timeSinceExitShipStart = 0;
            }
            nearShip = false;

            if (onShip)
            {
                location.X = playerOnShip.GetBoundingBox().Center.ToVector2().X;
                location.Y = playerOnShip.GetBoundingBox().Center.ToVector2().Y;
            }
            else if (moving && !inCombat)
            {
                // walking animation
                if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame == 7) // stop before combat frames
                        currColumnFrame = 0;
                    timeSinceLastWalkFrame = 0;
                }

                // actual "regular" movement
                location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1);
                location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2);
            }
            else
            {
                if (!inCombat)
                {
                    currColumnFrame = 0;
                }
            }
        }

        public void DrawSwimming(SpriteBatch sb, Camera camera)
        {
            Rectangle tRect = new Rectangle(targetRectangle.X, targetRectangle.Y, targetRectangle.Width, targetRectangle.Height);
            tRect.X = (_texture.Width / nColumns) * currColumnFrame;
            tRect.Y = (_texture.Height / nRows) * currRowFrame;

            // cut the bottom half of the targetRectangle off to hide the "under water" portion of the body
            tRect.Height = (_texture.Height / nRows) / 2;

            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            targetRectangle.Width = (_texture.Width / nColumns);
            targetRectangle.Height = tRect.Height;

            SetBoundingBox();
            sb.Begin(camera);
            sb.Draw(_texture, location, tRect, Color.White , 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }

        public void DrawEnterShip(SpriteBatch sb, Camera camera)
        {
            SpriteFont font = _content.Load<SpriteFont>("helperFont");
            sb.Begin(camera);
            sb.DrawString(font, "x", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
            sb.End();
        }

        public void DrawOnShip(SpriteBatch sb, Camera camera)
        {

            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            targetRectangle.Width = (_texture.Width / nColumns);
            targetRectangle.Height = (_texture.Height / nRows);

            SetBoundingBox();
            sb.Begin(camera);
            sb.Draw(_texture, location, targetRectangle, Color.White * 0.0f, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
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
                Rectangle dead = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 130, 60, 7);
                Rectangle alive = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 130, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
        }
    }
}
