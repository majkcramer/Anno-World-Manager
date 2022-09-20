using FluentResults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Anno_World_Manager.model
{
    internal  class Pngs
    {
        //  Maybe build a Png Cache ?!?

        internal  void Initialize()
        {
            Log.Logger.Trace("called");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static BitmapSource CreateEmptyBitmapImage()
        {
            return (BitmapSource)BitmapImage.Create(
             2,
             2,
             96,
             96,
             PixelFormats.Indexed1,
             new BitmapPalette(new List<Color> { Colors.Transparent }),
             new byte[] { 0, 0, 0, 0 },
             1
             );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gamedata_image_path"></param>
        /// <returns></returns>
        internal static Result<BitmapImage> LoadVanillaPng(string gamedata_image_path)
        {


            System.Windows.Media.Imaging.BitmapImage? png = new();
            try
            {
                //using Stream? stream = Settings.Instance.DataArchive?.OpenRead(island.ImageFile);
                using Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead(gamedata_image_path);
                if (stream is not null)
                {
                    png.BeginInit();
                    png.StreamSource = stream;
                    png.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    png.EndInit();
                    png.Freeze();
                    return Result.Ok(png);
                }
            }
            catch
            {
                
            }
            return Result.Fail(String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void doMaybe_magic_data_dump_pngs()
        {
            string foldername = AppContext.BaseDirectory + @"\" + Runtime.Secret_Magic_Data_Dump_Png_Folder;
            if (Directory.Exists(foldername))
            {
                //  Get every png path
                IEnumerable<string> all_png_path = Runtime.Anno1800GameData.DataArchive.Find("**/*.png");
                //  Iterate 
                foreach (String png_path in all_png_path)
                {
                    System.Windows.Media.Imaging.BitmapImage? png = new();
                    try
                    {
                        //using Stream? stream = Settings.Instance.DataArchive?.OpenRead(island.ImageFile);
                        using Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead(png_path);
                        if (stream is not null)
                        {
                            png.BeginInit();
                            png.StreamSource = stream;
                            png.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                            png.EndInit();
                            png.Freeze();

                            System.Windows.Media.Imaging.PngBitmapEncoder encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                            Guid photoID = System.Guid.NewGuid();
                            String filename = png_path.Replace('/', '#');
                            String photolocation = foldername + filename + ".png";

                            encoder.Frames.Add(BitmapFrame.Create((BitmapImage)png));

                            using (var filestream = new FileStream(photolocation, FileMode.Create))
                                encoder.Save(filestream);
                        }
                    }
                    catch
                    {
                        png = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pngPath"></param>
        /// <returns></returns>
        internal bool IsDataPathReadable(String pngPath)
        {
            bool retval = false;

            using Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead(pngPath);
            if (stream is not null)
            {
                retval = true;
            }

                return retval;
        }
    }
}
