using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GEngine.Framework;
namespace GEngine.Core
{
    public class GEngineGame : Game
    {
        private GraphicsDeviceManager _graphics;
        internal SpriteBatch _spriteBatch;
        Core generalCore;
        Scene startScene;
        public GEngineGame(Scene StartScene, GameSettings gameSettings = null)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            this.startScene = StartScene;
            Config.Settings = gameSettings ?? new ();
        }
        protected override void Initialize() => base.Initialize();
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            generalCore = new Core(this);
            Core.SetFirstScene(startScene);
        }
        protected override void Update(GameTime gameTime)
        {
            generalCore.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);
            generalCore.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
