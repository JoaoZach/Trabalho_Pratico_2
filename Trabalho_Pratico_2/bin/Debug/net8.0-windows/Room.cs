using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Trabalho_Pratico_2
{
    public class Room
    {
        public Rectangle Bounds { get; set; }
        public Texture2D Background { get; set; }
        public Vector2 EntryPoint { get; set; }
        public Texture2D Pixel { get; set; }

        public List<Platform> Platforms { get; set; } = new List<Platform>();
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public Vector2 StairPosition { get; set; }

        public Room(Rectangle bounds, Texture2D background, Vector2 entryPoint, Texture2D pixel)
        {
            Bounds = bounds;
            Background = background;
            EntryPoint = entryPoint;
            Pixel = pixel;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (var platform in Platforms)
                platform.Draw(spriteBatch, pixel);

            foreach (var enemy in Enemies)
                enemy.Draw(spriteBatch, pixel);
        }
    }
}
