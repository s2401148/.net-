using Raylib_cs;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        Program pong = new Program();
        pong.RunGame();
    }

    
    Vector2 player1;
    Vector2 player2;

    int player1Score = 0;
    int player2Score = 0;

    Vector2 playerSize = new Vector2(40, 200);
    float playerSpeed = 800;

    Vector2 ballPosition;
    Vector2 ballDirection;
    float ballSpeed = 600;

    void RunGame()
    {
        Raylib.InitWindow(1000, 800, "Pong");
        Raylib.SetTargetFPS(60);

        int ScreenWidth = Raylib.GetScreenWidth();
        int ScreenHeight = Raylib.GetScreenHeight();

        int playerToWall = 60;

        player1 = new Vector2(
            playerToWall,
            ScreenHeight / 2 - playerSize.Y / 2
        );

        player2 = new Vector2(
            ScreenWidth - playerSize.X - playerToWall,
            ScreenHeight / 2 - playerSize.Y / 2
        );

        ballPosition = Raylib.GetScreenCenter();
        ballDirection = Vector2.Normalize(new Vector2(1, 0.5f));

        while (!Raylib.WindowShouldClose())
        {
            float dt = Raylib.GetFrameTime();

            UpdatePlayers(dt);
            UpdateBall(dt);
            DrawGame();
        }

        Raylib.CloseWindow();
    }

    void UpdatePlayers(float dt)
    {
        int ScreenHeight = Raylib.GetScreenHeight();

        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            player1.Y -= playerSpeed * dt;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            player1.Y += playerSpeed * dt;
        }

        if (Raylib.IsKeyDown(KeyboardKey.Up))
        {
            player2.Y -= playerSpeed * dt;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.Down))
        {
            player2.Y += playerSpeed * dt;
        }

        if (player1.Y < 0) player1.Y = 0;
        if (player1.Y + playerSize.Y > ScreenHeight)
            player1.Y = ScreenHeight - playerSize.Y;

        if (player2.Y < 0) player2.Y = 0;
        if (player2.Y + playerSize.Y > ScreenHeight)
            player2.Y = ScreenHeight - playerSize.Y;
    }

    void UpdateBall(float dt)
    {
        int ScreenWidth = Raylib.GetScreenWidth();
        int ScreenHeight = Raylib.GetScreenHeight();

        ballPosition += ballDirection * ballSpeed * dt;

        if (ballPosition.Y < 0 || ballPosition.Y > ScreenHeight)
        {
            ballDirection.Y *= -1;
        }

        Rectangle player1Rect = new Rectangle(player1, playerSize);
        Rectangle player2Rect = new Rectangle(player2, playerSize);

        //törmäys pelaajiin
        if (Raylib.CheckCollisionPointRec(ballPosition, player1Rect))
        {
            ballDirection.X *= -1;
            ballPosition.X = player1.X + playerSize.X + 1;
        }

        if (Raylib.CheckCollisionPointRec(ballPosition, player2Rect))
        {
            ballDirection.X *= -1;
            ballPosition.X = player2.X - 1;
        }

        //piste
        if (ballPosition.X < 0)
        {
            player2Score++;
            ResetBall();
        }
        else if (ballPosition.X > ScreenWidth)
        {
            player1Score++;
            ResetBall();
        }
    }

    void ResetBall()
    {
        ballPosition = Raylib.GetScreenCenter();
        ballDirection = Vector2.Normalize(new Vector2(-1, 0.5f));
    }

    void DrawGame()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        //pelaajat
        Raylib.DrawRectangleV(player1, playerSize, Color.White);
        Raylib.DrawRectangleV(player2, playerSize, Color.White);

        //pallo
        Raylib.DrawCircleV(ballPosition, 8, Color.White);

        //pisteet
        Raylib.DrawText(player1Score.ToString(), 400, 40, 40, Color.White);
        Raylib.DrawText(player2Score.ToString(), 560, 40, 40, Color.White);

        Raylib.EndDrawing();
    }
}