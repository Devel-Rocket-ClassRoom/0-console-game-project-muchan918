using System;

// 맵에 드롭될 수 있는 아이템
public interface IDroppable
{
    int WorldX {  get; set; }
    int WorldY { get; set; } 
    void OnPickup(Player player);
}