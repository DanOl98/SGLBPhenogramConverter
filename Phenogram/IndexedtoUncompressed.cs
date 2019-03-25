using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PhenogramImages
{
    internal static class IndexedToUncompressed
    {
        public static void Convert(FileStream input, string save, string filename, ListBox lb)
        {
            int[] colors = new int[256];
            int width = 0;
            int height = 0;
            lb.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                lb.Items.Add("Converting " + filename);
                lb.Refresh();
                lb.TopIndex = lb.Items.Count - 1;
            });
            try
            {
                using (BinaryReader binaryreader = new BinaryReader(input))
                {
                    width = binaryreader.ReadInt16();
                    height = binaryreader.ReadInt16();
                    input.Position = 8;
                    for (int i = 0; i < 256; i++)
                    {
                        colors[i] = binaryreader.ReadInt32();
                    }
                    byte[] buffer = new byte[input.Length - 1032];
                    input.Read(buffer, 0, buffer.Length);
                    byte[] buffer2 = new byte[buffer.Length * 4];
                    if (!Directory.Exists(Path.Combine(save)))
                    {
                        Directory.CreateDirectory(Path.Combine(save));
                    }
                    using (FileStream fileStream = File.Create(Path.Combine(save, filename))) using (MemoryStream memOutput2 = new MemoryStream())
                    {
                        for (long i = 0; i < buffer.Length; i++)
                        {
                            fileStream.Write(BitConverter.GetBytes(colors[buffer[i]]), 0, 4);
                            if (i % 2048 == 0)
                            {
                                fileStream.Flush();
                            }
                        }
                        fileStream.Close();
                    }
                }
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg\\ffmpeg.exe");
                startInfo.Arguments = "-y  -vcodec rawvideo   -f rawvideo   -pix_fmt rgb32 -s " + width + "x" + height + " -i " + Path.Combine(save, filename) + " -f image2  -vcodec png " + Path.Combine(save, filename) + ".png ";
                startInfo.RedirectStandardOutput = true;
                try
                {
                    using (Process process = Process.Start(startInfo))
                    {
                        while (!process.StandardOutput.EndOfStream)
                        {
                            string line = process.StandardOutput.ReadLine();
                            Console.WriteLine(line);
                        }
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                File.Delete(Path.Combine(save, filename));
                System.GC.Collect();
                lb.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    lb.Items.Add(filename + " Converted successfully");
                    lb.Refresh();
                    lb.TopIndex = lb.Items.Count - 1;
                });
            }
            catch
            {
                lb.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    lb.Items.Add("Error in converting " + filename);
                    lb.Refresh();
                    lb.TopIndex = lb.Items.Count - 1;
                });
            }
        }
    }
}
