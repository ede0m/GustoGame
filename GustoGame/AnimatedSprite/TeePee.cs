using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Gusto.Models;
using Gusto.Models.Menus;
using Gusto.Models.Animated;
using System.Linq;
using Gusto.Utility;
using Gusto.Models.Interfaces;

namespace Gusto.AnimatedSprite
{
    public class TeePee : Structure
    {
        public TeePee(TeamType team, string region, Vector2 location, ContentManager content, GraphicsDevice graphics) : base(team, content, graphics)
        {

            string objKey = "teePee";

            //MapModelMovementVectorValues();
            Texture2D texture = content.Load<Texture2D>("TeePee");
            Texture2D textureBB = null;
            if (Gusto.GameOptions.ShowBoundingBox)
                textureBB = new Texture2D(graphics, texture.Width, texture.Height);
            Asset asset = new Asset(texture, textureBB, 4, 1, 0.14f, objKey, region);

            // inventory
            List<Sprite> interiorObjs = null;
            if (team != TeamType.Player)
            {
                List<Tuple<string, int>> itemDrops = RandomEvents.RandomNPDrops(objKey, 5);
                interiorObjs = ItemUtility.CreateInteriorItems(itemDrops, team, region, location, content, graphics);
            }

            structureInterior = new Interior("teePee", this, content, graphics);

            // set the random drops as interior objects
            if (interiorObjs != null)
            {
                foreach (var obj in interiorObjs)
                {
                    structureInterior.interiorObjects.Add(obj);

                    // need to do this for containers so they drop items within ship
                    if (obj is IContainer)
                    {
                        Container c = (Container)obj;
                        c.inInteriorId = structureInterior.interiorId;
                    }
                }
            }

            SetSpriteAsset(asset, location);
        }
    }
}
