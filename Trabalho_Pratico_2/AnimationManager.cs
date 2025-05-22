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
        }

        // Propriedade pública para acessar o frame atual
        public int CurrentFrameIndex => currentFrame;

        public void SetAnimation(Animation newAnimation)
        {
            if (animation == newAnimation) return;

            animation = newAnimation;
            currentFrame = 0;
            timer = 0;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame >= animation.FrameCount)
                {
                    if (animation.IsLooping)
                        currentFrame = 0;
                    else
                        currentFrame = animation.FrameCount - 1;
                }
                timer = 0;
            }
        }

        public Rectangle GetFrame()
        {
            int width = (int)animation.FrameSize.X;
            int height = (int)animation.FrameSize.Y;

            int column = currentFrame % animation.FrameColumns;
            int row = currentFrame / animation.FrameColumns;

            return new Rectangle(column * width, row * height, width, height);
        }

        public bool HasFinished()
        {
            return !animation.IsLooping && currentFrame == animation.FrameCount - 1;
        }
    }
}
