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
}
