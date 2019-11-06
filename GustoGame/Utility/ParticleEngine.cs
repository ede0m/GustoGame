using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustoGame.Utility
{
    public class Particle
    {
        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector2 Position { get; set; }        // The current position of the particle        
        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance
        public float Angle { get; set; }            // The current angle of rotation of the particle
        public float AngularVelocity { get; set; }    // The speed that the angle is changing
        public Color Color { get; set; }            // The color of the particle
        public float Size { get; set; }                // The size of the particle
        public int TTL { get; set; }                // The 'time to live' of the particle
        public int LifeTime { get; set; }
        public float Transparency { get; set; }     // the transparancey for fade out

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float transparency, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Transparency = transparency;
            Size = size;
            TTL = ttl;
            LifeTime = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
            Transparency = Math.Min((float)TTL / (float)LifeTime, 0.2f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * Transparency,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }

    public class WakeParticleEngine
    {
        public Vector2 EmitterLocation { get; set; }
        public int WakeDisplacement { get; set; }
        public int MaxParticle { get; set; }

        private Random random;
        private List<Particle> particles;
        private List<Texture2D> textures;

        public WakeParticleEngine(ContentManager content, Vector2 location)
        {
            EmitterLocation = location;
            textures = new List<Texture2D>();
            //textures.Add(content.Load<Texture2D>("circle_particle"));
            //textures.Add(content.Load<Texture2D>("star_particle"));
            //textures.Add(content.Load<Texture2D>("diamond_particle"));
            textures.Add(content.Load<Texture2D>("Particle1"));
            textures.Add(content.Load<Texture2D>("Particle2"));
            textures.Add(content.Load<Texture2D>("Particle3"));
            this.particles = new List<Particle>();
            random = new Random();

            // defaults
            WakeDisplacement = 8;
            MaxParticle = 5;
        }

        private Particle GenerateNewParticle(Vector2 velocity, int displacement)
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            position.X += random.Next(-displacement, displacement);
            position.Y += random.Next(-displacement, displacement);

            Vector2 velocityInvert = velocity * -1;
            float randomAngleOffset = (float)random.NextDouble() * 15;
            Vector2 finalVelocity = Vector2.Transform(velocityInvert, Matrix.CreateRotationX(randomAngleOffset));

            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = new Color(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble());
            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(40);

            return new Particle(texture, position, finalVelocity, angle, angularVelocity, Color.White, 0.2f, size, ttl);
        }

        public void Update(Vector2 velocity)
        {
            int total = MaxParticle;
            if (Math.Abs(velocity.X) > 0.7f || Math.Abs(velocity.Y) > 0.7f)
                total = MaxParticle + 1;

            for (int i = 0; i < total; i++)
                particles.Add(GenerateNewParticle(velocity, WakeDisplacement));

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            spriteBatch.Begin(cam);
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
            spriteBatch.End();
        }

    }
}
