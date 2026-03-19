using System;
using Framework.Engine;

public class CoreKeeper : GameApp
{
    private readonly SceneManager<Scene> _scenes = new SceneManager<Scene>();

    public CoreKeeper() : base(80, 20)
    {

    }

    public CoreKeeper(int width, int height) : base(width, height)
    {

    }

    protected override void Draw()
    {
        _scenes.CurrentScene?.Draw(Buffer);
    }

    protected override void Initialize()
    {
        ChangeToTitle();
    }

    protected override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            Quit();
            return;
        }
    }

    // 씬 변경 플로우
    private void ChangeToTitle()
    {
        var title = new TitleScene();
        title.StartRequested += ChangeToPlay;
        _scenes.ChangeScene(title);
    }

    private void ChangeToPlay()
    {
        //var play = new PlayScene();
        //play.PlayAgainRequested += ChangeToTitle;
        //_scenes.ChangeScene(play);
    }
}