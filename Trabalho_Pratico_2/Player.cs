using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Trabalho_Pratico_2
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private float speed = 8f;
        private float gravity = 0.5f;
        private float jumpStrength = -16f;
        private bool isOnGround = false;
        private bool isJumping = false;
        private bool facingRight = true;
        private int maxHealth = 3;

        private Texture2D idleTexture;
        private Texture2D walkTexture;
        private Texture2D jumpTexture;
        private Texture2D attackTexture;
        private Texture2D currentTexture;

        private AnimationManager animationManager;
        private Animation walkAnimation;
        private Animation idleAnimation;
        private Animation jumpAnimation;
        private Animation attackAnimation;

        public Rectangle Hitbox { get; private set; }

        private int health;
        private bool isDamaged = false;
        private double damageTimer = 0;
        private double damageDuration = 500;

        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackFriction = 0.9f;

        public Rectangle AttackHitbox { get; private set; } = Rectangle.Empty;
        private int attackWidth = 100;
        private int attackHeight = 150;

        private bool isAttacking = false;
        private double attackTimer = 0;
        private double attackDuration = 600;

        public Player(Texture2D idle, Texture2D walk, Texture2D jump, Texture2D attack,
                      Animation idleAnim, Animation walkAnim, Animation jumpAnim, Animation attackAnim,
                      int groundY, Vector2 startPosition)
        {
            idleTexture = idle;
            walkTexture = walk;
            jumpTexture = jump;
            attackTexture = attack;

            idleAnimation = idleAnim;
            walkAnimation = walkAnim;
            jumpAnimation = jumpAnim;
            attackAnimation = attackAnim;

            animationManager = new AnimationManager(idleAnimation);
            currentTexture = idleTexture;

            Position = startPosition;
            health = maxHealth;
        }

        private void UpdateHitbox()
        {
            int spriteW = 300;
            int spriteH = 300;
            int hitboxW = 120;
            int hitboxH = 180;
            int centerX = (int)Position.X + spriteW / 2;
            int centerY = (int)Position.Y + spriteH / 2;
            Hitbox = new Rectangle(centerX - hitboxW / 2, centerY - hitboxH / 2, hitboxW, hitboxH);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Elevator elevator, List<int> floorLevels, List<Platform> platforms)
        {
            Vector2 input = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                input.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                input.X += 1;

            bool jumpPressed = keyboardState.IsKeyDown(Keys.Space);

            if (input != Vector2.Zero)
            {
                Position += Vector2.Normalize(input) * speed;
                facingRight = input.X > 0;
            }

            if (!isAttacking && keyboardState.IsKeyDown(Keys.Z))
            {
                isAttacking = true;
                attackTimer = 0;
                animationManager.SetAnimation(attackAnimation);
                currentTexture = attackTexture;
            }

            // Detectar se está em cima do elevador
            bool onElevator = false;
            if (elevator != null)
            {
                Rectangle elevatorHitbox = elevator.Hitbox;
                Rectangle playerFeet = new Rectangle(Hitbox.X, Hitbox.Bottom, Hitbox.Width, 5);
                onElevator = playerFeet.Intersects(elevatorHitbox) && Velocity.Y >= 0;
            }

            // Determinar se pode pular (chão OU elevador)
            bool canJump = isOnGround || onElevator;

            if (isAttacking)
            {
                // Ataque em andamento
                attackTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                int spriteWidth = 300;
                int attackX = facingRight ? (int)(Position.X + spriteWidth - 100) : (int)(Position.X - attackWidth + 100);
                int attackY = (int)Position.Y + 100;
                AttackHitbox = new Rectangle(attackX, attackY, attackWidth, attackHeight);

                animationManager.Update(new GameTime(gameTime.TotalGameTime,
                    TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds * 1.5)));

                if (attackTimer >= attackDuration || animationManager.HasFinished())
                {
                    isAttacking = false;
                    AttackHitbox = Rectangle.Empty;
                }
            }
            else
            {
                AttackHitbox = Rectangle.Empty;

                // Agora, só bloqueia o pulo durante ataque, mas permite pular se estiver no chão OU elevador
                if (jumpPressed && canJump && !isAttacking)
                {
                    Velocity.Y = jumpStrength;
                    isOnGround = false;
                    isJumping = true;

                    animationManager.SetAnimation(jumpAnimation);
                    currentTexture = jumpTexture;
                }
            }

            UpdateHitbox();

            if (elevator != null)
            {
                Rectangle elevatorHitbox = elevator.Hitbox;
                Rectangle playerFeet = new Rectangle(Hitbox.X, Hitbox.Bottom, Hitbox.Width, 5);

                if (playerFeet.Intersects(elevatorHitbox) && Velocity.Y >= 0)
                {
                    float previousElevatorY = elevator.PreviousY;
                    float deltaY = elevator.Position.Y - previousElevatorY;

                    Position.Y += deltaY;
                    UpdateHitbox();

                    int correction = Hitbox.Bottom - elevatorHitbox.Top;
                    if (correction > 0)
                    {
                        Position.Y -= correction;
                    }

                    Velocity.Y = 0;
                    isOnGround = true;
                    isJumping = false;
                }
            }

            if (!onElevator)
            {
                Velocity.Y += gravity;
                Position.Y += Velocity.Y;
            }

            UpdateHitbox();

            foreach (var platform in platforms)
            {
                if (Hitbox.Intersects(platform.LeftWall))
                {
                    Position.X += platform.LeftWall.Right - Hitbox.Left;
                    UpdateHitbox();
                }
                else if (Hitbox.Intersects(platform.RightWall))
                {
                    Position.X -= Hitbox.Right - platform.RightWall.Left;
                    UpdateHitbox();
                }
            }

            isOnGround = false;
            foreach (int floorY in floorLevels)
            {
                if (Hitbox.Bottom >= floorY && Hitbox.Bottom <= floorY + 20 && Velocity.Y >= 0)
                {
                    Position.Y -= Hitbox.Bottom - floorY;
                    Velocity.Y = 0;
                    isOnGround = true;
                    isJumping = false;
                    UpdateHitbox();
                    break;
                }
            }

            if (!isAttacking)
            {
                if (isJumping)
                {
                    if (animationManager.HasFinished())
                    {
                        isJumping = false;
                        animationManager.SetAnimation(input != Vector2.Zero ? walkAnimation : idleAnimation);
                        currentTexture = input != Vector2.Zero ? walkTexture : idleTexture;
                    }
                }
                else
                {
                    animationManager.SetAnimation(input != Vector2.Zero ? walkAnimation : idleAnimation);
                    currentTexture = input != Vector2.Zero ? walkTexture : idleTexture;
                }
            }

            animationManager.Update(gameTime);
            UpdateHitbox();

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

        public void TakeDamage(Vector2 attackerPosition, float force)
        {
            if (isDamaged) return;

            health--;
            if (health < 0) health = 0;

            isDamaged = true;
            damageTimer = 0;

            Vector2 direction = Position - attackerPosition;
            if (direction.LengthSquared() < 0.01f)
            {
                direction = new Vector2(facingRight ? 1 : -1, 0);
            }
            else
            {
                direction.Normalize();
            }

            if (direction.Y > 0) direction.Y = 0;
            knockbackVelocity = direction * force;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects effects, Texture2D pixel)
        {
            Color drawColor = isDamaged && (int)(damageTimer / 100) % 2 == 1
                ? Color.Transparent
                : Color.White;

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

            spriteBatch.Draw(pixel, Hitbox, Color.Blue * 0.4f);

            if (AttackHitbox != Rectangle.Empty)
            {
                spriteBatch.Draw(pixel, AttackHitbox, Color.Red * 0.5f);
            }
        }

        public bool FacingRight => facingRight;
    }
}
