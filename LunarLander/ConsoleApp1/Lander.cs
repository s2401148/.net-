using Raylib_cs;
using System.Numerics;

namespace LunarLander
{
    internal class Lander
    {
        static void Main(string[] args)
        {
            Lander game = new Lander();
            game.Init();
            game.GameLoop();
        }

        private int screenWidth = 800;
        private int screenHeight = 500;
        private const float GRAVITY = 4.0f;

        private float delta;
        private Ship ship;
        private Vector2[] peaks;
        private List<Rectangle> platforms = new();
        private Random rng = new Random();

        void Init()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Lunar Lander");
            Raylib.SetTargetFPS(60);
            Reset();
        }

        void Reset()
        {
            GeneratePlatforms();
            GeneratePeaks();
            ship = new Ship(screenWidth / 2f, 60f);
        }

        void GeneratePlatforms()
        {
            platforms.Clear();
            int platW = 160;
            int platH = 12;
            int count = 4;
            int margin = 80;

            List<int> usedX = new();
            for (int i = 0; i < count; i++)
            {
                int x, attempts = 0;
                do
                {
                    x = rng.Next(margin, screenWidth - margin - platW);
                    attempts++;
                } while (usedX.Exists(ux => Math.Abs(ux - x) < platW + 40) && attempts < 50);

                usedX.Add(x);

                
                int y;
                if (i == count - 1)
                    y = screenHeight - 60;
                else
                    y = rng.Next(screenHeight / 2, screenHeight - 120);

                platforms.Add(new Rectangle(x, y, platW, platH));
            }
        }

        void GeneratePeaks()
        {
            int count = 42;
            peaks = new Vector2[count];
            float h = screenHeight * 0.45f;
            float step = (float)screenWidth / (count - 1);

            for (int i = 0; i < count; i++)
            {
                peaks[i] = new Vector2(step * i, h);
                h += rng.Next(-28, 28);
                h = Math.Clamp(h, 80, screenHeight - 80);
            }
        }

        void GameLoop()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }
            Raylib.CloseWindow();
        }

        void Update()
        {
            delta = Raylib.GetFrameTime();
            if (ship.GameOver && Raylib.IsKeyPressed(KeyboardKey.R))
            {
                Reset();
                return;
            }
            ship.Update(delta, GRAVITY, platforms);
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(8, 8, 24, 255));

            DrawStars();
            DrawMountains();
            ship.Draw(platforms, screenWidth, screenHeight);

            if (!ship.GameOver)
                Raylib.DrawText(
                    "W = moottori   <>AD = sivut   R = uudelleen",
                    10, screenHeight - 20, 13,
                    new Color(120, 120, 140, 200));

            Raylib.EndDrawing();
        }

        void DrawStars()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = (i * 83 + 17) % screenWidth;
                int y = (i * 47 + 11) % (screenHeight - 40);
                if (i % 7 == 0)
                    Raylib.DrawRectangle(x, y, 2, 2, new Color(255, 255, 220, 200));
                else
                    Raylib.DrawPixel(x, y, new Color(180, 180, 220, 160));
            }
        }

        void DrawMountains()
        {
            if (peaks == null || peaks.Length < 2) return;

            for (int i = 1; i < peaks.Length; i++)
            {
                Vector2 a = peaks[i - 1];
                Vector2 b = peaks[i];
                float bottom = screenHeight;

                Raylib.DrawTriangle(
                    new Vector2(a.X, bottom), a, b,
                    new Color(20, 22, 45, 255));
                Raylib.DrawTriangle(
                    new Vector2(a.X, bottom), b,
                    new Vector2(b.X, bottom),
                    new Color(20, 22, 45, 255));

                Raylib.DrawLineEx(a, b, 1.5f, new Color(200, 200, 230, 220));
            }
        }
    }
}