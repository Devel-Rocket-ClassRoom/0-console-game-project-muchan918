using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Map map;
    private Player player;

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);
    }

    public override void Load()
    {
        map = new Map(this);
        AddGameObject(map);

        player = new Player(this, map, 400, 100);
        AddGameObject(player);
    }

    public override void Unload()
    {
        
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);
        map.SetViewPosition(player.HeadPosition.X, player.HeadPosition.Y);
    }
}