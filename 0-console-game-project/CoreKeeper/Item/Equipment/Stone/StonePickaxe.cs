using System;
using Framework.Engine;

public class StonePickaxe : Item, IInventoryItem, IEquippable, ICraftable, IMotionable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.RightHand;
    public ConsoleColor Color => ConsoleColor.Gray;
    public void Equip(Player player) { player.IncreaseMiningDamage(2); }
    public void Unequip(Player player) { player.DecreaseMiningDamage(2); }
    public string Effect => $"Mining +{2}";

    // ICraftable
    public (string itemName, int count)[] Recipe => new[] { ("Stone", 4), ("Wood", 2) };
    public string EffectDescription => "Mining +2";

    public StonePickaxe(Scene scene) : base(scene) { Name = "StonePickaxe"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public ConsoleColor MotionColor => ConsoleColor.Gray;
    public char GetMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▏', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▕', ConsoleColor.Gray, ConsoleColor.DarkGray);

    }
}