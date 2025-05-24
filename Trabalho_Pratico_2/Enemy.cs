using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Trabalho_Pratico_2
{
    public class Enemy
    {
        public Vector2 Position;
        private Texture2D Texture;
        private Animation animation;
        private AnimationManager animationManager;
        public Rectangle Hitbox;
        public bool IsAlive = true;

        private float velocityY = 0f;
        private float gravity = 0.5f;
        private int groundLevel = 800;

        private bool isDamaged = false;
        private double damageTimer = 0;
        private double damageDuration = 500; // ms
        private Color baseColor = Color.White;

        private SpriteEffects spriteEffect = SpriteEffects.None;

        private const int spriteSize = 500; // tamanho do sprite do inimigo

        private int health = 2;
        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackFriction = 0.9f; // controle da desaceleração do knockback

        private float verticalVelocity = 0f;

        public Enemy(Texture2D texture, Vector2 position, Animation animation)
        {
            Texture = texture;
            Position = position;
            this.animation = animation;
            animationManager = new AnimationManager(animation);
            UpdateHitbox();
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if (!IsAlive) return;

            // Gravidade
            verticalVelocity += gravity;
            Position.Y += verticalVelocity;

            if (Position.Y + spriteSize >= groundLevel)
            {
                Position.Y = groundLevel - spriteSize;
                verticalVelocity = 0f;
            }

            // Movimento em direção ao player
            float speed = 1.2f;
            if (playerPosition.X < Position.X)
            {
                Position.X -= speed;
                spriteEffect = SpriteEffects.None; // Olha para a esquerda
            }
            else if (playerPosition.X > Position.X)
            {
                Position.X += speed;
                spriteEffect = SpriteEffects.FlipHorizontally; // Olha para a direita
            }


            animationManager.Update(gameTime);
            UpdateHitbox();

            // Temporizador de dano
            if (isDamaged)
            {
                damageTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (damageTimer >= damageDuration)
                {
                    isDamaged = false;
                    damageTimer = 0;
                }
            }

            Position += knockbackVelocity;
            knockbackVelocity *= knockbackFriction;

            if (knockbackVelocity.Length() < 0.1f)
                knockbackVelocity = Vector2.Zero;

        }

        private void UpdateHitbox()
        {
            int width = 320;
            int height = 180;
            int centerX = (int)Position.X + spriteSize / 2;
            int centerY = (int)Position.Y + spriteSize / 2;

            Hitbox = new Rectangle(centerX - width / 2, centerY - height / 2, width, height);
        }

        public void TakeDamage(Vector2 attackerPosition, float force)
        {
            if (!IsAlive) return;

            if (!isDamaged)
            {
                isDamaged = true;
                damageTimer = 0;
                health--;

                // Calcula a direção e aplica knockback com força personalizada
                Vector2 direction = Position - attackerPosition;
                direction.Normalize();
                knockbackVelocity = direction * force;

                if (health <= 0)
                {
                    IsAlive = false;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (!IsAlive) return;

            Color drawColor = isDamaged ? Color.Red : baseColor;

            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X + spriteSize / 2, Position.Y + spriteSize / 2),
                animationManager.GetFrame(),
                drawColor,
                0f,
                new Vector2(spriteSize / 2, spriteSize / 2),
                1f,
                spriteEffect,
                0f
            );


            // Desenhar a hitbox
            spriteBatch.Draw(pixel, Hitbox, Color.Red * 0.4f);
        }
    }
}
