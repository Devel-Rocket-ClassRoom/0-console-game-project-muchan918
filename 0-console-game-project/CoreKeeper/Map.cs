using System;
using System.ComponentModel.Design;
using Framework.Engine;

public class Map : GameObject
{
    // 타일 배열 크기
    private readonly int _tileWidth = 200;
    private readonly int _tileHeight = 100;

    // viewPosition은 월드 좌표 기준
    private (int X, int Y) _viewPosition = (10, 10);
    public (int X, int Y) ViewPosition => _viewPosition;

    private readonly Tile[,] _tiles; // [tileY, tileX]

    private readonly Random _random = new Random();

    public Map(Scene scene) : base(scene)
    {
        _tiles = new Tile[_tileHeight, _tileWidth];

        // 1단계: 전체 Ground
        for (int ty = 0; ty < _tileHeight; ty++)
            for (int tx = 0; tx < _tileWidth; tx++)
                _tiles[ty, tx] = new Tile(TileType.Ground);

        // 2단계: Soil 군집
        for (int ty = 0; ty < _tileHeight; ty++)
            for (int tx = 0; tx < _tileWidth; tx++)
                if (_random.NextDouble() < 0.04f)
                    Spread(tx, ty, 5, TileType.Soil, 0.6f);

        // 3단계: Wood 군집
        for (int ty = 0; ty < _tileHeight; ty++)
            for (int tx = 0; tx < _tileWidth; tx++)
                if (_random.NextDouble() < 0.008f)
                    Spread(tx, ty, 3, TileType.Wood, 0.5f);

        // 4단계: 스폰 구역 보호
        int cx = _tileWidth / 2;
        int cy = _tileHeight / 2;
        for (int ty = cy - 3; ty <= cy + 3; ty++)
            for (int tx = cx - 3; tx <= cx + 3; tx++)
                if (tx >= 0 && ty >= 0 && tx < _tileWidth && ty < _tileHeight)
                    _tiles[ty, tx] = new Tile(TileType.Ground);
    }

    private void Spread(int tx, int ty, int depth, TileType type, float spreadChance)
    {
        if (depth <= 0 || tx < 0 || ty < 0 || tx >= _tileWidth || ty >= _tileHeight) return;
        if (_tiles[ty, tx].Type == type) return;

        _tiles[ty, tx] = new Tile(type);

        if (_random.NextDouble() < spreadChance) Spread(tx + 1, ty, depth - 1, type, spreadChance);
        if (_random.NextDouble() < spreadChance) Spread(tx - 1, ty, depth - 1, type, spreadChance);
        if (_random.NextDouble() < spreadChance) Spread(tx, ty + 1, depth - 1, type, spreadChance);
        if (_random.NextDouble() < spreadChance) Spread(tx, ty - 1, depth - 1, type, spreadChance);
    }

    public override void Update(float deltaTime)
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        // 타일 1개 = 월드 4×2 이므로 화면에 보이는 타일 수
        int viewTileW = buffer.Width / 4; // 80/4 = 20
        int viewTileH = buffer.Height / 2; // 20/2 = 10

        // 월드 → 타일 변환
        int viewTileX = _viewPosition.X / 4;
        int viewTileY = _viewPosition.Y / 2;

        int startTileX = viewTileX - viewTileW / 2;
        int startTileY = viewTileY - viewTileH / 2;

        for (int ty = 0; ty < viewTileH; ty++)
        {
            int tileY = startTileY + ty;
 
            for (int tx = 0; tx < viewTileW; tx++)
            {
                int tileX = startTileX + tx;
 
                if (tileX < 0 || tileY < 0 || tileX >= _tileWidth || tileY >= _tileHeight)
                {
                    DrawOutSide(buffer, tx, ty, '░', ConsoleColor.DarkGray);
                    continue;
                }
 
                Tile tile = _tiles[tileY, tileX];
                DrawTile(buffer, tx, ty, tile.DisplayChar, tile.ForeColor);
            }
        }
    }

    // 월드 타일 좌표(tx, ty)를 스크린 4×2로 그리는 함수
    private void DrawOutSide(ScreenBuffer buffer, int tx, int ty, char ch, ConsoleColor color)
    {
        int sx = tx * 4; // DrawTile이랑 동일한 기준
        int sy = ty * 2;

        // 4×2 전체 채우기
        for (int dy = 0; dy < 2; dy++)
            for (int dx = 0; dx < 4; dx++)
                buffer.SetCell(sx + dx, sy + dy, ch, color);
    }

    // 타일 1개를 스크린 (sx,sy)(sx+2,sy)(sx,sy+1)(sx+2,sy+1) 4군데에 찍기
    // tx, ty = 뷰 기준 타일 인덱스
    // 스크린 좌표: sx = tx*4 기준으로 0,2 / sy = ty*2 기준으로 0,1
    private void DrawTile(ScreenBuffer buffer, int tx, int ty, char ch, ConsoleColor color)
    {
        int sx = tx * 4; // 타일 1개 = 가로 4칸
        int sy = ty * 2; // 타일 1개 = 세로 2칸

        buffer.SetCell(sx, sy, ch, color); // (0,0)
        buffer.SetCell(sx + 2, sy, ch, color); // (2,0)
        buffer.SetCell(sx, sy + 1, ch, color); // (0,1)
        buffer.SetCell(sx + 2, sy + 1, ch, color); // (2,1)
    }

    // 월드 좌표로 viewPosition 설정
    public void SetViewPosition(int worldX, int worldY)
    {
        _viewPosition = (worldX, worldY);
    }
}