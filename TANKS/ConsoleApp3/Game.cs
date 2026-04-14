using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class Game
{
    Tank player1;
    Tank player2;

    List<Wall> walls = new List<Wall>();

    int player1Score = 0;
    int player2Score = 0;

    public void Run()
    {
        Init();
        GameLoop();
    }

    private void Init()
    {
        Raylib.InitWindow(1000, 800, "Tank Game");
        Raylib.SetTargetFPS(60);

        player1 = new Tank(new Vector2(200, 400), Color.Green,
            KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D, KeyboardKey.Space);

        player2 = new Tank(new Vector2(800, 400), Color.Red,
            KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right, KeyboardKey.RightControl);

        walls.Add(new Wall(450, 300, 100, 200));
    }

    private void GameLoop()
    {
        while (!Raylib.WindowShouldClose())
        {
            UpdateGame();
            DrawGame();
        }

        Raylib.CloseWindow();
    }

    private void UpdateGame()
    {
        player1.Update(walls);
        player2.Update(walls);

        player1.UpdateBullet(walls);
        player2.UpdateBullet(walls);

        //bullet vs tank
        if (player1.bullet.active &&
            Raylib.CheckCollisionPointRec(player1.bullet.position, player2.GetRect()))
        {
            player1Score++;
            Reset();
        }

        if (player2.bullet.active &&
            Raylib.CheckCollisionPointRec(player2.bullet.position, player1.GetRect()))
        {
            player2Score++;
            Reset();
        }
    }

    void Reset()
    {
        player1.position = new Vector2(200, 400);
        player2.position = new Vector2(800, 400);

        player1.bullet.active = false;
        player2.bullet.active = false;
    }

    private void DrawGame()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DarkGreen);

        foreach (Wall wall in walls)
            wall.Draw();

        player1.Draw();
        player2.Draw();

        player1.DrawBullet();
        player2.DrawBullet();

        Raylib.DrawText(player1Score.ToString(), 400, 20, 40, Color.White);
        Raylib.DrawText(player2Score.ToString(), 550, 20, 40, Color.White);

        Raylib.EndDrawing();
    }
}