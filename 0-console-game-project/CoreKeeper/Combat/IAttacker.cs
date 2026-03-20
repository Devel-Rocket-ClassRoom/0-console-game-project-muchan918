using System;

public interface IAttacker
{
    int AttackDamage {  get; }
    void Attack(IDefender target);
}