using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Trabalho_Pratico_2;

public class SlimeBoss
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
    private const int spriteSize = 500;

    private int groundLevel;
    private SpriteEffects spriteEffect = SpriteEffects.None;

    private int direction = 1;
    private float moveSpeed = 6f;

    private int health = 5;
    private Vector2 knockbackVelocity = Vector2.Zero;
    private float knockbackFriction = 0.55f;

    private bool isDamaged = false;
    private double damageTimer = 0;
    private double damageDuration = 500;
    private Color baseColor = Color.White;

    private List<Platform> platforms;
    private SoundEffect deathSound;
    private SoundEffect hurtSound;

    // Pulo automático
    private bool isJumping = false;
    private float jumpVelocity = -20f;
    private double jumpCooldown = 2000;
    private double jumpTimer = 0;

    public SlimeBoss(Texture2D texture, Vector2 position, Animation animation, int groundLevel, List<Platform> platforms, SoundEffect deathSound, SoundEffect hurtSound)
    {
        Texture = texture;
        Position = position;
        this.animation = animation;
        this.groundLevel = groundLevel;
        this.platforms = platforms;
        animationManager = new AnimationManager(animation);
        UpdateHitbox();
        this.deathSound = deathSound;
        this.hurtSound = hurtSound;
    }

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        if (!IsAlive) return;

        double elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

        // Dano
        if (isDamaged)
        {
            damageTimer += elapsed;
            if (damageTimer >= damageDuration)
            {
                isDamaged = false;
                damageTimer = 0;
            }
        }

        // Movimento horizontal
        Position.X += direction * moveSpeed;

        // Limites de patrulha
        int leftLimit = 100;
        int rightLimit = 1800;

        if (Position.X < leftLimit)
        {
            Position.X = leftLimit;
            direction = 1;
        }
        else if (Position.X > rightLimit)
        {
            Position.X = rightLimit;
            direction = -1;
        }

        spriteEffect = direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        // Pulo automático
        jumpTimer += elapsed;
        if (!isJumping && jumpTimer >= jumpCooldown)
        {
            isJumping = true;
            verticalVelocity = jumpVelocity;
            jumpTimer = 0;
        }

        // Gravidade e pulo
        verticalVelocity += gravity;
        Position.Y += verticalVelocity;

        if (Position.Y + spriteSize - verticalOffset >= groundLevel)
        {
            Position.Y = groundLevel - spriteSize + verticalOffset;
            verticalVelocity = 0f;
            isJumping = false;
        }

        // Knockback
        Position += knockbackVelocity;
        knockbackVelocity *= knockbackFriction;

        if (knockbackVelocity.Length() < 0.1f)
            knockbackVelocity = Vector2.Zero;

        animationManager.Update(gameTime);
        UpdateHitbox();
    }

    private void UpdateHitbox()
    {
        int width = 280;
        int height = 180;
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

        hurtSound?.Play();

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
            new Vector2(Position.X + spriteSize / 2, Position.Y + spriteSize / 3),
            animationManager.GetFrame(),
            drawColor,
            0f,
            new Vector2(spriteSize / 2, spriteSize / 2),
            1f,
            spriteEffect,
            0f
        );

        // Hitbox (visual)
        //spriteBatch.Draw(pixel, Hitbox, Color.OrangeRed * 0.4f);
    }
}
