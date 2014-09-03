using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace my_first_game
{
    namespace particles
    {
        public abstract class particleSystem: DrawableGameComponent
        {
            private Game1 game;
            private Texture2D texture;
            private Vector2 origin;
            private static Random random = new Random();
            public static Random Random
            {
                get { return random; }
            }
            private int howManyEffects;
            Particle[] particles;
            Queue<Particle> freeParticles;
            public int FreeParticleCount
            {
            get {return freeParticles.Count;}
            }
            public const int AlphaBlendDrawOrder = 100;
            public const int additiveDrawOrder = 200;
            protected SpriteBlendMode spriteBlendMode;
            protected int minNumParticles;
            protected int maxNumParticles;
            protected float minLifeTime;
            protected float maxLifeTime;
            protected string textureFilename;
            protected float minInitialSpeed;
            protected float maxInitialSpeed;
            protected float minAcceleration;
            protected float maxAcceleration;
            protected float minRotationSpeed;
            protected float maxRotationSpeed;
            protected float minScale;
            protected float maxScale;

            protected particleSystem(Game1 game, int howManyEffects, string textureFilename)
                : base(game)
            {
                this.game = game;
                this.howManyEffects = howManyEffects;
                this.textureFilename = textureFilename;
            }

            public override void Initialize()
            {
                InitializeConstants();
                particles = new Particle[howManyEffects * maxNumParticles];
                freeParticles = new Queue<Particle>(howManyEffects * maxNumParticles);
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i] = new Particle();
                    freeParticles.Enqueue(particles[i]);

                }

                base.Initialize();
            }
            protected abstract void InitializeConstants();

            protected override void LoadContent()
            {
                texture = game.Content.Load<Texture2D>(textureFilename);
                origin.X = texture.Width / 2;
                origin.Y = texture.Height / 2;
                base.LoadContent();
            }

            public void AddParticles(Vector2 where)
            {
                int numParticles = random.Next(minNumParticles, maxNumParticles);
                for (int i = 0; i < numParticles && freeParticles.Count > 0; i++)
                {
                    Particle p = freeParticles.Dequeue();
                    InitializeParticle(p, where);
                }
            }

            protected virtual Vector2 PickRandomDirection()
            {
                float angle = RandomBetween(0, MathHelper.TwoPi);
                return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            public static float RandomBetween(float min, float max)
            {
                return min + (float)random.NextDouble() * (max - min);
            }

            protected virtual void InitializeParticle(Particle p, Vector2 where)
            {
                Vector2 direction = PickRandomDirection();
                float velocity = RandomBetween(minInitialSpeed, maxInitialSpeed);
                float acceleration = RandomBetween(minAcceleration, maxAcceleration);
                float lifeTime = RandomBetween(minLifeTime, maxLifeTime);
                float scale = RandomBetween(minScale, maxScale);
                float rotationSpeed = RandomBetween(minRotationSpeed, maxRotationSpeed);
                p.Initialize(where, velocity * direction, acceleration * direction, lifeTime, scale, rotationSpeed);
            }

            public override void Update(GameTime gameTime)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (Particle p in particles)
                {
                    if (p.Active)
                    {
                        p.Update(dt);
                    }
                    if (!p.Active)
                    {
                        freeParticles.Enqueue(p);
                    }
                }
                base.Update(gameTime);
            }
            public override void Draw(GameTime gameTime)
            {
                game.SpriteBatch.Begin(spriteBlendMode);
                foreach (Particle p in particles)
                {
                    if (!p.Active)
                    {
                        continue;
                    }
                    float normalisedLifeTime = p.timeSinceStart / p.lifetime;
                    float alpha = 4 * normalisedLifeTime * (1 - normalisedLifeTime);
                    float scale = p.scale * (float).75f + .25f * normalisedLifeTime;
                    Color color = new Color(new Vector4(1,1,1,alpha));
                    game.SpriteBatch.Draw(texture, p.position, null, color, p.rotation, origin, scale, SpriteEffects.None, 0.0f);

                }
                game.SpriteBatch.End();
                base.Draw(gameTime);
            }


        }
    }
}
