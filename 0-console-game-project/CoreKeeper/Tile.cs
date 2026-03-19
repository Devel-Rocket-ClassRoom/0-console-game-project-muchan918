using System;
using Framework.Engine;

public enum TileType
{
    Ground,
    Wood,
    Soil,
}

public struct Tile
{
    private TileType _type;

    public TileType Type => _type;

    public readonly bool IsMovable => Type == TileType.Ground;
    public readonly bool IsMinable => Type == TileType.Wood || Type == TileType.Soil;

    public Tile(TileType type)
    {
        _type = type;
    }

    public readonly char DisplayChar => Type switch
    {
        TileType.Wood => '▨',
        TileType.Soil => '▩',
        _ => ' ',
    };

    public readonly ConsoleColor ForeColor => Type switch
    {
        TileType.Wood => ConsoleColor.DarkYellow,
        TileType.Soil => ConsoleColor.DarkGreen,
        _ => ConsoleColor.DarkGray,
    };
}