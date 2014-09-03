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


        public class Particle
        {
            public Vector2 position;
            public Vector2 velocity;
            public Vector2 acceleration;
            public float lifetime;
            public float timeSinceStart;
            public float scale;
            public float rotation;
            public float rotationSpeed;
            public bool Active
            {
                get { return timeSinceStart < lifetime; }
            }

            public void Initialize( Vector2 position, Vector2 velocity, Vector2 acceleration, float lifetime, float scale, float rotationSpeed)
            {
                this.position = position;
                this.velocity = velocity;
                this.acceleration = acceleration;
                this.lifetime = lifetime;
                this.scale = scale;
                this.rotationSpeed = rotationSpeed;
                this.timeSinceStart = 0.0f;
                this.rotation = 0.0f;
            }

            public void Update(float dt)
            {
                velocity += acceleration * dt;
                position += velocity * dt;
                rotation += rotationSpeed * dt;
                timeSinceStart += dt;
            }

        }
    }
}
