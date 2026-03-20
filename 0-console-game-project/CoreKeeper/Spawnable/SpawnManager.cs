using System;
using System.Collections.Generic;
using Framework.Engine;

public class SpawnManager : GameObject
{
    private readonly Map _map;
    private readonly Random _random = new Random();

    // 스폰 체크 주기 (초)
    private const float k_SpawnInterval = 3f;
    private float _spawnTimer = 0f;

    // 관리할 Spawner 타입 목록
    private readonly List<Type> _spawnerTypes = new();

    private int GetMaxCount(Type type)
        => (int)type.GetField("s_MaxCount")!.GetValue(null)!;

    private int GetCurrentCount(Type type)
        => (int)type.GetField("s_CurrentCount")!.GetValue(null)!;

    public SpawnManager(Scene scene, Map map) : base(scene)
    {
        Name = "SpawnManager";
        _map = map;
    }

    // 관리할 Spawner 타입 등록
    public void Register<T>() where T : Spawner
    {
        _spawnerTypes.Add(typeof(T));
    }

    private void SpawnOne(Type type)
    {
        var (tx, ty) = GetRandomMovableTile();
        var obj = (Spawner)Activator.CreateInstance(type, Scene, _map, tx, ty)!;
        Scene.AddGameObject(obj);
    }

    public void SpawnAll()
    {
        foreach (var type in _spawnerTypes)
            while (GetCurrentCount(type) < GetMaxCount(type))
                SpawnOne(type);
    }

    private (int tx, int ty) GetRandomMovableTile()
    {
        int tx, ty;
        do
        {
            tx = _random.Next(0, _map.TileWidth);
            ty = _random.Next(0, _map.TileHeight);
        } while (!_map.IsMovable(tx, ty));
        return (tx, ty);
    }

    public override void Draw(ScreenBuffer buffer)
    {

    }

    public override void Update(float deltaTime)
    {
        _spawnTimer += deltaTime;
        if (_spawnTimer < k_SpawnInterval) return;
        _spawnTimer = 0;

        foreach (var type in _spawnerTypes)
            if (GetCurrentCount(type) < GetMaxCount(type))
                SpawnOne(type);
    }
}