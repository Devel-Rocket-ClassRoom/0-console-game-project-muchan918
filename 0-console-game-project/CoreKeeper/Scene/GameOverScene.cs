using Framework.Engine;
using System;

public class GameOverScene : Scene
{
    public event Action? RestartRequested;

    private static readonly string[] s_GameOverArt =
    {
        @"    ██████╗  █████╗ ███╗   ███╗███████╗     ██████╗ ██╗   ██╗███████╗██████╗ ",
        @"   ██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ██╔═══██╗██║   ██║██╔════╝██╔══██╗",
        @"   ██║  ███╗███████║██╔████╔██║█████╗      ██║   ██║██║   ██║█████╗  ██████╔╝",
        @"   ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝      ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗",
        @"   ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗    ╚██████╔╝ ╚████╔╝ ███████╗██║  ██║",
        @"    ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝",
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
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Red);

        // GAME OVER 아스키 아트
        int startY = buffer.Height / 2 - 6;
        buffer.WriteLines(1, startY, s_GameOverArt, ConsoleColor.Red);

        // 구분선
        buffer.WriteTextCentered(startY + 7, "─────────────────────────────────", ConsoleColor.DarkRed);

        // 메시지들
        buffer.WriteTextCentered(startY + 9, "You have fallen in battle...", ConsoleColor.Gray);
        buffer.WriteTextCentered(startY + 11, "The darkness claims another soul.", ConsoleColor.DarkGray);
        buffer.WriteTextCentered(startY + 13, "[ ENTER ] Try Again", ConsoleColor.White);
        buffer.WriteTextCentered(startY + 14, "[ ESC   ] Quit", ConsoleColor.DarkGray);
    }
}