using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System;

class Program
{
    struct Item
    {
        public Rectangle Rect;
        public Color Color;
        public int Type; 
        public bool Active;
    }

    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;
        Raylib.InitWindow(screenWidth, screenHeight, "Catch & Dodge Game");
        Raylib.InitAudioDevice();

    
        Texture2D playerTex = Raylib.LoadTexture("player.png");
     
        Raylib.SetTextureFilter(playerTex, TextureFilter.Point);

        Sound collectSound = Raylib.LoadSound("collect.wav");
        Sound explodeSound = Raylib.LoadSound("explode.wav");

        float scale = 2.0f;
        
        Rectangle player = new Rectangle(
            screenWidth / 2 - (playerTex.Width * scale) / 2,
            screenHeight - (playerTex.Height * scale) - 10,
            playerTex.Width * scale,
            playerTex.Height * scale
        );

        int score = 0;
        int lives = 3;
        bool gameOver = false;

        List<Item> items = new List<Item>();
        Random rng = new Random();
        float spawnTimer = 0;

        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            if (!gameOver)
            {
                float dt = Raylib.GetFrameTime();

                player.X = Raylib.GetMouseX() - player.Width / 2;

                if (player.X < 0) player.X = 0;
                if (player.X > screenWidth - player.Width) player.X = screenWidth - player.Width;

                spawnTimer += dt;
                if (spawnTimer > 0.8f)
                {
                    int type = rng.Next(0, 10) > 7 ? 1 : 0;
                    items.Add(new Item
                    {
                        Rect = new Rectangle(rng.Next(0, screenWidth - 30), -30, 30, 30),
                        Type = type,
                        Color = (type == 0) ? Color.Gold : Color.Red,
                        Active = true
                    });
                    spawnTimer = 0;
                }

                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    item.Rect.Y += 250 * dt;

                    if (item.Active && Raylib.CheckCollisionRecs(player, item.Rect))
                    {
                        item.Active = false;
                        if (item.Type == 0)
                        {
                            score += 10;
                            Raylib.PlaySound(collectSound);
                        }
                        else
                        {
                            lives--;
                            Raylib.PlaySound(explodeSound);
                            if (lives <= 0) gameOver = true;
                        }
                    }

                    if (item.Rect.Y > screenHeight) item.Active = false;
                    items[i] = item;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                score = 0; lives = 3; items.Clear(); gameOver = false;
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            if (!gameOver)
            {
                foreach (var item in items)
                {
                    if (item.Active)
                    {
                        if (item.Type == 0) Raylib.DrawCircle((int)item.Rect.X + 15, (int)item.Rect.Y + 15, 12, item.Color);
                        else Raylib.DrawRectangleRec(item.Rect, item.Color);
                    }
                }
                if (playerTex.Id > 0)
                {
                    Vector2 position = new Vector2(player.X, player.Y);
                    Raylib.DrawTextureEx(playerTex, position, 0.0f, scale, Color.White);
                }
                else
                {
                    Raylib.DrawRectangleRec(player, Color.Green);
                }

                Raylib.DrawText($"Score: {score}", 10, 10, 25, Color.White);
                Raylib.DrawText($"Lives: {lives}", 10, 40, 25, Color.Red);
            }
            else
            {
                Raylib.DrawText("GAME OVER", screenWidth / 2 - 100, screenHeight / 2 - 20, 40, Color.White);
                Raylib.DrawText($"Final Score: {score}", screenWidth / 2 - 80, screenHeight / 2 + 30, 20, Color.Gray);
                Raylib.DrawText("Press R to Restart", screenWidth / 2 - 90, screenHeight / 2 + 60, 20, Color.Yellow);
            }

            Raylib.EndDrawing();
        }

        Raylib.UnloadTexture(playerTex);
        Raylib.UnloadSound(collectSound);
        Raylib.UnloadSound(explodeSound);
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}