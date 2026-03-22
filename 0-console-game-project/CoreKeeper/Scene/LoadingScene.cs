using Framework.Engine;
using System;

public class LoadingScene : Scene
{
    public event Action? LoadingComplete;

    private float _timer = 0f;
    private const float k_LoadingDuration = 3f;
    private int _dotCount = 0;
    private float _dotTimer = 0f;

    private static readonly string[] s_LoadingArt =
    {
        @"           ██╗      ██████╗  █████╗ ██████╗ ██╗███╗   ██╗ ██████╗ ",
        @"           ██║     ██╔═══██╗██╔══██╗██╔══██╗██║████╗  ██║██╔════╝ ",
        @"           ██║     ██║   ██║███████║██║  ██║██║██╔██╗ ██║██║  ███╗",
        @"           ██║     ██║   ██║██╔══██║██║  ██║██║██║╚██╗██║██║   ██║",
        @"           ███████╗╚██████╔╝██║  ██║██████╔╝██║██║ ╚████║╚██████╔╝",
        @"            ╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝ ╚═╝╚═╝  ╚═══╝ ╚═════╝ ",
    };

    public override void Load() { }
    public override void Unload() { }

    public override void Update(float deltaTime)
    {
        _timer += deltaTime;

        _dotTimer += deltaTime;
        if (_dotTimer >= 0.4f)
        {
            _dotCount = (_dotCount + 1) % 4;
            _dotTimer = 0f;
        }

        if (_timer >= k_LoadingDuration)
            LoadingComplete?.Invoke();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.DarkYellow);

        // 로딩 아트
        int startY = buffer.Height / 2 - 5;
        buffer.WriteLines(0, startY, s_LoadingArt, ConsoleColor.Yellow);

        // 프로그레스 바
        int barWidth = 40;
        int filled = (int)(_timer / k_LoadingDuration * barWidth);
        filled = Math.Min(filled, barWidth);

        int barX = (buffer.Width - barWidth - 2) / 2;
        int barY = startY + 8;

        buffer.WriteText(barX, barY, "[", ConsoleColor.White);
        for (int i = 0; i < barWidth; i++)
        {
            char ch = i < filled ? '█' : '░';
            ConsoleColor col = i < filled ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
            buffer.SetCell(barX + 1 + i, barY, ch, col);
        }
        buffer.WriteText(barX + barWidth + 1, barY, "]", ConsoleColor.White);

        // 퍼센트
        int percent = (int)(_timer / k_LoadingDuration * 100);
        percent = Math.Min(percent, 100);
        buffer.WriteTextCentered(barY + 1, $"{percent}%", ConsoleColor.Yellow);

        // 로딩 텍스트
        string dots = new string('.', _dotCount);
        buffer.WriteTextCentered(barY + 2, $"Generating world{dots}", ConsoleColor.Gray);
    }
}