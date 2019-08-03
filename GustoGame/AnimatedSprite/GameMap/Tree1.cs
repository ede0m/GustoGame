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
    public class Tree1 : Tree
        {
        public Tree1(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            nHitsToDestory = 5;
            msRespawn = GameOptions.GameDayLengthMs / 2;
            string objKey = "tree1";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 2);
            drops = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D textureTree1 = content.Load<Texture2D>("Tree1");
            Texture2D textureTree1BB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTree1BB = new Texture2D(graphics, textureTree1.Width, textureTree1.Height);
            Asset tree1Asset = new Asset(textureTree1, textureTree1BB, 6, 2, 0.4f, objKey, region);
            SetSpriteAsset(tree1Asset, location);
        }
    }
}
