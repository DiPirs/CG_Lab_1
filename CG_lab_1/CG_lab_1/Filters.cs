using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace CG_lab_1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourseImage, int x, int y);

        public Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 101)); // для прогресс бара( для вызова ProgressChanged )

                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }

            return value;
        }
    }

    class InvertFilter : Filters
    {
        public InvertFilter() { }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);

            return resultColor;
        }
    } // инверсия

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourseImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourseImage.Height - 1);

                    Color neighborColor = sourseImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }

            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultB, 0, 255),
                Clamp((int)resultG, 0, 255)
                );
        }

    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;

            kernel = new float[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }

        }
    } // обычное размытие

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKarnel(int radius, float sigma)
        {
            // определяем размер ядра
            int size = 2 * radius + 1;
            // создаем ядро фильтра
            kernel = new float[size, size];
            // коэффицент нормировки ядра
            float norm = 0;
            // рассчитываем ядро линейного фильтра
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            // нормируем ядро
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }
        public GaussianFilter()
        {
            createGaussianKarnel(3, 2);
        }
    } // размытие по Гаусу

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);
            int grayScale = (int)((sourceColor.R * 0.36) + (sourceColor.G * 0.53) + (sourceColor.B * 0.11));
            Color resultColor = Color.FromArgb(grayScale, grayScale, grayScale); // делает цвета по R, G, B соответственно

            return resultColor;
        }
    } // черно-белый фильтр

    class Sepiya : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            int intensity = (int)((sourceColor.R * 0.36) + (sourceColor.G * 0.53) + (sourceColor.B * 0.11));

            int resultR = (int)(intensity + 2 * 9);
            int resultB = (int)(intensity + 0.5 * 9);
            int resultG = (int)(intensity - 1 * 9);

            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    } // сепия

    class BrightnessFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourceColor = sourseImage.GetPixel(x, y);

            int resultR = sourceColor.R + 50;
            int resultG = sourceColor.G + 50;
            int resultB = sourceColor.B + 50;

            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    } // увеличивание яркости

    class SobelFilter : MatrixFilter
    {

    } // === НЕ СДЕЛАНО === 

    class SharpnessFilter : MatrixFilter
    {
      
    } // === НЕ СДЕЛАНО ===

}
