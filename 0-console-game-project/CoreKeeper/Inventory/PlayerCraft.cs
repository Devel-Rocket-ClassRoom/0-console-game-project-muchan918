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

    // 메시지 출력 타이머
    private string _message = "";
    private float _messageTimer = 0f;
    private const float k_MessageDuration = 2f;

    public PlayerCraft(Scene scene) : base(scene)
    {
        Name = "PlayerCraft";
        _slots = new Slot[k_SlotCountY, k_SlotCountX];
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x] = new Slot();

        // 레시피 고정 배치
        _slots[0, 0].SetItem(new WorkbenchItem(scene));
        _slots[0, 1].SetItem(new WoodHelmet(scene));
        _slots[0, 2].SetItem(new WoodArmor(scene));
        _slots[1, 0].SetItem(new BoxItem(scene));
        _slots[1, 1].SetItem(new WoodSword(scene));
        _slots[1, 2].SetItem(new WoodPickaxe(scene));
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
            if (_selectedY == 0)
            {
                inventory.ReturnToInventory();
                _isFocused = false;
            }
            else _selectedY--;
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

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
        {
            TryCraft(inventory);
        }
    }

    private void TryCraft(Inventory inventory)
    {
        var slot = _slots[_selectedY, _selectedX];

        // 빈 슬롯이거나 ICraftable이 아니면 무시
        if (slot.IsEmpty || slot.Item is not ICraftable craftable) return;

        // 재료 확인
        foreach (var (itemName, count) in craftable.Recipe)
        {
            if (!inventory.HasItem(itemName, count))
            {
                ShowMessage("Not enough materials.");
                return;
            }
        }

        // 재료 소모
        foreach (var (itemName, count) in craftable.Recipe)
            inventory.ConsumeItem(itemName, count);

        // 제작 결과물 생성 - 슬롯 아이템과 같은 타입의 새 인스턴스
        Item result = slot.Item switch
        {
            WorkbenchItem => new WorkbenchItem(Scene),
            WoodHelmet => new WoodHelmet(Scene),
            WoodArmor => new WoodArmor(Scene),
            BoxItem => new BoxItem(Scene),
            WoodSword => new WoodSword(Scene),
            WoodPickaxe => new WoodPickaxe(Scene),
            _ => null!
        };

        if (result != null)
            inventory.AddItem(result);
    }

    private void ShowMessage(string message)
    {
        _message = message;
        _messageTimer = k_MessageDuration;
    }

    public string GetMessage() => _messageTimer > 0 ? _message : "";

    public override void Update(float deltaTime)
    {
        if (_messageTimer > 0)
            _messageTimer -= deltaTime;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
            {
                bool selected = (x == _selectedX && y == _selectedY && _isFocused);
                _slots[y, x].Draw(buffer, k_StartTileX + x, k_StartTileY + y, selected, false);
            }

        if (_isFocused)
        {
            // 메시지 출력 (재료 부족 등)
            if (_messageTimer > 0)
            {
                buffer.WriteTextCentered(7 * 2 + 1, _message, ConsoleColor.Red, ConsoleColor.DarkGray);
                return;
            }

            // 선택된 슬롯 아이템 정보 출력
            var slot = _slots[_selectedY, _selectedX];
            if (!slot.IsEmpty && slot.Item is ICraftable craftable)
            {
                string recipeStr = string.Join(", ",
                    System.Linq.Enumerable.Select(craftable.Recipe, r => $"{r.itemName} x{r.count}"));

                buffer.WriteTextCentered(4 * 2 + 1, $"Requires | {recipeStr}",
                    ConsoleColor.White, ConsoleColor.DarkGray);

                if (!string.IsNullOrEmpty(craftable.EffectDescription))
                    buffer.WriteTextCentered(7 * 2 + 1, $"Stats | {craftable.EffectDescription}",
                        ConsoleColor.Yellow, ConsoleColor.DarkGray);
            }
        }    
    }

    public void LoseFocus()
    {
        _isFocused = false;
    }
}