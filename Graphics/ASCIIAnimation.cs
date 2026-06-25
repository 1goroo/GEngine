using GEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace GEngine.Graphics
{
    public class ASCIIAnimation
    {
        public ASCIIImage[] frames;
        public float delay = 1;
        public float height;
        public float width;
        private float lastDelay = 0;
        private int frameIndex = 0;
        public bool InPause { get; private set; }
        public ASCIIAnimation(ASCIIImage[] frames, float delay = 1)
        {
            this.frames = frames;
            this.delay = delay;
            height = frames[0].text.Length * frames[0].spf.LineHeight;
            width = 0;
            for (int i = 0; i < frames[0].text.Length; i++)
            {
                float maxWidth = frames[0].spf.MeasureString(frames[0].text[i]).X;
                if (width < maxWidth) width = maxWidth;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position) => frames[frameIndex].Draw(spriteBatch, position);
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale) => frames[frameIndex].Draw(spriteBatch, position, scale);
        public void Draw(SpriteBatch spriteBatch, Transform transform) => frames[frameIndex].Draw(spriteBatch, transform);
        public void Update()
        {
            if (InPause) return;
            lastDelay += (float)EngineTime.DeltaTime.TotalSeconds;
            if (lastDelay >= delay)
            {
                lastDelay = 0;
                frameIndex++;
                if (frameIndex >= frames.Length) frameIndex = 0;
            }
        }
        public void Pause() => InPause = true;
        public void Resume() => InPause = false;
    }
}
