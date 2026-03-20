using System;
using Framework.Engine;

// WoodItem
public class WoodItem : Item, IDroppable, IInventoryItem
{
    public override string Name { get; set; } = "Wood";
    public override ConsoleColor Color => ConsoleColor.DarkYellow;

    // IDroppable - 타일 좌표
    public int TileX { get; set; }
    public int TileY { get; set; }

    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 10;

    private readonly Map _map;

    public WoodItem(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Update(float deltaTime)
    {
        var player = (Player)Scene.FindGameObject("Player");
        if (player == null) return;

        // 플레이어 Position이랑 같으면 PickUp
        if (player.Position == (TileX, TileY))
        {
            OnPickup(player);
        }
    }

    public void OnPickup(Player player)
    {
        player.Inventory.AddItem(this);
        Scene.RemoveGameObject(this);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '▗', Color);
        buffer.SetCell(sx + 2, sy, '▖', Color);
        buffer.SetCell(sx + 1, sy + 1, '▝', Color);
        buffer.SetCell(sx + 2, sy + 1, '▘', Color);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        buffer.SetCell(sx + 1, sy, '▗', Color, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▖', Color, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', Color, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', Color, ConsoleColor.DarkGray);
    }

    public override void Use(Player player) { }
}