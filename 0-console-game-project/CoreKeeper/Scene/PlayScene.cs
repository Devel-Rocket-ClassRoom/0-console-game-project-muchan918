using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Map map;
    private Player player;
    private QuickSlot quickSlot;

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);
        player.Inventory.Draw(buffer);
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Yellow);
    }

    public override void Load()
    {
        map = new Map(this);
        AddGameObject(map);
        
        player = new Player(this, map, 100, 50);
        AddGameObject(player);

        quickSlot = new QuickSlot(this, player.Inventory);
        AddGameObject(quickSlot);
    }

    public override void Unload()
    {
        
    }

    public override void Update(float deltaTime)
    {
        // Tab 처리 (PlayScene에서 관리)
        if (Input.IsKeyDown(ConsoleKey.Tab))
        {
            player.Inventory.Toggle();
            quickSlot.IsActive = !player.Inventory.IsOpen; // 인벤토리 열리면 QuickSlot 숨김
            return;
        }

        UpdateGameObjects(deltaTime);
        map.SetViewPosition(player.Position.X, player.Position.Y);
    }
}