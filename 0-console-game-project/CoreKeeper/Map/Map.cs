using System;
using System.ComponentModel.Design;
using Framework.Engine;

public class Map : GameObject
{
    // 타일 배열 크기
    private readonly int _tileWidth = 200;
    private readonly int _tileHeight = 100;

    public int TileWidth => _tileWidth;
    public int TileHeight => _tileHeight;

    // viewPosition은 월드 좌표 기준
    private (int X, int Y) _viewPosition = (100, 50);
    public (int X, int Y) ViewPosition => _viewPosition;
    public (int X, int Y) BossSpawnPoint { get; private set; }

    private readonly Tile[,] _tiles; // [tileY, tileX]

    private readonly Random _random = new Random();

    public Map(Scene scene) : base(scene)
    {
        _tiles = new Tile[_tileHeight, _tileWidth];

        // 1단계: 전체 Ground
        for (int ty = 0; ty < _tileHeight; ty++)
            for (int tx = 0; tx < _tileWidth; tx++)
                _tiles[ty, tx] = new Tile(TileType.Ground);

        // 2단계: Soil + Stone군집
        for (int ty = 0; ty < _tileHeight; ty++)
            for (int tx = 0; tx < _tileWidth; tx++)
                if (_random.NextDouble() < 0.04f)
                {
                    TileType type = _random.NextDouble() < 0.75 ? TileType.Soil : TileType.Stone;
                    Spread(tx, ty, 5, type, 0.6f);
                }

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

        // 5단계: 보스 구역 설정 (맵 끝쪽 랜덤)
        var random2 = new Random();
        (int bx, int by) = GetBossSpawnPoint(random2);
        BossSpawnPoint = (bx, by);
        ClearBossArea(bx, by);
    }

    private (int bx, int by) GetBossSpawnPoint(Random random)
    {
        // 맵 4개 끝 중 하나 선택
        int edge = random.Next(4);
        int bx, by;
        int margin = 15; // 끝에서 15타일 안쪽

        switch (edge)
        {
            case 0: // 위쪽
                bx = random.Next(margin, _tileWidth - margin);
                by = margin;
                break;
            case 1: // 아래쪽
                bx = random.Next(margin, _tileWidth - margin);
                by = _tileHeight - margin;
                break;
            case 2: // 왼쪽
                bx = margin;
                by = random.Next(margin, _tileHeight - margin);
                break;
            default: // 오른쪽
                bx = _tileWidth - margin;
                by = random.Next(margin, _tileHeight - margin);
                break;
        }
        return (bx, by);
    }

    private void ClearBossArea(int cx, int cy)
    {
        int radius = 10; // 20x20 범위
        for (int ty = cy - radius; ty <= cy + radius; ty++)
            for (int tx = cx - radius; tx <= cx + radius; tx++)
                if (InBounds(tx, ty))
                    _tiles[ty, tx] = new Tile(TileType.Ground);
    }

    public bool IsNearBoss(int tx, int ty)
    {
        int radius = 12;
        return Math.Abs(tx - BossSpawnPoint.X) <= radius &&
               Math.Abs(ty - BossSpawnPoint.Y) <= radius;
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

    private bool InBounds(int tx, int ty) => tx >= 0 && ty >= 0 && tx < _tileWidth && ty < _tileHeight;

    public override void Update(float deltaTime)
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        int viewTileW = buffer.Width / 4;
        int viewTileH = buffer.Height / 2;

        int startTileX = _viewPosition.X - viewTileW / 2;
        int startTileY = _viewPosition.Y - viewTileH / 2;

        for (int ty = 0; ty < viewTileH; ty++)
        {
            int tileY = startTileY + ty;
 
            for (int tx = 0; tx < viewTileW; tx++)
            {
                int tileX = startTileX + tx;
 
                if (!InBounds(tileX, tileY))
                {
                    DrawOutside(buffer, tx, ty);
                    continue;
                }
 
                Tile tile = _tiles[tileY, tileX];

                // Object 타입이면 InstalledItem의 DrawIcon 호출
                if (tile.Type == TileType.Object && tile.InstalledItem is IInstallable installable)
                    installable.DrawInstalled(tx, ty, buffer);
                else
                    DrawTile(buffer, tx, ty, tile.DisplayChar, tile.ForeColor);
            }
        }
    }

    private void DrawOutside(ScreenBuffer buffer, int tx, int ty)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        for (int dy = 0; dy < 2; dy++)
            for (int dx = 0; dx < 4; dx++)
                buffer.SetCell(sx + dx, sy + dy, '░', ConsoleColor.DarkGray);
    }

    // 타일 1개를 스크린 (sx,sy)(sx+2,sy)(sx,sy+1)(sx+2,sy+1) 4군데에 찍기
    // tx, ty = 뷰 기준 타일 인덱스
    // 스크린 좌표: sx = tx*4 기준으로 0,2 / sy = ty*2 기준으로 0,1
    private void DrawTile(ScreenBuffer buffer, int tx, int ty, char ch, ConsoleColor color)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx, sy, ch, color);
        buffer.SetCell(sx + 2, sy, ch, color);
        buffer.SetCell(sx, sy + 1, ch, color);
        buffer.SetCell(sx + 2, sy + 1, ch, color);
    }

    public void SetViewPosition(int tileX, int tileY)
    {
        _viewPosition = (tileX, tileY);
    }

    public bool IsMovable(int tileX, int tileY)
    {
        if (!InBounds(tileX, tileY)) return false;
        return _tiles[tileY, tileX].IsMovable;
    }

    public bool IsMinable(int tileX, int tileY)
    {
        if (!InBounds(tileX, tileY)) return false;
        return _tiles[tileY, tileX].IsMinable;
    }

    // Object 타입 설치
    public void SetTile(int tileX, int tileY, TileType type, Item? installedItem = null)
    {
        if (!InBounds(tileX, tileY)) return;
        if (!_tiles[tileY, tileX].IsMovable) return;
        _tiles[tileY, tileX] = new Tile(type, installedItem);
    }

    public TileType BreakTile(int tileX, int tileY)
    {
        if (!InBounds(tileX, tileY)) return TileType.Ground;
        TileType broken = _tiles[tileY, tileX].Type;
        _tiles[tileY, tileX] = new Tile(TileType.Ground);
        return broken;
    }

    // 채굴 - broken 여부와 InstalledItem 반환
    public (bool broken, Item? installedItem) MineTile(int tileX, int tileY, int damage)
    {
        if (!InBounds(tileX, tileY)) return (false, null);
        var tile = _tiles[tileY, tileX];
        if (!tile.IsMinable) return (false, null);

        int newHp = tile.Hp - damage;

        if (newHp <= 0)
        {
            var installedItem = tile.InstalledItem;
            _tiles[tileY, tileX] = new Tile(TileType.Ground);
            return (true, installedItem);
        }

        _tiles[tileY, tileX] = new Tile(tile.Type, newHp, tile.InstalledItem);
        return (false, null);
    }

    // ── 타일 좌표 → 스크린 좌표 변환 ─────
    // 외부에서 쓸 수 있도록 public
    public (int sx, int sy) TileToScreen(int tileX, int tileY, ScreenBuffer buffer)
    {
        int viewTileW = buffer.Width / 4;
        int viewTileH = buffer.Height / 2;

        int startTileX = _viewPosition.X - viewTileW / 2;
        int startTileY = _viewPosition.Y - viewTileH / 2;

        int sx = (tileX - startTileX) * 4;
        int sy = (tileY - startTileY) * 2;
        return (sx, sy);
    }

    public TileType GetTileType(int tileX, int tileY)
    {
        if (!InBounds(tileX, tileY)) return TileType.Ground;
        return _tiles[tileY, tileX].Type;
    }

    public Item? GetInstalledItem(int tileX, int tileY)
    {
        if (!InBounds(tileX, tileY)) return null;
        return _tiles[tileY, tileX].InstalledItem;
    }
}