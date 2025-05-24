using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trabalho_Pratico_2
{
    public class Elevator
    {
        public Vector2 Position;
        public Vector2 Size;
        public float Speed;
        public float UpperLimit;
        public float LowerLimit;
        private bool goingUp = true;
        public float PreviousY { get; private set; }

        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

        public Elevator(Vector2 position, Vector2 size, float speed, float upperLimit, float lowerLimit)
        {
            Position = position;
            Size = size;
            Speed = speed;
            UpperLimit = upperLimit;
            LowerLimit = lowerLimit;
        }

        public Rectangle BoundingBox => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y
        );


        public void Update(GameTime gameTime)
        {

            PreviousY = Position.Y;

            if (goingUp)
            {
                Position.Y -= Speed;
                if (Position.Y <= UpperLimit)
                    goingUp = false;
            }
            else
            {
                Position.Y += Speed;
                if (Position.Y >= LowerLimit)
                    goingUp = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Hitbox, Color.Gray);
        }
    }

}