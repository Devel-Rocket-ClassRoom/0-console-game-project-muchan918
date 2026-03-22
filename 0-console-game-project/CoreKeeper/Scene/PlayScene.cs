using Framework.Engine;
using System;

public class PlayScene : Scene
{
    private Map map;
    private Player player;
    private QuickSlot quickSlot;
    private SpawnManager spawnManager;
    private HpBar hpBar;
    private WorkbenchUI workbenchUI;
    private BoxUI boxUI;

    public event Action? GameOverRequested;
    public event Action? GameClearRequested;

    public void RequestGameOver() => GameOverRequested?.Invoke();
    public void RequestGameClear() => GameClearRequested?.Invoke();

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        if (workbenchUI.IsOpen)
            workbenchUI.Draw(buffer);
        else if (boxUI.IsOpen)
            boxUI.Draw(buffer);
        else
            player.Inventory.Draw(buffer);

        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Yellow);
    }

    public override void Load()
    {
        Slime.s_CurrentCount = 0;
        Mushroom.s_CurrentCount = 0;
        SpawnerBox.s_CurrentCount = 0;
        Boss.s_CurrentCount = 0;

        map = new Map(this);
        AddGameObject(map);

        player = new Player(this, map, 100, 50);
        AddGameObject(player);

        quickSlot = new QuickSlot(this, player.Inventory);
        AddGameObject(quickSlot);
        player.SetQuickSlot(quickSlot);

        spawnManager = new SpawnManager(this, map);
        AddGameObject(spawnManager);

        spawnManager.Register<Slime>();
        spawnManager.Register<Mushroom>();
        spawnManager.Register<SpawnerBox>();
        spawnManager.SpawnAll();

        hpBar = new HpBar(this, player);
        AddGameObject(hpBar);

        workbenchUI = new WorkbenchUI(this, player.Inventory);
        AddGameObject(workbenchUI);
        player.SetWorkbenchUI(workbenchUI);

        boxUI = new BoxUI(this, player.Inventory);
        AddGameObject(boxUI);
        player.SetBoxUI(boxUI);

        var boss = new Boss(this, map, map.BossSpawnPoint.X, map.BossSpawnPoint.Y);
        AddGameObject(boss);
    }

    public override void Unload()
    {

    }

    public override void Update(float deltaTime)
    {
        // Tab 처리 (PlayScene에서 관리)
        if (Input.IsKeyDown(ConsoleKey.Tab))
        {
            if (!workbenchUI.IsOpen && !boxUI.IsOpen)  // 추가
            {
                player.Inventory.Toggle();
                quickSlot.IsActive = !player.Inventory.IsOpen;
            }
            return;
        }

        if (!player.IsAlive)
            RequestGameOver();

        UpdateGameObjects(deltaTime);
        map.SetViewPosition(player.Position.X, player.Position.Y);
    }

    public IDefender? FindDefender(int tileX, int tileY)
    {
        var spawner = GetGameObjects<Spawner>()
        .FirstOrDefault(s => s.TileX == tileX && s.TileY == tileY) as IDefender;
        if (spawner != null) return spawner;

        // 보스 3x3 범위 체크
        return GetGameObjects<Boss>()
                .FirstOrDefault(b =>
                    tileX >= b.TileX - 1 && tileX < b.TileX + 4 &&
                    tileY >= b.TileY - 1 && tileY < b.TileY + 4) as IDefender;
    }
}