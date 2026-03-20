using System;
using Framework.Engine;

public class Mushroom : Spawner, IDefender
{
    // static으로 전체 수량 관리
    public static int s_MaxCount = 60;
    public static int s_CurrentCount = 0;

    // IDefender - 한 번 맞으면 사라짐
    public int MaxHp { get; private set; } = 1;
    public int Hp { get; private set; } = 1;
    public bool IsAlive => Hp > 0;

    public Mushroom(Scene scene, Map map, int tileX, int tileY)
        : base(scene, map, tileX, tileY)
    {
        Name = "Mushroom";
        s_CurrentCount++;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsInView(buffer)) return;
        var (sx, sy) = GetScreenPos(buffer);

        buffer.SetCell(sx, sy, '░', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy, '▓', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '▓', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy, '░', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx, sy + 1, ' ', ConsoleColor.White, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▐', ConsoleColor.White, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▌', ConsoleColor.White, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy + 1, ' ', ConsoleColor.White, ConsoleColor.Black);
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - amount);
        if (!IsAlive)
        {
            s_CurrentCount--;
            //DropItem();
            Scene.RemoveGameObject(this);
        }
    }

    public override void Update(float deltaTime)
    {
        
    }
}
