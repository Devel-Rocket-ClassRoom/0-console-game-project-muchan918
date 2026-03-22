using System;
using Framework.Engine;

public class MushroomItem : Item, IDroppable, IInventoryItem
{
    public int TileX { get; set; }
    public int TileY { get; set; }

    public int Count { get; set; } = 1;
    public int MaxStack => 10;

    private const int k_HealAmount = 3;

    private readonly Map _map;

    public MushroomItem(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        Name = "Mushroom";
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '♠', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '♠', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '╹', ConsoleColor.White, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '╹', ConsoleColor.White, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        buffer.SetCell(sx + 1, sy, '♠', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '♠', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '╹', ConsoleColor.White, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '╹', ConsoleColor.White, ConsoleColor.DarkGray);
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

        if (player.Position == (TileX, TileY))
        {
            OnPickup(player);
        }
    }

    public override void Use(Player player)
    {
        if (player.Hp >= player.MaxHp) return;
        player.Heal(k_HealAmount);
        ConsumeFromInventory(player);
    }
}
