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
    private bool _isFocused = false;

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

    public void SetSelected(int x, int y)
    {
        _selectedX = x;
        _selectedY = y;
    }

    public void HandleInput(Inventory inventory)
    {
        _isFocused = true;

        if (Input.IsKeyDown(ConsoleKey.A))
            _selectedX = (_selectedX - 1 + k_SlotCountX) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _selectedX = (_selectedX + 1) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.W))
        {
            if (_selectedY == 0) inventory.ReturnToInventory();
            _selectedY--;
        }
        else if (Input.IsKeyDown(ConsoleKey.S))
        {
            if (_selectedY < k_SlotCountY - 1)
                _selectedY++;
            else // 최하단에서 S → 인벤토리로 복귀
            {
                _isFocused = false;
                inventory.ReturnToInventory();
            }
        }
    }

    public override void Update(float deltaTime)
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
            {
                bool selected = (x == _selectedX && y == _selectedY && _isFocused);
                _slots[y, x].Draw(buffer, k_StartTileX + x, k_StartTileY + y, selected, false);
            }
    }
}