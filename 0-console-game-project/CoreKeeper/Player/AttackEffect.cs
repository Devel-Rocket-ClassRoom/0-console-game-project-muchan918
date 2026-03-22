using System;
using Framework.Engine;

public class AttackEffect : GameObject
{
    private readonly Map _map;
    private readonly int _tileX;
    private readonly int _tileY;
    private readonly char _char;
    private readonly ConsoleColor _color;

    private float _timer;
    private const float k_Duration = 0.15f;

    public AttackEffect(Scene scene, Map map, int tileX, int tileY, char ch, ConsoleColor color)
        : base(scene)
    {
        _map = map;
        _tileX = tileX;
        _tileY = tileY;
        _char = ch;
        _color = color;
        _timer = k_Duration;
    }

    public override void Update(float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer <= 0)
            Scene.RemoveGameObject(this);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(_tileX, _tileY, buffer);
        if (sx < 0 || sy < 0 || sx + 3 >= buffer.Width || sy + 1 >= buffer.Height) return;

        // 타일 중앙에 이펙트 출력
        buffer.SetCell(sx + 1, sy, _char, _color, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, _char, _color, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, _char, _color, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, _char, _color, ConsoleColor.Black);
    }
}