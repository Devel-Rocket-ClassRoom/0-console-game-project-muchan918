using System;

public interface IDefender
{
    int MaxHp { get; }
    int Hp { get; }
    bool IsAlive { get; }
    void TakeDamage(int amount);
}