using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelDemo.Helpers
{
    internal class ImageHelper
    {
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
