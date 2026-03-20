using System;
using Framework.Engine;

public abstract class Spawner : GameObject
{
    public int TileX { get; protected set; }
    public int TileY { get; protected set; }

    protected readonly Map Map;

    protected Spawner(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        Map = map;
        TileX = tileX;
        TileY = tileY;
    }

    // 타일 좌표 → 스크린 좌표 변환
    protected (int sx, int sy) GetScreenPos(ScreenBuffer buffer)
        => Map.TileToScreen(TileX, TileY, buffer);

    // 화면 안에 있는지 확인
    protected bool IsInView(ScreenBuffer buffer)
    {
        var (sx, sy) = GetScreenPos(buffer);
        return sx >= 0 && sy >= 0 && sx + 3 < buffer.Width && sy + 1 < buffer.Height;
    }
}