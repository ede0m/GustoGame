using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.Models.Animated;
using Gusto.Models.Interfaces;
using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.Models
{
    public class TilePiece : Sprite, ITilePiece
    {

        GraphicsDevice _graphics;
        ContentManager _content;

        public int tileKey;

        public List<Sprite> groundObjects; // any tree, rock, etc asset we want to set to this tile.
        public bool wallPiece;
        public bool shorePiece;
        public bool shallowWaterPiece;

        // for a* path finding
        public byte weight;
        //public Point? tileGridPoint;
        

        // for digging in a tile
        bool shoveled;
        bool fill;

        public bool canFillHole;

        public TilePiece(int key, Point? p, List<Sprite> _groundObjects, ContentManager content, GraphicsDevice graphics) : base(null)
        {
            mapCordPoint = p;
            weight = 0;
            _graphics = graphics;
            _content = content;
            tileKey = key;
            groundObjects = _groundObjects;
        }

        public override void HandleCollision(Sprite collidedWith, Rectangle overlap)
        {
            // land tiles are only run through collision detection


            // shoveling && treasure
            if (collidedWith.bbKey.Equals("playerPirate"))
            {
                PiratePlayer p = (PiratePlayer)collidedWith;
                if (p.inHand != null && p.inHand.bbKey.Equals("shovel") && p.inHand.inCombat)
                {
                    Rectangle bottomOfShovel = new Rectangle(p.inHand.GetBoundingBox().Left, p.inHand.GetBoundingBox().Bottom - (p.inHand.GetBoundingBox().Height / 3), 
                        p.inHand.GetBoundingBox().Width, p.inHand.GetBoundingBox().Height / 3);

                    if (bottomOfShovel.Intersects(GetBoundingBox()))
                        shoveled = true;
                }
                else if (shoveled)
                {
                    
                    if (fill)
                    {
                        currColumnFrame--;
                        if (currColumnFrame <= 0)
                        {
                            currColumnFrame = 0;
                            fill = false;
                        }
                        canFillHole = false;
                    }
                    else
                    {
                        currColumnFrame++;
                        if (currColumnFrame >= nColumns)
                        {
                            // Check for Treasure! Arrrg
                            foreach (var treasureMap in BoundingBoxLocations.treasureLocationsList)
                            {
                                Rectangle digLocRect = new Rectangle((int)treasureMap.digTileLoc.X, (int)treasureMap.digTileLoc.Y, GameOptions.tileWidth, GameOptions.tileHeight);
                                if (digLocRect.Intersects(this.GetBoundingBox()))
                                {
                                    // the contents of the treasure was set by player
                                    if (treasureMap.rewarded != null)
                                    {
                                        treasureMap.rewarded.remove = false;
                                        treasureMap.rewarded.location = new Vector2(treasureMap.digTileLoc.X + RandomEvents.rand.Next(-5, 5), treasureMap.digTileLoc.Y + RandomEvents.rand.Next(-5, 5));
                                        ItemUtility.ItemsToUpdate.Add(treasureMap.rewarded);
                                        treasureMap.solved = true;
                                    }
                                    else
                                    {
                                        // TODO: use the map tier to tier the loot here
                                        if (!treasureMap.solved)
                                        {
                                            Storage reward = new BaseChest(TeamType.Player, regionKey, location, _content, _graphics);
                                            reward.remove = false;
                                            ItemUtility.ItemsToUpdate.Add(reward);
                                            treasureMap.solved = true;
                                        }
                                    }
                                    break;
                                }
                            }

                            canFillHole = true;
                            currColumnFrame = nColumns - 1;
                            fill = true;
                        }
                        else
                            canFillHole = false;
                    }

                    shoveled = false;

                }
            }
        }

        public void DrawTile(SpriteBatch sb)
        {
            int width = _texture.Width / nColumns;
            int height = _texture.Height / nRows;

            // update target index frame on sprite sheet (x and Y are cords of the sprite)
            targetRectangle.X = width * currColumnFrame;
            targetRectangle.Y = height * currRowFrame;
            targetRectangle.Width = width;
            targetRectangle.Height = height;

            // update bounding box (x and y are cords of the screen here) -- WONT UPDATE STATIC SPRITES
            SetBoundingBox();


            Vector2 origin = new Vector2(width / 2, height / 2);
            if (this is ITilePiece)
            {
                origin = Vector2.Zero;
            }
            // normal drawing call
            sb.Draw(_texture, location, targetRectangle, Color.White * transparency, rotation,
                origin, spriteScale, SpriteEffects.None, 0f);
        }

    }
}
