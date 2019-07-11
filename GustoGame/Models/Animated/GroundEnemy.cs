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

namespace Gusto.Models.Animated
{
    public class GroundEnemy : Sprite, IWalks, IVulnerable, ICanUpdate
    {
        public float timeSinceLastTurnFrame;
        public float timeSinceLastWalkFrame;
        public float timeSinceSwordSwing;
        public float timeSinceExitShipStart;
        public float timeSinceStartDying;
        public float millisecondsPerTurnFrame;
        public float millisecondsPerWalkFrame;
        public float millisecondsCombatSwing;
        public float millisecondToDie;

        public float health;
        public float fullHealth;
        public float damage;
        public bool showHealthBar;
        public int timeShowingHealthBar;
        public bool dying;
        public float dyingTransparency;

        int directionalFrame; // sprite doesn't have frames for diagnoal, but we still want to use 8 directional movements. So we use dirFrame instead of rowFrame for direction vector values
        public bool swimming;
        public bool nearShip;
        public bool onShip;
        public bool inCombat;
        public bool roaming;
        public List<InventoryItem> inventory;
        public Ship playerOnShip;
        public Sprite randomRegionRoamTile;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;
        public Random rand;

        public GroundEnemy(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;
            timeShowingHealthBar = 0;
            rand = new Random();
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.GetType().BaseType == typeof(Gusto.Models.Animated.HandHeld))
            {
                HandHeld handHeld = (HandHeld)collidedWith;
                showHealthBar = true;
                health -= handHeld.damage;
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
            else if (collidedWith is IAmmo)
            {
                showHealthBar = true;
                Ammo ball = (Ammo)collidedWith;
                if (!ball.exploded)
                    health -= ball.groundDamage;
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

            // dying
            if (health <= 0)
            {
                // drop items
                foreach (var item in inventory)
                {
                    item.inInventory = false;
                    // scatter items
                    item.location.X = location.X + rand.Next(-10, 10);
                    item.location.Y = location.Y + rand.Next(-10, 10);
                    item.onGround = true;
                    ItemUtility.ItemsToUpdate.Add(item);
                }
                inventory.Clear();

                dying = true;
                currRowFrame = 2;

                timeSinceStartDying += gameTime.ElapsedGameTime.Milliseconds;
                dyingTransparency = 1 - (timeSinceStartDying / millisecondToDie);
                if (dyingTransparency <= 0)
                    remove = true;
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
            if (moving && !inCombat && !dying)
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
                if (timeSinceSwordSwing > millisecondsCombatSwing && !dying)
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

        public void DrawDying(SpriteBatch sb, Camera camera)
        {
            targetRectangle.X = (_texture.Width / nColumns) * currColumnFrame;
            targetRectangle.Y = (_texture.Height / nRows) * currRowFrame;
            sb.Begin(camera);
            sb.Draw(_texture, location, targetRectangle, Color.White * dyingTransparency, 0f,
                new Vector2((_texture.Width / nColumns) / 2, (_texture.Height / nRows) / 2), spriteScale, SpriteEffects.None, 0f);
            sb.End();
        }
    }
}
