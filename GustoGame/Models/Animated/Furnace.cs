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

        string oreType;
        public bool canCraft;
        public bool smelting;
        float msPerFrame;
        float msThisFrame;
        float msToSmelt; 
        float msSmelting;

        public Light emittingLight;


        Random rand;
        PiratePlayer playerNearItem;
        public TeamType teamType;

        public Furnace(TeamType type, ContentManager content, GraphicsDevice graphics) : base(graphics)
        {
            _content = content;
            _graphics = graphics;

            teamType = type;
            rand = new Random();
            msToSmelt = 10000;
            msPerFrame = 200;

        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            if (collidedWith.bbKey.Equals("playerPirate") && !canCraft)
            {
                playerNearItem = (PiratePlayer)collidedWith;

                // check inventory to see if we have required materials
                int nWood = 0;
                int nGrass = 0;
                int nCoal = 0;
                int nOre = 0;
                foreach (var item in playerNearItem.inventory)
                {
                    if (item is IWood )
                        nWood = item.amountStacked;
                    if (item is IGrass) 
                        nGrass = item.amountStacked;
                    if (item is IOre)
                    {
                        if (item.GetType() == typeof(Gusto.AnimatedSprite.InventoryItems.Coal))
                            nCoal = item.amountStacked;
                        else
                            nOre = item.amountStacked;
                    }
                }
                canCraft = true; //TEMPORARY@!
                if (nWood > 1 && nGrass > 1 && nCoal > 0 && nOre > 7) // 2 wood, 2 grass, 1 coal, 8 ore required
                    canCraft = true;
            }
        }        
        

        public void Update(KeyboardState kstate, GameTime gameTime, Camera camera)
        {
            if (canCraft && kstate.IsKeyDown(Keys.C)) // TODO: and keypress time
            {
                // TODO: animate

                bool hasOre = false;
                bool hasGrass = false;
                bool hasWood = false;
                bool hasCoal = false;

                // Remove items from inv TODO: for now this just takes the first ore, grass, wood etc in inventory
                if (!smelting)
                {
                    foreach (var item in playerNearItem.inventory)
                    {
                        if (item is IWood && !hasWood)
                        {
                            item.amountStacked -= 2;
                        }
                        if (item is IGrass && !hasGrass)
                        {
                            item.amountStacked -= 2;
                        }
                        if (item is IOre)
                        {
                            if (item.GetType() == typeof(Gusto.AnimatedSprite.InventoryItems.Coal) && !hasCoal)
                                item.amountStacked -= 1;
                            else if (!hasOre)
                            {
                                oreType = CheckOreType(item.GetType());
                                item.amountStacked -= 8;
                            }
                        }
                    }
                    smelting = true;
                }
                    
            }

            if (smelting)
            {
                emittingLight.lit = true;
                // smelting so animate and being timer
                msSmelting += gameTime.ElapsedGameTime.Milliseconds;
                msThisFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (msThisFrame > msPerFrame)
                {
                    currColumnFrame++;
                    msThisFrame = 0;
                    if (currColumnFrame == nColumns)
                        currColumnFrame = 1;
                }
            }

            // create and drop bar when done smelting
            if (msSmelting > msToSmelt)
            {
                if (oreType != null)
                {
                    InventoryItem bar = null;
                    Vector2 dropLoc = new Vector2(GetBoundingBox().Center.ToVector2().X, GetBoundingBox().Center.ToVector2().Y + 40);
                    switch (oreType)
                    {
                        case "iron":
                            bar = new IronBar(teamType, regionKey, dropLoc, _content, _graphics);
                            break;
                    }
                    bar.onGround = true;
                    bar.amountStacked = 1;
                    ItemUtility.ItemsToUpdate.Add(bar);
                }

                // reset
                smelting = false;
                msSmelting = 0;
                oreType = null;
                currColumnFrame = 0;
                emittingLight.lit = false;
            }

            // lighting the furnace when running
            if (emittingLight.lit)
                emittingLight.Update(kstate, gameTime, GetBoundingBox().Center.ToVector2());


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
