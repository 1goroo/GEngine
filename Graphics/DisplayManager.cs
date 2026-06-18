using GEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace GEngine.Graphics
{
    public class DisplayManager
    {
        RenderTarget2D renderTarget;
        public GraphicsDevice graphicsDevice;
        Color clearColor = Config.BackgroundColor;
        int targetWidth;
        int targetHeight;
        // Draw
        float coefWidth;
        float coefHeight;
        internal float minCoef { get; private set; }
        Vector2 center;
        internal Vector2 BackBufferSize { get; private set; }
        public DisplayManager(GraphicsDevice GraphicsDevice, GameWindow Window)
        {
            graphicsDevice = GraphicsDevice;

            RecreateRenderTarget();
            ChangeBufferSize();

            Config.OnScreenSizeChanged += RecreateRenderTarget;
            Window.ClientSizeChanged += OnWindowSizeChange;
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
        public void Draw(SpriteBatch _spriteBatch) =>
            _spriteBatch.Draw(renderTarget, new Rectangle((int)center.X, (int)center.Y,
                (int)(targetWidth * minCoef), (int)(targetHeight * minCoef)), Color.White);

        private void OnWindowSizeChange(object sender, EventArgs e) { RecreateRenderTarget(); ChangeBufferSize(); }
        private void RecreateRenderTarget()
        {
            targetWidth = Config.pixelScreenWidth;
            targetHeight = Config.pixelScreenHeight;
            renderTarget?.Dispose();
            renderTarget = new RenderTarget2D(graphicsDevice, targetWidth, targetHeight);
        }
        private void ChangeBufferSize()
        {
            BackBufferSize = new(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
            coefWidth = BackBufferSize.X / targetWidth;
            coefHeight = BackBufferSize.Y / targetHeight;
            minCoef = Math.Min(coefWidth, coefHeight);

            center = new((BackBufferSize.X - targetWidth * minCoef) * Config.ScreenDrawAnchor.X,
                (BackBufferSize.Y - targetHeight * minCoef) * Config.ScreenDrawAnchor.Y);
        }
    }
}
