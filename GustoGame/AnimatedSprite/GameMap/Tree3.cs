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
    public class Tree3 : Tree
    {
        public Tree3(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            nHitsToDestory = 5;
            msRespawn = GameOptions.GameDayLengthMs / 2;
            string objKey = "tree3";

            List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 2);
            drops = ItemUtility.CreateNPInventory(itemDrops, team, region, location, content, graphics);

            Texture2D textureTree = content.Load<Texture2D>("Tree3");
            Texture2D textureTreeBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureTreeBB = new Texture2D(graphics, textureTree.Width, textureTree.Height);
            Asset treeAsset = new Asset(textureTree, textureTreeBB, 4, 2, 0.6f, objKey, region);
            SetSpriteAsset(treeAsset, location);
        }
    }
}
