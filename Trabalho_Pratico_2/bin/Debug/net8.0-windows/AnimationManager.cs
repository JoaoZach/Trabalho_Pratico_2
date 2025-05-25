using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Trabalho_Pratico_2
{
    public class AnimationManager
    {
        private Animation animation;
        private int currentFrame;
        private double timer;
        private double interval = 100; // ms per frame

        public AnimationManager(Animation animation)
        {
            this.animation = animation;
            currentFrame = 0;
            timer = 0;
        }

        // Propriedade pública para acessar o frame atual
        public int CurrentFrameIndex => currentFrame;

        public void SetAnimation(Animation newAnimation)
        {
            if (newAnimation == null)
                return;

            if (animation == newAnimation)
                return;

            animation = newAnimation;
            currentFrame = 0;
            timer = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (animation == null)
                return;

            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= animation.FrameCount)
                {
                    currentFrame = animation.IsLooping ? 0 : animation.FrameCount - 1;
                }
                timer = 0;
            }
        }

        public Rectangle GetFrame()
        {
            if (animation == null)
                return Rectangle.Empty;

            int width = (int)animation.FrameSize.X;
            int height = (int)animation.FrameSize.Y;

            int column = currentFrame % animation.FrameColumns;
            int row = currentFrame / animation.FrameColumns;

            return new Rectangle(column * width, row * height, width, height);
        }

        public bool HasFinished()
        {
            if (animation == null)
                return true;

            return !animation.IsLooping && currentFrame == animation.FrameCount - 1;
        }
    }
}
