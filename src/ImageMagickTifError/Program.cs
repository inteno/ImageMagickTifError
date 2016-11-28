using ImageMagick;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageMagickTifError
{
    public class Program
    {

        public static void MagickNET_Log(object sender, LogEventArgs arguments)
        {
            Trace.WriteLine(arguments.Message, arguments.EventType.ToString());
        }
        public static void Main(string[] args)
        {
            Stream logStream = File.Create($@"..\..\Logs\Log-{DateTime.Now.ToString("yyMMdd-HHmm")}.txt");

            TextWriterTraceListener myTextListener = new
            TextWriterTraceListener(logStream);
            Trace.Listeners.Add(myTextListener);
            Console.WriteLine("Image processing. Please wait...");
            try
            {
                MagickNET.SetLogEvents(LogEvents.All | LogEvents.Trace);
                // Set the log handler (all threads use the same handler)
                MagickNET.Log += MagickNET_Log;

                using (var image = new MagickImage(@"..\..\BQ.020.010.100-01_CATORIG.jpg"))
                {
                    Trace.WriteLine("Trim...");
                    image.Trim();


                    Trace.WriteLine("Change color profile");
                    // First add a colour profile, if the image does not contain a colour profile.
                    image.AddProfile(ColorProfile.SRGB);

                    // Adding the second profile will transform the colourspace to CMYK
                    image.AddProfile(ColorProfile.USWebCoatedSWOP);


                    Trace.WriteLine("Resize...");
                    image.Resize(500, 500);

                    Trace.WriteLine("Change format...");
                    image.Format = MagickFormat.Tif;

                    Trace.WriteLine("Change endian...");
                    image.Endian = Endian.LSB;

                    Trace.WriteLine("Change compression method...");
                    image.CompressionMethod = CompressionMethod.LZW;

                    Trace.WriteLine("Change depth...");
                    image.Depth = 8;

                    Trace.WriteLine("To byte array...");
                    var bytes = image.ToByteArray();

                    Trace.WriteLine("Write...");
                    File.WriteAllBytes(@"..\..\converted.tif", bytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); ;
            }
            finally
            {
                Trace.Flush();
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}
