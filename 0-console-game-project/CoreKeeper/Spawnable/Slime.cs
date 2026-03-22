using System;
using Framework.Engine;

public class Slime : Spawner, IAttacker, IDefender, IChaseable, IKnockbackable
{
    // static으로 전체 수량 관리
    public static int s_MaxCount = 80;
    public static int s_CurrentCount = 0;

    // IAttacker
    public int AttackDamage { get; private set; } = 3;

    // IDefender
    public int MaxHp { get; private set; } = 10;
    public int Hp { get; private set; } = 10;
    public bool IsAlive => Hp > 0;

    // IChaseable
    public float DetectRange => 4f;
    public float MoveInterval => 0.5f;

    // 이동 타이머
    private float _moveTimer = 0f;

    public Slime(Scene scene, Map map, int tileX, int tileY)
        : base(scene, map, tileX, tileY)
    {
        Name = "SlimeInstance";
        s_CurrentCount++;
    }

    public void Attack(IDefender target)
    {
        target.TakeDamage(AttackDamage);
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsInView(buffer)) return;
        var (sx, sy) = GetScreenPos(buffer);

        buffer.SetCell(sx, sy, '▗', ConsoleColor.Green, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy, '▆', ConsoleColor.Green, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy, '▆', ConsoleColor.Green, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy, '▖', ConsoleColor.Green, ConsoleColor.Black);
        buffer.SetCell(sx, sy + 1, '▝', ConsoleColor.DarkGreen, ConsoleColor.Black);
        buffer.SetCell(sx + 1, sy + 1, '▀', ConsoleColor.DarkGreen, ConsoleColor.Black);
        buffer.SetCell(sx + 2, sy + 1, '▀', ConsoleColor.DarkGreen, ConsoleColor.Black);
        buffer.SetCell(sx + 3, sy + 1, '▘', ConsoleColor.DarkGreen, ConsoleColor.Black);
    }

    // 슬라임 넉백 - 플레이어 반대 방향으로 최대 2칸
    public void Knockback(int fromDx, int fromDy)
    {
        // fromDx, fromDy = 플레이어 → 슬라임 방향, 반대로 밀어냄
        int nx = TileX + fromDx;
        int ny = TileY + fromDy;
        if (Map.IsMovable(nx, ny)) { TileX = nx; TileY = ny; }

        nx = TileX + fromDx;
        ny = TileY + fromDy;
        if (Map.IsMovable(nx, ny)) { TileX = nx; TileY = ny; }
    }

    public void TakeDamage(int amount)
    {
        Hp = Math.Max(0, Hp - amount);
        if (!IsAlive)
        {
            s_CurrentCount--;
            Scene.AddGameObject(new SlimeItem(Scene, Map, TileX, TileY));
            Scene.RemoveGameObject(this);
        }    
    }

    public override void Update(float deltaTime)
    {
        var player = Scene.FindGameObject("Player") as Player;
        if (player == null) return;

        _moveTimer += deltaTime;
        if (_moveTimer < MoveInterval) return;
        _moveTimer = 0f;

        // 거리 정수형으로 계산
        int distX = player.Position.X - TileX;
        int distY = player.Position.Y - TileY;
        int dist = (int)MathF.Sqrt(distX * distX + distY * distY);

        if (dist > (int)DetectRange) return;

        // 이동 방향 계산
        int dx = Math.Sign(distX);
        int dy = Math.Sign(distY);
        
        TryMove(player, dx, dy); 
    }

    private void TryMove(Player player, int dx, int dy)
    {
        bool xMovable = dx != 0 && Map.IsMovable(TileX + dx, TileY);
        bool yMovable = dy != 0 && Map.IsMovable(TileX, TileY + dy);

        int moveX = 0, moveY = 0;

        if (xMovable && yMovable)
        {
            if (Math.Abs(player.Position.X - TileX) >= Math.Abs(player.Position.Y - TileY))
                moveX = dx;
            else
                moveY = dy;
        }
        else if (xMovable)
            moveX = dx;
        else if (yMovable)
            moveY = dy;
        else
            return; // 막혀있으면 아무것도 안 함

        int nextX = TileX + moveX;
        int nextY = TileY + moveY;

        if (nextX == player.Position.X && nextY == player.Position.Y)
        {
            Attack(player);
            player.Knockback(moveX, moveY);
            return; // 공격했으면 이동 안 함
        }

        TileX = nextX;
        TileY = nextY;
    }
}