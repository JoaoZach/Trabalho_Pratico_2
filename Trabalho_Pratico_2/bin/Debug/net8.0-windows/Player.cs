using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private float jumpStrength = -18f;
        private bool isOnGround = false;
        private bool isJumping = false;
        private bool facingRight = true;
        private int maxHealth = 4;

        private Texture2D idleTexture;
        private Texture2D walkTexture;
        private Texture2D jumpTexture;
        private Texture2D attackTexture;
        private Texture2D dodgeTexture; // Nova textura para esquiva
        private Texture2D currentTexture;

        private AnimationManager animationManager;
        private Animation walkAnimation;
        private Animation idleAnimation;
        private Animation jumpAnimation;
        private Animation attackAnimation;
        private Animation dodgeAnimation; // Nova animação para esquiva

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

        // Variáveis para esquiva
        private bool isDodging = false;
        private double dodgeTimer = 0;
        private double dodgeDuration = 400; // Duração da esquiva em ms
        private float dodgeSpeed = 15f; // Velocidade da esquiva
        private Vector2 dodgeDirection = Vector2.Zero;
        private float dodgeDistance = 200f; // Distância da esquiva
        private Vector2 dodgeStartPosition;
        private double dodgeCooldownTimer = 0;
        private double dodgeCooldown = 1000; // Cooldown de 1 segundo

        public int Health => health;
        public int MaxHealth => maxHealth;
        public bool IsDead => health <= 0;
        public bool IsInvulnerable => isDodging; // Propriedade para verificar invulnerabilidade

        private SoundEffect attackSound;
        private SoundEffect jumpSound;
        private SoundEffect hurtSound;
        private SoundEffect dodgeSound; // Som da esquiva

        public Player(Texture2D idle, Texture2D walk, Texture2D jump, Texture2D attack, Texture2D dodge,
                      Animation idleAnim, Animation walkAnim, Animation jumpAnim, Animation attackAnim, Animation dodgeAnim,
                      int groundY, Vector2 startPosition, SoundEffect attackSfx, SoundEffect jumpSfx, SoundEffect hurtSfx, SoundEffect dodgeSfx)
        {
            idleTexture = idle;
            walkTexture = walk;
            jumpTexture = jump;
            attackTexture = attack;
            dodgeTexture = dodge;

            idleAnimation = idleAnim;
            walkAnimation = walkAnim;
            jumpAnimation = jumpAnim;
            attackAnimation = attackAnim;
            dodgeAnimation = dodgeAnim;

            animationManager = new AnimationManager(idleAnimation);
            currentTexture = idleTexture;

            Position = startPosition;
            health = maxHealth;
            attackSound = attackSfx;
            jumpSound = jumpSfx;
            hurtSound = hurtSfx;
            dodgeSound = dodgeSfx;
        }

        private void UpdateHitbox()
        {
            int spriteW = 300;
            int spriteH = 300;
            int hitboxW = 120;
            int hitboxH = 140;
            int centerX = (int)Position.X + spriteW / 2;
            int centerY = (int)Position.Y + spriteH / 2;
            Hitbox = new Rectangle(centerX - hitboxW / 2, centerY - hitboxH / 2, hitboxW, hitboxH);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Elevator elevator, List<int> floorLevels, List<Platform> platforms)
        {
            Vector2 input = Vector2.Zero;

            // Atualiza cooldown da esquiva
            if (dodgeCooldownTimer > 0)
            {
                dodgeCooldownTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Input de movimento (apenas se não estiver esquivando)
            if (!isDodging)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                    input.X -= 1;
                if (keyboardState.IsKeyDown(Keys.Right))
                    input.X += 1;

                if (input != Vector2.Zero)
                {
                    Position += Vector2.Normalize(input) * speed;
                    facingRight = input.X > 0;
                }
            }

            bool jumpPressed = keyboardState.IsKeyDown(Keys.Up);

            // Esquiva (tecla X)
            if (!isDodging && !isAttacking && keyboardState.IsKeyDown(Keys.X) && dodgeCooldownTimer <= 0)
            {
                StartDodge();
            }

            // Ataque
            if (!isAttacking && !isDodging && keyboardState.IsKeyDown(Keys.Z))
            {
                isAttacking = true;
                attackTimer = 0;
                animationManager.SetAnimation(attackAnimation);
                currentTexture = attackTexture;
                attackSound?.Play();
            }

            bool onElevator = false;
            if (elevator != null)
            {
                Rectangle elevatorHitbox = elevator.Hitbox;
                Rectangle playerFeet = new Rectangle(Hitbox.X, Hitbox.Bottom, Hitbox.Width, 5);
                onElevator = playerFeet.Intersects(elevatorHitbox) && Velocity.Y >= 0;
            }

            bool canJump = isOnGround || onElevator;

            // Lógica da esquiva
            if (isDodging)
            {
                dodgeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                // Movimento da esquiva
                float progress = (float)(dodgeTimer / dodgeDuration);
                if (progress <= 1f)
                {
                    Vector2 targetPosition = dodgeStartPosition + dodgeDirection * dodgeDistance;
                    Position = Vector2.Lerp(dodgeStartPosition, targetPosition, progress);
                }

                animationManager.Update(new GameTime(gameTime.TotalGameTime,
                    TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds * 2.0)));

                if (dodgeTimer >= dodgeDuration || animationManager.HasFinished())
                {
                    isDodging = false;
                    dodgeCooldownTimer = dodgeCooldown;
                }
            }
            // Lógica do ataque
            else if (isAttacking)
            {
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

                if (jumpPressed && canJump)
                {
                    Velocity.Y = jumpStrength;
                    isOnGround = false;
                    isJumping = true;
                    animationManager.SetAnimation(jumpAnimation);
                    currentTexture = jumpTexture;
                    jumpSound?.Play();
                }
            }

            UpdateHitbox();

            // Resto da lógica de física (apenas se não estiver esquivando)
            if (!isDodging)
            {
                // Lógica do elevador
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

                // Gravidade e colisão com chão
                if (!onElevator)
                {
                    Velocity.Y += gravity;
                    float remainingMovement = Velocity.Y;

                    while (remainingMovement != 0)
                    {
                        float step = Math.Sign(remainingMovement) * Math.Min(2f, Math.Abs(remainingMovement));

                        Position.Y += step;
                        UpdateHitbox();

                        bool collidedWithFloor = false;
                        foreach (int floorY in floorLevels)
                        {
                            if (Hitbox.Bottom >= floorY && Hitbox.Bottom <= floorY + 20 && Velocity.Y >= 0)
                            {
                                Position.Y -= Hitbox.Bottom - floorY;
                                Velocity.Y = 0;
                                isOnGround = true;
                                isJumping = false;
                                UpdateHitbox();
                                collidedWithFloor = true;
                                break;
                            }
                        }

                        if (collidedWithFloor)
                            break;

                        remainingMovement -= step;
                    }
                }

                UpdateHitbox();

                // Colisão com plataformas
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

                // Verificação final do chão
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
            }

            // Animações (apenas se não estiver esquivando)
            if (!isAttacking && !isDodging)
            {
                if (isJumping)
                {
                    animationManager.Update(new GameTime(gameTime.TotalGameTime,
                   TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds * 1.5)));

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

            // Knockback (apenas se não estiver esquivando)
            if (!isDodging)
            {
                Position += knockbackVelocity;
                knockbackVelocity *= knockbackFriction;
                if (knockbackVelocity.Length() < 0.1f)
                    knockbackVelocity = Vector2.Zero;
            }
        }

        private void StartDodge()
        {
            isDodging = true;
            dodgeTimer = 0;
            dodgeStartPosition = Position;

            // Direção da esquiva baseada na direção que o jogador está olhando
            dodgeDirection = facingRight ? Vector2.UnitX : -Vector2.UnitX;

            // Configura animação da esquiva
            animationManager.SetAnimation(dodgeAnimation);
            currentTexture = dodgeTexture;

            // Toca som da esquiva
            dodgeSound?.Play();
        }

        public void TakeDamage(Vector2 attackerPosition, float force)
        {
            // Se estiver esquivando, é invulnerável
            if (isDamaged || isDodging) return;

            health--;
            if (health < 0) health = 0;

            isDamaged = true;
            damageTimer = 0;

            hurtSound?.Play();

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

        public void Draw(SpriteBatch spriteBatch, SpriteEffects effects, Texture2D pixel, Texture2D playerDodgeTexture)
        {
            Color drawColor = isDamaged && (int)(damageTimer / 100) % 2 == 1
                ? Color.Transparent
                : Color.White;

            Rectangle currentFrame = animationManager.GetFrame();
            Vector2 origin = new Vector2(currentFrame.Width / 2f, currentFrame.Height / 2f);
            Vector2 hitboxCenter = new Vector2(Hitbox.X + Hitbox.Width / 2, Hitbox.Y + Hitbox.Height / 2);

            if (isDodging)
            {
                Vector2 dodgeOrigin = new Vector2(playerDodgeTexture.Width / 2f, playerDodgeTexture.Height / 2f);

                spriteBatch.Draw(
                    playerDodgeTexture,
                    hitboxCenter,
                    currentFrame,
                    Color.White * 0.7f,
                    0f,
                    dodgeOrigin,
                    1f,
                    effects,
                    0f
                );
            }
            else
            {
                spriteBatch.Draw(
                    currentTexture,
                    hitboxCenter,
                    currentFrame,
                    drawColor,
                    0f,
                    origin,
                    1f,
                    effects,
                    0f
                );
            }

            // Desenha hitbox visual para debug
           /* Color hitboxColor = isDodging ? Color.Green * 0.4f : Color.Blue * 0.4f;
            spriteBatch.Draw(pixel, Hitbox, hitboxColor);*/

            /*if (AttackHitbox != Rectangle.Empty)
            {
                spriteBatch.Draw(pixel, AttackHitbox, Color.Red * 0.5f);
            }*/
        }

        public bool FacingRight => facingRight;
    }
}
