using System;

public interface ICraftable
{
    (string itemName, int count)[] Recipe {  get; }
}