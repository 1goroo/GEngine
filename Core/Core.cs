using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace GEngine.Core
{
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;
    using System;
    using System.Linq;
    using GEngine.Graphics;
    using GEngine.Input;

    public static class Config
    {
        public static int pixelScreenWidth = 1280;
        public static int pixelScreenHeight = 720;
        public static Scene startScene;
        public static Vector2 ScreenDrawAnchore = new Vector2(0.5f,0.5f);
#if DEBUG
        public static bool DeveloperMode = true;
#else
        public static bool DeveloperMode = false;
#endif
        public static bool DebugMode = false;
    }
    public interface IUpdatable { public void SystemUpdate(GameTime gameTime); }
    public interface IDrawable { public void Draw(SpriteBatch _spriteBatch); }
    public interface IAwakeable { public void Awake(); }
    public interface IStartable { public void Start(); }
    public interface IDestroyable { public void OnDestroy(); }
    public abstract class BaseObject: IUpdatable, IDrawable, IStartable, IDestroyable, IAwakeable
    {
        public string ObjectName;
        public string Tag = "Default";
        public int Layer { get; private set; } = 0;
        public bool DontDestroyOnLoad = false;
        private List<(Action action, float delay)> invokes = new();
        protected BaseObject(string name = null, string tag = null)
        {
            ObjectName = name ?? GetType().Name;
            Tag = tag ?? "Default";
        }
        public void SystemUpdate(GameTime gameTime)
        {
            Update();
            InvoksUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        private void InvoksUpdate(float deltaTime)
        {
            for (int i = invokes.Count - 1; i >= 0; i--)
            {
                var inv = invokes[i];
                inv.delay -= deltaTime;
                invokes[i] = inv;
                if (inv.delay <= 0)
                {
                    inv.action.Invoke();
                    invokes.RemoveAt(i);
                }
            }
        }
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch _spriteBatch) { }
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void OnDestroy() { }
        protected void Destroy(BaseObject baseObject) => Core.Instance.currentScene.Destroy(baseObject);
        protected void DestroyFromGlobal(BaseObject baseObject) => Core.Destroy(baseObject);
        public void SetLayer(int newLayer)
        {
            if (Layer == newLayer) return;
            Layer = newLayer;
            if (DontDestroyOnLoad) Core.Instance.ObjectListSort();
            else Core.Instance.currentScene.ObjectListSort();
        }
        public void Invoke(Action action, float time) => invokes.Add((action, time));
    }
    public abstract class Scene
    {
        public List<BaseObject> ObjectsList = new List<BaseObject>();
        public List<BaseObject> AddList = new List<BaseObject>();
        private List<BaseObject> RemoveList = new List<BaseObject>();

        public T Spawn<T>(T baseObject) where T : BaseObject
        {
            baseObject.Awake();
            AddObject(baseObject);
            return baseObject;
        }
        public void AddObject(BaseObject baseObject) => AddList.Add(baseObject);

        public T SpawnToGlobal<T>(T baseObject) where T : BaseObject => Core.Spawn(baseObject);

        public void Destroy(BaseObject baseObject) => RemoveObject(baseObject);
        public void RemoveObject(BaseObject baseObject) => RemoveList.Add(baseObject);

        public static T Find<T>(string name, SearchScope searchScope = SearchScope.All) where T : BaseObject => Core.Find<T>(name, searchScope);

        public void Update(GameTime gameTime)
        {
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
            for (int i = 0; i < ObjectsList.Count; i++) { ObjectsList[i].SystemUpdate(gameTime); }
            
            OnUpdate();

            // Remove
            if (RemoveList.Count > 0)
            {
                for (int i = 0; i < RemoveList.Count; i++) { RemoveList[i].OnDestroy(); }
                var toRemove = new HashSet<BaseObject>(RemoveList);
                ObjectsList.RemoveAll(item => toRemove.Contains(item));
                RemoveList.Clear();
            }
        }
        public virtual void OnUpdate() { }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw
            for (int i = 0; i < ObjectsList.Count; i++) { ObjectsList[i].Draw(spriteBatch); }
            OnDraw();
        }
        public virtual void OnDraw() { }
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public void ObjectListSort() => ObjectsList = ObjectsList.OrderBy(o => o.Layer).ToList();
    }
    public enum SearchScope { Local, Global, All }
    public class Core
    {
        public static Core Instance { get; private set; }
        public static readonly Color ConsoleBlack = new Color(12, 12, 12);
        public static readonly Color ConsoleGray = new Color(214, 214, 214);
        // Objects
        public List<BaseObject> ObjectsList = new List<BaseObject>();
        private List<BaseObject> AddList = new List<BaseObject>();
        private List<BaseObject> RemoveList = new List<BaseObject>();
        
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
        private ContentManager contentManager;
        public Core(SpriteBatch _spriteBatch, GraphicsDevice graphicsDevice, ContentManager contentManager, GameWindow window) 
        {
            spriteBatch = _spriteBatch;
            displayManager = new DisplayManager(graphicsDevice);
            Instance = this;
            this.graphicsDevice = graphicsDevice;
            this.contentManager = contentManager;
            ScreenBackground screenBackground = Spawn(new ScreenBackground());
            ImageWindow imageWindow = Spawn(new ImageWindow());
            GameConsole gameConsole = Spawn(new GameConsole());
            InformationConsole informationConsole = Spawn(new InformationConsole());
            this.window = window;
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
                {
                    if (Instance.currentScene.AddList[i].ObjectName == name) return (T)Instance.currentScene.AddList[i];
                }
            }
            if (searchScope == SearchScope.All || searchScope == SearchScope.Global)
            {
                for (int i = 0;i < Instance.ObjectsList.Count;i++) 
                {
                    if (Instance.ObjectsList[i].ObjectName == name) return (T)Instance.ObjectsList[i];
                }
            }

            if (searchScope == SearchScope.All || searchScope == SearchScope.Global)
            {
                for (int i = 0; i < Instance.AddList.Count; i++) 
                {
                    if (Instance.AddList[i].ObjectName == name) return (T)Instance.AddList[i];
                }
            }
 

            if (searchScope == SearchScope.All || searchScope == SearchScope.Local)
            {
                for (int i = 0; i < Instance.currentScene.ObjectsList.Count; i++)
                {
                    if (Instance.currentScene.ObjectsList[i].ObjectName == name) return (T)Instance.currentScene.ObjectsList[i];
                }
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
        public void ChangeScene()
        {
            if (nextScene != null)
            {
                Instance.currentScene?.OnExit();
                Instance.currentScene = nextScene;
                Instance.currentScene.OnEnter();
                nextScene = null;
            }
        }

        private float GetFPS(GameTime gameTime) => 1f/(float)gameTime.ElapsedGameTime.TotalSeconds;

        public class DisplayManager
        {
            RenderTarget2D renderTarget;
            public GraphicsDevice graphicsDevice;
            Color clearColor = ConsoleBlack;
            int targetWidth;
            int targetHeight;
            public DisplayManager(GraphicsDevice GraphicsDevice)
            {
                graphicsDevice = GraphicsDevice;
                targetWidth = Config.pixelScreenWidth;
                targetHeight = Config.pixelScreenHeight;
                renderTarget = new RenderTarget2D(graphicsDevice, targetWidth, targetHeight);
            }
            public void Start()
            {
                graphicsDevice.SetRenderTarget(renderTarget);
                graphicsDevice.Clear(clearColor);
            }
            public void End() => graphicsDevice.SetRenderTarget(null);
            public void Draw(SpriteBatch _spriteBatch)
            {
                Vector2 BackBufferSize = new Vector2(graphicsDevice.PresentationParameters.BackBufferWidth, graphicsDevice.PresentationParameters.BackBufferHeight);
                float coefWidth = BackBufferSize.X / targetWidth;
                float coefHeight = BackBufferSize.Y / targetHeight;
                float minCoef = coefWidth <= coefHeight ? coefWidth : coefHeight;

                Vector2 center = new Vector2(
                    (BackBufferSize.X - targetWidth * minCoef) * Config.ScreenDrawAnchore.X,
                    (BackBufferSize.Y - targetHeight * minCoef) * Config.ScreenDrawAnchore.Y);

                _spriteBatch.Draw(renderTarget,new Rectangle((int)center.X, (int)center.Y, (int)(targetWidth * minCoef),(int)(targetHeight * minCoef)),Color.White);
            }
        }
        
        public static class AssetsManager
        {
            public static class FontsManager
            {
                public static Dictionary<string,FontSystem> userFonts = new Dictionary<string,FontSystem>();
                public static FontSystem baseEngineFont = new FontSystem();
                static FontsManager()
                {
                    var assembly = typeof(Core).Assembly;
                    using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Content.Fonts.CascadiaCode-Light.otf"))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            baseEngineFont.AddFont(ms.ToArray());
                        }
                    }
                }
                public static void AddFont(string name, string path)
                {
                    userFonts.Add(name, new FontSystem());
                    userFonts[name].AddFont(File.ReadAllBytes(path));
                }
            }
            public static class ASCIIImageManager
            {
                static Dictionary<string, ASCIIImage> userASCIIImage = new Dictionary<string, ASCIIImage>();
                static string[] interrobangText = {
                    " --- ",
                    "| ? |",
                    " --- "
                };
                static ASCIIImage interrobangASCIIImage = new ASCIIImage(interrobangText, FontsManager.baseEngineFont.GetFont(18),Color.Wheat);
                public static void AddImage(string name, ASCIIImage image)
                {
                    if (userASCIIImage.ContainsKey(name))
                    {
                        Console.WriteLine($"[Exeption] GE0100: An element with the key {name} already exists (AddImage).");
                        return;
                    }
                    userASCIIImage.Add(name, image);
                }
                public static void AddImageFromFile(string name, SpriteFontBase spf, Color color, string path)
                {
                    if (userASCIIImage.ContainsKey(name))
                    {
                        Console.WriteLine($"[Exeption] GE0100: An element with the key {name} already exists (AddImageFromFile).");
                        return;
                    }
                    string pathDomain = AppDomain.CurrentDomain.BaseDirectory;
                    string contentPath = Path.Combine(pathDomain, "Content", path);
                    if (!File.Exists(contentPath)) 
                    { 
                        Console.WriteLine("[Exeption] GE0101: Content path not exist (AddImageFromFile).");
                        return; 
                    }
                    ASCIIImage image = new ASCIIImage(File.ReadAllLines(contentPath),spf,color);
                    userASCIIImage.Add(name, image);
                }
                public static ASCIIImage GetImage(string name) => userASCIIImage.ContainsKey(name) ? userASCIIImage[name] : interrobangASCIIImage;
            }
            public static class AudioManager
            {
                public static Dictionary<string,SoundEffect> userSoundEffects = new Dictionary<string,SoundEffect>();
                public static Dictionary<string,Song> userSongs = new Dictionary<string,Song>();
                public static void AddAudioElementFromMGCB<T>(string name, string MGCBPath)
                {
                    T asset = Instance.contentManager.Load<T>(MGCBPath);
                    if (asset == null) return;
                    if (asset is SoundEffect) 
                        userSoundEffects.Add(name, asset as SoundEffect);
                    else if (asset is Song)
                        userSongs.Add(name,asset as Song);
                }
                public static T GetAudioElement<T>(string name)
                {
                    if (typeof(T) == typeof(SoundEffect))
                    {
                        if (userSoundEffects.TryGetValue(name, out var soundEffect))
                            return (T)(object)soundEffect;
                    }
                    else if (typeof(T) == typeof(Song) && userSongs.ContainsKey(name))
                    {
                        if (userSongs.TryGetValue(name, out var song))
                            return (T)(object)song;
                    }
                    return default;
                }
            }
        }

        public class ScreenBackground : BaseObject
        {
            public ScreenBackground(string name = null) : base(name) { }
            public override void Draw(SpriteBatch spriteBatch)
            {
                var spf = AssetsManager.FontsManager.baseEngineFont.GetFont(18);
                int deltaY = 0;
                for (int i = 0; i < 40; i++) { spriteBatch.DrawString(spf, "|", new Vector2(1000, deltaY), ConsoleGray); deltaY += 18; }
                spriteBatch.DrawString(spf, "H------------------------------", new Vector2(1000, 280), ConsoleGray);
            }
        }
        public struct ConsoleLog
        {
            SpriteFontBase spriteFontBase;
            string text;
            Vector2 vector;
            Color color;
            public ConsoleLog(SpriteFontBase spriteFontBase,string text, Vector2 vector, Color color)
            {
                this.spriteFontBase = spriteFontBase;
                this.text = text;
                this.vector = vector;
                this.color = color;
            }
            public void WriteLine(SpriteBatch spriteBatch, float delta)
            {
                spriteFontBase.DrawText(spriteBatch, text, new Vector2(vector.X, vector.Y + delta), color);
            }
        }
        public class GameConsole : BaseObject
        {
            public static GameConsole Instance { get; private set; }
            float deltaY = 0;
            float scrollDelta = 0;
            float scrollSensivity = 10;
            List<ConsoleLog> logs = new List<ConsoleLog>();
            public GameConsole(string name = null) : base(name) { Instance = this; }
            public override void Update()
            {
                if (Input.GetNormalizedWheelScroll() == 1) scrollDelta += scrollSensivity;
                else if (Input.GetNormalizedWheelScroll() == -1) scrollDelta -= scrollSensivity;
            }
            public override void Draw(SpriteBatch _spriteBatch)
            {
                for(int i = 0; i < logs.Count; i++) { logs[i].WriteLine(_spriteBatch,scrollDelta); }
            }
            public void WriteLine(SpriteFontBase spf, string text,Color color)
            {
                logs.Add(new ConsoleLog(spf,text, new Vector2(0, 0 + deltaY),color));
                deltaY += spf.FontSize;
            }
            public void Clear()
            {
                logs.Clear();
                deltaY = 0;
                scrollDelta = 0;
            }
        }
        public class ImageWindow : BaseObject 
        {
            public static ImageWindow Instance { get; private set; }
            public ASCIIImage? currentImage { get; private set; }
            private float ignoreScale = 1;
            private Vector2 ignoreVector = Vector2.Zero;
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
                    Transform currTr = new Transform(new Vector2(520+ ignoreVector.X,220+ignoreVector.Y));
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
            public InformationConsole(string name = null) : base(name) { Instance = this; }
            public override void Draw(SpriteBatch _spriteBatch)
            {
                for (int i = 0; i < logs.Count; i++) { logs[i].WriteLine(_spriteBatch, 0); }
            }
            public void WriteLine(SpriteFontBase spf, string text, Color color)
            {
                logs.Add(new ConsoleLog(spf, text, new Vector2(1010, 290 + deltaY), color));
                deltaY += spf.FontSize;
            }
            public void Clear()
            {
                logs.Clear();
                deltaY = 0;
            }
        }
    }
    public static class Debug
    {
        public static Vector2 FPSCounterPosition = new Vector2(1220, 5);
        public static Vector2 MousePositionHandler = new Vector2(1235, 20);
        private static SpriteFontBase font14 = Core.AssetsManager.FontsManager.baseEngineFont.GetFont(14);
        private static SpriteFontBase font13 = Core.AssetsManager.FontsManager.baseEngineFont.GetFont(13);
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
    public class Transform
    {
        public Vector2 position;
        public Vector2 scale;
        public Transform(Vector2 position, Vector2? scale = null)
        {
            this.position = position;
            this.scale = scale ?? Vector2.One;
        }
        public Vector2 GetBasePosition(Vector2 size)
        {
            Vector2 graphicsSize = new Vector2(Config.pixelScreenWidth, Config.pixelScreenHeight);
            return new Vector2(graphicsSize.X * 0.5f + position.X - size.X * scale.X * 0.5f,
                graphicsSize.Y * 0.5f - position.Y - size.Y * scale.Y * 0.5f);
        }
        public static Vector2 ScreenToGamePosition(Vector2 position)
        {
            Vector2 BackBufferSize = new Vector2(Core.Instance.displayManager.graphicsDevice.PresentationParameters.BackBufferWidth,
                Core.Instance.displayManager.graphicsDevice.PresentationParameters.BackBufferHeight);
            float coefWidth = BackBufferSize.X / Config.pixelScreenWidth;
            float coefHeight = BackBufferSize.Y / Config.pixelScreenHeight;
            float minCoef = Math.Min(coefWidth, coefHeight);

            float offsetX = (BackBufferSize.X - Config.pixelScreenWidth * minCoef) * Config.ScreenDrawAnchore.X;
            float offsetY = (BackBufferSize.Y - Config.pixelScreenHeight * minCoef) * Config.ScreenDrawAnchore.Y;

            return new Vector2((position.X - offsetX) / minCoef - Config.pixelScreenWidth * Config.ScreenDrawAnchore.X,
                -((position.Y - offsetY) / minCoef - Config.pixelScreenHeight * Config.ScreenDrawAnchore.Y));
        }
        public static Vector2 GameToScreenPosition(Vector2 position, float width, float height)
            => new Vector2(Config.pixelScreenWidth * 0.5f + position.X - width * 0.5f, Config.pixelScreenHeight * 0.5f - position.Y - height * 0.5f);
    }
}
namespace Utilities
{
    public static class Converter
    {
        [Obsolete("Use Transform.GameToScreenPosition")]
        public static Vector2 Vector2Position(Vector2 vector, float height, float width) => new Vector2(vector.X - width * 0.5f, vector.Y - height * 0.5f);
    }
}
