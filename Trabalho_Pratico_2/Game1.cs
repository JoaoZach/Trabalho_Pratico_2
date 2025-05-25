using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

namespace Trabalho_Pratico_2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D pixel;

        private Animation idleAnimation, walkAnimation, jumpAnimation, attackAnimation;
        private Texture2D skellyIdleTexture, skellyWalkTexture, skellyJumpTexture, skellyAttackTexture;

        private Player player;
        private Vector2 cameraPosition;
        private float cameraFollowSpeed = 0.1f;

        private int groundLevel;
        private int worldWidth = 1280 * 2;
        private int worldHeight = 3200;

        private List<int> floorLevels = new List<int> { 3050, 2180, 1270 };

        private List<Platform> platforms = new List<Platform>();
        private List<Elevator> elevators = new List<Elevator>();

        private Texture2D slimeTexture;
        private Animation slimeAnimation;
        private List<Enemy> enemies = new List<Enemy>();

        private Texture2D batTexture;
        private Animation batAnimation;
        private List<BatEnemy> bats = new List<BatEnemy>();

        private Texture2D slimeBossTexture;
        private Animation slimeBossAnimation;
        private SlimeBoss slimeBoss;

        private Song backgroundMusic;
        private SoundEffect attackSound;
        private SoundEffect enemyDeathSound;
        private SoundEffect batDeathSound;
        private SoundEffect enemyHurtSound;
        private SoundEffect batHurtSound;
        private SoundEffect hurtSfx;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1680;
            _graphics.PreferredBackBufferHeight = 920;
            _graphics.ApplyChanges();

            groundLevel = 2850;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("background2");
            skellyIdleTexture = Content.Load<Texture2D>("skelly");
            skellyWalkTexture = Content.Load<Texture2D>("skelly walking");
            skellyJumpTexture = Content.Load<Texture2D>("skelly jump");
            skellyAttackTexture = Content.Load<Texture2D>("skelly attack 1");
            slimeTexture = Content.Load<Texture2D>("slime enemy");
            batTexture = Content.Load<Texture2D>("Morcego feio");
            slimeBossTexture = Content.Load<Texture2D>("slime boss");

            backgroundMusic = Content.Load<Song>("Sound/game-music-loop");
            attackSound = Content.Load<SoundEffect>("Sound/sword-sound");
            enemyDeathSound = Content.Load<SoundEffect>("Sound/monster-death-grunt");
            batDeathSound = Content.Load<SoundEffect>("Sound/monster-death-grunt");
            SoundEffect jumpSound = Content.Load<SoundEffect>("Sound/jump");
            enemyHurtSound = Content.Load<SoundEffect>("Sound/hurt");
            batHurtSound = Content.Load<SoundEffect>("Sound/hurt");
            hurtSfx = Content.Load<SoundEffect>("Sound/player-hurt");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(backgroundMusic);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            Vector2 elevatorSize = new Vector2(200, 20);
            int elevatorMargin = 50;

            for (int i = 0; i < 2; i++)
            {
                int floorY = floorLevels[i];
                float y = floorY - elevatorSize.Y;

                float x = (i == 1)
                    ? elevatorMargin
                    : worldWidth - elevatorSize.X - elevatorMargin;

                elevators.Add(new Elevator(
                    new Vector2(x, y),
                    elevatorSize,
                    5f,
                    y - 800,
                    y + 50
                ));
            }

            foreach (int y in floorLevels)
            {
                platforms.Add(new Platform(y, worldWidth, 40, 300));
            }

            Vector2 frameSize = new Vector2(450, 300);
            Vector2 frameSize2 = new Vector2(500, 500);

            idleAnimation = new Animation(11, 3, frameSize);
            walkAnimation = new Animation(19, 4, frameSize);
            jumpAnimation = new Animation(20, 4, frameSize, isLooping: false);
            attackAnimation = new Animation(12, 3, frameSize, isLooping: false);
            batAnimation = new Animation(5, 2, new Vector2(400, 400));
            slimeAnimation = new Animation(20, 5, frameSize2);
            slimeBossAnimation = new Animation(7, 3, frameSize2); // mesma divisão que o slime normal

            player = new Player(
                skellyIdleTexture, skellyWalkTexture, skellyJumpTexture, skellyAttackTexture,
                idleAnimation, walkAnimation, jumpAnimation, attackAnimation,
                groundLevel, new Vector2(50, 2600), attackSound, jumpSound, hurtSfx
            );

            for (int floorIndex = 0; floorIndex < floorLevels.Count; floorIndex++)
            {
                int floorY = floorLevels[floorIndex];
                float enemyY = floorY - 500;

                // Apenas adiciona inimigos comuns e morcegos nos dois primeiros andares
                if (floorIndex < 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        float enemyX = 300 + i * 500;
                        enemies.Add(new Enemy(slimeTexture, new Vector2(enemyX, enemyY), slimeAnimation, floorY, platforms, enemyDeathSound, enemyHurtSound));

                        float batY = enemyY - 300;
                        var bat = new BatEnemy(batTexture, new Vector2(enemyX, batY), batAnimation, batDeathSound, batHurtSound);
                        bat.SetPlatforms(platforms);
                        bats.Add(bat);
                    }
                }
                else if (floorIndex == 2) // Criação do SlimeBoss no 3º andar
                {
                    slimeBoss = new SlimeBoss(
                        slimeBossTexture,
                        new Vector2(worldWidth / 2, floorLevels[2] - 500),
                        slimeBossAnimation,
                        floorLevels[2],      // groundLevel
                        platforms,           // lista de plataformas
                        enemyDeathSound,     // deathSound
                        enemyHurtSound       // hurtSound
                    );
                }
            }


        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (player.IsDead)
            {
                Exit();
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();

            Vector2 targetCameraPosition = player.Position - new Vector2(
                _graphics.PreferredBackBufferWidth / 2 - 150,
                _graphics.PreferredBackBufferHeight / 2 - 150
            );

            targetCameraPosition.X = MathHelper.Clamp(
                targetCameraPosition.X,
                0,
                worldWidth - _graphics.PreferredBackBufferWidth
            );

            targetCameraPosition.Y = MathHelper.Clamp(
                targetCameraPosition.Y,
                0,
                worldHeight - _graphics.PreferredBackBufferHeight
            );

            cameraPosition = Vector2.Lerp(cameraPosition, targetCameraPosition, cameraFollowSpeed);

            foreach (var enemy in enemies.ToList())
            {
                enemy.Update(gameTime, player.Position);

                if (enemy.IsAlive && enemy.Hitbox.Intersects(player.Hitbox))
                    player.TakeDamage(enemy.Position, 50f);

                if (enemy.IsAlive && player.AttackHitbox.Intersects(enemy.Hitbox))
                    enemy.TakeDamage(player.Position, 30f);
            }

            foreach (var bat in bats.ToList())
            {
                bat.Update(gameTime, player.Position);

                if (bat.IsAlive && bat.Hitbox.Intersects(player.Hitbox))
                    player.TakeDamage(bat.Position, 30f);

                if (bat.IsAlive && player.AttackHitbox.Intersects(bat.Hitbox))
                    bat.TakeDamage(player.Position, 30f);
            }

            if (slimeBoss.IsAlive)
            {
                slimeBoss.Update(gameTime, player.Position);

                if (slimeBoss.IsAlive && slimeBoss.Hitbox.Intersects(player.Hitbox))
                    player.TakeDamage(slimeBoss.Position, 60f);

                if (slimeBoss.IsAlive && player.AttackHitbox.Intersects(slimeBoss.Hitbox))
                    slimeBoss.TakeDamage(player.Position, 40f);
            }

            // Se o SlimeBoss morreu, termina o jogo
            if (!slimeBoss.IsAlive)
            {
                Exit();
            }


            player.Position = new Vector2(
                MathHelper.Clamp(player.Position.X, 0, worldWidth - 300),
                player.Position.Y
            );

            foreach (var elev in elevators)
                elev.Update(gameTime);

            Elevator currentElevator = elevators.FirstOrDefault(e => player.Hitbox.Bottom >= e.Position.Y && player.Hitbox.Intersects(e.Hitbox));

            player.Update(gameTime, keyboardState, currentElevator, floorLevels, platforms);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix transform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0));

            _spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, worldWidth, worldHeight), Color.White);

            foreach (var platform in platforms)
                platform.Draw(_spriteBatch, pixel);

            foreach (var elev in elevators)
                elev.Draw(_spriteBatch, pixel);

            player.Draw(_spriteBatch, player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, pixel);

            foreach (var enemy in enemies)
                enemy.Draw(_spriteBatch, pixel);

            foreach (var bat in bats)
                bat.Draw(_spriteBatch, pixel);

            if (slimeBoss.IsAlive)
                slimeBoss.Draw(_spriteBatch, pixel);

            _spriteBatch.End();

            _spriteBatch.Begin();

            int lifeIconSize = 30;
            int padding = 10;
            for (int i = 0; i < player.Health; i++)
            {
                _spriteBatch.Draw(pixel, new Rectangle(padding + i * (lifeIconSize + padding), padding, lifeIconSize, lifeIconSize), Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
