using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using my_first_game.particles;

namespace my_first_game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {


        enum ScreenState
        {
            TitleScreen,
            GameScreen,
            DeadScreen, 
            ControlScreen
        }

        ScreenState mCurrentScreen;
        Texture2D startScreen, controlScreen, deadScreen;

        GraphicsDeviceManager graphics;
        Texture2D backgroundTexture;
        Rectangle viewportRect;
        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get {return spriteBatch;}
        }
        Jackson cannon;
        gameObject turret;
        gameObject turretStand;
        const int maxBullet = 200;
        const int maxBullet2 = 200;
        gameObject[] Bullet;
        gameObject[] Bullet2;
        KeyboardState previousKeyboardState = Keyboard.GetState();
        MouseState previousMouseState = Mouse.GetState();
        const int maxZombies = 5;
        const float maxZombieHeight = 0.1f;
        const float minZombieHeight = 0.9f;
        const float maxZombieVelocity = 0.1f;
        const float minZombieVelocity = 0.05f;
        const float maxZombieWidth = 0.9f;
        const float minZombieWidth = 0.1f;
        Random random = new Random();
        Zombie[] zombies;
        int score;
        int lives = 5;
        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);
        particleSystem explosion;
        particleSystem smoke;
        gameObject gun;
        gameObject mouseIcon;
        int FiredBullet2s;
        int enemiesKilled;// stores enemies killed, then allows gun upgrade after certain kills
        int deathTime;
        double lastBulletTime;

        Vector2 weaponDrawPoint = new Vector2(0.1f, 0.3f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            explosion = new explosionParticleSystem(this, 1, "Sprites//BloodDrops");
            Components.Add(explosion);
            smoke = new explosionSmokeParticleSystem(this, 1, "Sprites//BloodSpray");
            Components.Add(smoke);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("Sprites\\BackGround V1");
            cannon = new Jackson(Content.Load<Texture2D>("Sprites\\MJTest"));
            turret = new gameObject(Content.Load<Texture2D>("Sprites\\turretHead"));
            turretStand = new gameObject(Content.Load<Texture2D>("Sprites\\turretStand"));


            mouseIcon = new gameObject(Content.Load<Texture2D>("Sprites\\MouseTarget 2.0"));

            cannon.position = new Vector2(120, graphics.GraphicsDevice.Viewport.Height);


            Bullet = new gameObject[maxBullet];
            for (int i = 0; i < maxBullet; i++)
            {
                Bullet[i] = new gameObject(Content.Load<Texture2D>("Sprites\\cannonball"));
            }

            
            gun = new gameObject(Content.Load<Texture2D>("Sprites\\gun"));
            gun.position = new Vector2(450, graphics.GraphicsDevice.Viewport.Height - 190);

            Bullet2 = new gameObject[maxBullet2];
            for (int i = 0; i < maxBullet2; i++)
            {
                Bullet2[i] = new gameObject(Content.Load<Texture2D>("Sprites\\cannonball2"));
            }
            zombies = new Zombie[maxZombies];
            for (int i = 0; i < maxZombies; i++)
            {
                zombies[i] = new Zombie(Content.Load<Texture2D>("Sprites\\ZombieTest"));
            }
            font = Content.Load<SpriteFont>("Fonts\\GameFont");
            viewportRect = new Rectangle(0, 0, 
                graphics.GraphicsDevice.Viewport.Width, 
                graphics.GraphicsDevice.Viewport.Height);

            startScreen = Content.Load<Texture2D>("StartScreen");
            controlScreen = Content.Load<Texture2D>("ControlScreen");
            deadScreen = Content.Load<Texture2D>("GameOver");

            mCurrentScreen = ScreenState.TitleScreen;

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
             KeyboardState keyboardState = Keyboard.GetState();
             MouseState mouseState = Mouse.GetState();
                       
            switch (mCurrentScreen)
            {
                case ScreenState.TitleScreen:
                    {
                        if (keyboardState.IsKeyDown(Keys.Space))
                        {
                            mCurrentScreen = ScreenState.GameScreen;
                        }

                        if (keyboardState.IsKeyDown(Keys.Enter))
                        {
                            mCurrentScreen = ScreenState.ControlScreen;
                        }

                        if (keyboardState.IsKeyDown(Keys.Escape))
                            this.Exit();
                        break;
                    }
                case ScreenState.ControlScreen:
                    {
                        if (keyboardState.IsKeyDown(Keys.B) )
                        {
                                mCurrentScreen = ScreenState.TitleScreen;
                        }
                        break;
                    }

                #region Game Screen Update code
                case ScreenState.GameScreen:
                    {
                        // Allows the game to exit
                        if (lives == -1)
                        {
                            mCurrentScreen = ScreenState.DeadScreen;
                        }

                        if (keyboardState.IsKeyDown(Keys.Escape))
                        {
                            mCurrentScreen = ScreenState.TitleScreen;
                        }


                        if (keyboardState.IsKeyDown(Keys.W))
                        {
                            if (cannon.position.Y < 0)
                            {
                            }
                            else
                            {
                                cannon.position.Y -= 5;
                            }
                        }
                        if (keyboardState.IsKeyDown(Keys.S))
                        {
                            if (cannon.position.Y > (graphics.PreferredBackBufferHeight))
                            {
                            }
                            else
                            {
                                cannon.position.Y += 5;
                            }
                        }

                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            if (cannon.position.X < 0)
                            {
                            }
                            else
                            {
                                cannon.position.X -= 5;
                            }
                        }
                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            if (cannon.position.X > graphics.PreferredBackBufferWidth)
                            {
                            }
                            else
                            {
                                cannon.position.X += 5;
                            }
                        }

                        if (keyboardState.IsKeyDown(Keys.P) || mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (!turret.alive)
                            {
                                turret.alive = true;
                                turretStand.alive = true;
                                turretStand.position = cannon.position;
                                turret.position = cannon.position;
                            }
                        }
                        cannon.rotation = MathHelper.Clamp(cannon.rotation, -MathHelper.PiOver2, 0);

                        MouseState mState = Mouse.GetState();

                        Vector2 mousePos = new Vector2(mState.X, mState.Y);

                        Vector2 direction = cannon.position - mousePos;
                        direction.Normalize();

                        cannon.rotation = (float)Math.Atan2(-direction.X, direction.Y) - MathHelper.PiOver2;

                        foreach (gameObject zombie in zombies)
                        {
                            zombie.rotation = MathHelper.Clamp(zombie.rotation, -MathHelper.PiOver2, 0);

                            Vector2 Zombdirection = zombie.position - cannon.position;
                            Zombdirection.Normalize();

                            zombie.rotation = (float)Math.Atan2(-Zombdirection.X, Zombdirection.Y) - MathHelper.PiOver2;


                        }

                        // Turret Fake Ai
                        if (turret.alive == true)
                        {
                            int index = 0;
                            foreach (gameObject zombie in zombies)
                            {
                                if (zombie.alive != true)
                                {
                                    index++;
                                    if (index > maxZombies)
                                        index = 0;
                                }


                            }
                            turret.rotation = MathHelper.Clamp(turret.rotation, -MathHelper.PiOver2, 0);

                            Vector2 Turdirection = turret.position - zombies[index].position;
                            Turdirection.Normalize();

                            turret.rotation = (float)Math.Atan2(-Turdirection.X, Turdirection.Y) - MathHelper.PiOver2;
                        }



                        if (enemiesKilled == 10)
                        {
                            gun.alive = true;//Shows the gun pickup
                            enemiesKilled = 0;


                        }
                        

                        if ((cannon.hasWeaponUpgrade == false) && (mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released))
                        {

                            FireBullet();



                        }



                        if ((cannon.hasWeaponUpgrade == true) && (mouseState.LeftButton == ButtonState.Pressed))// && previousMouseState.LeftButton == ButtonState.Released))
                        {

                            FireBullet2();
                            FiredBullet2s--;
                        }


                        if (FiredBullet2s == 0)
                        {
                            cannon.hasWeaponUpgrade = false;
                            FiredBullet2s = 200;
                        }

                        if (turret.alive == true)
                        {
                            fireturret(gameTime);
                        }



                        UpdateGun();
                        UpdateEnemies();
                        UpdateBullet2();
                        UpdateBullet();

                        // TODO: Add your update logic here
                        previousMouseState = mouseState;

                        // When player is hit this 
                        //sets the player not not alive
                        Rectangle playerRect = new Rectangle((int)cannon.position.X, (int)cannon.position.Y, cannon.sprite.Width, cannon.sprite.Height);

                        foreach (gameObject zombie in zombies)
                        {
                            Rectangle zombieRect = new Rectangle((int)zombie.position.X, (int)zombie.position.Y, zombie.sprite.Width, zombie.sprite.Height);

                            if (playerRect.Intersects(zombieRect) && cannon.alive == true)
                            {
                                cannon.alive = false;
                                zombie.alive = false;
                                deathTime = gameTime.TotalRealTime.Seconds;
                                deathTime += 3;

                                if (deathTime > 60)
                                    deathTime -= 60;

                                cannon.position.X = viewportRect.Center.X;
                                cannon.position.Y = viewportRect.Center.Y;

                                lives--;
                            }

                        }

                        if (gameTime.TotalRealTime.TotalSeconds >= deathTime && cannon.alive == false)
                        {

                            cannon.alive = true;

                        }
                        break;
                    }
                #endregion

                case ScreenState.DeadScreen:
                    {
                        if (keyboardState.IsKeyDown(Keys.Space))
                        {
                            mCurrentScreen = ScreenState.GameScreen;
                        }

                        if (keyboardState.IsKeyDown(Keys.Escape))
                        {
                            mCurrentScreen = ScreenState.TitleScreen;
                        }
                        break;
                    }
            }



            base.Update(gameTime);
        }

        public void UpdateEnemies()
        {
            foreach (gameObject zombie in zombies)
            {
                if (zombie.alive)
                {
                    // Zombie Ai
                    if (zombie.position.X > cannon.position.X)
                    {
                        zombie.position.X -= 2;
                    }
                    else
                    {
                        zombie.position.X
                            += 2;
                    }

                    if (zombie.position.Y > cannon.position.Y)
                    {
                        zombie.position.Y -= 2;
                    }
                    else
                    {
                        zombie.position.Y += 2;
                    }

                    if (!viewportRect.Contains(new Point((int)zombie.position.X, (int)zombie.position.Y)))
                    {
                        zombie.alive = false;
                    }
                }
                else
                {
                    zombie.alive = true;
                    double spawnLocation;

                    spawnLocation = random.NextDouble();
                    spawnLocation *= 10;
                    while (spawnLocation > 5)
                    {
                        spawnLocation = random.NextDouble();
                        spawnLocation *= 10;
                    }

        
                    switch ((int)spawnLocation)
                    {
                        case 1:
                            zombie.position = new Vector2(MathHelper.Lerp(
                            (float)viewportRect.Width * minZombieWidth,
                            (float)viewportRect.Width * maxZombieWidth,
                            (float)random.NextDouble()), viewportRect.Bottom);
                            zombie.velocity = new Vector2(0, (MathHelper.Lerp
                            (minZombieVelocity, maxZombieVelocity,
                            (float)random.NextDouble())))
                    ; break;
                        case 2:
                            zombie.position = new Vector2(MathHelper.Lerp(
                            (float)viewportRect.Width * minZombieWidth,
                            (float)viewportRect.Width * maxZombieWidth,
                            (float)random.NextDouble()), viewportRect.Top);
                            zombie.velocity = new Vector2(0, (MathHelper.Lerp
                            (minZombieVelocity, maxZombieVelocity,
                            (float)random.NextDouble())))
                   ; break;
                        case 3:
                            zombie.position = new Vector2(viewportRect.Left, MathHelper.Lerp(
                            (float)viewportRect.Height * minZombieHeight,
                            (float)viewportRect.Height * maxZombieHeight,
                            (float)random.NextDouble()));
                            zombie.velocity = new Vector2(MathHelper.Lerp
                            (-minZombieVelocity, -maxZombieVelocity,
                            (float)random.NextDouble()), 0);
                            ; break;
                        case 4:
                            zombie.position = new Vector2(viewportRect.Right, MathHelper.Lerp(
                            (float)viewportRect.Height * minZombieHeight,
                            (float)viewportRect.Height * maxZombieHeight,
                            (float)random.NextDouble()));
                            zombie.velocity = new Vector2(MathHelper.Lerp
                            (-minZombieVelocity, -maxZombieVelocity,
                            (float)random.NextDouble()), 0);
                            ; break;


                    }
                }
            }
        }
     
        public void FireBullet()
        {
            foreach (gameObject bullet in Bullet)
            {
                if (!bullet.alive)
                {
                    bullet.alive = true;
                    bullet.position = cannon.position - bullet.center;
                    bullet.velocity = new Vector2((float)Math.Cos(cannon.rotation), (float)Math.Sin(cannon.rotation)) * 5.0f;
                    return;
                }
            }
        }

        public void FireBullet2()
        {
            foreach (gameObject bullet2 in Bullet2)
            {
                if (!bullet2.alive)
                {
                    bullet2.alive = true;
                    bullet2.position = cannon.position - bullet2.center;
                    bullet2.velocity = new Vector2((float)Math.Cos(cannon.rotation), (float)Math.Sin(cannon.rotation)) * 5.0f;
                    return;
                }
            }
        }
        public void fireturret(GameTime gameTime)
        {
            foreach (gameObject bullet in Bullet2)
            {
                if (!bullet.alive)
                {
                    
                    double currentTime = gameTime.TotalGameTime.TotalMilliseconds;  
                    float rateOfFire = 500.0f;
                    if (currentTime - lastBulletTime > rateOfFire)     // Fire a bullet every rateOfFire (ms)  
                    {
                        
                        bullet.alive = true;
                        bullet.position = turret.position - bullet.center;
                        bullet.velocity = new Vector2((float)Math.Cos(turret.rotation), (float)Math.Sin(turret.rotation)) * 10.0f;
                        lastBulletTime = currentTime;
                        return;
                    }
                }
            }
        }
        public void UpdateGun()
        {

            if (gun.alive == true)
            {

                Rectangle gunRect = new Rectangle((int)gun.position.X, (int)gun.position.Y, gun.sprite.Width, gun.sprite.Height);
                Rectangle cannonRect = new Rectangle((int)cannon.position.X, (int)cannon.position.Y, cannon.sprite.Width, cannon.sprite.Height);

                if (gunRect.Intersects(cannonRect))
                {
                    cannon.hasWeaponUpgrade = true;
                    gun.alive = false;
                }
            }
        }
        public void UpdateBullet2()
        {
            foreach (gameObject bullet2 in Bullet2)
            {
                if (bullet2.alive)
                {
                    bullet2.position += bullet2.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet2.position.X, (int)bullet2.position.Y)))
                    {
                        bullet2.alive = false;
                        continue;
                    }
                    Rectangle bulletRect = new Rectangle((int)bullet2.position.X, (int)bullet2.position.Y, bullet2.sprite.Width, bullet2.sprite.Height);
                    foreach (gameObject enemy in zombies)
                    {
                        Rectangle enemyRect = new Rectangle((int)enemy.position.X, (int)enemy.position.Y, enemy.sprite.Width, enemy.sprite.Height);

                        if (bulletRect.Intersects(enemyRect))
                        {
                            bullet2.alive = false;
                            enemy.alive = false;
                            score += 1;
                            break;
                        }
                    }
                }


            }
        }
        public void UpdateBullet()
        {
            foreach (gameObject bullet in Bullet)
            {
                if (bullet.alive)
                {
                    bullet.position += bullet.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet.position.X,(int)bullet.position.Y)))
                    {
                        bullet.alive = false;
                            continue;
                    }
                    Rectangle cannonbulletRect = new Rectangle((int)bullet.position.X,(int)bullet.position.Y, bullet.sprite.Width,bullet.sprite.Height);
                    foreach (gameObject zombie in zombies)
                    {
                        Rectangle zombieRect = new Rectangle((int)zombie.position.X, (int)zombie.position.Y, zombie.sprite.Width, zombie.sprite.Height);
                        if (cannonbulletRect.Intersects(zombieRect))
                        {

                            bullet.alive = false;
                            zombie.alive = false;
                            enemiesKilled++;
                            score += 10;
                            smoke.AddParticles(zombie.position);
                            explosion.AddParticles(zombie.position);
                            break;
                            
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);


            switch (mCurrentScreen)
            {
                case ScreenState.TitleScreen:
                    {
                        spriteBatch.Draw(startScreen, Vector2.Zero, Color.White);
                        break;
                    }

                case ScreenState.ControlScreen:
                    {
                        spriteBatch.Draw(controlScreen, Vector2.Zero, Color.White);
                        break;
                    }

                #region Gamescreen draw code
                case ScreenState.GameScreen:
                    {
                        spriteBatch.Draw(backgroundTexture, viewportRect, Color.White);
                        if (cannon.alive == true)
                        {
                            spriteBatch.Draw(cannon.sprite,
                                cannon.position,
                                null, Color.White,
                                cannon.rotation,
                                cannon.center,
                                1.0f,
                                SpriteEffects.None, 0);

                            //Redraws cannon sprite but darker to simulate a shadow
                            spriteBatch.Draw(cannon.sprite,
                                new Vector2(cannon.position.X + 5, cannon.position.Y - 20),
                                null, new Color(0, 0, 0, 80),
                                cannon.rotation,
                                cannon.center,
                                1.0f,
                                SpriteEffects.None, 0.6f);
                        }
                        else
                        {
                            spriteBatch.Draw(cannon.sprite,
                            cannon.position,
                            null, Color.Green,
                            cannon.rotation,
                            cannon.center,
                            1.0f,
                            SpriteEffects.None, 0);

                            //Redraws cannon sprite but darker to simulate a shadow
                            spriteBatch.Draw(cannon.sprite,
                                new Vector2(cannon.position.X + 5, cannon.position.Y - 20),
                                null, new Color(0, 0, 0, 80),
                                cannon.rotation,
                                cannon.center,
                                1.0f,
                                SpriteEffects.None, 0.6f);
                        }


                        foreach (gameObject zombie in zombies)
                        {
                            if (zombie.alive)
                            {

                                spriteBatch.Draw(zombie.sprite,
                            zombie.position,
                            null, Color.White,
                            zombie.rotation,
                            zombie.center,
                            1.0f,
                            SpriteEffects.None, 0);

                                //zombie shadows

                                spriteBatch.Draw(zombie.sprite,
                                new Vector2(zombie.position.X + 5, zombie.position.Y - 20),
                                null, new Color(0, 0, 0, 80),
                                zombie.rotation,
                                zombie.center,
                                1.0f,
                                SpriteEffects.None, 0.6f);
                            }
                        }

                        foreach (gameObject bullet in Bullet)
                        {
                            if (bullet.alive)
                            {
                                spriteBatch.Draw(bullet.sprite, bullet.position, Color.Black);
                                //Redraws cannon sprite but darker to simulate a shadow
                                spriteBatch.Draw(bullet.sprite,
                                    new Vector2(bullet.position.X + 5, bullet.position.Y - 20),
                                    null, new Color(0, 0, 0, 80),
                                    cannon.rotation,
                                    cannon.center,
                                    1.0f,
                                    SpriteEffects.None, 0.6f);
                            }
                        }
                        foreach (gameObject bullet2 in Bullet2)
                        {
                            if (bullet2.alive)
                            {
                                spriteBatch.Draw(bullet2.sprite, bullet2.position, Color.White);
                                //simulates shadow
                                spriteBatch.Draw(bullet2.sprite, new Vector2(bullet2.position.X + 5, bullet2.position.Y - 15), new Color(0, 0, 0, 80));


                            }
                        }
                        if (turret.alive == true)
                        {
                            spriteBatch.Draw(turretStand.sprite,
                            turretStand.position,
                            null, Color.White,
                            turretStand.rotation,
                            turretStand.center,
                            1.0f,
                            SpriteEffects.None, 0);

                            spriteBatch.Draw(turret.sprite,
                            turret.position,
                            null, Color.White,
                            turret.rotation,
                            turret.center,
                            1.0f,
                            SpriteEffects.None, 0);
                            //turret shadow
                            spriteBatch.Draw(turret.sprite,
                             new Vector2(turret.position.X + 5, turret.position.Y - 20),
                            null, new Color(0, 0, 0, 80),
                            turret.rotation,
                            turret.center,
                            1.0f,
                            SpriteEffects.None, 0.6f);

                        }


                        spriteBatch.DrawString(font, "Score: " + score.ToString() + " Lives: " + lives.ToString(),
                            new Vector2(scoreDrawPoint.X * viewportRect.Width,
                                scoreDrawPoint.Y * viewportRect.Height),
                                Color.Yellow);


                        MouseState mState = Mouse.GetState();

                        Vector2 mousePos = new Vector2(mState.X - mouseIcon.center.X, mState.Y - mouseIcon.center.Y);

                        spriteBatch.Draw(mouseIcon.sprite, mousePos, Color.White);

                        if (gun.alive)
                        {
                            spriteBatch.Draw(gun.sprite, gun.position, null, Color.White, 0, gun.center, 0.8f, SpriteEffects.None, 0);
                        }

                        spriteBatch.Draw(mouseIcon.sprite, mousePos, Color.White);
                        if (cannon.hasWeaponUpgrade == true)
                        {
                            spriteBatch.DrawString(font, "Bullets Remaining:" + FiredBullet2s.ToString(), new Vector2(weaponDrawPoint.X + 0.10f * viewportRect.Width, weaponDrawPoint.Y * viewportRect.Height - 85), Color.White);

                        }
                        break;
                    }
                #endregion

                case ScreenState.DeadScreen:
                    {
                        spriteBatch.Draw(deadScreen, Vector2.Zero, Color.White);
                        break;
                    }
            }
            
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

    }
}
