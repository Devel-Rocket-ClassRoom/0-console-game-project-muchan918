using System;
using Framework.Engine;

public class BoxUI : GameObject
{
    // 박스 슬롯: (4,3) ~ (16,3) 한 줄 12칸
    private const int k_BoxSlotStartX = 4;
    private const int k_BoxSlotStartY = 3;
    private const int k_BoxSlotCount = 12;

    // 인벤토리 슬롯: (4,5) ~ (16,6) 두 줄 12칸
    private const int k_InvSlotStartX = 4;
    private const int k_InvSlotStartY = 5;
    private const int k_InvSlotCountX = 12;
    private const int k_InvSlotCountY = 2;

    private enum FocusPanel { Box, Inventory }
    private FocusPanel _focus = FocusPanel.Box;

    private int _boxSelectedX = 0;
    private int _invSelectedX = 0;
    private int _invSelectedY = 0;

    // 그랩 - (-1이면 없음)
    private int _grabbedPanel = -1; // 0 = Box, 1 = Inventory
    private int _grabbedX = -1;
    private int _grabbedY = -1;
    private bool IsGrabbing => _grabbedX != -1;

    private string _message = "";
    private float _messageTimer = 0f;
    private const float k_MessageDuration = 2f;

    private Slot[] _boxSlots;
    private readonly Inventory _inventory;

    public bool IsOpen { get; private set; } = false;

    public BoxUI(Scene scene, Inventory inventory) : base(scene)
    {
        Name = "BoxUI";
        IsActive = false;
        _inventory = inventory;
        _boxSlots = new Slot[k_BoxSlotCount];
        for (int i = 0; i < k_BoxSlotCount; i++)
            _boxSlots[i] = new Slot();
    }

    public void Open()
    {
        IsOpen = true;
        IsActive = true;
        _focus = FocusPanel.Box;
        _boxSelectedX = 0;
        _invSelectedX = 0;
        _invSelectedY = 0;
        _grabbedX = -1;
        _grabbedY = -1;
        _grabbedPanel = -1;
        _message = "";
        _messageTimer = 0f;
    }

    public void Close()
    {
        IsOpen = false;
        IsActive = false;
    }

    public override void Update(float deltaTime)
    {
        if (!IsOpen) return;
        if (_messageTimer > 0) _messageTimer -= deltaTime;
        HandleInput();
    }

    private void HandleInput()
    {
        if (_focus == FocusPanel.Box)
            HandleBoxInput();
        else
            HandleInventoryInput();
    }

    private void HandleBoxInput()
    {
        if (Input.IsKeyDown(ConsoleKey.A))
            _boxSelectedX = (_boxSelectedX - 1 + k_BoxSlotCount) % k_BoxSlotCount;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _boxSelectedX = (_boxSelectedX + 1) % k_BoxSlotCount;
        else if (Input.IsKeyDown(ConsoleKey.S))
        {
            // 박스 → 인벤토리로 포커스 이동
            _focus = FocusPanel.Inventory;
            _invSelectedX = _boxSelectedX < k_InvSlotCountX ? _boxSelectedX : 0;
            _invSelectedY = 0;
        }

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
            HandleGrab(0, _boxSelectedX, 0);
    }

    private void HandleInventoryInput()
    {
        if (Input.IsKeyDown(ConsoleKey.A))
            _invSelectedX = (_invSelectedX - 1 + k_InvSlotCountX) % k_InvSlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _invSelectedX = (_invSelectedX + 1) % k_InvSlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.W))
        {
            if (_invSelectedY == 0)
            {
                // 인벤토리 → 박스로 포커스 이동
                _focus = FocusPanel.Box;
                _boxSelectedX = _invSelectedX < k_BoxSlotCount ? _invSelectedX : 0;
            }
            else
                _invSelectedY--;
        }
        else if (Input.IsKeyDown(ConsoleKey.S))
            _invSelectedY = (_invSelectedY + 1) % k_InvSlotCountY;

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
            HandleGrab(1, _invSelectedX, _invSelectedY);
    }

    private void HandleGrab(int panel, int x, int y)
    {
        if (!IsGrabbing)
        {
            _grabbedPanel = panel;
            _grabbedX = x;
            _grabbedY = y;
            return;
        }

        // 같은 슬롯이면 취소
        if (_grabbedPanel == panel && _grabbedX == x && _grabbedY == y)
        {
            _grabbedX = -1;
            _grabbedY = -1;
            _grabbedPanel = -1;
            return;
        }

        // 슬롯 가져오기
        Slot? from = GetSlot(_grabbedPanel, _grabbedX, _grabbedY);
        Slot? to = GetSlot(panel, x, y);

        if (from == null || to == null) return;

        // 교환
        var tmp = from.Item;
        from.Item = to.Item;
        to.Item = tmp;

        _grabbedX = -1;
        _grabbedY = -1;
        _grabbedPanel = -1;
    }

    private Slot? GetSlot(int panel, int x, int y)
    {
        if (panel == 0) // Box
            return x < k_BoxSlotCount ? _boxSlots[x] : null;
        else            // Inventory
            return _inventory.GetSlot(y, x);
    }

    private void ShowMessage(string message)
    {
        _message = message;
        _messageTimer = k_MessageDuration;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsOpen) return;

        // 전체 배경
        for (int ty = 2; ty < 8; ty++)
            for (int tx = 4; tx < 16; tx++)
                DrawTile(buffer, tx, ty, ' ', ConsoleColor.White, ConsoleColor.DarkGray);

        // 박스 슬롯 한 줄
        for (int x = 0; x < k_BoxSlotCount; x++)
        {
            bool selected = (_focus == FocusPanel.Box && x == _boxSelectedX);
            bool isGrabbed = (IsGrabbing && _grabbedPanel == 0 && _grabbedX == x);
            _boxSlots[x].Draw(buffer, k_BoxSlotStartX + x, k_BoxSlotStartY, selected, false, isGrabbed);
        }

        // 인벤토리 슬롯 두 줄
        for (int y = 0; y < k_InvSlotCountY; y++)
            for (int x = 0; x < k_InvSlotCountX; x++)
            {
                bool selected = (_focus == FocusPanel.Inventory && x == _invSelectedX && y == _invSelectedY);
                bool isQuick = (y == 0 && x < 6);
                bool isGrabbed = (IsGrabbing && _grabbedPanel == 1 && _grabbedX == x && _grabbedY == y);
                _inventory.GetSlot(y, x).Draw(buffer, k_InvSlotStartX + x, k_InvSlotStartY + y, selected, isQuick, isGrabbed);
            }

        // 메시지
        if (_messageTimer > 0)
            buffer.WriteTextCentered(7 * 2 + 1, _message, ConsoleColor.Red, ConsoleColor.DarkGray);
        else
        {
            // 포커스된 슬롯 아이템 이름 표시
            Slot? slot = _focus == FocusPanel.Box
                ? _boxSlots[_boxSelectedX]
                : _inventory.GetSlot(_invSelectedY, _invSelectedX);

            if (slot != null && !slot.IsEmpty && slot.Item is IInventoryItem inv)
                buffer.WriteTextCentered(7 * 2 + 1,
                    $"{slot.Item!.Name} x{inv.Count}", ConsoleColor.Yellow, ConsoleColor.DarkGray);
        }
    }

    private void DrawTile(ScreenBuffer buffer, int tx, int ty, char ch, ConsoleColor fg, ConsoleColor bg)
    {
        int sx = tx * 4;
        int sy = ty * 2;
        for (int dy = 0; dy < 2; dy++)
            for (int dx = 0; dx < 4; dx++)
                buffer.SetCell(sx + dx, sy + dy, ch, fg, bg);
    }
}