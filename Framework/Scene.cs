using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace GEngine.Framework
{
    using GEngine.Core;
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

        public static void Exit() => Core.Exit();

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
        public bool IsLoad { get; internal set; } = false;
        public virtual void OnLoad() { }
        public void ObjectListSort() => ObjectsList = ObjectsList.OrderBy(o => o.Layer).ToList();
    }
}
