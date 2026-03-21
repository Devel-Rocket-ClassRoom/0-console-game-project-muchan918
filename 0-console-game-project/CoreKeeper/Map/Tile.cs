using System;
using Framework.Engine;

public enum TileType
{
    Ground,
    Wood,
    Soil,
    Stone
}

public class Tile
{
    private TileType _type;
    private int _hp;

    public TileType Type => _type;
    public int Hp => _hp;

    public bool IsMovable => Type == TileType.Ground;
    public bool IsMinable => Type != TileType.Ground;
    public bool IsAlive => _hp > 0;

    public Tile(TileType type)
    {
        _type = type;
        _hp = _type switch
        {
            TileType.Wood => 2,
            TileType.Soil => 3,
            TileType.Stone => 5,
            _ => 0
        };
    }

    public Tile(TileType type, int hp)
    {
        _type = type;
        _hp = hp;
    }

    public char DisplayChar => Type switch
    {
        TileType.Wood => '▨',
        TileType.Soil => '▩',
        TileType.Stone => '▦',
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