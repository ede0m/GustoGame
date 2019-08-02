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
    public class Furnace : Sprite, ICanUpdate, IPlaceable, ICraftingObject
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public bool canCraft;
        float msPerFrame;


        Random rand;
        PiratePlayer playerNearItem;
        public TeamType teamType;

        public Furnace(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            rand = new Random();

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("playerPirate") && !canCraft)
            {
                playerNearItem = (PiratePlayer)collidedWith;

                // check inventory to see if we have required materials
                bool hasWood = false;
                bool hasGrass = false;
                bool hasCoal = false;
                bool hasOre = false;
                string oreType = null;
                foreach (var item in playerNearItem.inventory)
                {
                    if (item is IWood && item.amountStacked >= 2) // 2 wood needed to start furnace
                        hasWood = true;
                    if (item is IGrass && item.amountStacked >= 2) // 2 grass needed to start furnace
                        hasGrass = true;
                    if (item is IOre)
                    {
                        if (item.GetType() == typeof(Gusto.AnimatedSprite.InventoryItems.Coal))
                            hasCoal = true;
                        else
                        {
                            oreType = CheckOreType(item.GetType());
                            if (item.amountStacked >= 8)
                                hasOre = true;
                        }
                    }
                }
                if (hasWood && hasGrass && hasCoal && hasOre)
                {
                    canCraft = true;
                    switch (oreType)
                    {
                        // TODO: take items from inventory and return a new bar of type ore type, then set canCraft = false
                    }
                }
            }
        }        
        

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            if (canCraft)
            {
                if (kstate.IsKeyDown(Keys.C))
                {

                }
            }


            canCraft = false;
        }

        public void DrawCanCraft(SpriteBatch sb, Camera camera)
        {
            if (canCraft)
            {
                SpriteFont font = _content.Load<SpriteFont>("helperFont");
                sb.Begin(camera);
                sb.DrawString(font, "c", new Vector2(GetBoundingBox().X, GetBoundingBox().Y - 50), Color.Black);
                sb.End();
            }
        }

        
        private string CheckOreType(Type t)
        {
            string ret = null;
            if (t == typeof(Gusto.AnimatedSprite.InventoryItems.IronOre))
                ret = "iron";

            return ret;

        }

    }
}
