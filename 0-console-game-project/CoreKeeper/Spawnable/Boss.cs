using System;
using Framework.Engine;

public class Boss : Spawner, IAttacker, IDefender
{
    public static int s_MaxCount = 1;
    public static int s_CurrentCount = 0;

    // IAttacker
    public int AttackDamage { get; private set; } = 8;

    // IDefender
    public int MaxHp { get; private set; } = 200;
    public int Hp { get; private set; } = 200;
    public bool IsAlive => Hp > 0;

    // 보스 구역 (스폰 중심 기준 ±10)
    private readonly int _areaX;
    private readonly int _areaY;
    private const int k_AreaRadius = 10;

    // 원래 자리
    private readonly int _homeX;
    private readonly int _homeY;

    // 상태
    private enum BossState { Idle, Chase, ReturnHome }
    private BossState _state = BossState.Idle;

    // 플레이어에게 공격받았는지
    private bool _engaged = false;

    // 일반 이동
    private float _moveTimer = 0f;
    private const float k_MoveInterval = 0.6f;

    // 돌진 스킬
    private float _chargeTimer = 0f;
    private const float k_ChargeInterval = 4f;
    private const float k_ChargeCooldown = 1f;
    private bool _isCharging = false;
    private float _chargeCooldownTimer = 0f;
    private int _chargeDx = 0;
    private int _chargeDy = 0;
    private int _chargeRemaining = 0;
    private float _chargeStepTimer = 0f;
    private const float k_ChargeStepInterval = 0.08f;

    // 귀환 중 HP 회복
    private float _healTimer = 0f;
    private const float k_HealInterval = 0.5f;

    // 피격 깜빡임
    private float _hitFlashTimer = 0f;
    private const float k_HitFlashDuration = 0.2f;

    public Boss(Scene scene, Map map, int tileX, int tileY)
        : base(scene, map, tileX, tileY)
    {
        Name = "Boss";
        s_CurrentCount++;
        _areaX = tileX;
        _areaY = tileY;
        _homeX = tileX;
        _homeY = tileY;
    }

    public void Attack(IDefender target) => target.TakeDamage(AttackDamage);

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - amount);
        _hitFlashTimer = k_HitFlashDuration;

        if (!_engaged)
        {
            _engaged = true;
            _state = BossState.Chase;  // 추가
        }

        if (!IsAlive)
        {
            s_CurrentCount--;
            var playScene = Scene as PlayScene;
            playScene?.RequestGameClear();
            Scene.RemoveGameObject(this);
        }
    }

    public override void Update(float deltaTime)
    {
        var player = Scene.FindGameObject("Player") as Player;
        if (player == null) return;

        if (_hitFlashTimer > 0f) _hitFlashTimer -= deltaTime;

        // 교전 전이면 가만히 피 회복
        if (!_engaged)
        {
            _healTimer += deltaTime;
            if (_healTimer >= k_HealInterval)
            {
                _healTimer = 0f;
                Hp = Math.Min(MaxHp, Hp + 5);
            }
            return;
        }

        // 플레이어가 보스 구역 밖이면 귀환
        bool playerInArea = IsInBossArea(player.Position.X, player.Position.Y);

        if (!playerInArea && _state != BossState.ReturnHome)
        {
            _state = BossState.ReturnHome;
            _isCharging = false;
        }

        switch (_state)
        {
            case BossState.Chase:
                UpdateChaseState(player, deltaTime);
                break;
            case BossState.ReturnHome:
                UpdateReturnHome(player, deltaTime);
                break;
        }
    }

    private void UpdateChaseState(Player player, float deltaTime)
    {
        if (_chargeCooldownTimer > 0f)
        {
            _chargeCooldownTimer -= deltaTime;
            return;
        }

        if (_isCharging)
        {
            UpdateCharge(player, deltaTime);
            return;
        }

        _chargeTimer += deltaTime;
        if (_chargeTimer >= k_ChargeInterval)
        {
            _chargeTimer = 0f;
            StartCharge(player);
            return;
        }

        _moveTimer += deltaTime;
        if (_moveTimer >= k_MoveInterval)
        {
            _moveTimer = 0f;
            ChasePlayer(player);
        }
    }

    private void UpdateReturnHome(Player player, float deltaTime)
    {
        // HP 회복
        _healTimer += deltaTime;
        if (_healTimer >= k_HealInterval)
        {
            _healTimer = 0f;
            Hp = Math.Min(MaxHp, Hp + 5);
        }

        // 집으로 이동
        _moveTimer += deltaTime;
        if (_moveTimer >= k_MoveInterval)
        {
            _moveTimer = 0f;

            if (TileX == _homeX && TileY == _homeY)
            {
                // 집 도착 - engaged 리셋해서 플레이어가 다시 공격하기 전까지 가만히
                _engaged = false;
                _state = BossState.Idle;
                return;
            }

            int dx = Math.Sign(_homeX - TileX);
            int dy = Math.Sign(_homeY - TileY);

            if (Math.Abs(_homeX - TileX) >= Math.Abs(_homeY - TileY))
                TileX += dx;
            else
                TileY += dy;
        }
    }

    private bool IsInBossArea(int tx, int ty)
    {
        return Math.Abs(tx - _areaX) <= k_AreaRadius &&
               Math.Abs(ty - _areaY) <= k_AreaRadius;
    }

    private bool IsMovableInArea(int tx, int ty)
    {
        return Map.IsMovable(tx, ty) && IsInBossArea(tx, ty);
    }

    private void StartCharge(Player player)
    {
        int distX = player.Position.X - TileX;
        int distY = player.Position.Y - TileY;

        if (Math.Abs(distX) >= Math.Abs(distY))
        {
            _chargeDx = Math.Sign(distX);
            _chargeDy = 0;
        }
        else
        {
            _chargeDx = 0;
            _chargeDy = Math.Sign(distY);
        }

        _chargeRemaining = 5;
        _isCharging = true;
        _chargeStepTimer = 0f;
    }

    private void UpdateCharge(Player player, float deltaTime)
    {
        _chargeStepTimer += deltaTime;
        if (_chargeStepTimer < k_ChargeStepInterval) return;
        _chargeStepTimer = 0f;

        if (_chargeRemaining <= 0)
        {
            _isCharging = false;
            _chargeCooldownTimer = k_ChargeCooldown;
            return;
        }

        int nextX = TileX + _chargeDx;
        int nextY = TileY + _chargeDy;

        if (!IsMovableInArea(nextX, nextY))
        {
            _isCharging = false;
            _chargeCooldownTimer = k_ChargeCooldown;
            return;
        }

        TileX = nextX;
        TileY = nextY;
        _chargeRemaining--;

        CheckPlayerCollision(player, AttackDamage * 2);
    }

    private void ChasePlayer(Player player)
    {
        int distX = player.Position.X - TileX;
        int distY = player.Position.Y - TileY;

        int dx = Math.Sign(distX);
        int dy = Math.Sign(distY);

        bool xMovable = dx != 0 && IsMovableInArea(TileX + dx, TileY);
        bool yMovable = dy != 0 && IsMovableInArea(TileX, TileY + dy);

        int moveX = 0, moveY = 0;

        if (xMovable && yMovable)
        {
            if (Math.Abs(distX) >= Math.Abs(distY)) moveX = dx;
            else moveY = dy;
        }
        else if (xMovable) moveX = dx;
        else if (yMovable) moveY = dy;
        else return;

        TileX += moveX;
        TileY += moveY;

        // 플레이어 감지 후 Chase 상태로
        _state = BossState.Chase;

        CheckPlayerCollision(player, AttackDamage);
    }

    private void CheckPlayerCollision(Player player, int damage)
    {
        int px = player.Position.X;
        int py = player.Position.Y;

        // 보스 3x3 전체 범위
        if (px >= TileX && px < TileX + 3 &&
            py >= TileY && py < TileY + 3)
        {
            player.TakeDamage(damage);
            int kbX = Math.Sign(px - (TileX + 1));
            int kbY = Math.Sign(py - (TileY + 1));
            player.Knockback(kbX, kbY);
        }
    }

    protected new bool IsInView(ScreenBuffer buffer)
    {
        var (sx, sy) = GetScreenPos(buffer);
        return sx >= 0 && sy >= 0 && sx + 11 < buffer.Width && sy + 5 < buffer.Height;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        Console.Title = $"Boss=({TileX},{TileY}) State={_state} Engaged={_engaged}";

        var (sx, sy) = GetScreenPos(buffer);

        // 화면에 아예 안보이면 스킵
        if (sx + 11 < 0 || sx >= buffer.Width || sy + 4 < 0 || sy >= buffer.Height) return;

        ConsoleColor main = _hitFlashTimer > 0f ? ConsoleColor.White : ConsoleColor.Red;
        ConsoleColor dark = _hitFlashTimer > 0f ? ConsoleColor.Gray : ConsoleColor.DarkRed;

        // sy+0
        SC(buffer, sx, sy, '▛', main, ConsoleColor.Black);
        for (int i = 1; i <= 10; i++)
            SC(buffer, sx + i, sy, '▀', main, ConsoleColor.Black);
        SC(buffer, sx + 11, sy, '▜', main, ConsoleColor.Black);

        // sy+1 - 눈
        SC(buffer, sx, sy + 1, '▌', main, ConsoleColor.Black);
        SC(buffer, sx + 2, sy + 1, '◉', ConsoleColor.Yellow, ConsoleColor.Black);
        SC(buffer, sx + 9, sy + 1, '◉', ConsoleColor.Yellow, ConsoleColor.Black);
        SC(buffer, sx + 11, sy + 1, '▐', main, ConsoleColor.Black);

        // sy+2 - 입
        SC(buffer, sx, sy + 2, '▌', main, ConsoleColor.Black);
        for (int i = 3; i <= 8; i++)
            SC(buffer, sx + i, sy + 2, '─', dark, ConsoleColor.Black);
        SC(buffer, sx + 11, sy + 2, '▐', main, ConsoleColor.Black);

        // sy+3 - 발
        SC(buffer, sx, sy + 3, '▌', main, ConsoleColor.Black);
        for (int i = 3; i <= 8; i++)
            SC(buffer, sx + i, sy + 3, '▄', dark, ConsoleColor.Black);
        SC(buffer, sx + 11, sy + 3, '▐', main, ConsoleColor.Black);

        // sy+4
        SC(buffer, sx, sy + 4, '▙', main, ConsoleColor.Black);
        for (int i = 1; i <= 10; i++)
            SC(buffer, sx + i, sy + 4, '▄', main, ConsoleColor.Black);
        SC(buffer, sx + 11, sy + 4, '▟', main, ConsoleColor.Black);

        // HP바
        int barWidth = 10;
        int filled = (int)((float)Hp / MaxHp * barWidth);
        SC(buffer, sx, sy - 1, '[', ConsoleColor.White, ConsoleColor.Black);
        for (int i = 0; i < barWidth; i++)
        {
            char ch = i < filled ? '█' : '░';
            ConsoleColor col = i < filled ? ConsoleColor.Red : ConsoleColor.DarkGray;
            SC(buffer, sx + 1 + i, sy - 1, ch, col, ConsoleColor.Black);
        }
        // HP 숫자는 화면 범위 안일 때만
        if (sx + 11 >= 0 && sx + 11 < buffer.Width && sy - 1 >= 0 && sy - 1 < buffer.Height)
            buffer.WriteText(sx + 11, sy - 1, $"] {Hp}/{MaxHp}", ConsoleColor.White, ConsoleColor.Black);
    }

    // 범위 체크 후 셀 출력
    private void SC(ScreenBuffer buffer, int x, int y, char ch, ConsoleColor fg, ConsoleColor bg)
    {
        if (x < 0 || x >= buffer.Width || y < 0 || y >= buffer.Height) return;
        buffer.SetCell(x, y, ch, fg, bg);
    }
}