using System.Collections.Generic;

namespace Framework.Engine
{
    public abstract class Scene
    {
        // 씬은 게임 오브젝트들의 집합으로 구성된다
        private readonly List<GameObject> _gameObjects = new List<GameObject>(); // GameObject를 바로 넣는게 아니고 추가할거 삭제할거를 미리 담아두고 나중에 한번에 적용하기
        private readonly List<GameObject> _pendingAdd = new List<GameObject>();
        private readonly List<GameObject> _pendingRemove = new List<GameObject>();
        private bool _isUpdating;

        public abstract void Load();
        public abstract void Update(float deltaTime);
        public abstract void Draw(ScreenBuffer buffer);
        public abstract void Unload(); //씬 종료될때

        public void AddGameObject(GameObject gameObject)
        {
            if (_isUpdating)
            {
                _pendingAdd.Add(gameObject);//업데이트 할 때는 업데이트를 하지 않는다
            }
            else
            {
                _gameObjects.Add(gameObject);
            }
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (_isUpdating)
            {
                _pendingRemove.Add(gameObject); //업데이트 할 때는 업데이트를 하지 않는다
            }
            else
            {
                _gameObjects.Remove(gameObject);
            }
        }

        public void ClearGameObjects()
        {
            _gameObjects.Clear();
            _pendingAdd.Clear();
            _pendingRemove.Clear();
        }

        protected void UpdateGameObjects(float deltaTime)
        {
            FlushPending();
            _isUpdating = true;

            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].IsActive)
                {
                    _gameObjects[i].Update(deltaTime);
                }
            }

            _isUpdating = false;
        }

        protected void DrawGameObjects(ScreenBuffer buffer)
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].IsActive)
                {
                    _gameObjects[i].Draw(buffer);
                }
            }
        }

        public GameObject FindGameObject(string name)
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if (_gameObjects[i].Name == name)
                {
                    return _gameObjects[i];
                }
            }

            for (int i = 0; i < _pendingAdd.Count; i++)
            {
                if (_pendingAdd[i].Name == name)
                {
                    return _pendingAdd[i];
                }
            }

            return null;
        }

        public IEnumerable<T> GetGameObjects<T>() where T : GameObject
        {
            return _gameObjects.OfType<T>();
        }

        private void FlushPending()
        {
            if (_pendingRemove.Count > 0)
            {
                for (int i = 0; i < _pendingRemove.Count; i++)
                {
                    _gameObjects.Remove(_pendingRemove[i]); // 업데이트때 적용 안시켰던걸 한번에 업데이트 해주기
                }
                _pendingRemove.Clear();
            }

            if (_pendingAdd.Count > 0)
            {
                _gameObjects.AddRange(_pendingAdd);
                _pendingAdd.Clear();
            }
        }
    }
}
