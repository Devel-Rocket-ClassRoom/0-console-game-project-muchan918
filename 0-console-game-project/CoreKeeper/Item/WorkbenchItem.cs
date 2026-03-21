using System;
using Framework.Engine;

public class WorkbenchItem : Item, IInventoryItem, ICraftable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // ICraftable
    public (string itemName, int count)[] Recipe => new[]
    {
        ("Wood", 5)
    };

    public WorkbenchItem(Scene scene) : base(scene)
    {
        Name = "Workbench";
    }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        //buffer.SetCell(sx, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        //buffer.SetCell(sx + 3, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
    }
}