using Framework.Engine;
using System;
using System.Numerics;

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

    // 인벤토리 - 제작 - 장비 focus 변수
    private enum FocusPanel { Inventory, Craft, Equipment }
    private FocusPanel _focus = FocusPanel.Inventory;

    // 인벤토리 선택 좌표
    private int _selectedX = 0;
    private int _selectedY = 0;
    private bool _isFocused = false;

    // 아이템 교환용 - 잡힌 슬롯 좌표 (-1이면 없음)
    private int _grabbedX = -1;
    private int _grabbedY = -1;
    private bool IsGrabbing => _grabbedX != -1;

    public bool IsOpen { get; private set; } = false;
    public int SelectedX => _selectedX;

    private Slot[,] _slots;

    public PlayerCraft Craft { get; private set; }
    public PlayerEquipment Equipment { get; private set; }

    public Slot GetSlot(int y, int x) => _slots[y, x];

    public void Toggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            _selectedX = 0;
            _selectedY = 0;
            _focus = FocusPanel.Inventory;
            _isFocused = true;
            _grabbedX = -1;
            _grabbedY = -1;
        }
    }

    public Inventory(Scene scene, Player player) : base(scene)
    {
        Name = "Inventory";
        _slots = new Slot[k_SlotCountY, k_SlotCountX];
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x] = new Slot();

        Craft = new PlayerCraft(scene);
        Equipment = new PlayerEquipment(scene, player);
    }

    public override void Update(float deltaTime)
    {
        if (!IsOpen) return;

        switch (_focus)
        {
            case FocusPanel.Inventory: 
                UpdateInventory(); 
                break;
            case FocusPanel.Craft: 
                Craft.HandleInput(this);
                _isFocused = false;
                break;
            case FocusPanel.Equipment: 
                Equipment.HandleInput(this);
                _isFocused = false;
                break;
        }
    }

    private void UpdateInventory()
    {
        _isFocused = true;

        if (Input.IsKeyDown(ConsoleKey.A))
            _selectedX = (_selectedX - 1 + k_SlotCountX) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _selectedX = (_selectedX + 1) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.S))
        {
            if (_selectedY == 7)
            {
                if (_selectedX < 10)
                {
                    _focus = FocusPanel.Craft;
                    Craft.SetSelected(0, 0);
                }
                else
                {
                    _focus = FocusPanel.Equipment;
                    Equipment.SetSelected(0);
                }
            }
            _selectedY = (_selectedY + 1) % k_SlotCountY;
        }
        else if (Input.IsKeyDown(ConsoleKey.W))
        {
            if (_selectedY > 0)
            {
                _selectedY--;
            }
            else
            {
                if (_selectedX < 6)
                {
                    _focus = FocusPanel.Craft;
                    Craft.SetSelected(0, 0);
                }
                else
                {
                    _focus = FocusPanel.Equipment;
                    Equipment.SetSelected(0);
                }
            }
        }

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
        {
            HandleSwap();
        }
    }

    private void HandleSwap()
    {
        if (!IsGrabbing)
        {
            _grabbedX = _selectedX;
            _grabbedY = _selectedY;
            return;
        }

        if (_grabbedX == _selectedX && _grabbedY == _selectedY)
        {
            // 같은 슬롯 → 취소
            _grabbedX = -1;
            _grabbedY = -1;
            return;
        }

        // 교환
        var tmp = _slots[_grabbedY, _grabbedX].Item;
        _slots[_grabbedY, _grabbedX].Item = _slots[_selectedY, _selectedX].Item;
        _slots[_selectedY, _selectedX].Item = tmp;

        _grabbedX = -1;
        _grabbedY = -1;

    }

    public void ReturnToInventory()
    {
        _focus = FocusPanel.Inventory;
        _selectedX = 0;
        _selectedY = 0;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsOpen) return;

        // 창 영역 전체 타일 기준으로 회색 채우기
        for (int ty = k_StartTileY; ty < k_EndTileY; ty++)
            for (int tx = k_StartTileX; tx < k_EndTileX; tx++)
                DrawTile(buffer, tx, ty, ' ', ConsoleColor.White, ConsoleColor.DarkGray);

        // 하위 패널 출력
        Craft.Draw(buffer);
        Equipment.Draw(buffer);

        // 슬롯 (4,6) ~ (16,7)
        for (int y = 0; y < k_SlotCountY; y++)
        {
            for (int x = 0; x < k_SlotCountX; x++)
            {
                bool selected = (x == _selectedX && y == _selectedY && _isFocused);
                bool isQuick = (y == 0 && x >= 0 && x < 6);
                bool isGrabbed = (x == _grabbedX && y == _grabbedY);
                _slots[y, x].Draw(buffer, k_SlotStartX + x, k_SlotStartY + y, selected, isQuick, isGrabbed);
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