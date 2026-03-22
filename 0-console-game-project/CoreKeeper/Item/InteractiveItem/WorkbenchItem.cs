using System;
using Framework.Engine;

public class WorkbenchItem : Item, IInventoryItem, ICraftable, IDroppable, IInstallable, IInteractable
{
    private Map? _map;

    // IInventoryItem
    public int Count { get; set; } = 1;
    public int MaxStack => 1;

    // ICraftable
    public (string itemName, int count)[] Recipe => new[]
    {
        ("Wood", 5)
    };
    public string EffectDescription => "Allows crafting of advanced items";

    // IDroppable
    public int TileX { get; set; }
    public int TileY { get; set; }

    public WorkbenchItem(Scene scene) : base(scene)
    {
        Name = "Workbench";
    }

    public WorkbenchItem(Scene scene, Map map, int tileX, int tileY) : base(scene)
    {
        Name = "Workbench";
        _map = map;
        TileX = tileX;
        TileY = tileY;
    }

    public override void Update(float deltaTime)
    {
        if (_map == null) return;

        var player = (Player)Scene.FindGameObject("Player");
        if (player == null) return;

        if (player.Position == (TileX, TileY))
            OnPickup(player);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (_map == null) return;

        var (sx, sy) = _map.TileToScreen(TileX, TileY, buffer);
        if (sx < 0 || sy < 0 || sx + 1 >= buffer.Width || sy + 1 >= buffer.Height) return;

        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.DarkYellow, ConsoleColor.Black);
    }

    public override void Use(Player player)
    {
        var (tx, ty) = player.GetFrontTile();
        var map = player.GetMap();

        // 설치 시 map과 좌표를 가진 새 인스턴스 생성해서 타일에 저장
        var installed = new WorkbenchItem(Scene, map, tx, ty);
        map.SetTile(tx, ty, TileType.Object, installed);
        Scene.AddGameObject(installed);
        ConsumeFromInventory(player);
    }

    public override void DrawIcon(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.DarkYellow, ConsoleColor.DarkGray);
    }

    public void OnPickup(Player player)
    {
        player.Inventory.AddItem(this);
        Scene.RemoveGameObject(this);
    }

    public void DrawInstalled(int tx, int ty, ScreenBuffer buffer)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        buffer.SetCell(sx + 1, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '_', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▝', ConsoleColor.DarkYellow, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▘', ConsoleColor.DarkYellow, ConsoleColor.Black);
    }

    public void Interact(Player player)
    {
        var workbenchUI = player.Scene.FindGameObject("WorkbenchUI") as WorkbenchUI;
        if (workbenchUI == null) return;
        workbenchUI.Open();
    }
}