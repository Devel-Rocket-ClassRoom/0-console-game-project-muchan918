using System;
using Framework.Engine;

public class Portal : GameObject
{
    private readonly Map _map;
    private float _animTimer = 0f;
    private int _animFrame = 0;

    public int TileX { get; private set; }
    public int TileY { get; private set; }

    public Portal(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        Name = "Portal";
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Update(float deltaTime)
    {
        // 애니메이션
        _animTimer += deltaTime;
        if (_animTimer >= 0.3f)
        {
            _animFrame = (_animFrame + 1) % 3;
            _animTimer = 0f;
        }

        var player = Scene.FindGameObject("Player") as Player;
        if (player == null) return;

        if (player.Position == (TileX, TileY))
        {
            var playScene = Scene as PlayScene;
            playScene?.RequestBossScene();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 3 >= buffer.Width || sy + 1 >= buffer.Height) return;

        ConsoleColor col = _animFrame switch
        {
            0 => ConsoleColor.Magenta,
            1 => ConsoleColor.DarkMagenta,
            _ => ConsoleColor.Blue,
        };

        buffer.SetCell(sx, sy, '░', col, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy, '▓', col, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '▓', col, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy, '░', col, ConsoleColor.Black);
        buffer.SetCell(sx, sy + 1, '▒', col, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '█', col, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '█', col, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy + 1, '▒', col, ConsoleColor.Black);
    }
}