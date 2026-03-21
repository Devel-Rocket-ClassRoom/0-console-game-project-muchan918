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

    public override void Draw(ScreenBuffer buffer)
    {
        // 슬롯 그리기
        for (int i = 0; i < k_SlotCount; i++)
        {
            var (tx, ty) = k_SlotPos[i];
            _slots[i].Draw(buffer, tx, ty, false, false);
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
}