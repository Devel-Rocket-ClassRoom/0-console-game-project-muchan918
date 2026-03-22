using Framework.Engine;
using System;

public class GameClearScene : Scene
{
    public event Action? RestartRequested;

    private static readonly string[] s_GameClearArt =
    {
        @"           ██╗   ██╗ ██████╗ ██╗   ██╗    ██╗    ██╗██╗███╗   ██╗ ██╗",
        @"           ╚██╗ ██╔╝██╔═══██╗██║   ██║    ██║    ██║██║████╗  ██║ ██║",
        @"            ╚████╔╝ ██║   ██║██║   ██║    ██║ █╗ ██║██║██╔██╗ ██║ ██║",
        @"             ╚██╔╝  ██║   ██║██║   ██║    ██║███╗██║██║██║╚██╗██║ ╚═╝",
        @"              ██║   ╚██████╔╝╚██████╔╝    ╚███╔███╔╝██║██║ ╚████║ ██╗",
        @"              ╚═╝    ╚═════╝  ╚═════╝      ╚══╝╚══╝ ╚═╝╚═╝  ╚═══╝ ╚═╝",
    };

    public override void Load() { }
    public override void Unload() { }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
            RestartRequested?.Invoke();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Yellow);

        // YOU WIN 아스키 아트
        int startY = buffer.Height / 2 - 6;
        buffer.WriteLines(1, startY, s_GameClearArt, ConsoleColor.Yellow);

        // 구분선
        buffer.WriteTextCentered(startY + 7, "─────────────────────────────────", ConsoleColor.DarkYellow);

        // 메시지들
        buffer.WriteTextCentered(startY + 9, "The ancient evil has been defeated!", ConsoleColor.White);
        buffer.WriteTextCentered(startY + 11, "Peace returns to the land once more.", ConsoleColor.Gray);
        buffer.WriteTextCentered(startY + 13, "[ ENTER ] Play Again", ConsoleColor.Green);
        buffer.WriteTextCentered(startY + 14, "[ ESC   ] Quit", ConsoleColor.DarkGray);
    }
}