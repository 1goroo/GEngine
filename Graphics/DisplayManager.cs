using GEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GEngine.Graphics
{
    public class DisplayManager
    {
        RenderTarget2D renderTarget;
        public GraphicsDevice graphicsDevice;
        Color clearColor = Config.BackgroundColor;
        int targetWidth;
        int targetHeight;
        public DisplayManager(GraphicsDevice GraphicsDevice)
        {
            graphicsDevice = GraphicsDevice;
            RecreateRenderTarget();
            Config.OnScreenSizeChanged += RecreateRenderTarget;
        }
        public void Start()
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(clearColor);
        }
        public void End()
        {
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.Clear(Config.LetterboxingColor);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            Vector2 BackBufferSize = new(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
            float coefWidth = BackBufferSize.X / targetWidth;
            float coefHeight = BackBufferSize.Y / targetHeight;
            float minCoef = coefWidth <= coefHeight ? coefWidth : coefHeight;

            Vector2 center = new((BackBufferSize.X - targetWidth * minCoef) * Config.ScreenDrawAnchore.X,
                (BackBufferSize.Y - targetHeight * minCoef) * Config.ScreenDrawAnchore.Y);

            _spriteBatch.Draw(renderTarget, new Rectangle((int)center.X, (int)center.Y, (int)(targetWidth * minCoef), (int)(targetHeight * minCoef)), Color.White);
        }
        private void RecreateRenderTarget()
        {
            targetWidth = Config.pixelScreenWidth;
            targetHeight = Config.pixelScreenHeight;
            renderTarget = new RenderTarget2D(graphicsDevice, targetWidth, targetHeight);
        }
    }
}
