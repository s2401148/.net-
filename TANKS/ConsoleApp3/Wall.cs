using Raylib_cs;
using System.Numerics;

public class Wall
{
    Vector2 position;
    Vector2 size;

    public Wall(float x, float y, float w, float h)
    {
        position = new Vector2(x, y);
        size = new Vector2(w, h);
    }

    public void Draw()
    {
        Raylib.DrawRectangleV(position, size, Color.Gray);
    }

    public Rectangle GetRect()
    {
        return new Rectangle(position, size);
    }
}