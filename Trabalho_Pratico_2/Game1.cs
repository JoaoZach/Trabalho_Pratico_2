using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Trabalho_Pratico_2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D skellyIdleTexture;
        private Texture2D skellyWalkTexture;
        private Texture2D skellyJumpTexture;
        private Texture2D skellyAttack1Texture;
        private Texture2D skellyAttack2Texture;
        private Texture2D skellyAttack3Texture;
        private Texture2D currentTexture;

        private AnimationManager animationManager;
        private Animation walkAnimation;
        private Animation idleAnimation;
        private Animation jumpAnimation;
        private Animation attack1Animation;
        private Animation attack2Animation;
        private Animation attack3Animation;

        private Vector2 spritePosition;
        private Vector2 cameraPosition;
        private float cameraFollowSpeed = 0.1f;
        private float speed = 8f;
        private bool facingRight = true;

        private Texture2D backgroundTexture;
        private Rectangle gameBounds;
        private int groundLevel;

        private Vector2 velocity;
        private float gravity = 0.5f;
        private float jumpStrength = -12f;
        private bool isOnGround = false;
        private bool isJumping = false;

        private Texture2D pixel;
        private Rectangle playerHitbox;

        private bool isAttacking = false;
        private Rectangle attackHitbox;
        private int attackWidth = 100;
        private int attackHeight = 150;

        private Enemy slimeEnemy;
        private Texture2D slimeTexture;
        private Animation slimeAnimation;

        private int worldWidth = 1280 * 3;

        private Player player;


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
            gameBounds = new Rectangle(0, 0, worldWidth, _graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("background");
            skellyIdleTexture = Content.Load<Texture2D>("skelly");
            skellyWalkTexture = Content.Load<Texture2D>("skelly walking");
            skellyJumpTexture = Content.Load<Texture2D>("skelly jump");
            skellyAttack1Texture = Content.Load<Texture2D>("skelly attack 1");
            skellyAttack2Texture = Content.Load<Texture2D>("skelly attack 2");
            skellyAttack3Texture = Content.Load<Texture2D>("skelly attack 3");

            slimeTexture = Content.Load<Texture2D>("slime enemy");

            Vector2 frameSize = new Vector2(450, 300);
            Vector2 frameSize2 = new Vector2(500, 500);

            idleAnimation = new Animation(11, 3, frameSize);
            walkAnimation = new Animation(19, 4, frameSize);
            jumpAnimation = new Animation(20, 4, frameSize, isLooping: false);
            attack1Animation = new Animation(13, 3, frameSize, isLooping: false);
            attack2Animation = new Animation(18, 3, frameSize, isLooping: false);
            attack3Animation = new Animation(21, 4, frameSize, isLooping: false);

            slimeAnimation = new Animation(20, 5, frameSize2);

            animationManager = new AnimationManager(idleAnimation);
            currentTexture = skellyIdleTexture;

            spritePosition = new Vector2(300, 300);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            player = new Player(
                 skellyIdleTexture,
                 skellyWalkTexture,
                 skellyJumpTexture,
                 idleAnimation,
                 walkAnimation,
                 jumpAnimation,
                 groundLevel,
                 new Vector2(300, 300)
             );



            slimeEnemy = new Enemy(slimeTexture, new Vector2(800, 0), slimeAnimation);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();
            Vector2 input = Vector2.Zero;

            bool zPressed = state.IsKeyDown(Keys.Z);
            bool xPressed = state.IsKeyDown(Keys.X);
            bool cPressed = state.IsKeyDown(Keys.C);
            bool jumpPressed = state.IsKeyDown(Keys.Space);

            attackHitbox = Rectangle.Empty;

            // Gerenciar ataque
            if (isAttacking)
            {
                animationManager.Update(gameTime);

                int currentFrame = animationManager.CurrentFrameIndex;

                // Ativa a hitbox somente entre os frames 5 e 10
                if (currentFrame >= 5 && currentFrame <= 10)
                {
                    int attackX = facingRight ? (int)(spritePosition.X + 300) : (int)(spritePosition.X - attackWidth);
                    int attackY = (int)spritePosition.Y + 100;
                    attackHitbox = new Rectangle(attackX, attackY, attackWidth, attackHeight);

                    if (attackHitbox.Intersects(slimeEnemy.Hitbox) && slimeEnemy.IsAlive)
                    {
                        float attackForce = 12f; // ajuste este valor para mais ou menos knockback
                        slimeEnemy.TakeDamage(spritePosition, attackForce);

                    }
                }

                if (animationManager.HasFinished())
                {
                    isAttacking = false;
                    attackHitbox = Rectangle.Empty;
                }
            }
            else
            {
                if (zPressed)
                {
                    isAttacking = true;
                    PlayAttackAnimation(1);
                }
                else if (xPressed)
                {
                    isAttacking = true;
                    PlayAttackAnimation(2);
                }
                else if (cPressed)
                {
                    isAttacking = true;
                    PlayAttackAnimation(3);
                }
            }

            // Movimento
            if (state.IsKeyDown(Keys.Left)) input.X -= 1;
            if (state.IsKeyDown(Keys.Right)) input.X += 1;

            if (jumpPressed && isOnGround)
            {
                velocity.Y = jumpStrength;
                isOnGround = false;
                isJumping = true;
                animationManager.SetAnimation(jumpAnimation);
                currentTexture = skellyJumpTexture;
            }

            velocity.Y += gravity;
            spritePosition.Y += velocity.Y;

            if (input != Vector2.Zero)
            {
                spritePosition += Vector2.Normalize(input) * speed;
                facingRight = input.X > 0;
            }

            if (spritePosition.Y + 150 >= groundLevel)
            {
                spritePosition.Y = groundLevel - 150;
                velocity.Y = 0;
                isOnGround = true;
                isJumping = false;
            }

            if (!isJumping && !isAttacking)
            {
                if (input != Vector2.Zero)
                {
                    animationManager.SetAnimation(walkAnimation);
                    currentTexture = skellyWalkTexture;
                }
                else
                {
                    animationManager.SetAnimation(idleAnimation);
                    currentTexture = skellyIdleTexture;
                }
            }

            animationManager.Update(gameTime);

            Vector2 targetCameraPos = spritePosition - new Vector2(_graphics.PreferredBackBufferWidth / 2 - 150, _graphics.PreferredBackBufferHeight / 2 - 150);
            cameraPosition = Vector2.Lerp(cameraPosition, targetCameraPos, cameraFollowSpeed);


            int spriteWidth = 300;
            int spriteHeight = 300;
            int hitboxWidth = 120;
            int hitboxHeight = 180;

            int centerX = (int)spritePosition.X + spriteWidth / 2;
            int centerY = (int)spritePosition.Y + spriteHeight / 2;

            player.Update(gameTime, Keyboard.GetState());
            spritePosition = player.Position;  // sincronizar posição para câmera e inimigos
            playerHitbox = player.Hitbox;
            // Atualizar inimigo
            slimeEnemy.Update(gameTime, spritePosition);

            // Verificar colisão entre player e inimigo para causar dano ao player
            if (slimeEnemy.IsAlive && slimeEnemy.Hitbox.Intersects(playerHitbox))
            {
                // Aplica dano e knockback no player
                float damageForce = 10f;
                // Assumindo que você tem uma instância do Player chamada "player"
                player.TakeDamage(slimeEnemy.Position, damageForce);
            }


            spritePosition.X = MathHelper.Clamp(spritePosition.X, 0, worldWidth - 300);

            base.Update(gameTime);
        }

        private void PlayAttackAnimation(int step)
        {
            switch (step)
            {
                case 1:
                    animationManager.SetAnimation(attack1Animation);
                    currentTexture = skellyAttack1Texture;
                    break;
                case 2:
                    animationManager.SetAnimation(attack2Animation);
                    currentTexture = skellyAttack2Texture;
                    break;
                case 3:
                    animationManager.SetAnimation(attack3Animation);
                    currentTexture = skellyAttack3Texture;
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Matrix transform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0));
            _spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);

            player.Draw(_spriteBatch, player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally);


            for (int x = 0; x < worldWidth; x += backgroundTexture.Width)
            {
                _spriteBatch.Draw(backgroundTexture, new Rectangle(x, 0, backgroundTexture.Width, 720), Color.White);
            }

            _spriteBatch.Draw(
                currentTexture,
                new Rectangle((int)spritePosition.X, (int)spritePosition.Y, 300, 300),
                animationManager.GetFrame(),
                Color.White,
                0f,
                Vector2.Zero,
                facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );

            _spriteBatch.Draw(pixel, playerHitbox, Color.Blue * 0.4f);

            if (isAttacking && attackHitbox != Rectangle.Empty)
            {
                _spriteBatch.Draw(pixel, attackHitbox, Color.Yellow * 0.5f);
            }

            slimeEnemy.Draw(_spriteBatch, pixel);

            

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
