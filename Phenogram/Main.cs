using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using MetroFramework.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhenogramImages
{
    public partial class Form1 : MetroForm
    {
        int mode = 0;
        private UncompressedToIndexed writer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            writer = new UncompressedToIndexed();
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "ffmpeg")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "ffmpeg"));
                MessageBox.Show("Make sure to include FFmpeg in \"FFmpeg\" folder", "FFmpeg");
            }
        }


       

        private void button5_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog open = new CommonOpenFileDialog())
            {
                open.IsFolderPicker = true;
                if (open.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txtDirectoryDataUn.Text = open.FileName;
                    BtnUnPack.Enabled = true;
                }
            }
        }

        private async void BtnUnPack_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                lbTextUn.Text = "Wait...";
                await Task.Run(delegate
                {
                    switch (mode)
                    {
                        case 0:
                            var filesWithoutExtension = System.IO.Directory.GetFiles(Path.Combine(txtDirectoryDataUn.Text)).Where(filPath => String.IsNullOrEmpty(System.IO.Path.GetExtension(filPath)));
                            foreach (string path in filesWithoutExtension)
                            {
                                UnPack(path);
                            }
                            break;
                        case 1:
                            var filesWithoutExtension2 = System.IO.Directory.GetFiles(Path.Combine(txtDirectoryDataUn.Text)).Where(filPath => System.IO.Path.GetExtension(filPath).Equals(".png"));
                            foreach (string path in filesWithoutExtension2)
                            {
                                Pack(path);
                            }
                            break;
                    }
                });
                lbTextUn.Text = "Done";
            }
            catch
            {
                lbTextUn.Text = "Err";
            }
        }


       

        private void UnPack(String filename)
        {
            using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                filename = Path.GetFileNameWithoutExtension(filename);
                IndexedToUncompressed.Convert(fileStream, Path.Combine(txtDirectoryDataUn.Text, "Converted"), filename, listBox1);
            }
        }
        private void Pack(String filename)
        {
            using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                filename = Path.GetFileNameWithoutExtension(filename);
                UncompressedToIndexed.Convert(fileStream, Path.Combine(txtDirectoryDataUn.Text, "Converted"), filename, Path.Combine(txtDirectoryDataUn.Text), listBox1);
            }
        }
        
       

        private void SelectFolder(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog open = new CommonOpenFileDialog())
            {
                if (open.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    txtDirectoryDataUn.Text = open.FileName;
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) {
                radioButton2.Checked = false;
                mode = 0;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                mode = 1;
            }
        }

    }
}
