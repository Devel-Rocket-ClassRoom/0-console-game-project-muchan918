using System;

public enum EquipType { Helmet, Armor, RightHand }

public interface IEquippable
{
    EquipType EquipType { get; }
    ConsoleColor Color { get; }
    void Equip(Player player);
    void Unequip(Player player);
}
