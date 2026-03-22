using System;
using Framework.Engine;

public class StoneHelmet : Item, IInventoryItem, IEquippable, ICraftable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.Helmet;
    public ConsoleColor Color => ConsoleColor.Gray;
    public void Equip(Player player) { player.IncreaseMaxHp(10); }
    public void Unequip(Player player) { player.DecreaseMaxHp(10); }

    // ICraftable
    public (string itemName, int count)[] Recipe => new[] { ("Stone", 4) };

    public StoneHelmet(Scene scene) : base(scene) { Name = "StoneHelmet"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '▄', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▄', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▘', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▝', ConsoleColor.Gray, ConsoleColor.DarkGray);
    }
}