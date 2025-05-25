using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Trabalho_Pratico_2
{
    public class BatEnemy
    {
        public Vector2 Position;
        private Texture2D Texture;
        private Animation animation;
        private AnimationManager animationManager;
        public Rectangle Hitbox;
        public bool IsAlive = true;

        private const int spriteSize = 400;
        private int health = 2;
        private Vector2 velocity;
        private float moveSpeed = 3.2f;

        private SpriteEffects spriteEffect = SpriteEffects.None;
        private Color baseColor = Color.White;
        private bool isDamaged = false;
        private double damageTimer = 0;
        private double damageDuration = 500;
        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackFriction = 0.9f;

        private List<Platform> platforms;
        private SoundEffect deathSound;
        private SoundEffect hurtSound;


        public BatEnemy(Texture2D texture, Vector2 position, Animation animation, SoundEffect deathSound, SoundEffect hurtSound)
        {
            Texture = texture;
            Position = position;
            this.animation = animation;
            animationManager = new AnimationManager(animation);
            UpdateHitbox();
            this.deathSound = deathSound;
            this.hurtSound = hurtSound;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            if (!IsAlive) return;

            // Verifica se o morcego "vê" o jogador (sem plataforma no caminho)
            bool hasLineOfSight = true;
            if (platforms != null)
            {
                Rectangle sightLine = new Rectangle(
                    (int)Math.Min(Position.X, playerPosition.X),
                    (int)Math.Min(Position.Y, playerPosition.Y),
                    (int)Math.Abs(Position.X - playerPosition.X),
                    (int)Math.Abs(Position.Y - playerPosition.Y)
                );

                foreach (var platform in platforms)
                {
                    if (platform.Floor.Intersects(sightLine))
                    {
                        hasLineOfSight = false;
                        break;
                    }
                }
            }

            if (hasLineOfSight)
            {
                // Move-se em direção ao jogador
                Vector2 direction = playerPosition - Position;
                if (direction.Length() > 1f)
                {
                    direction.Normalize();
                    velocity = direction * moveSpeed;
                }
            }
            else
            {
                velocity = Vector2.Zero;
            }

            Position += velocity;

            // Knockback
            Position += knockbackVelocity;
            knockbackVelocity *= knockbackFriction;
            if (knockbackVelocity.Length() < 0.1f)
                knockbackVelocity = Vector2.Zero;

            spriteEffect = velocity.X > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

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

            animationManager.Update(gameTime);
            UpdateHitbox();
        }

        private void UpdateHitbox()
        {
            // Hitbox reduzida
            int width = 180;
            int height = 120;
            int centerX = (int)Position.X + spriteSize / 2;
            int centerY = (int)Position.Y + spriteSize / 2;

            Hitbox = new Rectangle(centerX - width / 2, centerY - height / 2, width, height);
        }

        public void TakeDamage(Vector2 attackerPosition, float force)
        {
            if (!IsAlive || isDamaged) return;

            isDamaged = true;
            damageTimer = 0;
            health--;

            hurtSound?.Play(); // <-- AQUI

            Vector2 direction = Position - attackerPosition;
            direction.Normalize();
            knockbackVelocity = direction * force;

            if (health <= 0)
            {
                IsAlive = false;
                deathSound?.Play();
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

            // Visualização da hitbox (opcional)
            spriteBatch.Draw(pixel, Hitbox, Color.OrangeRed * 0.4f);
        }

        // Permite injetar plataformas para detectar "visão"
        public void SetPlatforms(List<Platform> platforms)
        {
            this.platforms = platforms;
        }
    }
}
