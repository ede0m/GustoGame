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
    public class Nails : InventoryItem
    {
        public Nails(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "nails";
            msCraftTime = 2000;

            Texture2D textureIronOre = content.Load<Texture2D>("Nails");
            Texture2D textureIronOreBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureIronOreBB = new Texture2D(graphics, textureIronOre.Width, textureIronOre.Height);
            Asset IronOreAsset = new Asset(textureIronOre, textureIronOreBB, 1, 1, 0.4f, itemKey, region);
            SetSpriteAsset(IronOreAsset, location);
            stackable = true;
        }
    }
}

