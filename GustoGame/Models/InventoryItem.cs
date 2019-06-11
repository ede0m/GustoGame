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
    public class InventoryItem : Sprite, IInventoryItem, ICanUpdate
    {
        public string itemKey;
        PiratePlayer playerNearItem;

        public bool inInventory;
        public bool canPickUp;
        public bool stackable;
        public int amountStacked;

        private ContentManager _content;
        private GraphicsDevice _graphics;

        public TeamType teamType;

        public InventoryItem(TeamType team, ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;
            teamType = team;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            collidedWith.colliding = false;

            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                canPickUp = true;
                playerNearItem = (PiratePlayer)collidedWith;
            }
        }

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {

            if (kstate.IsKeyDown(Keys.E) && canPickUp && !inInventory)
            {
                remove = true;
                inInventory = true;
                teamType = playerNearItem.teamType;
                playerNearItem.inventory.Add(this);
            }

            canPickUp = false;
        }

        public void DrawPickUp(SpriteBatch sb, Camera camera)
        {
            if (canPickUp)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(camera);
                sb.DrawString(font, "e", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
                sb.End();
            }
        }

    }
}
