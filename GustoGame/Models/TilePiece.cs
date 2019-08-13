using Comora;
using Gusto.AnimatedSprite;
using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.Models.Animated;
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
    public class TilePiece : Sprite
    {

        GraphicsDevice _graphics;
        ContentManager _content;

        public int tileKey;

        public Sprite groundObject; // any tree, rock, etc asset we want to set to this tile.
        bool shoveled;
        bool fill;

        public bool canFillHole;

        public TilePiece(int key, Sprite _groundObject, ContentManager content, GraphicsDevice graphics) : base(null)
        {
            _graphics = graphics;
            _content = content;
            tileKey = key;
            groundObject = _groundObject;
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
                                if (treasureMap.digTile == this)
                                {
                                    // the contents of the treasure was set by player
                                    if (treasureMap.rewarded != null)
                                    {
                                        treasureMap.rewarded.remove = false;
                                        treasureMap.rewarded.location = treasureMap.digTile.location;
                                        ItemUtility.ItemsToUpdate.Add(treasureMap.rewarded);
                                        treasureMap.solved = true;
                                    }
                                    else
                                    {
                                        // TODO: use the map tier to tier the loot here
                                        Storage reward = new BaseChest(TeamType.Player, regionKey, location, _content, _graphics);
                                        reward.remove = false;
                                        ItemUtility.ItemsToUpdate.Add(reward);
                                        treasureMap.solved = true;
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

    }
}
