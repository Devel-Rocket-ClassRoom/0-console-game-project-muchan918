using Framework.Engine;
using System;

public class TitleScene : Scene
{
    public event GameAction StartRequested;

    public override void Load()
    {

    }

    public override void Unload()
    {

    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            StartRequested?.Invoke();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Yellow);
        buffer.WriteLines(6, 5, TextData.GameTitleText);
        buffer.WriteTextCentered(12, "Press ENTER to Start", ConsoleColor.Green);
        buffer.WriteTextCentered(15, "ESC: Quit");
    }
}