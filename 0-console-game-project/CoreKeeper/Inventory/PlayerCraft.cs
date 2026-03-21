using System;
using Framework.Engine;

public class PlayerCraft : GameObject
{
    private const int k_StartTileX = 6;
    private const int k_StartTileY = 2;
    private const int k_SlotCountX = 3;
    private const int k_SlotCountY = 2;

    private int _selectedX = 0;
    private int _selectedY = 0;

    public int SelectedX => _selectedX;
    public int SelectedY => _selectedY;

    private Slot[,] _slots;

    public PlayerCraft(Scene scene) : base(scene)
    {
        Name = "PlayerCraft";
        _slots = new Slot[k_SlotCountY, k_SlotCountX];
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x] = new Slot();
    }

    public Slot GetSlot(int y, int x) => _slots[y, x];

    public override void Update(float deltaTime)
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x].Draw(buffer, k_StartTileX + x, k_StartTileY + y, false, false);
    }
}