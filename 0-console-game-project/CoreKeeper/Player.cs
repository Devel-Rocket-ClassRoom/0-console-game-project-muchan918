using System;
using Framework.Engine;

public class Player : GameObject
{
    private float _moveTimer;
    private float _moveInterval = 0.1f;
    private (int X, int Y) _direction = (0, 0);
    private (int X, int Y) _lastDirection;

    private Map _map;

    private (int X, int Y) _position;
    public (int X, int Y) Position => _position;

    public Player(Scene scene, Map map, int startX, int startY) : base(scene)
    {
        Name = "Player";
        _map = map;
        _position = (startX, startY);
        _moveTimer = _moveInterval;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        // 항상 화면 중앙 타일에 고정
        int cx = (buffer.Width / 4 / 2) * 4;
        int cy = (buffer.Height / 2 / 2) * 2;

        // 타일 4×2 안에 캐릭터 표현
        buffer.SetCell(cx + 1, cy, 'o', ConsoleColor.Yellow); // 머리
        buffer.SetCell(cx + 1, cy + 1, '&', ConsoleColor.Cyan);   // 몸통
        buffer.SetCell(cx + 2, cy + 1, '/', ConsoleColor.White);  // 손
    }

    public override void Update(float deltaTime)
    {
        HandleInput();

        _moveTimer += deltaTime;
        if (_moveTimer >= _moveInterval)
        {
            Move();
            _moveTimer = 0;
        }
    }

    private void HandleInput()
    {
        int dx = 0, dy = 0;

        if (Input.IsKey(ConsoleKey.W)) { dy = -1; _lastDirection = (0, -1); }
        else if (Input.IsKey(ConsoleKey.S)) { dy = 1; _lastDirection = (0, 1); }
        else if (Input.IsKey(ConsoleKey.A)) { dx = -1; _lastDirection = (-1, 0); }
        else if (Input.IsKey(ConsoleKey.D)) { dx = 1; _lastDirection = (1, 0); }

        _direction = (dx, dy);

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
            Action();
    }

    public void Move()
    {
        if (_direction == (0, 0)) return;

        int nx = _position.X + _direction.X;
        int ny = _position.Y + _direction.Y;

        if (_map.IsMovable(nx, ny))
        {
            _position = (nx, ny);
        }

        return;
    }

    private void Action()
    {
        int targetX = _position.X + _lastDirection.X;
        int targetY = _position.Y + _lastDirection.Y;

        if (_map.IsMinable(targetX, targetY))
        {
            Mine(targetX, targetY);
            return;
        }

    }

    private void Mine(int tileX, int tileY)
    {
        TileType broken = _map.BreakTile(tileX, tileY);

        Item? item = broken switch
        {
            TileType.Wood => new WoodItem(Scene, _map, tileX, tileY),
            TileType.Soil => new SoilItem(Scene, _map, tileX, tileY),
            _ => null,
        };

        if (item != null)
            Scene.AddGameObject(item);
    }
}