using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private int worldWidth = 1280 * 3;

        private Enemy slimeEnemy;
        private Texture2D slimeTexture;
        private Animation slimeAnimation;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            groundLevel = 550;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("background");
            skellyIdleTexture = Content.Load<Texture2D>("skelly");
            skellyWalkTexture = Content.Load<Texture2D>("skelly walking");
            skellyJumpTexture = Content.Load<Texture2D>("skelly jump");
            skellyAttackTexture = Content.Load<Texture2D>("skelly attack 1");
            slimeTexture = Content.Load<Texture2D>("slime enemy");

            Vector2 frameSize = new Vector2(450, 300);
            Vector2 frameSize2 = new Vector2(500, 500);

            idleAnimation = new Animation(11, 3, frameSize);
            walkAnimation = new Animation(19, 4, frameSize);
            jumpAnimation = new Animation(20, 4, frameSize, isLooping: false);
            attackAnimation = new Animation(12, 3, frameSize, isLooping: false);

            slimeAnimation = new Animation(20, 5, frameSize2);

            player = new Player(skellyIdleTexture, skellyWalkTexture, skellyJumpTexture, skellyAttackTexture,
                                idleAnimation, walkAnimation, jumpAnimation, attackAnimation,
                                groundLevel, new Vector2(300, 300));

            slimeEnemy = new Enemy(slimeTexture, new Vector2(800, 0), slimeAnimation);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, Keyboard.GetState());

            cameraPosition = Vector2.Lerp(
                cameraPosition,
                player.Position - new Vector2(_graphics.PreferredBackBufferWidth / 2 - 150, _graphics.PreferredBackBufferHeight / 2 - 150),
                cameraFollowSpeed
            );

            slimeEnemy.Update(gameTime, player.Position);

            if (slimeEnemy.IsAlive && slimeEnemy.Hitbox.Intersects(player.Hitbox))
            {
                player.TakeDamage(slimeEnemy.Position, 50f);
            }

            if (slimeEnemy.IsAlive && player.AttackHitbox.Intersects(slimeEnemy.Hitbox))
            {
                slimeEnemy.TakeDamage(player.Position, 12f);
            }

            player.Position = new Vector2(
                MathHelper.Clamp(player.Position.X, 0, worldWidth - 300),
                player.Position.Y
            );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix transform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0));
            _spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

            for (int x = 0; x < worldWidth; x += backgroundTexture.Width)
            {
                _spriteBatch.Draw(backgroundTexture, new Rectangle(x, 0, backgroundTexture.Width, 720), Color.White);
            }

            player.Draw(_spriteBatch, player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally, pixel);
            slimeEnemy.Draw(_spriteBatch, pixel);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
