using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class Tank
{
    public Vector2 position;
    Vector2 direction = new Vector2(1, 0);

    Vector2 tankSize = new Vector2(60, 60);
    Vector2 turretSize = new Vector2(20, 20);

    float speed = 200;

    KeyboardKey up, down, left, right, shoot;

    public Bullet bullet;

    Color color;

    double lastShootTime = 0;
    double shootInterval = 1;

    public Tank(Vector2 startPos, Color c,
        KeyboardKey u, KeyboardKey d, KeyboardKey l, KeyboardKey r, KeyboardKey s)
    {
        position = startPos;
        color = c;

        up = u;
        down = d;
        left = l;
        right = r;
        shoot = s;

        bullet = new Bullet();
    }

    public void Update(List<Wall> walls)
    {
        float dt = Raylib.GetFrameTime();
        Vector2 move = Vector2.Zero;

        if (Raylib.IsKeyDown(up))
        {
            move.Y -= speed * dt;
            direction = new Vector2(0, -1);
        }

        if (Raylib.IsKeyDown(down))
        {
            move.Y += speed * dt;
            direction = new Vector2(0, 1);
        }

        if (Raylib.IsKeyDown(left))
        {
            move.X -= speed * dt;
            direction = new Vector2(-1, 0);
        }

        if (Raylib.IsKeyDown(right))
        {
            move.X += speed * dt;
            direction = new Vector2(1, 0);
        }

        position += move;

        foreach (Wall wall in walls)
        {
            if (Raylib.CheckCollisionRecs(GetRect(), wall.GetRect()))
            {
                position -= move;
            }
        }

        Shoot();
    }

    void Shoot()
    {
        if (Raylib.IsKeyDown(shoot))
        {
            if (Raylib.GetTime() - lastShootTime > shootInterval)
            {
                bullet.Fire(position, direction);
                lastShootTime = Raylib.GetTime();
            }
        }
    }

    public void UpdateBullet(List<Wall> walls)
    {
        bullet.Update();

        foreach (Wall wall in walls)
        {
            if (bullet.active &&
                Raylib.CheckCollisionPointRec(bullet.position, wall.GetRect()))
            {
                bullet.active = false;
            }
        }
    }

    public void Draw()
    {
        Vector2 topLeft = position - tankSize / 2;
        Raylib.DrawRectangleV(topLeft, tankSize, color);

        Vector2 turretPos = position + direction * (tankSize.X / 2 + turretSize.X / 2);
        Vector2 turretTopLeft = turretPos - turretSize / 2;

        Raylib.DrawRectangleV(turretTopLeft, turretSize, Color.Black);
    }

    public void DrawBullet()
    {
        bullet.Draw();
    }

    public Rectangle GetRect()
    {
        Vector2 topLeft = position - tankSize / 2;
        return new Rectangle(topLeft, tankSize);
    }
}