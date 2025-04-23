using SkiaSharp;

public class Captcha
{
    const string Letters = "2346789ABCDEFGHJKLMNPRTUVWXYZ";

    public static string GenerateCaptchaCode(int length = 4)
    {
        Random rand = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => Letters[rand.Next(Letters.Length)])
            .ToArray());
    }

    public static CaptchaResult GenerateCaptchaImage(int width, int height, string captchaCode)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        Random rand = new Random();

        // Fondo blanco
        canvas.Clear(SKColors.White);

        // Dibujar texto distorsionado y rotado
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            TextSize = height * 0.7f,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        };

        float xPosition = 10; // Posición inicial en X

        for (int i = 0; i < captchaCode.Length; i++)
        {
            paint.Color = GetRandomDeepColor();
            float angle = rand.Next(-25, 25); // Rotación aleatoria entre -25 y 25 grados
            float yOffset = (float)(Math.Sin(i * 0.5) * 10); // Ondulación del texto

            canvas.Save();
            canvas.RotateDegrees(angle, xPosition, height / 2);
            canvas.DrawText(captchaCode[i].ToString(), xPosition, height / 2 + yOffset, paint);
            canvas.Restore();

            xPosition += width / captchaCode.Length; // Espacio entre letras
        }

        // Añadir curvas de ruido en lugar de solo líneas rectas
        DrawCurvedNoiseLines(canvas, width, height, rand);

        // Añadir puntos de ruido
        DrawNoiseDots(canvas, width, height, rand);

        // Guardar la imagen en un byte array
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return new CaptchaResult
        {
            CaptchaCode = captchaCode,
            CaptchaByteData = data.ToArray(),
            Timestamp = DateTime.Now
        };
    }

    // Método para dibujar curvas de ruido
    private static void DrawCurvedNoiseLines(SKCanvas canvas, int width, int height, Random rand)
    {
        var linePaint = new SKPaint
        {
            Color = SKColors.Gray.WithAlpha(100),
            StrokeWidth = 2,
            IsAntialias = true
        };

        for (int i = 0; i < 3; i++) // Dibujar 3 curvas aleatorias
        {
            var path = new SKPath();
            path.MoveTo(rand.Next(0, width), rand.Next(0, height));

            for (int j = 0; j < 4; j++) // Dibujar una curva usando 4 puntos de control
            {
                path.QuadTo(rand.Next(0, width), rand.Next(0, height), rand.Next(0, width), rand.Next(0, height));
            }

            canvas.DrawPath(path, linePaint);
        }
    }

    // Método para dibujar puntos de ruido
    private static void DrawNoiseDots(SKCanvas canvas, int width, int height, Random rand)
    {
        var dotPaint = new SKPaint
        {
            Color = SKColors.Gray,
            StrokeWidth = 2
        };

        for (int i = 0; i < 200; i++) // Dibujar 200 puntos aleatorios
        {
            var x = rand.Next(0, width);
            var y = rand.Next(0, height);
            dotPaint.Color = GetRandomDeepColor().WithAlpha(150); // Colores variados y semitransparentes
            canvas.DrawPoint(x, y, dotPaint);
        }
    }

    private static SKColor GetRandomDeepColor()
    {
        Random rand = new Random();
        return new SKColor(
            (byte)rand.Next(0, 128),
            (byte)rand.Next(0, 128),
            (byte)rand.Next(0, 128)
        );
    }

    public class CaptchaResult
    {
        public string CaptchaCode { get; set; }
        public byte[] CaptchaByteData { get; set; }
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
    }
}