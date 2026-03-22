using System;
using Framework.Engine;

public class WorkbenchUI : GameObject
{
    private const int k_StartTileX = 7;
    private const int k_StartTileY = 2;
    private const int k_SlotCountX = 6;
    private const int k_SlotCountY = 2;

    private const int k_InvSlotStartX = 4;
    private const int k_InvSlotStartY = 5;
    private const int k_InvSlotCountX = 12;
    private const int k_InvSlotCountY = 2;

    private int _selectedX = 0;
    private int _selectedY = 0;
    private bool _isFocused = false;

    private string _message = "";
    private float _messageTimer = 0f;
    private const float k_MessageDuration = 2f;

    private Slot[,] _slots;
    private readonly Inventory _inventory;

    public bool IsOpen { get; private set; } = false;

    public WorkbenchUI(Scene scene, Inventory inventory) : base(scene)
    {
        Name = "WorkbenchUI";
        IsActive = false;
        _inventory = inventory;
        _slots = new Slot[k_SlotCountY, k_SlotCountX];
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
                _slots[y, x] = new Slot();

        // 1행
        _slots[0, 0].SetItem(new WorkbenchItem(scene));
        _slots[0, 1].SetItem(new WoodHelmet(scene));
        _slots[0, 2].SetItem(new WoodArmor(scene));
        _slots[0, 3].SetItem(new StoneHelmet(scene));
        _slots[0, 4].SetItem(new StoneArmor(scene));
        _slots[0, 5].SetItem(new SlimeHelmet(scene));

        // 2행
        _slots[1, 0].SetItem(new BoxItem(scene));
        _slots[1, 1].SetItem(new WoodSword(scene));
        _slots[1, 2].SetItem(new WoodPickaxe(scene));
        _slots[1, 3].SetItem(new StoneSword(scene));
        _slots[1, 4].SetItem(new StonePickaxe(scene));
        _slots[1, 5].SetItem(new MushroomPotion(scene));
    }

    public void Open()
    {
        IsActive = true;
        IsOpen = true;
        _selectedX = 0;
        _selectedY = 0;
        _isFocused = true;
        _message = "";
        _messageTimer = 0f;
    }

    public void Close()
    {
        IsActive = false;
        IsOpen = false;
        _isFocused = false;
    }

    public override void Update(float deltaTime)
    {
        if (!IsActive) return;
        if (_messageTimer > 0) _messageTimer -= deltaTime;
        HandleInput();
    }

    private void HandleInput()
    {
        _isFocused = true;

        if (Input.IsKeyDown(ConsoleKey.A))
            _selectedX = (_selectedX - 1 + k_SlotCountX) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.D))
            _selectedX = (_selectedX + 1) % k_SlotCountX;
        else if (Input.IsKeyDown(ConsoleKey.W))
            _selectedY = (_selectedY - 1 + k_SlotCountY) % k_SlotCountY;
        else if (Input.IsKeyDown(ConsoleKey.S))
            _selectedY = (_selectedY + 1) % k_SlotCountY;

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
            TryCraft();
    }

    private void TryCraft()
    {
        var slot = _slots[_selectedY, _selectedX];
        if (slot.IsEmpty || slot.Item is not ICraftable craftable) return;

        foreach (var (itemName, count) in craftable.Recipe)
        {
            if (!_inventory.HasItem(itemName, count))
            {
                ShowMessage("Not enough materials.");
                return;
            }
        }

        foreach (var (itemName, count) in craftable.Recipe)
            _inventory.ConsumeItem(itemName, count);

        Item result = slot.Item switch
        {
            WorkbenchItem => new WorkbenchItem(Scene),
            WoodHelmet => new WoodHelmet(Scene),
            WoodArmor => new WoodArmor(Scene),
            WoodSword => new WoodSword(Scene),
            WoodPickaxe => new WoodPickaxe(Scene),
            BoxItem => new BoxItem(Scene),
            StoneHelmet => new StoneHelmet(Scene),
            StoneArmor => new StoneArmor(Scene),
            StoneSword => new StoneSword(Scene),
            StonePickaxe => new StonePickaxe(Scene),
            SlimeHelmet => new SlimeHelmet(Scene),
            MushroomPotion => new MushroomPotion(Scene),
            _ => null!
        };

        if (result != null)
            _inventory.AddItem(result);
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

        // 제작 슬롯
        for (int y = 0; y < k_SlotCountY; y++)
            for (int x = 0; x < k_SlotCountX; x++)
            {
                bool selected = _isFocused && (x == _selectedX && y == _selectedY);
                _slots[y, x].Draw(buffer, k_StartTileX + x, k_StartTileY + y, selected, false);
            }

        // 인벤토리 슬롯 (출력만)
        for (int y = 0; y < k_InvSlotCountY; y++)
            for (int x = 0; x < k_InvSlotCountX; x++)
            {
                bool isQuick = (y == 0 && x < 6);
                _inventory.GetSlot(y, x).Draw(buffer, k_InvSlotStartX + x, k_InvSlotStartY + y, false, isQuick);
            }

        if (_messageTimer > 0)
        {
            buffer.WriteTextCentered(7 * 2 + 1, _message, ConsoleColor.Red, ConsoleColor.DarkGray);
            return;
        }

        var slot = _slots[_selectedY, _selectedX];
        if (!slot.IsEmpty && slot.Item is ICraftable craftable)
        {
            string recipeStr = string.Join(", ",
                System.Linq.Enumerable.Select(craftable.Recipe, r => $"{r.itemName} x{r.count}"));

            buffer.WriteTextCentered(7 * 2, $"Requires | {recipeStr}",
                ConsoleColor.White, ConsoleColor.DarkGray);

            if (!string.IsNullOrEmpty(craftable.EffectDescription))
                buffer.WriteTextCentered(7 * 2 + 1, $"Stats | {craftable.EffectDescription}",
                    ConsoleColor.Yellow, ConsoleColor.DarkGray);
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