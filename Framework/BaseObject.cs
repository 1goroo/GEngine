using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GEngine.Framework
{
    using GEngine.Core;
    public abstract class BaseObject : IUpdatable, IDrawable, IStartable, IDestroyable, IAwakeable
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
        protected static void Destroy(BaseObject baseObject) => Core.Instance.currentScene.Destroy(baseObject);
        protected static void DestroyFromGlobal(BaseObject baseObject) => Core.Destroy(baseObject);
        public void SetLayer(int newLayer)
        {
            if (Layer == newLayer) return;
            Layer = newLayer;
            if (DontDestroyOnLoad) Core.Instance.ObjectListSort();
            else Core.Instance.currentScene.ObjectListSort();
        }
        public void Invoke(Action action, float time) => invokes.Add((action, time));
    }
}
