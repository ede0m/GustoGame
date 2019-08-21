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
        public Vector2 Start { get { return Segments[0].A; } }
        public Vector2 End { get { return Segments.Last().B; } }

        public float Alpha { get; set; }
        public float FadeOutRate { get; set; }
        public Color Tint { get; set; }

        ContentManager _content;

        public bool IsComplete { get { return Alpha <= 0; } }

        public LightningBolt(Vector2 source, Vector2 dest, ContentManager content) : this(source, dest, new Color(0.9f, 0.8f, 1f), content)
        {
            _content = content;

        }

        public LightningBolt(Vector2 source, Vector2 dest, Color color, ContentManager content)
        {

            Segments = CreateBolt(source, dest, 1.5f, content);

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

        // Returns the point where the bolt is at a given fraction of the way through the bolt. Passing
        // zero will return the start of the bolt, and passing 1 will return the end.
        public Vector2 GetPoint(float position)
        {
            var start = Start;
            float length = Vector2.Distance(start, End);
            Vector2 dir = (End - start) / length;
            position *= length;

            var line = Segments.Find(x => Vector2.Dot(x.B - start, dir) >= position);
            float lineStartPos = Vector2.Dot(line.A - start, dir);
            float lineEndPos = Vector2.Dot(line.B - start, dir);
            float linePos = (position - lineStartPos) / (lineEndPos - lineStartPos);

            return Vector2.Lerp(line.A, line.B, linePos);
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
