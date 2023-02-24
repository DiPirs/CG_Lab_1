using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CG_lab_1
{
    public partial class Form1 : Form
    {
        Bitmap image;

        public Form1()
        {
            InitializeComponent();

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e) // Загрузка фото по клику
        {
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e) // Проверка файла и открытие
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp; | All Files (*.*) | *.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter filter = new InvertFilter();
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).proccessImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true) { image = newImage; }
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
        }

        private void собельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        } // === НЕ НАПИСАН ===

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e) // === НЕ НАПИСАН ===
        {
            Filters filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
}
