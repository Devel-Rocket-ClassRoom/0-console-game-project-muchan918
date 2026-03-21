using System;
using Framework.Engine;

public class Player : GameObject, IAttacker, IDefender
{
    private PlayScene? _scene;

    // Move Timer
    private float _moveTimer;
    private float _moveInterval = 0.1f;

    // Action Timer
    private float _actionTimer = 0f;
    private float _actionInterval = 0.5f;
    private bool _actionReady = true;

    private (int X, int Y) _direction = (0, 0);

    private readonly Map _map;
    public readonly Inventory Inventory;

    private (int X, int Y) _position;
    public (int X, int Y) Position => _position;

    // IAttacker
    public int AttackDamage { get; private set; } = 5;
    public int MiningDamage { get; private set; } = 1;

    // IDefender
    public int MaxHp { get; private set; } = 20;
    public int Hp { get; private set; } = 20;
    public bool IsAlive => Hp > 0;

    public Player(Scene scene, Map map, int startX, int startY) : base(scene)
    {
        _scene = scene as PlayScene;
        Name = "Player";
        _map = map;
        _position = (startX, startY);
        _moveTimer = _moveInterval;

        Inventory = new Inventory(scene, this);
        scene.AddGameObject(Inventory);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        int cx = (buffer.Width / 4 / 2) * 4;
        int cy = (buffer.Height / 2 / 2) * 2;
        DrawAt(buffer, cx, cy, ConsoleColor.Black);
    }

    public void DrawAt(ScreenBuffer buffer, int sx, int sy, ConsoleColor bg)
    {
        var equip = Inventory.Equipment;

        bool hasHelmet = !equip.GetSlot(PlayerEquipment.EquipSlot.Helmet).IsEmpty;
        bool hasArmor = !equip.GetSlot(PlayerEquipment.EquipSlot.Armor).IsEmpty;
        bool hasWeapon = !equip.GetSlot(PlayerEquipment.EquipSlot.RightHand).IsEmpty;

        // 머리
        if (hasHelmet)
        {
            buffer.SetCell(sx, sy, '▐', ConsoleColor.Yellow, bg); // 모자 왼쪽
            buffer.SetCell(sx + 2, sy, '▌', ConsoleColor.Yellow, bg); // 모자 오른쪽
        }
        buffer.SetCell(sx + 1, sy, '☺', ConsoleColor.Yellow, bg); // 얼굴

        // 몸통
        if (hasArmor)
        {
            buffer.SetCell(sx, sy + 1, '░', ConsoleColor.DarkGray, bg); // 갑옷 왼쪽
            buffer.SetCell(sx + 2, sy + 1, '/', ConsoleColor.White, bg); // 갑옷 오른쪽
        }
        buffer.SetCell(sx + 1, sy + 1, '█', ConsoleColor.Cyan, bg); // 몸통

        // 오른손 장비
        if (hasWeapon)
            buffer.SetCell(sx + 3, sy + 1, '/', ConsoleColor.White, bg); // 무기
        else
            buffer.SetCell(sx + 3, sy + 1, '/', ConsoleColor.White, bg); // 맨손
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Tab))
        {
            Inventory.Toggle();
            return;
        }

        if (!_actionReady)
        {
            _actionTimer += deltaTime;
            if (_actionTimer >= _actionInterval)
            {
                _actionReady = true;
                _actionTimer = 0;
            }
        }

        _moveTimer += deltaTime;
        if (_moveTimer >= _moveInterval)
        {
            Move();
            _moveTimer = 0;
        }

        if (Inventory.IsOpen) return;

        HandleInput();
    }

    private void HandleInput()
    {
        int dx = 0, dy = 0;

        if (Input.IsKey(ConsoleKey.W)) { dy = -1; }
        else if (Input.IsKey(ConsoleKey.S)) { dy = 1; }
        else if (Input.IsKey(ConsoleKey.A)) { dx = -1; }
        else if (Input.IsKey(ConsoleKey.D)) { dx = 1; }

        _direction = (dx, dy);

        // action 가능하면 방향키로 Action 
        if (_actionReady)
        {
            if (Input.IsKey(ConsoleKey.UpArrow)) { Action(0, -1); _actionReady = false; }
            else if (Input.IsKey(ConsoleKey.DownArrow)) { Action(0, 1); _actionReady = false; }
            else if (Input.IsKey(ConsoleKey.LeftArrow)) { Action(-1, 0); _actionReady = false; }
            else if (Input.IsKey(ConsoleKey.RightArrow)) { Action(1, 0); _actionReady = false; }
        }
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

    private void Action(int dx, int dy)
    {
        int targetX = _position.X + dx;
        int targetY = _position.Y + dy;

        var defender = _scene!.FindDefender(targetX, targetY);
        if (defender != null)
        {
            Attack(defender);
            return;
        }

        if (_map.IsMinable(targetX, targetY))
        {
            Mine(targetX, targetY);
            return;
        }
        // 추후 공격 등
    }

    private void Mine(int tileX, int tileY)
    {
        TileType tileType = _map.GetTileType(tileX, tileY); // 먼저 타입 저장
        bool broken = _map.MineTile(tileX, tileY, MiningDamage);

        if (broken)
        {
            Item? item = tileType switch
            {
                TileType.Wood => new WoodItem(Scene, _map, tileX, tileY),
                TileType.Soil => new SoilItem(Scene, _map, tileX, tileY),
                TileType.Stone => new StoneItem(Scene, _map, tileX, tileY),
                _ => null,
            };

            if (item != null)
                Scene.AddGameObject(item);
        }
    }

    public void Attack(IDefender target)
    {
        target.TakeDamage(AttackDamage);
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - amount);
    }
}