using System;
using Framework.Engine;

public class LegendaryHelmet : Item, IInventoryItem, IEquippable, IDroppable
{
    private readonly int _hpBonus;
    private readonly Map _map;

    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    public int TileX { get; set; }
    public int TileY { get; set; }

    public EquipType EquipType => EquipType.Helmet;
    public ConsoleColor Color => ConsoleColor.DarkRed;
    public void Equip(Player player) => player.IncreaseMaxHp(_hpBonus);
    public void Unequip(Player player) => player.DecreaseMaxHp(_hpBonus);
    public string Effect => $"MaxHp +{_hpBonus}";

    public LegendaryHelmet(Scene scene, Map map, int tileX, int tileY, int hpBonus) : base(scene)
    {
        Name = "Phantom Helmet";
        _map = map;
        TileX = tileX;
        TileY = tileY;
        _hpBonus = hpBonus;
    }

    public override void Update(float deltaTime)
    {
        var player = Scene.FindGameObject("Player") as Player;
        if (player == null) return;
        if (player.Position == (TileX, TileY))
            OnPickup(player);
    }

    public void OnPickup(Player player)
    {
        player.Inventory.AddItem(this);
        Scene.RemoveGameObject(this);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, ' ', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '♛', ConsoleColor.Magenta, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, ' ', ConsoleColor.Magenta, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4; int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, ' ', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '♛', ConsoleColor.Magenta, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, ' ', ConsoleColor.Magenta, ConsoleColor.DarkGray);
    }

    public override void Use(Player player) { }
}