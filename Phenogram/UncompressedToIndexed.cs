using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PhenogramImages
{
    internal class UncompressedToIndexed
    {
        public static void Convert(Stream inputpng, string save, string filename, string origin, ListBox lb)
        {
            Int16 width = 0;
            Int16 height = 0;
            if (!Directory.Exists(Path.Combine(save)))
            {
                Directory.CreateDirectory(Path.Combine(save));
            }
            lb.Invoke((MethodInvoker)delegate
            {
                // Running on the UI thread
                lb.Items.Add("Converting " + filename);
                lb.Refresh();
                lb.TopIndex = lb.Items.Count - 1;
            });
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(inputpng);
                width = (Int16)img.Width;
                height = (Int16)img.Height;
                inputpng.Close();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg\\ffmpeg.exe");
                startInfo.Arguments = "-y -vcodec png -f image2 -i \"" + Path.Combine(origin, filename + ".png") + "\"  -s " + width + "x" + height + " " + "-vcodec rawvideo   -f rawvideo   -pix_fmt rgb32 -s " + width + "x" + height + " \"" + Path.Combine(origin, filename) + "\"";
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
                using (Stream input = File.OpenRead(Path.Combine(origin, filename))) using (BinaryReader binaryreader = new BinaryReader(input))
                {
                    int[] colors = new int[256];
                    int[] pixels = new int[input.Length];
                    long size = input.Length;
                    long pixelnum = size / 4;
                    int colorindex = 0;
                    Buffer.BlockCopy(binaryreader.ReadBytes((int) size), 0, pixels, 0, (int) size);
                    for (long i = 0; i < pixelnum; i++)
                    {
                        if (!Array.Exists(colors, element => element == pixels[i]))
                        {
                            if (colorindex < 256)
                            {
                                colors[colorindex] = pixels[i];
                                colorindex++;
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    byte[] buffer = new byte[input.Length - 1032];
                    input.Read(buffer, 0, buffer.Length);
                    using (FileStream fileStream = File.Create(Path.Combine(save, filename)))
                    {
                        fileStream.Write(BitConverter.GetBytes(width), 0 , 2);
                        fileStream.Write(BitConverter.GetBytes(height), 0, 2);
                        fileStream.Write(BitConverter.GetBytes((int) 8), 0, 4);
                        fileStream.Flush();
                        Array.Sort(colors);

                        for (int i = 0; i < 256; i++)
                        {
                            fileStream.Write(BitConverter.GetBytes(colors[i]), 0, 4);
                        }
                        fileStream.Flush();
                        for (long i = 0; i < pixelnum; i++)
                        {
                            fileStream.WriteByte((byte)Array.IndexOf(colors, pixels[i]));
                            if (i % 2048 == 0) {
                                fileStream.Flush();
                            }
                        }
                        fileStream.Close();
                    }
                }
                File.Delete(Path.Combine(origin, filename));
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
            System.GC.Collect();
        }
    }
}
