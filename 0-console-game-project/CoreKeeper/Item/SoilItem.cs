using System;
using Framework.Engine;

public class SoilItem : Item, IDroppable, IInventoryItem
{
    public override string Name { get; set; } = "Soil";
    public override ConsoleColor Color => ConsoleColor.DarkGreen;

    public int TileX { get; set; }
    public int TileY { get; set; }

    public int Count { get; set; } = 1;
    public int MaxStack => 99;

    private readonly Map _map;

    public SoilItem(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Update(float deltaTime)
    {
        var player = (Player)Scene.FindGameObject("Player");
        if (player == null) return;

        if (player.Position == (TileX, TileY))
        {
            OnPickup(player);
            Scene.RemoveGameObject(this);
        }
    }

    public void OnPickup(Player player)
    {
        // 추후 인벤토리에 추가
    }

    public override void Draw(ScreenBuffer buffer)
    {
        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '┏', Color);
        buffer.SetCell(sx + 2, sy, '┓', Color);
        buffer.SetCell(sx + 1, sy + 1, '┗', Color);
        buffer.SetCell(sx + 2, sy + 1, '┛', Color);
    }

    public override void Use(Player player) { }
}