﻿using System.Drawing;

public class Block
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 60; 
    public int Height { get; set; } = 20; 
    public bool IsActive { get; set; } = true; 

    public Rectangle GetRectangle()
    {
        return new Rectangle(X, Y, Width, Height);
    }
}
