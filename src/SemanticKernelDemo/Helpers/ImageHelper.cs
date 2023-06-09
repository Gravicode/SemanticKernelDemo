using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Helpers
{
    public class ImageHelper
    {
        public static async Task<byte[]> MakeTransparent(byte[] ImageData, byte[] Selection)
        {
            try
            {
                SKBitmap MaskImage = SKBitmap.Decode(Selection);
                SKBitmap MainImage = SKBitmap.Decode(ImageData);
                if (MaskImage.Width != MainImage.Width || MaskImage.Height != MainImage.Height) throw new Exception("image size is not same");
                SKImageInfo info = new SKImageInfo(MainImage.Width, MainImage.Height);
                SKSurface surface = SKSurface.Create(info);
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(SKColors.Empty);
               
                var col = SKColors.Empty;
                for (var x = 0; x < MaskImage.Width; x++)
                    for (var y = 0; y < MaskImage.Height; y++)
                    {
                        if (MaskImage.GetPixel(x, y) != SKColor.Empty)
                        {
                            MainImage.SetPixel(x, y, col);
                        }
                    }
                canvas.DrawBitmap(MainImage, 0, 0, null);
                surface.Draw(canvas, 0, 0, null);

                var encoded = surface.Snapshot().Encode();
                byte[] surfaceData = encoded.ToArray();
                return surfaceData;
            }
            catch (Exception ex)
            {
                return default;
            }

        }
        public static async Task<byte[]> ChangeToFuschia(byte[] ImageData)
        {
            try
            {
                SKBitmap webBitmap = SKBitmap.Decode(ImageData);
                SKImageInfo info = new SKImageInfo(webBitmap.Width, webBitmap.Height);
                SKSurface surface = SKSurface.Create(info);
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                var col = SKColors.Fuchsia;
                for (var x = 0; x < webBitmap.Width; x++)
                    for (var y = 0; y < webBitmap.Height; y++)
                    {
                        if (webBitmap.GetPixel(x, y) != SKColor.Empty)
                        {
                            webBitmap.SetPixel(x, y, col);
                        } 
                    }
                canvas.DrawBitmap(webBitmap, 0, 0, null);
                surface.Draw(canvas, 0, 0, null);
                
                var encoded = surface.Snapshot().Encode();
                byte[] surfaceData = encoded.ToArray();
                return surfaceData;
            }
            catch (Exception ex)
            {
                return default;
            }

        }
        public static async Task<byte[]> GetImageAsBytes(string url, int width, int height)
        {
            try
            {
                SKImageInfo info = new SKImageInfo(width, height);
                SKSurface surface = SKSurface.Create(info);
                SKCanvas canvas = surface.Canvas;
                canvas.Clear(SKColors.White);
                var httpClient = new HttpClient();
                using (Stream stream = await httpClient.GetStreamAsync(url))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    SKBitmap webBitmap = SKBitmap.Decode(memStream);
                    
                    canvas.DrawBitmap(webBitmap, 0, 0, null);
                    surface.Draw(canvas, 0, 0, null);
                };
                var encoded = surface.Snapshot().Encode();
                byte[] surfaceData = encoded.ToArray();
                return surfaceData;
            }
            catch (Exception ex)
            {
                return default;
            }
            
        }
    }
}
