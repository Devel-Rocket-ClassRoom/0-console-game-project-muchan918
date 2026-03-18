using System;

namespace Framework.Engine
{
    public abstract class GameObject
    {
        public string Name { get; set; } = ""; // 아이디
        public bool IsActive { get; set; } = true; // 비활성화 활성화 (활성화 일 때만 Draw를 한다)
        public Scene Scene { get; }

        protected GameObject(Scene scene)
        {
            Scene = scene;
        }

        public abstract void Update(float deltaTime);
        public abstract void Draw(ScreenBuffer buffer);
    }
}
