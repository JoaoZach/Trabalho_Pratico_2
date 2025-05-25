using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Trabalho_Pratico_2
{
    public class Platform
    {
        public Rectangle Floor;
        public Rectangle LeftWall;
        public Rectangle RightWall;

        public Platform(int y, int width, int wallWidth, int wallHeight)
        {
            Floor = new Rectangle(0, y, width, 40);
            LeftWall = new Rectangle(0, y - wallHeight, wallWidth, wallHeight);
            RightWall = new Rectangle(width - wallWidth, y - wallHeight, wallWidth, wallHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, Floor, Color.DarkGreen * 0.6f);
            spriteBatch.Draw(pixel, LeftWall, Color.SaddleBrown * 0.6f);
            spriteBatch.Draw(pixel, RightWall, Color.SaddleBrown * 0.6f);
        }
    }
}
