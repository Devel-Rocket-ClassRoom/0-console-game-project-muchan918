using System;
using Framework.Engine;

public class LegendaryPotion : Item, IInventoryItem, IDroppable
{
    private readonly int _healAmount;
    private readonly Map _map;

    public int Count { get; set; } = 1;
    public int MaxStack => 5;

    public int TileX { get; set; }
    public int TileY { get; set; }

    public LegendaryPotion(Scene scene, Map map, int tileX, int tileY, int healAmount) : base(scene)
    {
        Name = "Dragon Elixir";
        _map = map;
        TileX = tileX;
        TileY = tileY;
        _healAmount = healAmount;
    }

    public override void Update(float deltaTime)
    {
        var player = Scene.FindGameObject("Player") as Player;
        if (player == null) return;
        if (player.Position == (TileX, TileY))
            OnPickup(player);
    }

    public void OnPickup(Player player)
    {
        player.Inventory.AddItem(this);
        Scene.RemoveGameObject(this);
    }

    public override void Use(Player player)
    {
        if (player.Hp >= player.MaxHp) return;
        player.Heal(_healAmount);
        ConsumeFromInventory(player);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.DarkCyan, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '░', ConsoleColor.DarkCyan, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▓', ConsoleColor.DarkCyan, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▓', ConsoleColor.DarkCyan, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4; int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.DarkCyan, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '░', ConsoleColor.DarkCyan, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▓', ConsoleColor.DarkCyan, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▓', ConsoleColor.DarkCyan, ConsoleColor.DarkGray);
    }
}