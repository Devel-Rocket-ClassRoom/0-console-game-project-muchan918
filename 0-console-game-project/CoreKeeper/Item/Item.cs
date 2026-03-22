using System;
using Framework.Engine;

// Item 추상 클래스
public abstract class Item : GameObject
{
    public abstract void Use(Player player);
    public abstract void DrawIcon(int tx, int ty, ScreenBuffer buffer);

    protected Item(Scene scene) : base(scene) { }

    // 인벤토리에서 아이템 소비 공통 로직
    protected void ConsumeFromInventory(Player player)
    {
        if (this is not IInventoryItem inv) return;
        inv.Count--;
        if (inv.Count > 0) return;

        for (int y = 0; y < 2; y++)
            for (int x = 0; x < 12; x++)
            {
                var slot = player.Inventory.GetSlot(y, x);
                if (!slot.IsEmpty && slot.Item == this)
                {
                    slot.Clear();
                    return;
                }
            }
    }
}