using Raylib_cs;
using System.Numerics;

namespace LunarLander
{
    internal class Ship
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool EngineOn;
        public float Fuel;
        public bool GameOver;
        public bool Won;

        private const float ENGINE_FORCE = 18.0f;
        private const float FUEL_CONSUMPTION = 25.0f;
        private const float LATERAL_FORCE = 10.0f;
        private const float MAX_LANDING_SPEED = 7.0f;

        public Ship(float startX, float startY)
        {
            Position = new Vector2(startX, startY);
            Velocity = Vector2.Zero;
            Fuel = 200.0f;
        }

        public void Update(float delta, float gravity, List<Rectangle> platforms)
        {
            if (GameOver) return;

            //painovoima
            Velocity.Y += gravity * delta;
            EngineOn = false;

            //päämoottori
            if ((Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W)) && Fuel > 0)
            {
                Velocity.Y -= ENGINE_FORCE * delta;
                Fuel -= FUEL_CONSUMPTION * delta;
                EngineOn = true;
            }

            //sivumoottorit
            if ((Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A)) && Fuel > 0)
            {
                Velocity.X -= LATERAL_FORCE * delta;
                Fuel -= (FUEL_CONSUMPTION / 2) * delta;
            }
            if ((Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) && Fuel > 0)
            {
                Velocity.X += LATERAL_FORCE * delta;
                Fuel -= (FUEL_CONSUMPTION / 2) * delta;
            }

            Fuel = Math.Max(Fuel, 0);
            Position += Velocity * delta;

            int sw = Raylib.GetScreenWidth();
            if (Position.X < 8) { Position.X = 8; Velocity.X = 0; }
            if (Position.X > sw - 8) { Position.X = sw - 8; Velocity.X = 0; }

            foreach (var plat in platforms)
            {
                float shipBottom = Position.Y + 10;
                bool withinX = Position.X >= plat.X && Position.X <= plat.X + plat.Width;

                if (withinX && shipBottom >= plat.Y && shipBottom <= plat.Y + plat.Height + Math.Abs(Velocity.Y) * delta + 2)
                {
                    GameOver = true;
                    Won = Velocity.Length() <= MAX_LANDING_SPEED;
                    Position.Y = plat.Y - 10;
                    Velocity = Vector2.Zero;
                    return;
                }
            }
        }

        public void Draw(List<Rectangle> platforms, int screenWidth, int screenHeight)
        {
            foreach (var plat in platforms)
                DrawPlatform(plat);

            if (GameOver && !Won)
            {
                Raylib.DrawRectangle((int)Position.X - 14, (int)Position.Y - 14, 28, 28, Color.Orange);
                Raylib.DrawRectangle((int)Position.X - 8, (int)Position.Y - 8, 16, 16, Color.Red);
                Raylib.DrawRectangle((int)Position.X - 4, (int)Position.Y - 4, 8, 8, Color.Yellow);
            }
            else
            {
                DrawShip();
                if (EngineOn) DrawFlame();
            }

            DrawFuelBar(screenWidth);
            DrawHUD(screenWidth, screenHeight);
        }

        private void DrawShip()
        {
            int x = (int)Position.X;
            int y = (int)Position.Y;

            //raketti on suorakulmioina tehtävänannon mukaan
            Raylib.DrawRectangle(x - 6, y - 14, 12, 24, new Color(60, 100, 200, 255));

            Raylib.DrawRectangle(x - 3, y - 20, 6, 6, new Color(180, 210, 255, 255));
  
            Raylib.DrawRectangle(x - 12, y + 4, 6, 8, new Color(40, 70, 160, 255));
      
            Raylib.DrawRectangle(x + 6, y + 4, 6, 8, new Color(40, 70, 160, 255));

            Raylib.DrawRectangle(x - 4, y + 10, 8, 4, new Color(20, 20, 60, 255));
        }

        private void DrawFlame()
        {
            int x = (int)Position.X;
            int y = (int)Position.Y;
            int f = Raylib.GetRandomValue(6, 16);

            Raylib.DrawRectangle(x - 5, y + 14, 10, f, Color.Yellow);
            Raylib.DrawRectangle(x - 3, y + 14, 6, f - 3, Color.Orange);
            Raylib.DrawRectangle(x - 1, y + 14, 2, f - 6, Color.White);
        }

        private void DrawPlatform(Rectangle plat)
        {
            int px = (int)plat.X;
            int py = (int)plat.Y;
            int pw = (int)plat.Width;
            int ph = (int)plat.Height;

            int stripeW = 10;
            for (int i = 0; i < pw; i += stripeW * 2)
            {
                Raylib.DrawRectangle(px + i, py, Math.Min(stripeW, pw - i), ph, Color.Yellow);
                Raylib.DrawRectangle(px + i + stripeW, py, Math.Min(stripeW, pw - i - stripeW), ph, Color.Black);
            }
            Raylib.DrawRectangleLines(px, py, pw, ph, Color.White);

            bool blink = (Raylib.GetTime() % 1.0) < 0.5;
            if (blink)
            {
                Raylib.DrawCircle(px + 6, py + ph / 2, 4, Color.Red);
                Raylib.DrawCircle(px + pw - 6, py + ph / 2, 4, Color.Red);
            }
        }

        private void DrawFuelBar(int screenWidth)
        {
            int barX = 8, barY = 8, barH = 16;
            int labelW = Raylib.MeasureText("FUEL", 16);
            int fillX = barX + labelW + 6;
            int fillW = 180;

            Raylib.DrawRectangle(barX - 2, barY - 2, labelW + fillW + 12, barH + 4, Color.Black);
            Raylib.DrawText("FUEL", barX, barY, 16, Color.Yellow);
            Raylib.DrawRectangle(fillX, barY, fillW, barH, new Color(50, 0, 0, 255));

            int filled = (int)(fillW * (Fuel / 100f));
            Color fc = Fuel > 30 ? Color.Red : (Fuel > 10 ? Color.Orange : Color.DarkGray);
            for (int i = 0; i < filled; i += 4)
                Raylib.DrawRectangle(fillX + i, barY, 3, barH, fc);

            Raylib.DrawRectangleLines(fillX, barY, fillW, barH, Color.White);
        }

        private void DrawHUD(int screenWidth, int screenHeight)
        {
            float speed = Velocity.Length();
            Color sc = speed > 4.0f ? Color.Red : Color.Green;
            Raylib.DrawText($"VY: {Velocity.Y:F1}", 10, 30, 14, sc);
            Raylib.DrawText($"VX: {Velocity.X:F1}", 10, 46, 14, Color.LightGray);

            if (!GameOver) return;

            string msg = Won ? "ONNISTUNUT LASKEUTUMINEN!" : "ALUS TUHOUTUI!";
            Color color = Won ? Color.Green : Color.Red;
            int fs = 30;
            int tw = Raylib.MeasureText(msg, fs);
            int mx = screenWidth / 2 - tw / 2;
            int my = screenHeight / 2 - 24;

            Raylib.DrawRectangle(mx - 12, my, tw + 24, 48, Color.Black);
            Raylib.DrawRectangleLines(mx - 12, my, tw + 24, 48, color);
            Raylib.DrawText(msg, mx, my + 9, fs, color);

            string hint = "Paina R aloittaaksesi uudelleen";
            int hw = Raylib.MeasureText(hint, 16);
            Raylib.DrawText(hint, screenWidth / 2 - hw / 2, my + 56, 16, Color.White);
        }
    }
}