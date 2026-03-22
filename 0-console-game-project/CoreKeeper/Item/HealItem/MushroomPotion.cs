using System;
using Framework.Engine;

public class MushroomPotion : Item, IInventoryItem, ICraftable, IHealable
{
    public int Count { get; set; } = 1;
    public int MaxStack => 10;

    private const int k_HealAmount = 15;
    public int HealAmount => k_HealAmount;

    public (string itemName, int count)[] Recipe => new[] { ("Mushroom", 2), ("Wood", 1) };
    public string EffectDescription => "Instantly heals +15 HP";

    public MushroomPotion(Scene scene) : base(scene) { Name = "MushroomPotion"; }

    public override void Update(float deltaTime) { }
    public override void Draw(ScreenBuffer buffer) { }

    public override void Use(Player player)
    {
        if (player.Hp >= player.MaxHp) return;
        player.Heal(k_HealAmount);
        ConsumeFromInventory(player);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4; int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '▕', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '▏', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.Magenta, ConsoleColor.DarkGray);
    }
}