using System;
using Framework.Engine;

public class Game : GameApp
{
    public Game(int width, int height) : base(width, height)
    {

    }

    protected override void Draw()
    {
        Buffer.DrawBox(0, 0, 20, 20, ConsoleColor.Blue);
        Buffer.WriteText(0, 2, "use item", ConsoleColor.Red);
        Buffer.WriteTextCentered(3, "hiasasdi", ConsoleColor.Green);
        
    }

    protected override void Initialize()
    {
        
    }

    protected override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            Quit();
        }
    }
}