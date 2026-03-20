using System;
using Framework.Engine;

public class QuickSlot : GameObject
{
    private Inventory _inventory;
    private int _selectedIndex = 0;
    private const int k_SlotCount = 6;

    // 화면 하단 중앙 고정 위치 (타일 좌표)
    // 화면 타일 기준 20×10, 하단 y=9, 중앙 x = (20-6)/2 = 7
    private const int k_StartTileX = 7;
    private const int k_StartTileY = 8;

    public int SelectedIndex => _selectedIndex;

    public QuickSlot(Scene scene, Inventory inventory) : base(scene)
    {
        Name = "QuickSlot";
        _inventory = inventory;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        for (int i = 0; i < k_SlotCount; i++)
        {
            bool selected = (i == _selectedIndex);
            _inventory.GetSlot(0, i).Draw(buffer, k_StartTileX + i, k_StartTileY, selected);
        }
    }

    public override void Update(float deltaTime)
    {
        // 인벤토리 닫혀있으면 1~6키로만 선택
        if (!_inventory.IsOpen)
        {
            if (Input.IsKeyDown(ConsoleKey.D1)) _selectedIndex = 0;
            else if (Input.IsKeyDown(ConsoleKey.D2)) _selectedIndex = 1;
            else if (Input.IsKeyDown(ConsoleKey.D3)) _selectedIndex = 2;
            else if (Input.IsKeyDown(ConsoleKey.D4)) _selectedIndex = 3;
            else if (Input.IsKeyDown(ConsoleKey.D5)) _selectedIndex = 4;
            else if (Input.IsKeyDown(ConsoleKey.D6)) _selectedIndex = 5;
            return;
        }
    }

    public Slot GetSelectedSlot() => _inventory.GetSlot(0, _selectedIndex);
}