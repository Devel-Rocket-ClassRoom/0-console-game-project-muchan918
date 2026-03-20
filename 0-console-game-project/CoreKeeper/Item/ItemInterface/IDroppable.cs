using System;

// 맵에 드롭될 수 있는 아이템
public interface IDroppable
{
    int TileX { get; set; }
    int TileY { get; set; }
    void OnPickup(Player player);
}