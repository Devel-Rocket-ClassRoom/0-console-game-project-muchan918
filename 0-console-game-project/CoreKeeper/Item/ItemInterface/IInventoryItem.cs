using System;

public interface IInventoryItem
{
    int Count { get; set; }
    int MaxStack { get; }
}
