using System;
using Framework.Engine;

public class LegendaryPickaxe : Item, IInventoryItem, IEquippable, IDroppable, IMotionable
{
    private readonly int _miningBonus;
    private readonly Map _map;

    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    public int TileX { get; set; }
    public int TileY { get; set; }

    public EquipType EquipType => EquipType.RightHand;
    public ConsoleColor Color => ConsoleColor.DarkRed;
    public void Equip(Player player) => player.IncreaseMiningDamage(_miningBonus);
    public void Unequip(Player player) => player.DecreaseMiningDamage(_miningBonus);
    public string Effect => $"Mining +{_miningBonus}";

    public ConsoleColor MotionColor => ConsoleColor.DarkRed;
    public char GetMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };

    public LegendaryPickaxe(Scene scene, Map map, int tileX, int tileY, int miningBonus) : base(scene)
    {
        Name = "Titan Pickaxe";
        _map = map;
        TileX = tileX;
        TileY = tileY;
        _miningBonus = miningBonus;
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

        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Green, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '⛏', ConsoleColor.Red, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4; int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Green, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '⛏', ConsoleColor.Red, ConsoleColor.DarkGray);
    }

    public override void Use(Player player) { }
}