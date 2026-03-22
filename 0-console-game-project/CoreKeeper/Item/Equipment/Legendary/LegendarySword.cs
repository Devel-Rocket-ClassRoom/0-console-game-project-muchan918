using System;
using Framework.Engine;

public class LegendarySword : Item, IInventoryItem, IEquippable, IDroppable, IMotionable
{
    private readonly int _attackBonus;
    private readonly Map _map;

    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // IDroppable
    public int TileX { get; set; }
    public int TileY { get; set; }

    // IEquippable
    public EquipType EquipType => EquipType.RightHand;
    public ConsoleColor Color => ConsoleColor.DarkRed;
    public void Equip(Player player) => player.IncreaseAttackDamage(_attackBonus);
    public void Unequip(Player player) => player.DecreaseAttackDamage(_attackBonus);
    public string Effect => $"Attack +{_attackBonus}";

    // IMotionable
    public ConsoleColor MotionColor => ConsoleColor.DarkRed;
    public char GetMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };

    public LegendarySword(Scene scene, Map map, int tileX, int tileY, int attackBonus) : base(scene)
    {
        Name = "Blood Sword";
        _map = map;
        TileX = tileX;
        TileY = tileY;
        _attackBonus = attackBonus;
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

        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Red, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '⚔', ConsoleColor.Red, ConsoleColor.Black);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4; int sy = ty * 2;
        buffer.SetCell(sx + 1, sy, '★', ConsoleColor.Red, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '⚔', ConsoleColor.Red, ConsoleColor.DarkGray);
    }

    public override void Use(Player player) { }
}