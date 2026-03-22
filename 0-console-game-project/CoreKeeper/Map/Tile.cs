using System;
using Framework.Engine;

public enum TileType
{
    Ground,
    Wood,
    Soil,
    Stone,
    Object
}

public class Tile
{
    private TileType _type;
    private int _hp;

    public TileType Type => _type;
    public int Hp => _hp;
    public Item? InstalledItem { get; private set; }

    public bool IsMovable => Type == TileType.Ground;
    public bool IsMinable => Type != TileType.Ground;
    public bool IsAlive => _hp > 0;

    public Tile(TileType type, Item? installedItem = null)
    {
        _type = type;
        InstalledItem = installedItem;
        _hp = _type switch
        {
            TileType.Wood => 2,
            TileType.Soil => 3,
            TileType.Stone => 5,
            TileType.Object => 3,
            _ => 0
        };
    }

    public Tile(TileType type, int hp, Item? installedItem = null)
    {
        _type = type;
        _hp = hp;
        InstalledItem = installedItem;
    }

    public char DisplayChar => Type switch
    {
        TileType.Wood => '▨',
        TileType.Soil => '▩',
        TileType.Stone => '▦',
        TileType.Object => ' ',
        _ => ' ',
    };

    public ConsoleColor ForeColor => Type switch
    {
        TileType.Wood => ConsoleColor.DarkYellow,
        TileType.Soil => ConsoleColor.DarkGreen,
        TileType.Stone => ConsoleColor.Gray,
        _ => ConsoleColor.DarkGray,
    };
}