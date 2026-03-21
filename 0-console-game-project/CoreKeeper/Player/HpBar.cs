using System;
using Framework.Engine;

public class HpBar : GameObject
{
    private const int k_TileX = 0;
    private const int k_TileY = 0;
    private const int k_TileWidth = 3;

    private readonly Player _player;

    public HpBar(Scene scene, Player player) : base(scene)
    {
        Name = "HpBar";
        _player = player;
    }

    public override void Update(float deltaTime)
    {
        
    }

    public override void Draw(ScreenBuffer buffer)
    {
        int sx = k_TileX * 4 + 2;
        int sy = k_TileY * 2 + 1;
        int totalCells = k_TileWidth * 4; // 12셀

        float hpRatio = (float)_player.Hp / _player.MaxHp;
        int filledCells = (int)Math.Round(totalCells * hpRatio);

        // 배경 (빈 체력)
        for (int i = 0; i < totalCells; i++)
            buffer.SetCell(sx + i, sy, '█', ConsoleColor.DarkRed, ConsoleColor.Black);

        // 채워진 체력
        for (int i = 0; i < filledCells; i++)
            buffer.SetCell(sx + i, sy, '█', ConsoleColor.Red, ConsoleColor.Black);

        // HP 텍스트 (두 번째 행)
        string hpText = $"HP {_player.Hp}/{_player.MaxHp}";
        buffer.WriteText(sx + 4, sy + 1, hpText, ConsoleColor.White, ConsoleColor.Black);
    }
}