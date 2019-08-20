using Gusto.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gusto.GameMap.lightning
{
    public class BranchLightning
    {
        ContentManager _content;

        List<LightningBolt> bolts = new List<LightningBolt>();

        public bool IsComplete { get { return bolts.Count == 0; } }
        public Vector2 End { get; private set; }
        private Vector2 direction;

        static Random rand = new Random();

        public BranchLightning(ContentManager content)
        {
            _content = content;

            // random start and strike pos
            bool sideStrike = false;
            bool right = false;
            int startPosX = 0;
            int startPosY = 0;
            int xory = RandomEvents.rand.Next(0, 10);
            int leftOrRight = RandomEvents.rand.Next(0, 10);
            if (xory < 5)
                sideStrike = true;
            if (sideStrike)
            {
                if (leftOrRight < 5)
                    right = true;
                if (right)
                    startPosX = GameOptions.PrefferedBackBufferWidth;
                startPosY = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferHeight / 3);
            }
            else
            {
                startPosY = 0;
                startPosX = RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferWidth);
            }
            Vector2 startingPos = new Vector2(startPosX, startPosY);
            Vector2 strikePos = new Vector2(RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferWidth), RandomEvents.rand.Next(0, GameOptions.PrefferedBackBufferHeight));

            End = strikePos;
            direction = Vector2.Normalize(strikePos - startingPos);
            Create(startingPos, strikePos);
        }

        public void Update()
        {
            bolts = bolts.Where(x => !x.IsComplete).ToList();
            foreach (var bolt in bolts)
                bolt.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);
        }

        private void Create(Vector2 start, Vector2 end)
        {
            var mainBolt = new LightningBolt(start, end, _content);
            bolts.Add(mainBolt);

            int numBranches = rand.Next(3, 6);
            Vector2 diff = end - start;

            // pick a bunch of random points between 0 and 1 and sort them
            float[] branchPoints = Enumerable.Range(0, numBranches)
                .Select(x => Rand(0, 1f))
                .OrderBy(x => x).ToArray();

            for (int i = 0; i < branchPoints.Length; i++)
            {
                // Bolt.GetPoint() gets the position of the lightning bolt at specified fraction (0 = start of bolt, 1 = end)
                Vector2 boltStart = mainBolt.GetPoint(branchPoints[i]);

                // rotate 30 degrees. Alternate between rotating left and right.
                Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(30 * ((i & 1) == 0 ? 1 : -1)));
                Vector2 boltEnd = Vector2.Transform(diff * (1 - branchPoints[i]), rot) + boltStart;
                bolts.Add(new LightningBolt(boltStart, boltEnd, _content));
            }
        }

        static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
    }
}
