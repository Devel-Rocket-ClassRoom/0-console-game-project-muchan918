using System;
using Framework.Engine;

public class SpawnerBox : Spawner, IDefender, IInteractable
{
    public static int s_MaxCount = 10;
    public static int s_CurrentCount = 0;

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
            Scene.RemoveGameObject(this);
        }
    }

    public void Interact(Player player)
    {
        // 나중에 BoxUI 구현
    }

    public override void Update(float deltaTime)
    {

    }
}