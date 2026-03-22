using System;
using Framework.Engine;

public class Player : GameObject, IAttacker, IDefender, IKnockbackable
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
    private (int X, int Y) _lastDirection = (0, 0);

    // 피격 쿨타임
    private float _invincibleTimer = 0f;
    private const float k_InvincibleDuration = 1.5f;
    public bool IsInvincible => _invincibleTimer > 0f;

    private readonly Map _map;
    public readonly Inventory Inventory;

    private QuickSlot? _quickSlot;
    public void SetQuickSlot(QuickSlot quickSlot) => _quickSlot = quickSlot;

    private (int X, int Y) _position;
    public (int X, int Y) Position => _position;

    // IAttacker
    public int AttackDamage { get; private set; } = 5;
    public int MiningDamage { get; private set; } = 1;

    // IDefender
    public int MaxHp { get; private set; } = 20;
    public int Hp { get; private set; } = 20;
    public bool IsAlive => Hp > 0;

    public void Heal(int amount) => Hp = Math.Min(Hp + amount, MaxHp);

    public void IncreaseAttackDamage(int amount) => AttackDamage += amount;
    public void DecreaseAttackDamage(int amount) => AttackDamage -= amount;

    public void IncreaseMiningDamage(int amount) => MiningDamage += amount;
    public void DecreaseMiningDamage(int amount) => MiningDamage -= amount;

    public void IncreaseMaxHp(int amount) { MaxHp += amount; Hp += amount; }
    public void DecreaseMaxHp(int amount) { MaxHp -= amount; Hp = Math.Min(Hp, MaxHp); }

    public (int X, int Y) GetFrontTile() =>
        (_position.X + _lastDirection.X, _position.Y + _lastDirection.Y);

    public Map GetMap() => _map;

    private char GetDefaultMotionChar(int dx, int dy) => (dx, dy) switch
    {
        (0, -1) => '⮉',
        (0, 1) => '⮋',
        (-1, 0) => '⮈',
        (1, 0) => '⮊',
        _ => '*'
    };

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

        var helmetSlot = equip.GetSlot(PlayerEquipment.EquipSlot.Helmet);
        var armorSlot = equip.GetSlot(PlayerEquipment.EquipSlot.Armor);
        var weaponSlot = equip.GetSlot(PlayerEquipment.EquipSlot.RightHand);

        bool hasHelmet = !helmetSlot.IsEmpty;
        bool hasArmor = !armorSlot.IsEmpty;
        bool hasWeapon = !weaponSlot.IsEmpty;

        ConsoleColor helmetColor = hasHelmet && helmetSlot.Item is IEquippable h ? h.Color : ConsoleColor.Yellow;
        ConsoleColor armorColor = hasArmor && armorSlot.Item is IEquippable a ? a.Color : ConsoleColor.DarkGray;
        ConsoleColor weaponColor = hasWeapon && weaponSlot.Item is IEquippable w ? w.Color : ConsoleColor.DarkGray;

        // 머리
        if (hasHelmet)
        {
            buffer.SetCell(sx, sy, '▐', helmetColor, bg);
            buffer.SetCell(sx + 2, sy, '▌', helmetColor, bg);
        }
        buffer.SetCell(sx + 1, sy, '☺', ConsoleColor.Yellow, bg);

        // 몸통
        if (hasArmor)
        {
            buffer.SetCell(sx, sy + 1, '▐', armorColor, bg);
            buffer.SetCell(sx + 2, sy + 1, '▌', armorColor, bg);
        }
        buffer.SetCell(sx + 1, sy + 1, '█', ConsoleColor.Cyan, bg);

        // 오른손 장비
        buffer.SetCell(sx + 3, sy + 1, '/', ConsoleColor.White, bg); // 기본 손 항상 출력
        if (hasWeapon)
            buffer.SetCell(sx + 3, sy, '|', weaponColor, bg); // 무기 장착 시 위에 추가
    }


    public override void Update(float deltaTime)
    {
        if (_invincibleTimer > 0f)
            _invincibleTimer -= deltaTime;

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

        if (Input.IsKeyDown(ConsoleKey.Spacebar))
        {
            var slot = _quickSlot?.GetSelectedSlot();
            if (slot != null && !slot.IsEmpty)
                slot.Item!.Use(this);
        }

        _direction = (dx, dy);
        if (_direction != (0, 0)) _lastDirection = _direction;

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
    }

    private void Action(int dx, int dy)
    {
        int targetX = _position.X + dx;
        int targetY = _position.Y + dy;

        // 장착 무기가 IMotionable이면 이펙트 생성, 없으면 기본 이펙트
        var weaponSlot = Inventory.Equipment.GetSlot(PlayerEquipment.EquipSlot.RightHand);
        if (!weaponSlot.IsEmpty && weaponSlot.Item is IMotionable motionable)
            Scene.AddGameObject(new AttackEffect(Scene, _map, targetX, targetY,
                motionable.GetMotionChar(dx, dy), motionable.MotionColor));
        else
            Scene.AddGameObject(new AttackEffect(Scene, _map, targetX, targetY,
                GetDefaultMotionChar(dx, dy), ConsoleColor.White));

        var defender = _scene!.FindDefender(targetX, targetY);
        if (defender != null)
        {
            Attack(defender);
            if (defender is IKnockbackable knockbackable)
                knockbackable.Knockback(dx, dy);
            return;
        }

        if (_map.IsMinable(targetX, targetY))
        {
            Mine(targetX, targetY);
            return;
        }
    }

    private void Mine(int tileX, int tileY)
    {
        TileType tileType = _map.GetTileType(tileX, tileY);
        var (broken, installedItem) = _map.MineTile(tileX, tileY, MiningDamage);

        if (broken)
        {
            if (installedItem != null)
            {
                // IDroppable이면 좌표 설정 후 드랍
                if (installedItem is IDroppable droppable)
                {
                    droppable.TileX = tileX;
                    droppable.TileY = tileY;
                }
                Scene.AddGameObject(installedItem);
                return;
            }

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
        if (IsInvincible) return;
        Hp = Math.Max(0, Hp - amount);
        _invincibleTimer = k_InvincibleDuration;
    }

    // 넉백 - 방향으로 최대 2칸, Movable 체크
    public void Knockback(int dx, int dy)
    {
        int nx = _position.X + dx;
        int ny = _position.Y + dy;
        if (_map.IsMovable(nx, ny)) _position = (nx, ny);

        nx = _position.X + dx;
        ny = _position.Y + dy;
        if (_map.IsMovable(nx, ny)) _position = (nx, ny);
    }
}