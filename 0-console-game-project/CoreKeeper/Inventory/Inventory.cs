using System;
using Framework.Engine;

public class Inventory : GameObject
{
    // 창 영역 (뷰 타일 좌표 기준)
    private const int k_StartTileX = 4;
    private const int k_StartTileY = 2;
    private const int k_EndTileX = 16;
    private const int k_EndTileY = 8;

    // 슬롯: 창 하단 2줄 12개
    private const int k_SlotStartX = 4;
    private const int k_SlotStartY = 5;
    private const int k_SlotCountX = 12;
    private const int k_SlotCountY = 2;
    private const int k_QuickSlotCount = 6; // 0~5번은 QuickSlot 영역

    // 인벤토리 선택 좌표
    private int _selectedX = 0;
    private int _selectedY = 0;

    public bool IsOpen { get; private set; } = false;
    public int SelectedX => _selectedX;

    private Slot[,] _slots;

    public Slot GetSlot(int y, int x) => _slots[y, x];

    public void Toggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            _selectedX = 0;
            _selectedY = 0;
        }
    }

    public Inventory(Scene scene) : base(scene)
    {
        Name = "Inventory";
        _slots = new Slot[k_SlotCountY, k_SlotCountX];
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x] = new Slot();
    }

    public override void Update(float deltaTime)
    {
        if (!IsOpen) return;

        if (Input.IsKeyDown(ConsoleKey.A)) _selectedX = (_selectedX - 1 + k_SlotCountX) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.D)) _selectedX = (_selectedX + 1) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.W)) _selectedY = (_selectedY - 1 + k_SlotCountY) % k_SlotCountY;
        else if (Input.IsKeyDown(ConsoleKey.S)) _selectedY = (_selectedY + 1) % k_SlotCountY;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsOpen) return;

        // 창 영역 전체 타일 기준으로 회색 채우기
        for (int ty = k_StartTileY; ty < k_EndTileY; ty++)
            for (int tx = k_StartTileX; tx < k_EndTileX; tx++)
                DrawTile(buffer, tx, ty, ' ', ConsoleColor.White, ConsoleColor.DarkGray);

        // 슬롯 (4,6) ~ (16,8)
        for (int y = 0; y < k_SlotCountY; y++)
        {
            for (int x = 0; x < k_SlotCountX; x++)
            {
                bool selected = (x == _selectedX && y == _selectedY);
                _slots[y, x].Draw(buffer, k_SlotStartX + x, k_SlotStartY + y, selected);
            }
        }

        var selectedItem = _slots[_selectedY, _selectedX];
        if (!selectedItem.IsEmpty && selectedItem.Item is IInventoryItem inv)
            buffer.WriteTextCentered(7 * 2 + 1,
                $"{selectedItem.Item!.Name} x{inv.Count}", ConsoleColor.White, ConsoleColor.DarkGray);
    }

    // 맵과 동일한 방식 - 타일 1개를 스크린 4×2로 그리기
    private void DrawTile(ScreenBuffer buffer, int tx, int ty, char ch, ConsoleColor fg, ConsoleColor bg)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        for (int dy = 0; dy < 2; dy++)
            for (int dx = 0; dx < 4; dx++)
                buffer.SetCell(sx + dx, sy + dy, ch, fg, bg);
    }

    public void AddItem(Item item)
    {
        // 1단계: 같은 이름 슬롯에 겹치기 시도
        if (item is IInventoryItem newInv)
            for (int y = 0; y < k_SlotCountY; y++)
                for (int x = 0; x < k_SlotCountX; x++)
                {
                    var slot = _slots[y, x];
                    if (!slot.IsEmpty &&
                        slot.Item!.Name == item.Name &&
                        slot.Item is IInventoryItem existing &&
                        existing.Count < existing.MaxStack)
                    {
                        existing.Count++;
                        return;
                    }
                }

        // 2단계: 빈 슬롯에 추가
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                if (_slots[y, x].IsEmpty)
                {
                    _slots[y, x].SetItem(item);
                    return;
                }

        // 인벤토리 가득 참 (추후 처리)
    }
}