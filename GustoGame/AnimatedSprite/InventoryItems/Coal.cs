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
    public class Coal : InventoryItem, IOre
    {
        public Coal(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {
            itemKey = "coal";

            Texture2D textureCoal = content.Load<Texture2D>("coal");
            Texture2D textureCoalBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureCoalBB = new Texture2D(graphics, textureCoal.Width, textureCoal.Height);
            Asset CoalAsset = new Asset(textureCoal, textureCoalBB, 1, 1, 1.0f, itemKey, region);
            SetSpriteAsset(CoalAsset, location);
            stackable = true;
        }
    }
}

