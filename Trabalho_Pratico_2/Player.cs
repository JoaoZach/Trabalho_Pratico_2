using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Trabalho_Pratico_2
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private float speed = 8f;
        private float gravity = 0.5f;
        private float jumpStrength = -12f;
        private bool isOnGround = false;
        private bool isJumping = false;
        private bool facingRight = true;
        private int maxHealth = 3;

        private Texture2D idleTexture;
        private Texture2D walkTexture;
        private Texture2D jumpTexture;
        private Texture2D currentTexture;

        private AnimationManager animationManager;
        private Animation walkAnimation;
        private Animation idleAnimation;
        private Animation jumpAnimation;

        public Rectangle Hitbox { get; private set; }

        private int groundLevel;

        private int health;
        private bool isDamaged = false;
        private double damageTimer = 0;
        private double damageDuration = 500; // ms

        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackFriction = 0.9f;

        public Player(Texture2D idle, Texture2D walk, Texture2D jump,
                      Animation idleAnim, Animation walkAnim, Animation jumpAnim,
                      int groundY, Vector2 startPosition)
        {
            idleTexture = idle;
            walkTexture = walk;
            jumpTexture = jump;

            idleAnimation = idleAnim;
            walkAnimation = walkAnim;
            jumpAnimation = jumpAnim;

            animationManager = new AnimationManager(idleAnimation);
            currentTexture = idleTexture;

            Position = startPosition;
            groundLevel = groundY;

            health = maxHealth; // inicializa vida como vida máxima
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Vector2 input = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                input.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                input.X += 1;

            bool jumpPressed = keyboardState.IsKeyDown(Keys.Space);

            if (jumpPressed && isOnGround)
            {
                Velocity.Y = jumpStrength;
                isOnGround = false;
                isJumping = true;

                animationManager.SetAnimation(jumpAnimation);
                currentTexture = jumpTexture;
            }

            Velocity.Y += gravity;
            Position.Y += Velocity.Y;

            if (input != Vector2.Zero)
            {
                Position += Vector2.Normalize(input) * speed;
                facingRight = input.X > 0;
            }

            if (Position.Y + 150 >= groundLevel)
            {
                Position.Y = groundLevel - 150;
                Velocity.Y = 0;

                if (!isOnGround)
                {
                    isOnGround = true;
                    isJumping = false;
                }
            }
            else
            {
                isOnGround = false;
            }

            // Animation logic
            if (isJumping)
            {
                if (animationManager.HasFinished())
                {
                    isJumping = false;
                    if (input != Vector2.Zero)
                    {
                        animationManager.SetAnimation(walkAnimation);
                        currentTexture = walkTexture;
                    }
                    else
                    {
                        animationManager.SetAnimation(idleAnimation);
                        currentTexture = idleTexture;
                    }
                }
            }
            else
            {
                if (input != Vector2.Zero)
                {
                    animationManager.SetAnimation(walkAnimation);
                    currentTexture = walkTexture;
                }
                else
                {
                    animationManager.SetAnimation(idleAnimation);
                    currentTexture = idleTexture;
                }
            }

            animationManager.Update(gameTime);

            // Atualiza hitbox (centralizada)
            int spriteWidth = 300;
            int spriteHeight = 300;

            int hitboxWidth = 120;
            int hitboxHeight = 180;

            int centerX = (int)Position.X + spriteWidth / 2;
            int centerY = (int)Position.Y + spriteHeight / 2;

            Hitbox = new Rectangle(
                centerX - hitboxWidth / 2,
                centerY - hitboxHeight / 2,
                hitboxWidth,
                hitboxHeight
            );

            // Atualiza temporizador de dano
            if (isDamaged)
            {
                damageTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (damageTimer >= damageDuration)
                {
                    isDamaged = false;
                    damageTimer = 0;
                }
            }

            // Aplica knockback
            Position += knockbackVelocity;
            knockbackVelocity *= knockbackFriction;

            if (knockbackVelocity.Length() < 0.1f)
                knockbackVelocity = Vector2.Zero;
        }

        public void TakeDamage(Vector2 attackerPosition, float force)
        {
            if (isDamaged) return; // Invencibilidade curta após dano

            health--;
            if (health < 0) health = 0; // não deixa vida negativa

            if (health <= 0)
            {
                // Aqui você pode tratar a morte do player, reiniciar, etc.
            }

            isDamaged = true;
            damageTimer = 0;

            Vector2 direction = Position - attackerPosition;

            if (direction == Vector2.Zero)
            {
                direction = new Vector2(facingRight ? 1 : -1, 0);
            }
            else
            {
                direction.Normalize();

                // Garante que o knockback sempre empurre o player para frente
                if (direction.X * (facingRight ? 1 : -1) < 0)
                {
                    direction.X = -direction.X;
                }
            }

            knockbackVelocity = direction * force;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects effects)
        {
            Color drawColor = Color.White;

            if (isDamaged)
            {
                // Piscar: muda cor entre transparente e branca a cada 100ms
                int blinkFrequency = 100; // ms
                int time = (int)(damageTimer / blinkFrequency);
                drawColor = (time % 2 == 0) ? Color.White : Color.Transparent;
            }

            spriteBatch.Draw(
                currentTexture,
                new Rectangle((int)Position.X, (int)Position.Y, 300, 300),
                animationManager.GetFrame(),
                drawColor,
                0f,
                Vector2.Zero,
                effects,
                0f
            );
        }

        public bool FacingRight => facingRight;
    }
}
