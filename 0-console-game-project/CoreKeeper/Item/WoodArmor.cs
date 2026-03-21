using System;
using Framework.Engine;

public class WoodArmor : Item, IInventoryItem, IEquippable, ICraftable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.Armor;
    public void Equip(Player player) { }
    public void Unequip(Player player) { }
    public ConsoleColor Color => ConsoleColor.DarkYellow;

    // ICraftable
    public (string itemName, int count)[] Recipe => new[]
    {
        ("Wood", 4)
    };

    public WoodArmor(Scene scene) : base(scene)
    {
        Name = "WoodArmor";
    }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '▖', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▗', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
    }
}