using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace GameOfLife
{
    public class ImageSaver
    {
        public void SaveAsImage(UIElement controlToRender)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                Title = "Save Board Image"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    (int)controlToRender.RenderSize.Width,
                    (int)controlToRender.RenderSize.Height,
                    96d, 96d, PixelFormats.Pbgra32);

                controlToRender.Measure(new Size((int)controlToRender.RenderSize.Width, (int)controlToRender.RenderSize.Height));
                controlToRender.Arrange(new Rect(new Size((int)controlToRender.RenderSize.Width, (int)controlToRender.RenderSize.Height)));

                renderBitmap.Render(controlToRender);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Board image saved successfully.", "Save Image", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
