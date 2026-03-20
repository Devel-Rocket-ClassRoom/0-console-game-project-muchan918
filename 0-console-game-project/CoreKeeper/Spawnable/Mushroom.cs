using System;
using Framework.Engine;

public class Mushroom : Spawner, IDefender
{
    public static int s_MaxCount = 15;
    public static int s_CurrentCount = 0;

    public int MaxHp => throw new NotImplementedException();

    public int Hp => throw new NotImplementedException();

    public bool IsAlive => throw new NotImplementedException();

    public Mushroom(Scene scene, Map map, int tileX, int tileY) : base(scene, map, tileX, tileY)
    {

    }

    public override void Draw(ScreenBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        throw new NotImplementedException();
    }

    public override void Update(float deltaTime)
    {
        throw new NotImplementedException();
    }
}
