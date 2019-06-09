using Comora;
using Gusto.AnimatedSprite;
using Gusto.Bounding;
using Gusto.Models.Interfaces;
using Gusto.Utility;
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
    public class GroundEnemy : Sprite, IWalks, IVulnerable, ICanUpdate
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
        public bool showHealthBar;
        public int timeShowingHealthBar;

        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public bool roaming;
        public Ship playerOnShip;
        public Sprite randomRegionRoamTile;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;
        Random rand;

        public GroundEnemy(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;
            timeShowingHealthBar = 0;
            rand = new Random();
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Sword))
            {
                Sword sword = (Sword)collidedWith;
                showHealthBar = true;
                health -= sword.damage;
            }
            else if (collidedWith.bbKey.Equals("landTile"))
            {
                colliding = false;
                swimming = false;
            }
            else if (collidedWith is IWalks)
            {
                colliding = false;
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

            // Movement
            if (timeSinceLastTurnFrame > millisecondsPerTurnFrame)
            {
                Tuple<int, int> target = AIUtility.ChooseTarget(teamType, GetBoundingBox().Width * 2, GetBoundingBox());
                if (target != null)
                {
                    if (!inCombat)
                        currColumnFrame = 7;
                    inCombat = true;
                    Vector2 targetV = new Vector2(target.Item1, target.Item2);
                    Tuple<int, int> frames = AIUtility.SetAIGroundMovement(targetV, location);
                    currRowFrame = frames.Item1;
                    directionalFrame = frames.Item2;
                }
                else
                {
                    inCombat = false;
                    if (roaming)
                    {
                        moving = true;
                        // go towards random tile
                        Tuple<int, int> frames = AIUtility.SetAIGroundMovement(randomRegionRoamTile.location, location);
                        currRowFrame = frames.Item1;
                        directionalFrame = frames.Item2;
                        if (GetBoundingBox().Intersects(randomRegionRoamTile.GetBoundingBox()))
                            roaming = false;
                    }
                    else
                    {
                        randomRegionRoamTile = BoundingBoxLocations.RegionMap[regionKey][rand.Next(BoundingBoxLocations.RegionMap[regionKey].Count)];
                        roaming = true;
                    }
                }
                timeSinceLastTurnFrame = 0;
            }
            if (moving && !inCombat)
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
                location.X += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item1 * 0.5f);
                location.Y += (PlayerMovementVectorMappings.PlayerDirectionVectorValues[directionalFrame].Item2 * 0.5f);
            }
            else
            {
                if (timeSinceSwordSwing > millisecondsCombatSwing)
                {
                    currColumnFrame++;
                    if (currColumnFrame == nColumns)
                    {
                        inCombat = false;
                        currColumnFrame = 0;
                    }
                    timeSinceSwordSwing = 0;
                }
                timeSinceSwordSwing += gameTime.ElapsedGameTime.Milliseconds;
            }
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
                Rectangle dead = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 80, 60, 7);
                Rectangle alive = new Rectangle((int)GetBoundingBox().Center.X - 30, (int)GetBoundingBox().Center.Y - 80, (int)healthLeft, 7);
                sb.Begin(camera);
                sb.Draw(meterDead, dead, null, Color.IndianRed, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.Draw(meterAlive, alive, null, Color.DarkSeaGreen, 0, new Vector2(0, 0), SpriteEffects.None, 0);
                sb.End();
            }
        }
    }
}
