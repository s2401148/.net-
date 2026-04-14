using Raylib_cs;
using System.Numerics;

public class Bullet
{
    public Vector2 position;
    Vector2 direction;

    float speed = 500;

    public bool active = false;

    public void Fire(Vector2 startPos, Vector2 dir)
    {
        position = startPos;
        direction = dir;
        active = true;
    }

    public void Update()
    {
        if (!active) return;

        position += direction * speed * Raylib.GetFrameTime();

        if (position.X < 0 || position.X > Raylib.GetScreenWidth() ||
            position.Y < 0 || position.Y > Raylib.GetScreenHeight())
        {
            active = false;
        }
    }

    public void Draw()
    {
        if (active)
            Raylib.DrawCircleV(position, 6, Color.Yellow);
    }
}