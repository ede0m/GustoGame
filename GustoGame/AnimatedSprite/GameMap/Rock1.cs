using Gusto.Models;
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

namespace Gusto.AnimatedSprite.GameMap
{
    public class Rock1 : Rock
    {
        public Rock1(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            nHitsToDestory = 6;
            msRespawn = GameOptions.GameDayLengthMs / 2;
            string objKey = "rock1";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 2);
            drops = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D textureRock1 = content.Load<Texture2D>("Rock1");
            Texture2D textureRock1BB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureRock1BB = new Texture2D(graphics, textureRock1.Width, textureRock1.Height);
            Asset Grass1Asset = new Asset(textureRock1, textureRock1BB, 4, 2, 0.3f, objKey, region);
            SetSpriteAsset(Grass1Asset, location);
        }
    }
}
