using System;
using Framework.Engine;

public class StoneArmor : Item, IInventoryItem, IEquippable, ICraftable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.Armor;
    public ConsoleColor Color => ConsoleColor.Gray;
    public void Equip(Player player) { player.IncreaseMaxHp(15); }
    public void Unequip(Player player) { player.DecreaseMaxHp(15); }
    public string Effect => $"MaxHp +{15}";

    // ICraftable
    public (string itemName, int count)[] Recipe => new[] { ("Stone", 8) };
    public string EffectDescription => "MaxHp +15";

    public StoneArmor(Scene scene) : base(scene) { Name = "StoneArmor"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '▖', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▗', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.Gray, ConsoleColor.DarkGray);
    }
}