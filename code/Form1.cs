using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_lab_1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap startImage;
        //Filters lastFilter = null;

        public Form1()
        {
            InitializeComponent();

        }

        // === Визуал, backgroundWorker, кнопка "Отмена" ===
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).proccessImage(image, backgroundWorker1);

            if (backgroundWorker1.CancellationPending != true) 
            {
                //lastFilter = (Filters)e.Argument;
                image = newImage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        } // Кнопка отмены загрузки
        
        // === меню "Файл" ===
        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog LoadDialog = new OpenFileDialog();
            LoadDialog.Filter = "Image files | *.png; *.jpg; *.bmp; | All Files (*.*) | *.*";

            if (LoadDialog.ShowDialog() == DialogResult.OK) // Проверка файла
            {
                image = new Bitmap(LoadDialog.FileName);
                startImage = image;
            }
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog();

            SaveDialog.Filter = "Image files | *.png; *.jpg; *.bmp; | All Files (*.*) | *.*";

            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                image.Save(SaveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        // === меню "Правка" ===
        private void вернутьКИсходномуToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            image = startImage;
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        // === меню "Фильтры" ===
        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чернобелоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sepiya();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеПоГауссуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void собельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void щарраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharraFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void прюиттаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new PruittaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorldFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ErosionFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ClosingFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TopHatFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void blackHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlackHatFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gradToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GradFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
