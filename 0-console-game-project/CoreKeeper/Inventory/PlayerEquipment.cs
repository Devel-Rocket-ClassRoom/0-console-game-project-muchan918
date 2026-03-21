using System;
using Framework.Engine;

public class PlayerEquipment : GameObject
{
    public enum EquipSlot { Helmet, Armor, RightHand }
    private const int k_SlotCount = 3;

    private static readonly (int tx, int ty)[] k_SlotPos =
    {
        (12, 2), // Helmet
        (11, 3), // Armor
        (13, 3), // RightHand
    };

    // 각 슬롯에 대응하는 EquipType
    private static readonly EquipType[] k_SlotTypes =
    {
        EquipType.Helmet,
        EquipType.Armor,
        EquipType.RightHand,
    };

    private int _selectedIndex = 0;
    private bool _isFocused = false;

    // 메시지 출력 타이머
    private string _message = "";
    private float _messageTimer = 0f;
    private const float k_MessageDuration = 2f;

    private Slot[] _slots;
    private readonly Player _player;

    public PlayerEquipment(Scene scene, Player player) : base(scene)
    {
        Name = "PlayerEquipment";
        _slots = new Slot[k_SlotCount];
        for (int i = 0; i < k_SlotCount; i++)
            _slots[i] = new Slot();

        _player = player;
    }

    public Slot GetSlot(EquipSlot slot) => _slots[(int)slot];
    public EquipType GetSlotType() => k_SlotTypes[_selectedIndex];
    public int SelectedIndex => _selectedIndex;

    public void SetSelected(int index)
    {
        _selectedIndex = Math.Clamp(index, 0, k_SlotCount - 1);
        _isFocused = true;
    }

    public void ShowMessage(string message)
    {
        _message = message;
        _messageTimer = k_MessageDuration;
    }

    // 장착 처리 - Inventory에서 아이템 선택 후 호출
    public void TryEquip(Item item)
    {
        if (item is not IEquippable equippable)
        {
            ShowMessage("Cannot equip this item.");
            return;
        }

        if (equippable.EquipType != k_SlotTypes[_selectedIndex])
        {
            ShowMessage("Cannot equip this item.");
            return;
        }

        // 기존 장착 아이템 해제
        var currentSlot = _slots[_selectedIndex];
        if (!currentSlot.IsEmpty && currentSlot.Item is IEquippable old)
            old.Unequip(_player);

        // 장착
        equippable.Equip(_player);
        currentSlot.SetItem(item);
    }

    // 장착 해제 - 스페이스바로 빈 슬롯 선택 시
    public void TryUnequip()
    {
        var slot = _slots[_selectedIndex];
        if (slot.IsEmpty) return;

        if (slot.Item is IEquippable equippable)
            equippable.Unequip(_player);

        _player.Inventory.AddItem(slot.Item!);
        slot.Clear();
    }

    public void HandleInput(Inventory inventory)
    {
        _isFocused = true;

        if (Input.IsKeyDown(ConsoleKey.A))
            _selectedIndex = (_selectedIndex - 1 + k_SlotCount) % k_SlotCount;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _selectedIndex = (_selectedIndex + 1) % k_SlotCount;
        else if (Input.IsKeyDown(ConsoleKey.S) || Input.IsKeyDown(ConsoleKey.W))
        {
            _isFocused = false;
            inventory.ReturnToInventory();
        }

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
        {
            var slot = _slots[_selectedIndex];
            if (slot.IsEmpty)
            {
                // 빈 슬롯 → 인벤토리에서 장착할 아이템 선택
                _isFocused = false;
                inventory.StartEquipping(_selectedIndex);
            }
            else
                // 이미 장착된 아이템 → 해제
                TryUnequip();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        // 슬롯 그리기
        for (int i = 0; i < k_SlotCount; i++)
        {
            var (tx, ty) = k_SlotPos[i];
            bool selected = (i == _selectedIndex && _isFocused);
            _slots[i].Draw(buffer, tx, ty, selected, false);
        }

        // 플레이어 외형 출력 (12, 3) - 장비 착용 상태 그대로 반영
        int sx = 12 * 4;
        int sy = 3 * 2;
        _player.DrawAt(buffer, sx, sy, ConsoleColor.DarkGray);

        if (!_isFocused) return;

        // 메시지 또는 슬롯 정보 출력
        if (_messageTimer > 0)
            buffer.WriteTextCentered(7 * 2 + 1, _message, ConsoleColor.Red, ConsoleColor.DarkGray);
        else
        {
            string slotName = ((EquipSlot)_selectedIndex).ToString();
            var slot = _slots[_selectedIndex];
            string itemName = slot.IsEmpty ? "Empty" : slot.Item!.Name;
            buffer.WriteTextCentered(7 * 2 + 1,
                $"{slotName} | {itemName}", ConsoleColor.Yellow, ConsoleColor.DarkGray);
        }
    }

    public override void Update(float deltaTime)
    {
        if (_messageTimer > 0)
            _messageTimer -= deltaTime;
    }

    public void LoseFocus()
    {
        _isFocused = false;
    }
}