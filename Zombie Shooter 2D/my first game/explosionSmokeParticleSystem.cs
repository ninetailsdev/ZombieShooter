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
        class explosionSmokeParticleSystem: particleSystem
        {
            public explosionSmokeParticleSystem(Game1 game, int howManyEffects, string textureFilename)
                : base(game, howManyEffects, textureFilename)
            {
            }
            protected override void InitializeConstants()
            {
                minInitialSpeed = 6;
                maxInitialSpeed = 60;
                minAcceleration = -10;
                maxAcceleration = -50;
                minLifeTime = 1.0f;
                maxLifeTime = 2.5f;
                minScale = 1.0f;
                maxScale = 2.0f;
                minNumParticles = 10;
                maxNumParticles = 20;
                minRotationSpeed = -MathHelper.PiOver4;
                maxRotationSpeed = MathHelper.PiOver4;

                spriteBlendMode = SpriteBlendMode.AlphaBlend;
                DrawOrder = AlphaBlendDrawOrder;
            }
        }
    }
}
