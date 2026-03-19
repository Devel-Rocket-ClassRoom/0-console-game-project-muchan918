using System;
using Framework.Engine;

public class SoilItem : Item, IDroppable, IInventoryItem
{
    public override string Name { get; set; } = "나무";
    public override ConsoleColor Color => ConsoleColor.DarkGreen;

    // IDroppable
    public int WorldX { get; set; }
    public int WorldY { get; set; }
    public void OnPickup(Player player) { }

    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 99;

    private readonly Map _map;

    public SoilItem(Scene scene, Map map, int worldX, int worldY) : base(scene)
    {
        _map = map;
        WorldX = worldX;
        WorldY = worldY;
    }

    public override void Update(float deltaTime)
    {
        // 추후: 플레이어랑 위치 같으면 인벤토리에 추가
    }

    public override void Draw(ScreenBuffer buffer)
    {
        // 월드 → 타일 좌표 변환
        int viewTileW = buffer.Width / 4;
        int viewTileH = buffer.Height / 2;

        int viewTileX = _map.ViewPosition.X / 4;
        int viewTileY = _map.ViewPosition.Y / 2;

        int startTileX = viewTileX - viewTileW / 2;
        int startTileY = viewTileY - viewTileH / 2;

        // 아이템 월드 좌표 → 타일 좌표
        int itemTileX = WorldX / 4;
        int itemTileY = WorldY / 2;

        // 타일 좌표 → 스크린 좌표
        int sx = (itemTileX - startTileX) * 4;
        int sy = (itemTileY - startTileY) * 2;

        // 화면 밖이면 그리지 않음
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        // ▗▖
        // ▝▘ 출력
        buffer.SetCell(sx + 1, sy, '▗', Color);
        buffer.SetCell(sx + 2, sy, '▖', Color);
        buffer.SetCell(sx + 1, sy + 1, '▝', Color);
        buffer.SetCell(sx + 2, sy + 1, '▘', Color);
    }

    public override void Use(Player player) { }
}
