using DemoTest.Audio;
using DemoTest.Elements;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DemoTest;

/// <summary>
/// MainForm is the primary form for the demo application.
/// The font used is a bitmap font from Zingot Games: https://opengameart.org/content/bitmap-font-pack
/// The music used is Complications (1990) by Tomas Danko/Fairlight
/// </summary>
public partial class MainForm : Form
{
    record struct Point3D(float X, float Y, float Z);

    // Configuration constants for the demo
    readonly string scrollText = "Welcome to the 90s demo project in C#";
    readonly string subScrollerText = "The 1990s marked the golden age of the demoscene – with iconic groups like Future Crew, The Black Lotus, and Fairlight setting the standard. It was more than just digital art: it was a movement driven by creativity, competition, and a deep love for making machines do the impossible.";
    readonly string leftBottomCornerText = "(C) KJELL ARNE BRUDVIK";
    readonly string musicFile = "assets/complications.mp3";
    readonly Bitmap fontBitmap = new("assets/nfo-font_6x8.png");
    readonly int glyphWidth = 6, glyphHeight = 8;
    readonly int charsPerRow = 200;

    // Collections for various elements in the demo
    readonly Queue<float> waveformHistory = new();   
    readonly List<Orbiter> orbiters = [];
    readonly List<Particle> particles = [];
    readonly List<WarpParticle> warpParticles = [];
    readonly List<PointF> stars = [];
    readonly Random rng = new();

    // Graphics and rendering variables
    private MusicVisualizer? music;
    private System.Windows.Forms.Timer? timer;
    private int scrollX;
    private float time;      
    private float angleX = 0, angleY = 0, angleZ = 0;
    private Point3D[]? cubeVertices;
    private int[,]? cubeEdges;
    private float flashTime = 0f;
    private bool isFlashing = false;
    private Bitmap? renderBuffer;
    private Graphics? bufferGraphics;
    
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the KeyDown event for the main form. Closes the form when the Escape key is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainFormKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.Close();
        }
    }

    /// <summary>
    /// Handles the Load event for the main form. Initializes various components such as graphics area, background music, starfield, cube, orbiters, and timer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainFormLoad(object sender, EventArgs e)
    {
        // Initialize the graphics area
        InitializeGraphicsArea();

        // Initialize background music
        InitializeBackgroundMusic();

        // Initialize starfield
        InitializeStarField();

        // Initialize the cube.
        InitializeCube();

        // Initialize orbiters
        InitializeOrbiters();

        // Initialize the timer for the animation loop.
        InitializeTimer();
    }

    /// <summary>
    /// Initializes the graphics area for rendering. Sets properties like double buffering,
    /// background color, window state, and key preview.
    /// </summary>
    private void InitializeGraphicsArea()
    {
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        this.UpdateStyles();
        this.BackColor = Color.Black;
        this.WindowState = FormWindowState.Maximized;
        this.KeyPreview = true;
        KeyDown += MainFormKeyDown;
        renderBuffer = new Bitmap(this.Width, this.Height);
        bufferGraphics = Graphics.FromImage(renderBuffer);
        scrollX = this.Width;
    }

    /// <summary>
    /// Initializes the background music player with a specified audio file.
    /// </summary>
    private void InitializeBackgroundMusic()
    {
        music = new MusicVisualizer(musicFile);
        music.Play();
    }

    /// <summary>
    /// Initializes the timer for the animation loop. Using an interval of
    /// 16 milliseconds to achieve approximately 60 FPS.
    /// </summary>
    private void InitializeTimer()
    {
        timer = new System.Windows.Forms.Timer
        {
            Interval = 16
        };
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    /// <summary>
    /// Initializes the orbiters with varying radii, speeds, and phases.
    /// </summary>
    private void InitializeOrbiters()
    {
        for (int i = 0; i < 5; i++)
        {
            orbiters.Add(new Orbiter
            {
                Radius = 2.5f + i * 0.5f,
                Speed = 0.5f + i * 0.1f,
                Phase = i * 2f
            });
        }
    }

    /// <summary>
    /// Initializes the starfield with random star positions.
    /// </summary>
    private void InitializeStarField()
    {
        for (int i = 0; i < 200; i++)
        {
            stars.Add(new PointF(rng.Next(this.Width), rng.Next(this.Height)));
        }
    }

    /// <summary>
    /// Initializes the cube vertices and edges for 3D rendering.
    /// </summary>
    private void InitializeCube()
    {
        cubeVertices =
        [
            new(-1, -1, -1), new(1, -1, -1), new(1, 1, -1), new(-1, 1, -1),
            new(-1, -1, 1), new(1, -1, 1), new(1, 1, 1), new(-1, 1, 1)
        ];

        cubeEdges = new int[,]
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };
    }

    /// <summary>
    /// Timer tick event handler that updates the animation state.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Timer_Tick(object? sender, EventArgs e)
    {
        time += 0.1f;
        scrollX -= 2;

        for (int i = 0; i < 3; i++)
        {
            float angle = (float)(rng.NextDouble() * Math.PI * 2);
            warpParticles.Add(new WarpParticle
            {
                Angle = angle,
                Radius = 400f + (float)(rng.NextDouble() * 100f),
                Speed = 3f + (float)(rng.NextDouble() * 1.5f),
                SpiralTightness = 0.02f + (float)(rng.NextDouble() * 0.03f),
                Life = 1f,
                Color = Color.FromArgb(255, 100 + rng.Next(100), 255, 255)
            });
        }

        Invalidate();
    }

    /// <summary>
    /// Draws bitmap text using a preloaded bitmap font.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="text"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="scale"></param>
    private void DrawBitmapText(Graphics g, string text, float x, float y, float scale = 1f)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c < 32 || c > 126) continue;

            int index = c - 32;
            int sx = (index % charsPerRow) * glyphWidth;
            int sy = (index / charsPerRow) * glyphHeight;

            Rectangle src = new(sx, sy, glyphWidth, glyphHeight);
            RectangleF dest = new(x + i * glyphWidth * scale, y, glyphWidth * scale, glyphHeight * scale);

            g.DrawImage(fontBitmap, dest, src, GraphicsUnit.Pixel);
        }
    }

    /// <summary>
    /// Projects a 3D point onto a 2D plane using rotation angles and scaling.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <returns></returns>
    private PointF Project(Point3D p, float centerX, float centerY)
    {
        float cosa = (float)Math.Cos(angleX), sina = (float)Math.Sin(angleX);
        float y = p.Y * cosa - p.Z * sina;
        float z = p.Y * sina + p.Z * cosa;
        float cosb = (float)Math.Cos(angleY), sinb = (float)Math.Sin(angleY);
        float x = p.X * cosb + z * sinb;
        z = -p.X * sinb + z * cosb;
        float cosc = (float)Math.Cos(angleZ), sinc = (float)Math.Sin(angleZ);
        float x2 = x * cosc - y * sinc;
        float y2 = x * sinc + y * cosc;
        float baseScale = 300f;
        float pulse = (float)(Math.Sin(time * 0.5f) * 100f);
        float scale = (baseScale + pulse) / (z + 5f);
        return new PointF(x2 * scale + centerX, y2 * scale + centerY);
    }

    /// <summary>
    /// Draws bitmap text with a sine wave effect, creating a wavy motion.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="text"></param>
    /// <param name="baseX"></param>
    /// <param name="baseY"></param>
    /// <param name="time"></param>
    /// <param name="scale"></param>
    private void DrawSineBitmapText(Graphics g, string text, float baseX, float baseY, float time, float scale = 1f)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c < 32 || c > 126) continue;

            float yOffset = (float)(Math.Sin(time + i * 0.5) * 10);
            float x = baseX + i * glyphWidth * scale;
            float y = baseY + yOffset;

            int index = c - 32;
            int sx = (index % charsPerRow) * glyphWidth;
            int sy = (index / charsPerRow) * glyphHeight;

            Rectangle src = new(sx, sy, glyphWidth, glyphHeight);
            RectangleF dest = new(x, y, glyphWidth * scale, glyphHeight * scale);

            g.DrawImage(fontBitmap, dest, src, GraphicsUnit.Pixel);
        }
    }

    /// <summary>
    /// Draws scrolling text, moving left across the screen.
    /// </summary>
    /// <param name="g"></param>
    private void DrawScrollingText(Graphics g)
    {
        string scrollText = subScrollerText.ToUpper();
        float charsPerSecond = 3f;
        int visibleChars = Math.Min((int)(time * charsPerSecond), scrollText.Length);
        float textY = renderBuffer!.Height * 0.6f + 35;
        float charWidth = 8f * 3f;
        float scrollSpeed = 30f; // pixels per second

        // Smoothly move the text left as time increases
        float textX = renderBuffer.Width - (time * scrollSpeed);

        // Only draw up to visibleChars
        string visibleText = scrollText[..visibleChars];
        DrawBitmapText(g, visibleText, textX, textY, 3f);

        // Loop when the text is fully off-screen
        float totalTextWidth = scrollText.Length * charWidth;
        if (textX + totalTextWidth < 0)
        {
            time = 0f;
        }
    }

    /// <summary>
    /// Draw cube in 3D space, projecting its vertices onto a 2D plane.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    private void DrawCube(Graphics g, float centerX, float centerY)
    {
        float cubeScale = 1f;

        PointF[] projected = new PointF[cubeVertices!.Length];
        for (int i = 0; i < cubeVertices.Length; i++)
        {
            var scaled = new Point3D(
                cubeVertices[i].X * cubeScale,
                cubeVertices[i].Y * cubeScale,
                cubeVertices[i].Z * cubeScale
            );
            projected[i] = Project(scaled, centerX, centerY);
        }

        using var pen = new Pen(Color.Cyan, 2);
        for (int i = 0; i < cubeEdges!.GetLength(0); i++)
        {
            var a = projected[cubeEdges[i, 0]];
            var b = projected[cubeEdges[i, 1]];
            g.DrawLine(pen, a, b);
        }
    }

    /// <summary>
    /// Draw music bars based on the current music spectrum.
    /// </summary>
    /// <param name="g"></param>
    private void DrawMusicBars(Graphics g)
    {
        if (music?.Spectrum != null)
        {
            for (int i = 0; i < 64; i++)
            {
                float intensity = music.Spectrum[i] * 4000f;
                float barHeight = Math.Min(intensity, 150f);
                g.FillRectangle(Brushes.LimeGreen, 20 + i * 10, renderBuffer!.Height - barHeight - 10, 8, barHeight);
            }
        }
    }

    /// <summary>
    /// Draw orbiter orbs that orbit around the cube center.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="center2D"></param>
    private void DrawOrbs(Graphics g, float centerX, float centerY, PointF center2D)
    {
        foreach (var orb in orbiters)
        {
            float angle = time * orb.Speed + orb.Phase;
            float orbX = (float)(Math.Cos(angle) * orb.Radius);
            float orbY = (float)(Math.Sin(angle * 0.6f) * 1.0f);
            float orbZ = (float)(Math.Sin(angle) * orb.Radius);
            PointF orb2D = Project(new Point3D(orbX, orbY, orbZ), centerX, centerY);

            // Explosion detection (brute-force check)
            for (int i = 0; i < orbiters.Count; i++)
            {
                for (int j = i + 1; j < orbiters.Count; j++)
                {
                    // Skip if either trail is empty
                    if (orbiters[i].Trail.Count == 0 || orbiters[j].Trail.Count == 0)
                        continue; 

                    var pi = orbiters[i].Trail[0];
                    var pj = orbiters[j].Trail[0];
                    float dx = pi.X - pj.X;
                    float dy = pi.Y - pj.Y;
                    float distSq = dx * dx + dy * dy;

                    // If the distance squared is less than a threshold, spawn an explosion
                    if (distSq < 100)
                    {
                        SpawnExplosion(pi);
                    }
                }
            }

            // Save trail point
            orb.Trail.Insert(0, orb2D);
            if (orb.Trail.Count > 25) orb.Trail.RemoveAt(orb.Trail.Count - 1);

            // Draw trail
            for (int t = 0; t < orb.Trail.Count; t++)
            {
                float alpha = 1f - t / 25f;
                int size = 6 - t / 5;
                if (size < 1) size = 1;

                Color color = InterpolateColor(Color.White, Color.Yellow, alpha);
                using var trailBrush = new SolidBrush(Color.FromArgb((int)(alpha * 100), color));
                var pt = orb.Trail[t];
                g.FillEllipse(trailBrush, pt.X - size / 2, pt.Y - size / 2, size, size);
            }

            // Line to cube center
            using var linePen = new Pen(Color.FromArgb(100, 255, 255, 255), 1);
            g.DrawLine(linePen, center2D, orb2D);

            // Glow
            float pulse = (float)(Math.Sin(time * 2f + orb.Phase) * 0.5f + 0.5f);
            Color glowColor = InterpolateColor(Color.White, Color.Yellow, pulse);
            using var brush = new SolidBrush(glowColor);
            using var glowPen = new Pen(Color.FromArgb((int)(100 * pulse + 50), glowColor), 2);
            g.FillEllipse(brush, orb2D.X - 4, orb2D.Y - 4, 8, 8);
            g.DrawEllipse(glowPen, orb2D.X - 8, orb2D.Y - 8, 16, 16);
        }
    }

    /// <summary>
    /// Draw particles for the warp effect, simulating a burst of energy.
    /// </summary>
    /// <param name="g"></param>
    private void DrawWarpParticles(Graphics g)
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var p = particles[i];
            p.Position = new PointF(p.Position.X + p.Velocity.X, p.Position.Y + p.Velocity.Y);
            p.Life -= 0.03f;
            if (p.Life <= 0)
            {
                particles.RemoveAt(i);
                continue;
            }

            int alpha = (int)(p.Life * 255);
            using var brush = new SolidBrush(Color.FromArgb(alpha, p.Color));
            g.FillEllipse(brush, p.Position.X - 2, p.Position.Y - 2, 4, 4);
        }
    }

    /// <summary>
    /// Draw warp hole effect at the center of the screen.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    private void DrawWarpHole(Graphics g, float centerX, float centerY)
    {
        int pulseSize = (int)(20 + Math.Sin(time * 3) * 5);
        using var warpPen = new Pen(Color.Cyan, 2);

        warpPen.Color = Color.FromArgb(100 + (int)(Math.Sin(time * 2) * 50), 0, 255, 255);
        g.DrawEllipse(warpPen, centerX - pulseSize, centerY - pulseSize, pulseSize * 2, pulseSize * 2);

        for (int i = warpParticles.Count - 1; i >= 0; i--)
        {
            var wp = warpParticles[i];
            wp.Radius -= wp.Speed;
            wp.Angle += wp.SpiralTightness;
            wp.Life -= 0.015f;

            if (wp.Radius < 5 || wp.Life <= 0)
            {
                warpParticles.RemoveAt(i);
                continue;
            }

            float x = (float)(Math.Cos(wp.Angle) * wp.Radius) + centerX;
            float y = (float)(Math.Sin(wp.Angle) * wp.Radius) + centerY;

            int alpha = (int)(wp.Life * 200);
            int size = 2 + (int)(3 * wp.Life);
            using var b = new SolidBrush(Color.FromArgb(alpha, wp.Color));
            g.FillEllipse(b, x - size / 2, y - size / 2, size, size);
        }
    }

    /// <summary>
    /// Draw scrolling waveform visualization based on the music spectrum.
    /// </summary>
    /// <param name="g"></param>
    private void DrawScrollingWaveForm(Graphics g)
    {
        if (renderBuffer == null || music == null)
            return;

        float yBase = renderBuffer.Height * 0.9f;
        float xSpacing = renderBuffer.Width / 512f;

        if (music?.Spectrum != null)
        {
            float amplitude = 0f;
            for (int i = 0; i < 10; i++) amplitude += music.Spectrum[i];
            amplitude = Math.Clamp(amplitude * 500f, 0f, 1f);

            waveformHistory.Enqueue(amplitude);
            while (waveformHistory.Count > 512) waveformHistory.Dequeue();

            float[] waveformArray = [.. waveformHistory];
            for (int i = 1; i < waveformArray.Length; i++)
            {
                float x1 = renderBuffer.Width - (waveformArray.Length - i) * xSpacing;
                float y1 = yBase - waveformArray[i - 1] * 60f;
                float x2 = renderBuffer.Width - (waveformArray.Length - (i + 1)) * xSpacing;
                float y2 = yBase - waveformArray[i] * 60f;

                int alpha = (int)(255 * (i / (float)waveformArray.Length));
                int thickness = (int)(2 * (1 - i / (float)waveformArray.Length) + 1);
                using var pen2 = new Pen(Color.FromArgb(alpha, 0, 255, 255), thickness);
                g.DrawLine(pen2, x1, y1, x2, y2);
            }
        }
    }

    /// <summary>
    /// Draws 3D effects including a rotating cube, orbiters, and particle effects.
    /// </summary>
    /// <param name="g"></param>
    private void Draw3DEffects(Graphics g)
    {
        float centerX = this.Width / 2f;
        float centerY = this.Height / 2f;
        var center2D = Project(new Point3D(0, 0, 0), centerX, centerY);

        // Update angles for rotation
        angleX += 0.02f;
        angleY += 0.015f;
        angleZ += 0.01f;

        // Draw the music bars
        DrawMusicBars(g);

        // Draw the cube
        DrawCube(g, centerX, centerY);

        // Draw the bit map text in lower left corner
        DrawBitmapText(g, leftBottomCornerText, 50, renderBuffer!.Height - 40, 2f);

        // Draw the orbiters
        DrawOrbs(g, centerX, centerY, center2D);

        // Draw the particles around the warp hole
        DrawWarpParticles(g);

        // Draw the warp hole effect
        DrawWarpHole(g, centerX, centerY);

        // Draw the sine scroller for music visualization
        DrawScrollingWaveForm(g);

    }

    /// <summary>
    /// Spawns an explosion effect at the specified center point.
    /// </summary>
    /// <param name="center"></param>
    private void SpawnExplosion(PointF center)
    {
        isFlashing = true;
        flashTime = 1f;
        for (int i = 0; i < 20; i++)
        {
            float angle = (float)(rng.NextDouble() * Math.PI * 2);
            float speed = (float)(rng.NextDouble() * 2 + 1);
            var velocity = new PointF((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
            particles.Add(new Particle
            {
                Position = center,
                Velocity = velocity,
                Life = 1f,
                Color = Color.FromArgb(255, 255, rng.Next(200, 255), 0)
            });
        }
    }

    /// <summary>
    /// Interpolates between two colors based on a factor t (0 to 1).
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    private static Color InterpolateColor(Color a, Color b, float t)
    {
        int r = (int)(a.R + (b.R - a.R) * t);
        int g = (int)(a.G + (b.G - a.G) * t);
        int bC = (int)(a.B + (b.B - a.B) * t);
        return Color.FromArgb(r, g, bC);
    }

    /// <summary>
    /// Converts HSL color values to RGB.
    /// </summary>
    /// <param name="h"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    /// <returns></returns>
    private static Color HslToRgb(float h, float s, float l)
    {
        h %= 360;
        float c = (1 - Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = l - c / 2;
        float r1, g1, b1;

        if (h < 60) (r1, g1, b1) = (c, x, 0);
        else if (h < 120) (r1, g1, b1) = (x, c, 0);
        else if (h < 180) (r1, g1, b1) = (0, c, x);
        else if (h < 240) (r1, g1, b1) = (0, x, c);
        else if (h < 300) (r1, g1, b1) = (x, 0, c);
        else (r1, g1, b1) = (c, 0, x);

        return Color.FromArgb(
            (int)((r1 + m) * 255),
            (int)((g1 + m) * 255),
            (int)((b1 + m) * 255)
        );
    }

    /// <summary>
    /// Handles the Paint event to render the graphics.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
        if (renderBuffer == null || renderBuffer.Width != this.Width || renderBuffer.Height != this.Height)
        {
            renderBuffer?.Dispose();
            bufferGraphics?.Dispose();
            renderBuffer = new Bitmap(this.Width, this.Height);
            bufferGraphics = Graphics.FromImage(renderBuffer);
        }

        if (bufferGraphics == null)
            return;

        if (renderBuffer == null)
            return;

        bufferGraphics.Clear(Color.Black);

        // Draw white flash when a collision happens.
        DrawWhiteFlash(bufferGraphics);

        // Draw the floating Starfield
        DrawStarfield(bufferGraphics);

        // Draw the moving Raster Bars
        DrawRasterBars(bufferGraphics);

        // Draw scrolling text
        DrawScrollingText(bufferGraphics);

        // Draw 3D effects
        Draw3DEffects(bufferGraphics);

        // Sine Scroller with color cycling, white outline, and drop shadow
        DrawSineScrollerText(bufferGraphics);

        // Apply chromatic + shake
        ApplyChromaticAndShakeFast(e.Graphics);
    }

    /// <summary>
    /// Applies a chromatic aberration effect and camera shake to the rendered graphics.
    /// </summary>
    /// <param name="target"></param>
    private void ApplyChromaticAndShakeFast(Graphics target)
    {
        int width = renderBuffer!.Width;
        int height = renderBuffer.Height;

        int shakeAmount = isFlashing ? (int)(5 * flashTime * 2) : 0;
        int shakeX = rng.Next(-shakeAmount, shakeAmount + 1);
        int shakeY = rng.Next(-shakeAmount, shakeAmount + 1);
        int chromaOffset = isFlashing ? 2 : 0;

        // Lock original bitmap
        BitmapData srcData = renderBuffer.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        Bitmap combined = new(width, height, PixelFormat.Format32bppArgb);
        BitmapData dstData = combined.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        unsafe
        {
            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;

            int stride = srcData.Stride;

            for (int y = 0; y < height - chromaOffset; y++)
            {
                for (int x = 0; x < width - chromaOffset; x++)
                {
                    int srcIndex = y * stride + x * 4;

                    // RGB offset channels
                    int rIndex = (y) * stride + (x + chromaOffset) * 4;
                    int gIndex = (y) * stride + (x - chromaOffset + width) % width * 4;
                    int bIndex = (y + chromaOffset) * stride + x * 4;

                    byte r = srcPtr[rIndex + 2]; // Red
                    byte g = srcPtr[gIndex + 1]; // Green
                    byte b = srcPtr[bIndex + 0]; // Blue
                    byte a = srcPtr[srcIndex + 3];

                    dstPtr[srcIndex + 0] = b;
                    dstPtr[srcIndex + 1] = g;
                    dstPtr[srcIndex + 2] = r;
                    dstPtr[srcIndex + 3] = a;
                }
            }
        }

        renderBuffer.UnlockBits(srcData);
        combined.UnlockBits(dstData);

        // Apply camera shake offset
        target.DrawImage(combined, shakeX, shakeY);
        combined.Dispose();
    }

    /// <summary>
    /// Draws a white flash effect when a collision occurs.
    /// </summary>
    /// <param name="g"></param>
    private void DrawWhiteFlash(Graphics g)
    {
        if (isFlashing)
        {
            int alpha = (int)(flashTime * 255);
            using var flashBrush = new SolidBrush(Color.FromArgb(alpha, Color.White));
            g.FillRectangle(flashBrush, 0, 0, Width, Height);

            flashTime -= 0.10f; // fade speed
            if (flashTime <= 0)
            {
                flashTime = 0;
                isFlashing = false;
            }
        }
    }

    /// <summary>
    /// Draws a sine scroller text effect with color cycling, white outline, and drop shadow.
    /// </summary>
    /// <param name="g"></param>
    private void DrawSineScrollerText(Graphics g)
    {
        Font font = new("Consolas", 64, FontStyle.Bold);
        int centerY = this.Height / 2;
        int x = scrollX;
        for (int i = 0; i < scrollText.Length; i++)
        {
            char c = scrollText[i];
            float yOffset = (float)(Math.Sin(i * 0.5f + time) * 40);
            float hue = (time * 40 + i * 20) % 360;
            Color c1 = HslToRgb(hue, 1f, 0.5f);
            Color c2 = HslToRgb((hue + 60) % 360, 1f, 0.5f);

            using var gradient = new LinearGradientBrush(
                new RectangleF(x, centerY + yOffset, 40, 40),
                c1, c2, LinearGradientMode.Vertical
            );

            // Draw drop shadow
            using (var shadowBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
            {
                g.DrawString(c.ToString(), font, shadowBrush, x + 3, centerY + yOffset + 3);
            }

            // Draw white outline (8 directions)
            using (var outlineBrush = new SolidBrush(Color.White))
            {
                for (int ox = -2; ox <= 2; ox++)
                {
                    for (int oy = -2; oy <= 2; oy++)
                    {
                        if (ox == 0 && oy == 0) continue;
                        g.DrawString(c.ToString(), font, outlineBrush, x + ox, centerY + yOffset + oy);
                    }
                }
            }

            // Draw main gradient text
            g.DrawString(c.ToString(), font, gradient, x, centerY + yOffset);

            x += (int)g.MeasureString(c.ToString(), font).Width;
        }

        if (x < 0) scrollX = this.Width;
    }

    /// <summary>
    /// Draws a starfield effect with stars moving left across the screen.
    /// </summary>
    /// <param name="g"></param>
    private void DrawStarfield(Graphics g)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            var p = stars[i];
            g.FillRectangle(Brushes.White, p.X, p.Y, 2, 2);
            stars[i] = new PointF(p.X - 2, p.Y);
            if (stars[i].X < 0) stars[i] = new PointF(this.Width, rng.Next(this.Height));
        }
    }

    /// <summary>
    /// Draws raster bars with a sine wave effect, creating a moving pattern across the screen.
    /// </summary>
    /// <param name="g"></param>
    private void DrawRasterBars(Graphics g)
    {
        for (int i = 0; i < 6; i++)
        {
            float y = (float)(Math.Sin(time + i * 0.5) * 30) + this.Height / 2 + i * 10 + 50;
            using var brush = new SolidBrush(HslToRgb((time * 40 + i * 30) % 360, 1f, 0.5f));
            g.FillRectangle(brush, 0, y, this.Width, 8);
        }
    }
}
