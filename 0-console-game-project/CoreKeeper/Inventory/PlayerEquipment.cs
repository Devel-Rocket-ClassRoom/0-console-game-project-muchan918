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

    private int _selectedIndex = 0;
    private bool _isFocused = false;

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
    }

    public override void Update(float deltaTime)
    {
        
    }

    public Slot GetSlot(EquipSlot slot) => _slots[(int)slot];

    public void SetSelected(int index)
    {
        _selectedIndex = Math.Clamp(index, 0, k_SlotCount - 1);
    }
}