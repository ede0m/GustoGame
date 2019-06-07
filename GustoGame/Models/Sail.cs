using Comora;
using Gusto.AnimatedSprite;
using Gusto.Utility;
using Microsoft.Xna.Framework;
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
    public class Sail : Sprite
    {
        public int timeSinceLastFrame;
        public int millisecondsPerFrame; // turning speed
        public TeamType teamType;

        public float sailSpeed { get; set; }
        public float sinkingTransparency { get; set; }
        public int windWindowAdd { get; set; } // used for shipWindWindow bounds
        public int windWindowSub { get; set; } // ...
        public int sailIsLeftColumn { get; set; }
        public int sailIsRightColumn { get; set; }
        public int sailPositionInRespectToShip { get; set; }
        public bool sailDirectlyInWind { get; set; }
        public bool playerAboard { get; set; }

        private int windWindowMin;
        private int windWindowMax;
        private int addedWindWindow;
        private int lastWindDir;
        private int lastRowFrame;

        public Sail(TeamType type)
        {
            teamType = type;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("tower"))
            {

            }
        }

        // logic to find correct frame of sprite from user input and update movement values
        public void Update(KeyboardState kstate, GameTime gameTime, int windDir, int windSp)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                if (teamType != TeamType.Player)
                    AIUpdate(windDir);
                else
                    PlayerUpdate(kstate);
                timeSinceLastFrame -= millisecondsPerFrame;
            }
            lastWindDir = windDir;
            lastRowFrame = currRowFrame;
        }

        public void DrawSinking(SpriteBatch sb, Camera camera)
        {
            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            sb.Begin(camera);
            sb.Draw(_texture, location, targetRectangle, Color.White * sinkingTransparency, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }

        private void AIUpdate(int windDir)
        {

            // AI sail movement .. needs work lolz
            int sailRangeMin = currRowFrame + 1;
            int sailRangeMax = currRowFrame - 1;
            if (sailRangeMin > nRows - 1)
                sailRangeMin = 0;
            if (sailRangeMax < 0)
                sailRangeMax = nRows - 1;
            if (currRowFrame == windWindowMin || currRowFrame == windWindowMax || currRowFrame == addedWindWindow || currRowFrame == windDir)
            {
                // can we adjust sail to get direct wind?
                if ((sailRangeMin == windDir || sailRangeMax == windDir || currRowFrame == windDir) && !sailDirectlyInWind && moving)
                {
                    if (currColumnFrame == nColumns - 1)
                    {
                        currColumnFrame = 0;
                    }
                    else
                        currColumnFrame++;
                }
            }
            else
            {
                // can sail be moved into wind window?
                if ((sailRangeMax == windWindowMax || sailRangeMin == windWindowMax || sailRangeMax == windWindowMin || sailRangeMin == windWindowMin) && moving)
                {
                    if (currColumnFrame == nColumns - 1)
                    {
                        currColumnFrame = 0;
                    }
                    else
                        currColumnFrame++;
                }

            }
            Tuple<int, int> frames = BoundFrames(currRowFrame, currColumnFrame);
            currColumnFrame = frames.Item2;
            currRowFrame = frames.Item1;
        }

        private void PlayerUpdate(KeyboardState kstate)
        {
            
            if (playerAboard)
            {
                // sail turning
                if (kstate.IsKeyDown(Keys.LeftShift))
                {
                    if (kstate.IsKeyDown(Keys.A))
                        currColumnFrame++;
                    else if (kstate.IsKeyDown(Keys.D))
                        currColumnFrame--;
                }
                else
                {
                    // sail frame follows ship direction
                    if (kstate.IsKeyDown(Keys.A))
                        currRowFrame++;
                    else if (kstate.IsKeyDown(Keys.D))
                        currRowFrame--;
                }
            }
            Tuple<int, int> frames = BoundFrames(currRowFrame, currColumnFrame);
            currColumnFrame = frames.Item2;
            currRowFrame = frames.Item1;
        }

        public void SetWindWindow(int shipWindWindowMin, int shipWindWindowMax, int addedShipWindWindow)
        {
            windWindowMin = shipWindWindowMin;
            windWindowMax = shipWindWindowMax;
            addedWindWindow = addedShipWindWindow;
        }
    }
}
