using Framework.Engine;
using System;

public class TitleScene : Scene
{
    public event GameAction? StartRequested;

    private float _animTimer = 0f;
    private bool _blink = true;

    private static readonly string[] s_TitleArt =
{
    @"  ▄████▄   ▒█████   ██▀███  ▓█████   ██ ▄█▀▓█████ ▓█████  ██▓███  ▓█████  ██▀███  ",
    @" ██▀ ▀█  ▒██▒  ██▒▓██ ▒ ██▒▓█   ▀    ██▄█▒ ▓█   ▀ ▓█   ▀ ▓██░  ██▒▓█   ▀ ▓██ ▒ ██▒",
    @"▒▓█    ▄ ▒██░  ██▒▓██ ░▄█ ▒▒███     ▓███▄░ ▒███   ▒███   ▓██░ ██▓▒▒███   ▓██ ░▄█ ▒",
    @"▒▓▓▄ ▄██▒▒██   ██░▒██▀▀█▄  ▒▓█  ▄   ▓██ █▄ ▒▓█  ▄ ▒▓█  ▄ ▒██▄█▓▒ ▒▒▓█  ▄ ▒██▀▀█▄  ",
    @"▒ ▓███▀ ░░ ████▓▒░░██▓ ▒██▒░▒████▒ ▒██▒ █▄░▒████▒░▒████▒▒██▒ ░  ░░▒████▒░██▓ ▒██▒",
};

    private static readonly string[] s_DecoArt =
    {
        @"    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ",
        @"    ░       Mine resources        Fight monsters        Defeat the Boss     ░    ",
        @"    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    ",
    };

    public override void Load() { }
    public override void Unload() { }

    public override void Update(float deltaTime)
    {
        _animTimer += deltaTime;
        if (_animTimer >= 0.6f)
        {
            _blink = !_blink;
            _animTimer = 0f;
        }

        if (Input.IsKeyDown(ConsoleKey.Enter))
            StartRequested?.Invoke();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.DarkYellow);

        // 타이틀 아트
        buffer.WriteLines(0, 2, s_TitleArt, ConsoleColor.Yellow);

        // 구분선
        buffer.WriteTextCentered(9, "═══════════════════════════════════════════════════════", ConsoleColor.DarkYellow);

        // 설명
        buffer.WriteLines(0, 11, s_DecoArt, ConsoleColor.DarkGray);

        // 조작키 안내
        buffer.WriteTextCentered(15, "WASD : Move      Arrow Keys : Attack      Tab : Inventory", ConsoleColor.Gray);
        buffer.WriteTextCentered(16, "Space : Use Item      E : Interact", ConsoleColor.Gray);

        // 구분선
        buffer.WriteTextCentered(18, "═══════════════════════════════════════════════════════", ConsoleColor.DarkYellow);

        // 깜빡이는 시작 메시지
        if (_blink)
            buffer.WriteTextCentered(19, "▶  Press ENTER to Start  ◀", ConsoleColor.Green);

        buffer.WriteTextCentered(20, "ESC : Quit", ConsoleColor.DarkGray);
    }
}