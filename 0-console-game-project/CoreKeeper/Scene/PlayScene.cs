using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Map map;
    private Player player;

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(0, 0, buffer.Width, buffer.Height, ConsoleColor.Yellow);
        DrawGameObjects(buffer);
    }

    public override void Load()
    {
        map = new Map(this);
        AddGameObject(map);
        
        player = new Player(this, map, 50, 25);
        AddGameObject(player);
    }

    public override void Unload()
    {
        
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);
        map.SetViewPosition(player.Position.X, player.Position.Y);
    }
}