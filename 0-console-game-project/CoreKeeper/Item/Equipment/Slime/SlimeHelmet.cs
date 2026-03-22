using System;
using Framework.Engine;

public class SlimeHelmet : Item, IInventoryItem, IEquippable, ICraftable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.Helmet;
    public ConsoleColor Color => ConsoleColor.Green;
    public void Equip(Player player) { player.IncreaseMaxHp(3); }
    public void Unequip(Player player) { player.DecreaseMaxHp(3); }

    // ICraftable
    public (string itemName, int count)[] Recipe => new[] { ("Slime", 5) };

    public SlimeHelmet(Scene scene) : base(scene) { Name = "SlimeHelmet"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '▄', ConsoleColor.Green, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▄', ConsoleColor.Green, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▘', ConsoleColor.Green, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▝', ConsoleColor.Green, ConsoleColor.DarkGray);
    }
}