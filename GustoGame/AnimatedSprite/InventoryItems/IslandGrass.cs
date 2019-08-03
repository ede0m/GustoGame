using Gusto.Models;
using Gusto.Models.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.AnimatedSprite.InventoryItems
{
    public class IslandGrass : InventoryItem, IGrass
    {
        public IslandGrass(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "islandGrass";

            Texture2D textureIslandGrass = content.Load<Texture2D>("islandGrass");
            Texture2D textureIslandGrassBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureIslandGrassBB = new Texture2D(graphics, textureIslandGrass.Width, textureIslandGrass.Height);
            Asset islandGrassAsset = new Asset(textureIslandGrass, textureIslandGrassBB, 1, 1, 0.5f, "islandGrass", region);
            SetSpriteAsset(islandGrassAsset, location);
            stackable = true;
        }
    }
}

