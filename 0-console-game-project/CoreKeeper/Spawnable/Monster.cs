using System;
using Framework.Engine;

public class Monster : Spawner, IAttacker, IDefender
{
    // static으로 전체 수량 관리
    public static int s_MaxCount = 100;
    public static int s_CurrentCount = 0;

    public Monster(Scene scene, Map map, int tileX, int tileY)
        : base(scene, map, tileX, tileY)
    {
        Name = "Monster";
        s_CurrentCount++;
    }

    public int AttackDamage { get; private set; } = 3;

    public int MaxHp { get; private set; } = 10;
    public int Hp { get; private set; } = 10;
    public bool IsAlive => Hp > 0;

    public void Attack(IDefender target)
    {
        target.TakeDamage(AttackDamage);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsInView(buffer)) return;
        var (sx, sy) = GetScreenPos(buffer);

        buffer.SetCell(sx + 1, sy, '◢', ConsoleColor.Yellow, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '◣', ConsoleColor.Yellow, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.Cyan, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▝', ConsoleColor.Cyan, ConsoleColor.Black);
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

    public override void Update(float deltaTime)
    {

    }
}