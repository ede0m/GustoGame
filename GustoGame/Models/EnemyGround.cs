using Comora;
using Gusto.AnimatedSprite;
using Gusto.Models.Interfaces;
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
    public class EnemyGround : Sprite, IWalks, IVulnerable, ICanUpdate
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
        public Ship playerOnShip;
        public TeamType teamType;

        ContentManager _content;
        GraphicsDevice _graphics;

        public EnemyGround(TeamType type, ContentManager content, GraphicsDevice graphics)
        {
            teamType = type;
            _content = content;
            _graphics = graphics;
            timeShowingHealthBar = 0;
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
