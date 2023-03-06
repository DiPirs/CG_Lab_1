using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

namespace CG_lab_1
{
    abstract class GlobalFilters : Filters
    {
        protected float r1; // среднее по каналу R
        protected float g1; // среднее по каналу G
        protected float b1; // среднее по каналу B

        protected int maxR, minR;
        protected int maxG, minG;
        protected int maxB, minB;

        public void GetAverageColor(Bitmap sourceImage)
        {
            Color color = sourceImage.GetPixel(0, 0);

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            r1 = b1 = g1 = 0;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    color = sourceImage.GetPixel(i, j);

                    resultR += color.R;
                    resultG += color.G;
                    resultB += color.B;
                }
            }

            r1 = ((int)resultR / sourceImage.Width * sourceImage.Height);
            g1 = ((int)resultG / sourceImage.Width * sourceImage.Height);
            b1 = ((int)resultB / sourceImage.Width * sourceImage.Height);
        }

        public void GetMaxColor(Bitmap sourceImage)
        {
            maxR = maxG = maxB = 0;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color color = sourceImage.GetPixel(i, j);

                    maxR = Math.Max(maxR, color.R);
                    maxB = Math.Max(maxB, color.B);
                    maxG = Math.Max(maxG, color.G);
                }
            }
        }

        public void GetMinColor(Bitmap sourceImage)
        {
            minR = minG = minB = 255;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color color = sourceImage.GetPixel(i, j);

                    minR = Math.Min(minR, color.R);
                    minB = Math.Min(minB, color.B);
                    minG = Math.Min(minG, color.G);
                }
            }
        }
    }

    class GrayWorldFilter : GlobalFilters
    {
        protected float avg;

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            GetAverageColor(sourceImage);

            avg = (r1 + g1 + b1) / 3;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

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

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color color = sourceImage.GetPixel(x, y);

            float R = color.R * avg / r1;
            float G = color.G * avg / g1;
            float B = color.B * avg / b1;

            return Color.FromArgb(
                Clamp((int)R, 0, 255),
                Clamp((int)G, 0, 255),
                Clamp((int)B, 0, 255)
                );
        }
    }

    class AutoLevelsFilter : GlobalFilters
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            GetMaxColor(sourceImage);
            GetMinColor(sourceImage);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * (100 - 33 - 33)) + (33 + 33));
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color pixel = sourceImage.GetPixel(x, y);

            int newR = (pixel.R - minR) * 255 / (maxR - minR);
            int newG = (pixel.G - minG) * 255 / (maxG - minG);
            int newB = (pixel.B - minB) * 255 / (maxB - minB);

            return Color.FromArgb(Clamp((int)newR, 0, 255),
                                  Clamp((int)newG, 0, 255),
                                  Clamp((int)newB, 0, 255));
        }
    }
}
