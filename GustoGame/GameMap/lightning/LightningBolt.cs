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
    public class LightningBolt
    {
        public List<LightningSegment> Segments = new List<LightningSegment>();

        public float Alpha { get; set; }
        public float FadeOutRate { get; set; }
        public Color Tint { get; set; }

        ContentManager _content;

        public bool IsComplete { get { return Alpha <= 0; } }

        public LightningBolt(ContentManager content) : this(new Color(0.9f, 0.8f, 1f), content)
        {
            _content = content;

        }

        public LightningBolt(Color color, ContentManager content)
        {
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
            // end


            Segments = CreateBolt(startingPos, strikePos, 1, content);

            Tint = color;
            Alpha = 1f;
            FadeOutRate = 0.03f;

            _content = content;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(spriteBatch, Tint * (Alpha * 0.6f));
        }

        public virtual void Update()
        {
            Alpha -= FadeOutRate;
        }

        protected static List<LightningSegment> CreateBolt(Vector2 source, Vector2 dest, float thickness, ContentManager content)
        {
            var results = new List<LightningSegment>();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add((float)RandomEvents.rand.NextDouble());

            positions.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = (float)RandomEvents.rand.NextDouble() * (Sway - -Sway) + -Sway;
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new LightningSegment(prevPoint, point, content, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new LightningSegment(prevPoint, dest, content, thickness));

            return results;
        }

    }
}
