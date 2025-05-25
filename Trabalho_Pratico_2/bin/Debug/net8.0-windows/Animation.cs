using Microsoft.Xna.Framework;

namespace Trabalho_Pratico_2
{
    public class Animation
    {
        public int FrameCount { get; }
        public int FrameColumns { get; }
        public Vector2 FrameSize { get; }
        public bool IsLooping { get; }

        public Animation(int numFrames, int numColumns, Vector2 frameSize, bool isLooping = true)
        {
            FrameCount = numFrames;
            FrameColumns = numColumns;
            FrameSize = frameSize;
            IsLooping = isLooping;
        }
    }
}
