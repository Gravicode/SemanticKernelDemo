using SkiaSharp;

namespace SemanticKernelDemo.Data
{
    public class GraphicsExtensions
    {
        public static void DrawCircle(ICanvas canvas, Color pen,
                                      float centerX, float centerY, float radius)
        {
          
            canvas.StrokeColor = pen;
            canvas.StrokeSize = 2;
            canvas.DrawEllipse(centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
        public static byte[] CropImage(byte[] Data)
        {
            // decode the bitmap stream
            var resourceBitmap = SKBitmap.Decode(Data);

            if (resourceBitmap != null)
            {
                var widthImg = Math.Min(resourceBitmap.Width, resourceBitmap.Height);
                var info = new SKImageInfo(widthImg, widthImg, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var resizedBitmap = resourceBitmap.Resize(info, SKFilterQuality.High); //Resize to the canvas
                using (var plainSkSurface = SKSurface.Create(info))
                {
                    var plainCanvas = plainSkSurface.Canvas;
                    var BackgroundColor = SKColors.White;
                    plainCanvas.Clear(BackgroundColor);

                    using (var paintInfo = new SKPaint())
                    {
                        paintInfo.IsAntialias = true;
                        plainCanvas.DrawBitmap(resourceBitmap, new SKPoint(0, 0), paintInfo);
                        //paintInfo.Typeface = SKTypeface.FromFamilyName("Calibri", SKFontStyle.Bold);
                        //paintInfo.TextSize = FontSize;
                        //paintInfo.Color = SKColors.Red;
                        //paintInfo.IsAntialias = true;

                        //var xToDraw = (ImageWidth - paintInfo.MeasureText(text)) / 2;
                        //var yToDraw = (ImageHeight - FontSize) / 2 + FontSize;
                        //plainCanvas.DrawText(text, xToDraw, yToDraw, paintInfo);
                    }
                    plainCanvas.Flush();

                    return plainSkSurface.Snapshot().Encode().ToArray();

                }
            }
            return default;
            //Bitmap sourceImg;
            //using (var ms = new MemoryStream(Data))
            //{
            //    sourceImg = new Bitmap(ms);
            //}
            //if (sourceImg.Width == sourceImg.Height)
            //{
            //    return Data;
            //}
            //var width = Math.Min(sourceImg.Width, sourceImg.Height);
            //var newImg = new Bitmap(width, width);
            //using (var newGraphic = System.Drawing.Graphics.FromImage(newImg))
            //{
            //    newGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            //    newGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //    newGraphic.Clear(System.Drawing.Color.Transparent);
            //    newGraphic.DrawImage(sourceImg, 0, 0, width, width);
            //    newGraphic.Flush();
            //}
            //var mem = new MemoryStream();
            //newImg.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
            //return mem.ToArray();
        }
       

        
        public static void FillCircle(ICanvas canvas, Color brush,
                                      float centerX, float centerY, float radius)
        {
            canvas.FillColor = brush;
            canvas.FillEllipse(centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        /*
       void Test()
        {
            // Use Skia to create a Maui graphics context and canvas
            //BitmapExportContext bmpContext = SkiaGraphicsService.Instance.CreateBitmapExportContext(600, 400);
            //SizeF bmpSize = new(bmpContext.Width, bmpContext.Height);
            //ICanvas canvas = bmpContext.Canvas;

            // Draw on the canvas with abstract methods that are agnostic to the renderer
            //ClearBackground(canvas, bmpSize, Colors.Navy);
            //DrawRandomLines(canvas, bmpSize, 1000);
            //DrawBigTextWithShadow(canvas, "This is Maui.Graphics with Skia");
            //SaveFig(bmpContext, Path.GetFullPath("quickstart.jpg"));
        }*/

        public static void ClearBackground(ICanvas canvas, SizeF bmpSize, Color bgColor)
        {
            canvas.FillColor = Colors.Navy;
            canvas.FillRectangle(0, 0, bmpSize.Width, bmpSize.Height);
        }

        public static void DrawRandomLines(ICanvas canvas, SizeF bmpSize, int count = 1000)
        {
            Random rand = new();
            for (int i = 0; i < count; i++)
            {
                canvas.StrokeSize = (float)rand.NextDouble() * 10;

                canvas.StrokeColor = new Color(
                    red: (float)rand.NextDouble(),
                    green: (float)rand.NextDouble(),
                    blue: (float)rand.NextDouble(),
                    alpha: .2f);

                canvas.DrawLine(
                    x1: (float)rand.NextDouble() * bmpSize.Width,
                    y1: (float)rand.NextDouble() * bmpSize.Height,
                    x2: (float)rand.NextDouble() * bmpSize.Width,
                    y2: (float)rand.NextDouble() * bmpSize.Height);
            }
        }

        public static void DrawBigTextWithShadow(ICanvas canvas, string text, float x,float y, Color color,float fontSize=36, bool useShadow=true)
        {
            canvas.FontSize = fontSize;
            canvas.FontColor = color;
            if(useShadow)
                canvas.SetShadow(offset: new SizeF(2, 2), blur: 1, color: Colors.Black);
            canvas.DrawString(text,x,y, HorizontalAlignment.Left);
        }

        public static void SaveImage(BitmapExportContext bmp, string filePath)
        {
            bmp.WriteToFile(filePath);
            Console.WriteLine($"WROTE: {filePath}");
        }
    }
}
