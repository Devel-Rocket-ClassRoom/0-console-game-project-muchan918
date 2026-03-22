using System;
using Framework.Engine;

public class StoneItem : Item, IDroppable, IInventoryItem
{
    // IDroppable - 타일 좌표
    public int TileX { get; set; }
    public int TileY { get; set; }

    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 10;

    private readonly Map _map;

    public StoneItem(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        Name = "Stone";
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '▄', ConsoleColor.Gray, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '▄', ConsoleColor.Gray, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▀', ConsoleColor.Gray, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▀', ConsoleColor.Gray, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        buffer.SetCell(sx + 1, sy, '▄', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▄', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▀', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▀', ConsoleColor.Gray, ConsoleColor.DarkGray);
    }

    public void OnPickup(Player player)
    {
        player.Inventory.AddItem(this);
        Scene.RemoveGameObject(this);
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

    public override void Use(Player player)
    {
        var (tx, ty) = player.GetFrontTile();
        player.GetMap().SetTile(tx, ty, TileType.Stone);
        ConsumeFromInventory(player);
    }
}