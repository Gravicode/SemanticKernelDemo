using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;
using SemanticKernelDemo.Data;
using SkiaSharp;
using System.Drawing;
using System.IO;

namespace SemanticKernelDemo;

public partial class MaskPage : ContentPage
{
    GraphicsDrawable Pad;
    public MaskPage()
    {
        InitializeComponent();
        Pad = new GraphicsDrawable();
        DrawCanvas.Drawable = Pad;
    } 
    public MaskPage(float ImgWidth, float ImgHeight, byte[] Image)
    {
        InitializeComponent();
        Pad = new GraphicsDrawable( ImgWidth,  ImgHeight,  Image);
        DrawCanvas.Drawable = Pad;
    }
    //void OnOKButtonClicked(object? sender, EventArgs e) => Close(true);
    async void OnOKButtonClicked(object? sender, EventArgs e)
    {
       await Navigation.PopModalAsync();
    }
}
public class GraphicsDrawable : IDrawable
{
    public byte[] MaskImage { get; set; }
    public float WidthF { get; set; }
    public float HeightF { get; set; }
    public GraphicsDrawable()
    {
      
    }
    public GraphicsDrawable(float ImgWidth, float ImgHeight, byte[] Image)
    {
        MaskImage = Image;
        this.WidthF = ImgWidth;
        this.HeightF = HeightF;
        //MaskImage = SKBitmap.Decode(Image);
    }
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (MaskImage != null)
        {
            var img = ImageSource.FromStream(() => new MemoryStream(MaskImage));
            //var img = SKImage.FromBitmap(MaskImage);
            //canvas.DrawImage(img, 0, 0, WidthF, HeightF);
        }
        else
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 6;
            canvas.DrawLine(10, 50, 90, 100);
        }
    }
}