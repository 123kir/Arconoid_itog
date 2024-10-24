using System.Drawing; 

public class Ball
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Radius { get; set; } = 10; 
    public int XSpeed { get; set; } = 5; 
    public int YSpeed { get; set; } = -5; 

    public void Move()
    {
        X += XSpeed;
        Y += YSpeed;
    }

    
    public Rectangle GetRectangle()
    {
        return new Rectangle(X, Y, Radius * 2, Radius * 2);
    }
}
