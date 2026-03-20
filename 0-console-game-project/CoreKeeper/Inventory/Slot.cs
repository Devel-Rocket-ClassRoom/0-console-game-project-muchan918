using System;
using Framework.Engine;

public class Slot
{
    public Item? Item { get; set; }
    public bool IsEmpty => Item == null;

    public void SetItem(Item item) => Item = item;
    public void Clear() => Item = null;

    public void Draw(ScreenBuffer buffer, int tx, int ty, bool selected = false, bool isQuick = false)
    {
        int sx = tx * 4;
        int sy = ty * 2;

        ConsoleColor border = isQuick ? ConsoleColor.Black : ConsoleColor.Gray;
        border = selected ? ConsoleColor.Green : border;
        
        // 배경 먼저 채우기
        for (int dy = 0; dy < 2; dy++)
            for (int dx = 0; dx < 4; dx++)
                buffer.SetCell(sx + dx, sy + dy, ' ', ConsoleColor.White, ConsoleColor.DarkGray);

        buffer.SetCell(sx, sy, '┌', border, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 3, sy, '┐', border, ConsoleColor.DarkGray);
        buffer.SetCell(sx, sy + 1, '└', border, ConsoleColor.DarkGray);
        buffer.SetCell(sx + 3, sy + 1, '┘', border, ConsoleColor.DarkGray);

        // 아이템 있으면 아이콘 출력
        if (!IsEmpty)
            Item!.DrawIcon(tx, ty, buffer);
    }
}