using System;
using System.Threading;

namespace Framework.Engine
{
    // 전체 게임 어플리케이션
    public abstract class GameApp
    {
        private const int k_TargetFrameTime = 33; //이 값을 줄이면 빠르게 갱신
        private bool _isRunning; // 게임 종료? 

        // 게임을 그리는 버퍼
        protected ScreenBuffer Buffer { get; private set; }

        public event GameAction GameStarted;
        public event GameAction GameStopped;

        protected GameApp(int width, int height) // 콘솔 사이즈 (콘솔 사이즈 고정 시키는거 알아보기)
        {
            Buffer = new ScreenBuffer(width, height);
        }

        public void Run()
        {
            // 게임 루프 들어가기 전 초기화 부분
            Console.CursorVisible = false; // 깜빡거리는 커서 없애기
            Console.Clear();

            _isRunning = true;
            Initialize(); // 추상 함수로 구현된 초기화(초기화 할 객체가 있으면 이걸 오버라이드 해서 구현)
            GameStarted?.Invoke();

            int previousTime = Environment.TickCount;

            // 게임 루프
            while (_isRunning)
            {
                int currentTime = Environment.TickCount;
                float deltaTime = (currentTime - previousTime) / 1000f;
                previousTime = currentTime; // 프레임 간격 계산

                Input.Poll();
                Update(deltaTime);
                Buffer.Clear();
                Draw();
                Buffer.Present();

                int elapsed = Environment.TickCount - currentTime;
                int sleepTime = k_TargetFrameTime - elapsed;
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }

            GameStopped?.Invoke();
            Console.CursorVisible = true;
            Console.ResetColor();
            Console.Clear();
        }

        protected void Quit()
        {
            _isRunning = false;
        }

        protected abstract void Initialize();
        protected abstract void Update(float deltaTime);
        protected abstract void Draw();
    }
}
