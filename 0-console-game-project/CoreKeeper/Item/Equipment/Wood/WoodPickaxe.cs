using System;
using Framework.Engine;

public class WoodPickaxe : Item, IInventoryItem, IEquippable, ICraftable, IMotionable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.RightHand;
    public void Equip(Player player) { player.IncreaseMiningDamage(1); }
    public void Unequip(Player player) { player.DecreaseMiningDamage(1); }
    public string Effect => $"Mining +{1}";

    // ICraftable
    public (string itemName, int count)[] Recipe => new[]
    {
        ("Wood", 3)
    };
    public string EffectDescription => "Mining +1";

    public WoodPickaxe(Scene scene) : base(scene)
    {
        Name = "WoodPickaxe";
    }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }
    public ConsoleColor Color => ConsoleColor.DarkYellow;

    public ConsoleColor MotionColor => ConsoleColor.Cyan;

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▏', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▕', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        
    }

    public char GetMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };
}