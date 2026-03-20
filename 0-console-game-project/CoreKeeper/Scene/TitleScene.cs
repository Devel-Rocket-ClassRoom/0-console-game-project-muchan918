using Framework.Engine;
using System;

public class TitleScene : Scene
{
    public event GameAction StartRequested;
    public static readonly string[] GameTitleText =
    {
         @"  ____   ___   ____  _____       _  __ _____ _____ ____  _____ ____ ",
         @" / ___| / _ \ |  _ \| ____|     | |/ /| ____| ____|  _ \| ____|  _ \ ",
         @"| |    | | | || |_) |  _|       | ' / |  _| |  _| | |_| |  _| | |_) |",
         @"| |___ | |_| ||  _ <| |___      | . \ | |___| |___| |_ /| |___|  _ < ",
         @" \____| \___/ |_| \_\_____|     |_|\_\|_____|_____|_|   |___ _|_| \_\"
    };

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
        buffer.WriteLines(6, 5, GameTitleText);
        buffer.WriteTextCentered(12, "Press ENTER to Start", ConsoleColor.Green);
        buffer.WriteTextCentered(15, "ESC: Quit");
    }
}