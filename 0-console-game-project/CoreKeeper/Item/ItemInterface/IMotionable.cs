using System;

public interface IMotionable
{
    char GetMotionChar(int dx, int dy);
    ConsoleColor MotionColor { get; }
}