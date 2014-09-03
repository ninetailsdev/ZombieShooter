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
        class explosionParticleSystem: particleSystem
        {
            public explosionParticleSystem(Game1 game, int howManyEffects, string textureFilename)
                : base(game, howManyEffects, textureFilename)
            {
            }

            protected override void InitializeConstants()
            {
                minInitialSpeed = 4;
                maxInitialSpeed = 50;
                minAcceleration = 0;
                maxAcceleration = 0;
                minLifeTime = 0.5f;
                maxLifeTime = 1.0f;
                minScale = 0.1f;
                maxScale = 0.4f;
                minNumParticles = 20;
                maxNumParticles = 25;
                minRotationSpeed = -MathHelper.PiOver4;
                maxRotationSpeed = MathHelper.PiOver4;
                spriteBlendMode = SpriteBlendMode.Additive;
                DrawOrder = additiveDrawOrder;
            }

            protected override void InitializeParticle(Particle p, Vector2 where)
            {
                base.InitializeParticle(p, where);
                p.acceleration = -p.velocity / p.lifetime;
            }
        }
    }
}
