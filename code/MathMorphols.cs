﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;


namespace CG_lab_1
{
    abstract class MathMorphols : Filters
    {
        protected float[,] mask = null;
        protected int radiusX;
        protected int radiusY;

        // === Для расширения, сужения, открытия, закрытия лучше этот использовать, лучше убирает шум ===
        public MathMorphols()
        {
            mask = new float[,] {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 }};

            radiusX = mask.GetLength(0) / 2;
            radiusY = mask.GetLength(1) / 2;
        }

        // === Для TopHat,BlackHat,Grad лучше использовать такой структурный элемент ===
        //public MathMorphols()
        //{
        //    mask = new float[,] {
        //        { 0, 1, 0 },
        //        { 1, 1, 1 },
        //        { 0, 1, 0,}};

        //    radiusX = mask.GetLength(0) / 2;
        //    radiusY = mask.GetLength(1) / 2;
        //}

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return Color.FromArgb(0, 0, 0);
        }
    }

    class DilationFilter : MathMorphols
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int maxR = 0;
            int maxG = 0;
            int maxB = 0;

            for (int k = -radiusX; k <= radiusX; k++)
            {
                for (int l = -radiusY; l <= radiusY; l++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                    if (mask[k + radiusX, l + radiusY] == 1)
                    {
                        Color color = sourceImage.GetPixel(idX, idY);
                        maxR = Math.Max(maxR, color.R);
                        maxG = Math.Max(maxG, color.G);
                        maxB = Math.Max(maxB, color.B);
                    }
                }
            }
            return Color.FromArgb(maxR, maxG, maxB);
        }
    } // расширение ( подавляет "белый" шум с потерями для площади объекта )

    class ErosionFilter : MathMorphols
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int minR = 255;
            int minG = 255;
            int minB = 255;

            for (int k = -radiusX; k <= radiusX; k++)
            {
                for (int l = -radiusY; l <= radiusY; l++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                    if (mask[k + radiusX, l + radiusY] == 1)
                    {
                        Color color = sourceImage.GetPixel(idX, idY);
                        minR = Math.Min(minR, color.R);
                        minG = Math.Min(minG, color.G);
                        minB = Math.Min(minB, color.B);
                    }
                }
            }
            return Color.FromArgb(minR, minG, minB);
        }
    } // сужение ( подавляет "черный" шум с потерями для площади объекта )

    class OpeningFilter : MathMorphols
    {
        public override Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Filters first = new ErosionFilter();
            Filters second = new DilationFilter();

            return first.proccessImage(second.proccessImage(sourceImage, worker), worker);
        }
    } // открытие ( подавляет "белый" шум на объектах, если фон без шума)

    class ClosingFilter : MathMorphols
    {
        public override Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Filters first = new DilationFilter();
            Filters second = new ErosionFilter();

            return second.proccessImage(second.proccessImage(sourceImage, worker), worker);
        }
    } // закрытие ( подавляет "черный" шум на объектах, если фон без шума )

    class TopHatFilter : MathMorphols
    {
        protected Bitmap closingImage;

        public override Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Filters filter = new ClosingFilter();
            closingImage = filter.proccessImage(sourceImage, worker);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending) { return null; }

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
            Color color_closing = closingImage.GetPixel(x, y);

            return Color.FromArgb(
                        Clamp(color.R - color_closing.R, 0, 255),
                        Clamp(color.G - color_closing.G, 0, 255),
                        Clamp(color.B - color_closing.B, 0, 255));
        }
    } // Или же исходное фото - закрытие

    class BlackHatFilter : MathMorphols
    {
        protected Bitmap openingImage;

        public override Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Filters filter = new ErosionFilter(); // Так не правильно, если судить по формуле, но оно выдает то, что нужно
            //Filters filter = new OpeningFilter(); - по сути, нам нужно вот это
            openingImage = filter.proccessImage(sourceImage, worker);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));

                if (worker.CancellationPending) { return null; }

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
            Color color_closing = openingImage.GetPixel(x, y);

            return Color.FromArgb(
                        //Clamp(color_closing.R - color.R, 0, 255),
                        //Clamp(color_closing.G - color.G, 0, 255),
                        //Clamp(color_closing.B - color.B, 0, 255));
                        Clamp(color.R - color_closing.R, 0, 255),
                        Clamp(color.G - color_closing.G, 0, 255),
                        Clamp(color.B - color_closing.B, 0, 255));
        }
    } // Или же открытие - исходное фото | Работает странно, разобраться

    class GradFilter : MathMorphols
    {
        protected Bitmap ErosionImage;
        protected Bitmap DilationImage;

        public override Bitmap proccessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            Filters Erosion = new ErosionFilter();
            Filters Dilation = new DilationFilter();
            
            ErosionImage = Erosion.proccessImage(sourceImage, worker);
            DilationImage = Dilation.proccessImage(sourceImage, worker);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                
                if (worker.CancellationPending) { return null; }
               
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color colorErosion = ErosionImage.GetPixel(x, y);
            Color colorDilation = DilationImage.GetPixel(x, y);

            return Color.FromArgb(
                        Clamp(colorDilation.R - colorErosion.R, 0, 255),
                        Clamp(colorDilation.G - colorErosion.G, 0, 255),
                        Clamp(colorDilation.B - colorErosion.B, 0, 255));
        }
    } // Или же расширение - сужение
}