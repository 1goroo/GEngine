using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GEngine.Core
{
    using Microsoft.Xna.Framework.Content;
    using System;
    using GEngine.Graphics;
    using GEngine.Input;
    using GEngine.Assets;
    using GEngine.Framework;

    public static class Config
    {
        private static int _pixelScreenWidth = 1280;
        private static int _pixelScreenHeight = 720;
        public static event Action OnScreenSizeChanged;
        public static int pixelScreenWidth
        {
            get => _pixelScreenWidth;
            set 
            { 
                if (_pixelScreenWidth == value) return;
                _pixelScreenWidth = value;
                OnScreenSizeChanged?.Invoke();
            }
        }
        public static int pixelScreenHeight
        {
            get => _pixelScreenHeight;
            set
            {
                if (_pixelScreenHeight == value) return;
                _pixelScreenHeight = value;
                OnScreenSizeChanged?.Invoke();
            }
        }

        public static Scene startScene;
        public static Vector2 ScreenDrawAnchore = new Vector2(0.5f,0.5f);
        public static Color BackgroundColor = Core.ConsoleBlack;
        public static Color LetterboxingColor = Color.Black;
        public static GameSettings Settings { get; internal set; }
        public static bool baseGamePreset = true;
        public static bool IsMouseVisible 
        {
            get => Core.Instance.game.IsMouseVisible;
            set => Core.Instance.game.IsMouseVisible = value;  
        }
        public static string GameName { get; internal set; } = "GEGame";
#if DEBUG
        public static bool DeveloperMode = true;
#else
        public static bool DeveloperMode = false;
#endif
        public static bool DebugMode = false;
    }
    public enum StorageSpace { Cache, Game }
    public interface IUpdatable { public void SystemUpdate(GameTime gameTime); }
    public interface IDrawable { public void Draw(SpriteBatch _spriteBatch); }
    public interface IAwakeable { public void Awake(); }
    public interface IStartable { public void Start(); }
    public interface IDestroyable { public void OnDestroy(); }
    public enum SearchScope { Local, Global, All }
    public class Core
    {
        public static Core Instance { get; private set; }
        public GEngineGame game;
        public static readonly Color ConsoleBlack = new (12, 12, 12);
        public static readonly Color ConsoleGray = new (214, 214, 214);
        // Objects
        public List<BaseObject> ObjectsList = new ();
        private List<BaseObject> AddList = new ();
        private List<BaseObject> RemoveList = new ();
        
        // Graphics module
        public DisplayManager displayManager;
        private SpriteBatch spriteBatch;
        private GameWindow window;

        // Scene
        public Scene currentScene { get; private set; }
        private Scene nextScene;

        // Public filds
        public static float currentFPS;
        //
        private GraphicsDevice graphicsDevice;
        internal ContentManager contentManager;
        /// <summary>
        /// Create & launches engine core
        /// </summary>
        /// <param name="Menaged Game"></param>
        public Core(GEngineGame currentGame)
        {
            // Set Game
            game = currentGame;
            // Set Fields
            spriteBatch = currentGame._spriteBatch;
            graphicsDevice = currentGame.GraphicsDevice;
            contentManager = currentGame.Content;
            window = currentGame.Window;
            // Create Fields
            displayManager = new DisplayManager(currentGame.GraphicsDevice);
            // Set Instance
            Instance = this;
            // Set Events
            window.TextInput += Input.InputChar;
            // Spawn Core objects
            if (Config.baseGamePreset)
            {
                ScreenBackground screenBackground = Spawn(new ScreenBackground());
                ImageWindow imageWindow = Spawn(new ImageWindow());
                GameConsole gameConsole = Spawn(new GameConsole());
                InformationConsole informationConsole = Spawn(new InformationConsole());
            }
            else
            {
                ScreenBackground screenBackground = new ScreenBackground();
                ImageWindow imageWindow = new ImageWindow();
                GameConsole gameConsole = new GameConsole();
                InformationConsole informationConsole = new InformationConsole();
            }
        }
        public void AddObject(BaseObject baseObject) => AddList.Add(baseObject); 
        public void RemoveObject(BaseObject baseObject) => RemoveList.Add(baseObject);
        public static T Spawn<T>(T baseObject) where T : BaseObject
        {
            baseObject.Awake();
            Instance.AddObject(baseObject);
            return baseObject;
        }
        public static void Destroy(BaseObject baseObject) => Instance.RemoveObject(baseObject); 
        public static T Find<T>(string name, SearchScope searchScope = SearchScope.All) where T : BaseObject
        {
            if (searchScope == SearchScope.All || searchScope == SearchScope.Local)
            {
                for (int i = 0; i < Instance.currentScene.AddList.Count; i++) 
                    if (Instance.currentScene.AddList[i].ObjectName == name) return (T)Instance.currentScene.AddList[i];
            }
            if (searchScope == SearchScope.All || searchScope == SearchScope.Global)
            {
                for (int i = 0;i < Instance.ObjectsList.Count;i++) 
                    if (Instance.ObjectsList[i].ObjectName == name) return (T)Instance.ObjectsList[i];
            }
            if (searchScope == SearchScope.All || searchScope == SearchScope.Global)
            {
                for (int i = 0; i < Instance.AddList.Count; i++) 
                    if (Instance.AddList[i].ObjectName == name) return (T)Instance.AddList[i];
            }
            if (searchScope == SearchScope.All || searchScope == SearchScope.Local)
            {
                for (int i = 0; i < Instance.currentScene.ObjectsList.Count; i++)
                    if (Instance.currentScene.ObjectsList[i].ObjectName == name) return (T)Instance.currentScene.ObjectsList[i];
            }
            return null;
        }
        public void Update(GameTime gameTime)
        {
            // Update Input
            Input.Update();
            if (Input.GetKeyDown(Keys.F3) && Config.DeveloperMode) Config.DebugMode = !Config.DebugMode;
            // Add
            if (AddList.Count > 0)
            {
                for (int i = 0; i < AddList.Count; i++)
                {
                    AddList[i].Start();
                    ObjectsList.Add(AddList[i]);
                }
                
                AddList.Clear();
                ObjectListSort();
            }
            // Update
            for (int i = 0; i < ObjectsList.Count; i++) { ObjectsList[i].SystemUpdate(gameTime);}
            
            currentScene?.Update(gameTime);

            // Remove
            if (RemoveList.Count > 0)
            {
                for (int i = 0; i < RemoveList.Count; i++) { RemoveList[i].OnDestroy(); }
                var toRemove = new HashSet<BaseObject>(RemoveList);
                ObjectsList.RemoveAll(item => toRemove.Contains(item));
                RemoveList.Clear();
            }
            ChangeScene();
        }
        public void Draw(GameTime gameTime)
        {
            //
            displayManager.Start();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            //
            for (int i = 0; i < ObjectsList.Count; i++) { ObjectsList[i].Draw(spriteBatch); }
            currentScene?.Draw(spriteBatch);
            //
            if (Config.DebugMode) Debug.Draw(spriteBatch,graphicsDevice);
            //
            spriteBatch.End();
            displayManager.End();
            //
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointWrap,null,null,null,null);
            displayManager.Draw(spriteBatch);
            spriteBatch.End();
            currentFPS = GetFPS(gameTime);
        }

        public void ObjectListSort() => ObjectsList.Sort((a,b) => a.Layer.CompareTo(b.Layer));

        public static void SwitchScene<T>() where T : Scene, new() => Instance.nextScene = new T();
        internal static void SetFirstScene(Scene scene) => Instance.nextScene = scene;
        private void ChangeScene()
        {
            if (nextScene != null)
            {
                Instance.currentScene?.OnExit();
                AssetsManager.CleanCache();
                Instance.currentScene = nextScene;
                Instance.currentScene.OnLoad();
                Instance.currentScene.IsLoad = true;
                Instance.currentScene.OnEnter();
                nextScene = null;
            }
        }
        public static void Exit() => Instance.game.Exit();

        private float GetFPS(GameTime gameTime) => 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

        public class ScreenBackground : BaseObject
        {
            Transform transform = new (Transform.GetAnchore(Transform.Anchore.Right));
            float coef = Utilities.GetPercentByScreen(280,true);
            float co = Utilities.GetPercentByScreen(150, false);
            float c = Utilities.GetPercentByScreen(18, true);
            public ScreenBackground(string name = null) : base(name) { }
            public override void Draw(SpriteBatch spriteBatch)
            {
                var spf = AssetsManager.FontsManager.baseEngineFont.GetFont(c*Config.pixelScreenWidth);
                int deltaY = 0;
                transform.position = Transform.GetAnchore(Transform.Anchore.Right);
                Vector2 pos = transform.GetBasePosition(Vector2.Zero);
                for (int i = 0; i < 40; i++) { spriteBatch.DrawString(spf, "|", new Vector2(pos.X - Config.pixelScreenWidth * coef, deltaY), ConsoleGray); deltaY += spf.LineHeight; }
                spriteBatch.DrawString(spf, "H------------------------------", new Vector2(pos.X - Config.pixelScreenWidth * coef, pos.Y - co * Config.pixelScreenHeight), ConsoleGray);
            }
        }
        public struct ConsoleLog(SpriteFontBase spriteFontBase, string text, Vector2 vector, Color color)
        {
            public SpriteFontBase spriteFontBase = spriteFontBase;
            public string text = text;
            public Vector2 vector = vector;
            public Color color = color;
            public void WriteLine(SpriteBatch spriteBatch, float delta)
                => spriteFontBase.DrawText(spriteBatch, text, new Vector2(vector.X, vector.Y + delta), color);
        }
        public class GameConsole : BaseObject
        {
            public static GameConsole Instance { get; private set; }
            float deltaY = 0;
            float scrollDelta = 0;
            float scrollSensivity = 10;
            List<ConsoleLog> logs = new ();
            Transform position = new (Transform.GetAnchore(Transform.Anchore.TopLeft));
            bool reading = false;
            string readingBuffer;
            public string lastRead { get; private set; }
            public GameConsole(string name = null) : base(name) { Instance = this; }
            public override void Update()
            {
                if (Input.GetNormalizedWheelScroll() == 1) scrollDelta += scrollSensivity;
                else if (Input.GetNormalizedWheelScroll() == -1) scrollDelta -= scrollSensivity;
                if (reading)
                {
                    while (Input.HasChar())
                    {
                        var c = Input.GetNextChar();
                        if (c == '\b') 
                        {
                            if (readingBuffer.Length > 0)
                                readingBuffer = readingBuffer.Remove(readingBuffer.Length - 1);
                        }
                        else if (c >= ' ')
                            readingBuffer += c;
                    }

                    var log = logs[logs.Count - 1];
                    log.text = readingBuffer;
                    logs[logs.Count - 1] = log;
                    if (Input.GetKeyDown(Keys.Enter)) { reading = false; lastRead = readingBuffer; readingBuffer = ""; return; }
                    return;
                }
            }
            public override void Draw(SpriteBatch _spriteBatch)
            {
                for (int i = 0; i < logs.Count; i++) { logs[i].WriteLine(_spriteBatch, scrollDelta); }
            }
            public void WriteLine(SpriteFontBase spf, string text, Color color)
            {
                if (reading) return;
                Vector2 LogPunkt = position.GetBasePosition(Vector2.Zero);
                LogPunkt = new Vector2(LogPunkt.X, LogPunkt.Y + deltaY);
                logs.Add(new ConsoleLog(spf, text, LogPunkt, color));
                deltaY += spf.LineHeight;
            }
            public void ReadLine(SpriteFontBase spf, Color color)
            {
                if (reading) return;
                WriteLine(spf, "", color);
                reading = true;
                if (Input.HasChar()) readingBuffer += Input.GetNextChar();
            }
            public ConsoleLog[] Clear()
            {
                ConsoleLog[] i = logs.ToArray();
                logs.Clear();
                deltaY = 0;
                scrollDelta = 0;
                return i;
            }
            void ReAnchore() => position = new(Transform.GetAnchore(Transform.Anchore.TopLeft));
            public override void Awake() => Config.OnScreenSizeChanged += ReAnchore;
            public override void OnDestroy() => Config.OnScreenSizeChanged -= ReAnchore;
        }
        public class ImageWindow : BaseObject 
        {
            public static ImageWindow Instance { get; private set; }
            public ASCIIImage? currentImage { get; private set; }
            private float ignoreScale = 1;
            private Vector2 ignoreVector = Vector2.Zero;
            float cx = Utilities.GetPercentByScreen(120, true);
            float cy = Utilities.GetPercentByScreen(220, false);
            public ImageWindow(string name = null) : base(name)
            {
                Instance = this;
                currentImage = null;
            }
            public override void Draw(SpriteBatch spriteBatch)
            {
                if (currentImage != null)
                {
                    ASCIIImage image = (ASCIIImage)currentImage;
                    Vector2 anc = Transform.GetAnchore(Transform.Anchore.TopRight);
                    Transform currTr = new Transform(new Vector2(anc.X + ignoreVector.X - cx * Config.pixelScreenWidth,
                        anc.Y - cy*Config.pixelScreenHeight +ignoreVector.Y));
                    float limit = 280f;
                    float s = Math.Min(limit / image.width, limit / image.height);
                    currTr.scale = new Vector2(s,s) * ignoreScale;
                    
                    currentImage?.Draw(spriteBatch, currTr);
                }
            }
            public void SetImage(ASCIIImage newImage) 
            {
                currentImage = newImage;
                ignoreScale = 1;
                ignoreVector = Vector2.Zero;
            }
            public void SetImage(ASCIIImage newImage, float ignoreScale, Vector2 ignoreVector)
            {
                currentImage = newImage;
                this.ignoreScale = ignoreScale;
                this.ignoreVector = ignoreVector;
            }
        }
        public class InformationConsole : BaseObject
        {
            public static InformationConsole Instance { get; private set; }
            float deltaY = 0;
            List<ConsoleLog> logs = new List<ConsoleLog>();
            Transform position = new Transform(Transform.GetAnchore(Transform.Anchore.Right));
            float c = Utilities.GetPercentByScreen(270,true);
            float cy = Utilities.GetPercentByScreen(100,false);
            public InformationConsole(string name = null) : base(name) { Instance = this; }
            public override void Draw(SpriteBatch _spriteBatch)
            {
                for (int i = 0; i < logs.Count; i++) { logs[i].WriteLine(_spriteBatch, 0); }
            }
            public void WriteLine(SpriteFontBase spf, string text, Color color)
            {
                Vector2 LogPunkt = position.GetBasePosition(Vector2.Zero);
                LogPunkt = new Vector2(LogPunkt.X - c * Config.pixelScreenWidth, LogPunkt.Y - cy*Config.pixelScreenHeight + deltaY);
                logs.Add(new ConsoleLog(spf, text, LogPunkt, color));
                deltaY += spf.LineHeight;
            }
            public ConsoleLog[] Clear()
            {
                ConsoleLog[] i = logs.ToArray();
                logs.Clear();
                deltaY = 0;
                return i;
            }
            void ReAnchore()
            {
                position = new(Transform.GetAnchore(Transform.Anchore.Right));
                foreach (var log in Clear())
                    WriteLine(log.spriteFontBase,log.text,log.color);
            }
            public override void Awake() => Config.OnScreenSizeChanged += ReAnchore;
            public override void OnDestroy() => Config.OnScreenSizeChanged -= ReAnchore;
        }
    }
    public static class Debug
    {
        public static Vector2 FPSCounterPosition = new (1220, 5);
        public static Vector2 MousePositionHandler = new (1235, 20);
        private static SpriteFontBase font14 = AssetsManager.FontsManager.baseEngineFont.GetFont(14);
        private static SpriteFontBase font13 = AssetsManager.FontsManager.baseEngineFont.GetFont(13);
        public static void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            void DrawCoordinateLine()
            {
                var graphicsDevice = Core.Instance.displayManager.graphicsDevice;
                List<VertexPositionColor> vertexPositionX = new List<VertexPositionColor>();

                List<VertexPositionColor> vertexPositionY = new List<VertexPositionColor>();

                vertexPositionX.Add(new VertexPositionColor(new Vector3(0,Config.pixelScreenHeight * 0.5f,0),Color.Red));
                vertexPositionX.Add(new VertexPositionColor(new Vector3(Config.pixelScreenWidth, Config.pixelScreenHeight * 0.5f, 0), Color.Red));

                vertexPositionY.Add(new VertexPositionColor(new Vector3(Config.pixelScreenWidth * 0.5f, 0, 0), Color.Green));
                vertexPositionY.Add(new VertexPositionColor(new Vector3(Config.pixelScreenWidth * 0.5f, Config.pixelScreenHeight, 0), Color.Green));

                BasicEffect basicEffect = new BasicEffect(graphicsDevice);
                basicEffect.VertexColorEnabled = true;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
                foreach(var pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertexPositionX.ToArray(), 0, 1);
                    graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertexPositionY.ToArray(), 0, 1);
                }
            }
            void DrawFPSCounter() => spriteBatch.DrawString(font14, $"FPS{Core.currentFPS.ToString("F2")}",FPSCounterPosition,Color.OrangeRed);
            void DrawMousePosition()
            {
                MouseState mouseState = Mouse.GetState();
                Vector2 newMouse = Transform.ScreenToGamePosition(new Vector2(mouseState.Position.X,mouseState.Position.Y));
                spriteBatch.DrawString(font13, $"X{newMouse.X.ToString("F0")}\nY{newMouse.Y.ToString("F0")}",
                    MousePositionHandler, Color.OrangeRed);
            }
            DrawCoordinateLine();
            DrawFPSCounter();
            DrawMousePosition();
        }
    }
}
