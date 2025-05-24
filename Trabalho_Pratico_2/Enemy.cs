using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private float gravity = 0.5f;
        private float verticalVelocity = 0f;
        private int verticalOffset = 160;

        private int groundLevel;
        private SpriteEffects spriteEffect = SpriteEffects.None;
        private const int spriteSize = 500;

        private int direction = 1; // 1 = direita, -1 = esquerda
        private float moveSpeed = 1.2f;

        private int health = 2;
        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackFriction = 0.9f;

        private bool isDamaged = false;
        private double damageTimer = 0;
        private double damageDuration = 500;
        private Color baseColor = Color.White;

        private List<Platform> platforms;

        public Enemy(Texture2D texture, Vector2 position, Animation animation, int groundLevel, List<Platform> platforms)
        {
            Texture = texture;
            Position = position;
            this.animation = animation;
            this.groundLevel = groundLevel;
            this.platforms = platforms;
            animationManager = new AnimationManager(animation);
            UpdateHitbox();
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if (!IsAlive) return;

            // Gravidade
            verticalVelocity += gravity;
            Position.Y += verticalVelocity;

            if (Position.Y + spriteSize - verticalOffset >= groundLevel)
            {
                Position.Y = groundLevel - spriteSize + verticalOffset;
                verticalVelocity = 0f;
            }

            // Movimento automático com verificação de colisão lateral
            Vector2 nextPosition = Position;
            nextPosition.X += direction * moveSpeed;

            Rectangle futureHitbox = new Rectangle(
                (int)nextPosition.X + spriteSize / 2 - Hitbox.Width / 2,
                (int)Position.Y + spriteSize / 2 - Hitbox.Height / 2,
                Hitbox.Width,
                Hitbox.Height
            );

            bool collided = platforms.Any(p => p.LeftWall.Intersects(futureHitbox) || p.RightWall.Intersects(futureHitbox));
            if (collided)
            {
                direction *= -1;
            }
            else
            {
                Position.X += direction * moveSpeed;
            }


            spriteEffect = direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            animationManager.Update(gameTime);
            UpdateHitbox();

            // Dano
            if (isDamaged)
            {
                damageTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (damageTimer >= damageDuration)
                {
                    isDamaged = false;
                    damageTimer = 0;
                }
            }

            // Knockback
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

            // Hitbox (visual)
            spriteBatch.Draw(pixel, Hitbox, Color.Red * 0.4f);
        }
    }
}
