using System;
using Framework.Engine;

public class Player : GameObject
{
    private float _moveTimer;
    private float _moveInterval = 0.1f;
    private (int X, int Y) _direction = (0, 0);

    private Map _map;

    private (int X, int Y) _headPosition;
    public (int X, int Y) HeadPosition => _headPosition;

    // 머리는 body 바로 위 (그릴 때만 사용, 충돌 없음)
    public (int X, int Y) BodyPosition => (_headPosition.X, _headPosition.Y + 1);

    // 손 위치 (body 오른쪽, 장비 아이콘 표시)
    public (int X, int Y) HandPosition => (_headPosition.X + 1, _headPosition.Y+1);

    public Player(Scene scene, Map map, int startX, int startY) : base(scene)
    {
        Name = "Player";
        _map = map;
        _headPosition = (startX, startY);
        _moveTimer = _moveInterval;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        // 항상 화면 중앙 고정
        int cx = buffer.Width / 2;
        int cy = buffer.Height / 2;

        // 머리
        buffer.SetCell(cx+1, cy, 'o');
        // 몸통
        buffer.SetCell(cx+1, cy+1, '&');
        // 손
        buffer.SetCell(cx + 2, cy + 1, '\\');
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

        if (Input.IsKey(ConsoleKey.W)) dy = -2; // 타일 1칸 = 월드 y+2
        else if (Input.IsKey(ConsoleKey.S)) dy = 2;
        else if (Input.IsKey(ConsoleKey.A)) dx = -4; // 타일 1칸 = 월드 x+4
        else if (Input.IsKey(ConsoleKey.D)) dx = 4;

        _direction = (dx, dy);
    }

    public void Move()
    {
        if (_direction == (0, 0)) return;

        int nx = _headPosition.X + _direction.X;
        int ny = _headPosition.Y + _direction.Y;

        if(_map.IsMovable(nx, ny))
        {
            _headPosition = (nx, ny);
        }

        return;
    }
}