using FluentResults;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Anno_World_Manager.model
{
    internal class A7tgamedata
    {

        /// <summary>
        /// The data were checked for plausibility and land mass calculations were made.
        /// </summary>
        internal bool IsReadyForUse { get; set; } = false;

        /// <summary>
        /// Dimension of the island on the X axis
        /// </summary>
        internal Int16 IslandSizeX { get; set; }

        /// <summary>
        /// Dimension of the island on the Y axis
        /// </summary>
        internal Int16 IslandSizeY { get; set; }

        /// <summary>
        /// List of all tiles of the island
        /// </summary>
        /// <remarks>
        /// The list should have exactly as many entries as the X and Y dimensions of the island.
        /// Count = IslandSizeX * IslandSizeY
        /// 
        /// Meaning of a list entry:
        /// 0 = Water
        /// 1 = Land mass that cannot(!) be built on
        /// > 1 = Land mass on which can be built
        /// </remarks>
        internal List<Int16> Tiles { get; set; }

        /// <summary>
        /// Number of tiles consisting of water
        /// </summary>
        internal int CountedWaterTiles { get; private set; }
        /// <summary>
        /// Number of tiles with non-buildable landmass
        /// </summary>
        internal int CountedNonBuildableTiles { get; private set; }
        /// <summary>
        /// Number of tiles with buildable land mass
        /// </summary>
        internal int CountedBuildableTiles { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal void Calculate()
        {
            if (Tiles.Count == (IslandSizeX * IslandSizeY))
            {
                foreach (var tile in Tiles)
                {
                    switch(tile)
                    {
                        case 0:
                            CountedWaterTiles += 1; 
                            break;
                        case 1:
                            CountedNonBuildableTiles += 1; 
                            break;
                        default:
                            CountedBuildableTiles += 1; 
                            break;
                    }
                }
                IsReadyForUse = true;
            }
            else
            {
                Log.Logger.Debug("Check this please");
            }
        }

        internal Result<System.Windows.Media.Imaging.WriteableBitmap> DrawTiles_RGB()
        {
            if (IsReadyForUse)
            {
                try
                {
                    int width = IslandSizeX;
                    int height = IslandSizeY;

                    System.Windows.Media.Imaging.WriteableBitmap image = new System.Windows.Media.Imaging.WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Rgb24, null);

                    /*
                    const int color_water = 0 << 16 | 0 << 8 | 255; // RGB Blue 
                    const int color_buildable = 50 << 16 | 50 << 8 | 50; // RGB Grey
                    const int color_notbuildable = 0 << 16 | 255 << 8 | 0;  // RGB Green
                    */

                    try
                    {
                        // Reserve the back buffer for updates.
                        image.Lock();

                        unsafe
                        {
                            // Get a pointer to the back buffer.
                            IntPtr buff = image.BackBuffer;
                            byte* pbuff = (byte*)buff.ToPointer();

                            int index = 0;

                            for (int x = 0; x < width; x++)
                            {
                                for (int y = 0; y < height; y++)
                                {
                                    // Find the address of the pixel to draw.
                                    int offset = x * image.BackBufferStride + y * 3;

                                    //Log.Logger.Debug(offset);

                                    switch (Tiles[index])
                                    {
                                        case 0:
                                            //  Water (blue)
                                            pbuff[offset + 0] = 17;     //  R
                                            pbuff[offset + 1] = 30;     //  G
                                            pbuff[offset + 2] = 130;    //  B
                                            break;
                                        case 1:
                                            //  NotBuildable (ocker)
                                            pbuff[offset + 0] = 176;    //  R
                                            pbuff[offset + 1] = 172;    //  G
                                            pbuff[offset + 2] = 137;    //  B
                                            break;
                                        default:
                                            //  Buildable (green)
                                            pbuff[offset + 0] = 0;    //  R
                                            pbuff[offset + 1] = 255;    //  G
                                            pbuff[offset + 2] = 0;     //  B
                                            
                                            break;
                                    }

                                    index += 1;
                                }
                            }
                        }

                        // Specify the area of the bitmap that changed.
                        image.AddDirtyRect(new Int32Rect(0, 0, width, height));
                    }
                    finally
                    {
                        // Release the back buffer and make it available for display.
                        image.Unlock();
                    }
                    return Result.Ok(image);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex);
                    return Result.Fail(ex.Message);
                }
            }
            else
            {
                String errorMessage = "The data are not sufficiently valid for a reliable map to be drawn.";
                Log.Logger.Error(errorMessage);
                return Result.Fail(errorMessage);
            }
        }
    }
}
