using System;
using Framework.Engine;

// Item 추상 클래스
public abstract class Item : GameObject
{
    public abstract string Name { get; set; }
    public abstract ConsoleColor Color { get; }
    public abstract void Use(Player player);
    public abstract void DrawIcon(int tx, int ty, ScreenBuffer buffer);

    protected Item(Scene scene) : base(scene) { }
}