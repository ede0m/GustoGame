using Comora;
using Gusto.AnimatedSprite;
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
    public class PlayerPirate : Sprite
    {
        public float timeSinceLastTurnFrame;
        public float timeSinceLastWalkFrame;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool nearShip;
        public bool onShip;
        public Ship playerOnShip;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;

        public PlayerPirate(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;
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

        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            timeSinceLastTurnFrame += gameTime.ElapsedGameTime.Milliseconds;
            timeSinceLastWalkFrame += gameTime.ElapsedGameTime.Milliseconds;

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
                } else
                {
                    moving = false;
                }

                timeSinceLastTurnFrame -= millisecondsPerTurnFrame;
            }

            if (nearShip && kstate.IsKeyDown(Keys.E))
            {
                location = playerOnShip.GetBoundingBox().Center.ToVector2();
                onShip = true;
                playerOnShip.playerAboard = true;
                playerOnShip.shipSail.playerAboard = true;
            }
            nearShip = false;

            if (onShip)
            {
                location.X = playerOnShip.GetBoundingBox().Center.ToVector2().X;
                location.Y = playerOnShip.GetBoundingBox().Center.ToVector2().Y;
            }
            else if (moving)
            {
                // walking animation
                if (timeSinceLastWalkFrame > millisecondsPerWalkFrame)
                {
                    currColumnFrame++;
                    if (currColumnFrame == nColumns)
                        currColumnFrame = 0;
                    timeSinceLastWalkFrame = 0;
                }

                // actual movement
                location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1);
                location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2);
            } else
            {
                currColumnFrame = 0;
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
            sb.DrawString(font, "e", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
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
    }
}
