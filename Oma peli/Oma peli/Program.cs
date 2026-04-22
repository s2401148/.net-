using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System;

class Program
{
    /* Use an enum for Type 
     * That way nobody has to remember what number means what
     */
    struct Item
    {
        public Rectangle Rect;
        public Color Color;
        public int Type; 
        public bool Active;
    }

    /* I would like to see different classes
     * and such. Not just everything in Main
     * Otherwise the structure is okay.
     */
    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;
        Raylib.InitWindow(screenWidth, screenHeight, "Catch & Dodge Game");
        Raylib.InitAudioDevice();

        /* The github did not have these images and sounds */
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

                /* Here you can use a Clamp to make it clear
                 * what is being done:
                 */
                player.X = Math.Clamp(player.X, 0, screenWidth - player.Width);

                if (player.X < 0) player.X = 0;
                if (player.X > screenWidth - player.Width) player.X = screenWidth - player.Width;

                /* I think it is more elegant to use 
                 * float lastSpawnTime and compare that to 
                 * Raylib.GetElapsed() to know how much time has passed.
                 * That way you don't have to worry about the Timer
                 */ 
                spawnTimer += dt;
                if (spawnTimer > 0.8f)
                {
                    /* Is 1 good or bad? This is why should use enums */
                    int type = rng.Next(0, 10) > 7 ? 1 : 0;
                    /* For some reason AI does this. This should not be
                     * done for several reasons:
                     * 1) Items can be created differently across the code
                     * 2) If a variable is added to Item class and you forget to set it here you don't get any errors
                     * 3) You need to type more and some logic leaks outside the class, like the Color selection here.
                     * 
                     * Write and use a constructor instead.
                     */
                    
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
            /* Putting multiple commands on a single line servers no 
             * purpose and makes things look messy. 
             */
            else if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                score = 0; lives = 3; items.Clear(); gameOver = false;
            }

            /* Since the code already cleanly separates Update and Draw
             * they could as well be in functions
             * void Update()
             * void Draw()
             */

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            if (!gameOver)
            {
                foreach (var item in items)
                {
					/* Here the item drawing logic is in the main loop
                    * and depends on the type of the item.
                    * This logic should be in the class itself
                    * and here should just be
                    * item.Draw();
                    */
                    if (item.Active)
                    {
                        
                        if (item.Type == 0) Raylib.DrawCircle((int)item.Rect.X + 15, (int)item.Rect.Y + 15, 12, item.Color);
                        else Raylib.DrawRectangleRec(item.Rect, item.Color);
                    }
                }
                /* Same for player. Move class dependent logic to be inside class
                 */
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