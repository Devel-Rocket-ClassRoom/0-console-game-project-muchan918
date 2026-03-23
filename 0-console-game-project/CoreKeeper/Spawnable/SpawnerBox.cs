using System;
using Framework.Engine;

public class SpawnerBox : Spawner, IDefender
{
    public static int s_MaxCount = 50;
    public static int s_CurrentCount = 0;

    private static readonly Random _random = new Random();

    // IDefender
    public int MaxHp { get; private set; } = 5;
    public int Hp { get; private set; } = 5;
    public bool IsAlive => Hp > 0;

    public SpawnerBox(Scene scene, Map map, int tileX, int tileY)
        : base(scene, map, tileX, tileY)
    {
        Name = "SpawnerBox";
        s_CurrentCount++;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsInView(buffer)) return;
        var (sx, sy) = GetScreenPos(buffer);

        buffer.SetCell(sx + 1, sy, '▛', ConsoleColor.Cyan, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '▜', ConsoleColor.Cyan, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▙', ConsoleColor.Cyan, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▟', ConsoleColor.Cyan, ConsoleColor.Black);
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - amount);
        if (!IsAlive)
        {
            s_CurrentCount--;
            DropLegendaryItem();
            Scene.RemoveGameObject(this);
        }
    }

    public override void Update(float deltaTime)
    {

    }

    private void DropLegendaryItem()
    {
        int roll = _random.Next(100);

        // 20% 꽝
        if (roll >= 80) return;

        // 80% 전설 아이템 - 각 16%
        Item item = roll switch
        {
            < 16 => new LegendarySword(Scene, Map, TileX, TileY,
                        _random.Next(15, 41)),
            < 32 => new LegendaryPickaxe(Scene, Map, TileX, TileY,
                        _random.Next(5, 16)),
            < 48 => new LegendaryHelmet(Scene, Map, TileX, TileY,
                        _random.Next(20, 36)),
            < 64 => new LegendaryArmor(Scene, Map, TileX, TileY,
                        _random.Next(30, 51)),
            _ => new LegendaryPotion(Scene, Map, TileX, TileY,
                        _random.Next(30, 61)),
        };

        Scene.AddGameObject(item);
    }
}