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
        _scenes.CurrentScene?.Update(deltaTime);
    }

    // 씬 변경 플로우
    private void ChangeToTitle()
    {
        var title = new TitleScene();
        title.StartRequested += ChangeToLoading;
        _scenes.ChangeScene(title);
    }

    private void ChangeToLoading()
    {
        var loading = new LoadingScene();
        loading.LoadingComplete += ChangeToPlay;
        _scenes.ChangeScene(loading);
    }

    private void ChangeToPlay()
    {
        var play = new PlayScene();
        play.GameOverRequested += ChangeToGameOver;
        play.GameClearRequested += ChangeToGameClear;
        _scenes.ChangeScene(play);
    }

    private void ChangeToGameOver()
    {
        var over = new GameOverScene();
        over.RestartRequested += ChangeToTitle;
        _scenes.ChangeScene(over);
    }

    private void ChangeToGameClear()
    {
        var clear = new GameClearScene();
        clear.RestartRequested += ChangeToTitle;
        _scenes.ChangeScene(clear);
    }
}