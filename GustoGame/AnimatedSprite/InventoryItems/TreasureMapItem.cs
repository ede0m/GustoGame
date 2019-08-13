﻿using Gusto.AnimatedSprite.InventoryItems;
using Gusto.Bounding;
using Gusto.Models;
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

namespace Gusto.AnimatedSprite
{
    public class TreasureMapItem : TreasureMap
    {
        public TreasureMapItem(Storage reward, TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "treasureMapItem";
            Texture2D texture = content.Load<Texture2D>("TreasureMap");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 1, 1, 0.5f, itemKey, region);

            // pass rewardItems for burying your own treasure
            if (reward != null)
            {
                rewarded = reward;
            }
            else
            {
                lootTier = 0;
                // random tier of loot
                int rand = RandomEvents.rand.Next(0, 100);
                if (rand > 40 && rand < 70)
                    lootTier = 1;
                if (rand > 70 && rand < 90)
                    lootTier = 2;
                if (rand > 90)
                    lootTier = 3;

                // randomly assign tile in region for dig up spot
                KeyValuePair<string, List<Sprite>> randEntry = BoundingBoxLocations.RegionMap.ElementAt(RandomEvents.rand.Next(0, BoundingBoxLocations.RegionMap.Count));
                List<Sprite> tilesInRegion = randEntry.Value;
                treasureInRegion = randEntry.Key;
                digTile = (TilePiece)tilesInRegion[RandomEvents.rand.Next(tilesInRegion.Count)];

            }

            SetSpriteAsset(asset, location);
            stackable = false;
        }
    }
}