using System;
using Framework.Engine;

public class StoneSword : Item, IInventoryItem, IEquippable, ICraftable, IMotionable
{
    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IEquippable
    public EquipType EquipType => EquipType.RightHand;
    public ConsoleColor Color => ConsoleColor.Gray;
    public void Equip(Player player) { player.IncreaseAttackDamage(8); }
    public void Unequip(Player player) { player.DecreaseAttackDamage(8); }

    // ICraftable
    public (string itemName, int count)[] Recipe => new[] { ("Stone", 3) };

    public ConsoleColor MotionColor => ConsoleColor.Gray;
    public char GetMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };

    public StoneSword(Scene scene) : base(scene) { Name = "StoneSword"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }
    public override void Use(Player player) { }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        buffer.SetCell(sx + 2, sy, '/', ConsoleColor.Gray, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '☌', ConsoleColor.Gray, ConsoleColor.DarkGray);
    }
}