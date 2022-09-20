using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Anno_World_Manager.helper;
using Anno_World_Manager.model;
using Anno_World_Manager.viewmodel.baseclasses;

namespace Anno_World_Manager.viewmodel
{
    public class IslandViewModel : ViewModelBase, ICloneable
    {
        public Island _island;

        private String _label;
        public String Label
        {
            get { return _label; }
            private set { SetProperty<String>(ref _label, value); }
        }
        
        private IslandCoordinates _coordinates;
        public IslandCoordinates Coordinates
            { 
            get { return _coordinates; }
            set { SetProperty<IslandCoordinates>(ref _coordinates, value); }

        }

        private BitmapSource _png = CreateEmptyBitmapImage();
        public BitmapSource Png
        {
            get { return _png; }
            set { SetProperty<BitmapSource>(ref _png, value); }
        }

        private int _mapSizeInTitles = 0;
        public int MapSizeInTitles
        {
            get { return _mapSizeInTitles; }
            set { SetProperty<int>(ref _mapSizeInTitles, value); }
        }

        private int _rotation = 0;
        public int Rotation
        {
            get { return _rotation; }
            private set { SetProperty<int>(ref _rotation, value); }
        }

        internal IslandViewModel(Island island)
        {
            //  Store Island Model
            _island = island;
            //  TODO: What should i do with this Coordinates ?!?
            _coordinates = new IslandCoordinates();

            Label = _island.Name;
            MapSizeInTitles = _island.SizeMapInTitle;

            //  TODO: Bessere Lösung als Zeitverzögert bauen
            /* HERE: Cap Treloney
            DelayFactory.DelayAction(500, new Action(() => { 
                LoadPng(@"data\dlc01\sessions\islands\pool\moderate_c_01\_gamedata\moderate_c_01\mapimage.png"); 
            }));
            */

            SetRotation(0);

            DelayFactory.DelayAction(500, new Action(() => {
                LoadPng(_island.VanillaImagePathMapimage);
            }));
        }

        private void SetRotation(int value)
        {
            bool _isValueValid = false;
            switch(value)
            {
                case 0:
                case 90:
                case 180:
                case 270:
                    _isValueValid = true; break;
                case 360:
                    value = 0; _isValueValid = true; break;

                default:
                    _isValueValid = false; break;
            }

            if(!_isValueValid)
            {
                Log.Logger.Warn("New Island Rotation Value is invalid: {0}", value);
            }
            else
            {
                Log.Logger.Debug("Rotating Island - Set proofed Rotating Angle: {0}°", value);
                Rotation = value;   
                // TODO: Prio 2 - Clearify why the Anno Map Editor does a Angle Conversion ?
                // Rotation = value * -90;         //  why ?!? (from Anno Map Editor)
                Log.Logger.Debug("Rotating Island - Angle Property: {0}°", Rotation);
            }
        }

        /// <summary>
        /// Rotates the island 90° to the right (clockwise)
        /// </summary>
        internal void RotateNextClockwise()
        {
            int current_rotation = Rotation;
            int current_real_rotation = current_rotation;
            //  TODO: Prio 2 - Same Question as before - why the Anno Map Editor does a Angle Conversion
            //int current_real_rotation = (current_rotation * -1) / 90;  //  Same as before : why ?!? (from Anno Map Editor)
            Log.Logger.Debug("Rotating Island - Rotating angle before: {0}°", current_real_rotation);
            int new_rotation = current_real_rotation + 90;
            Log.Logger.Debug("Rotating Island - Rotating angle after +90°: {0}°", new_rotation);
            SetRotation(new_rotation);
        }

        private void LoadPng(string gamedata_image_path)
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
                }
            }
            catch
            {
                png = null;
            }

            if (png != null) { Png = png; }

            /*  TODO: Muss ich noch SetPosition (?) übernehmen
            
            image.SetPosition(new Vector2(0, island.SizeInTiles - island.MapSizeInTiles));
            
            */
        }

        private static BitmapSource CreateEmptyBitmapImage()
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

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }



    public class IslandCoordinates : ViewModelBase
    {
        private int _x = 0;
        private int _y = 0;
        private int _width = 50;
        private int _height = 70;
        private int _rotation_angle = 0;

        public int X
        {
            get { return _x; }
            set { SetProperty<int>(ref _x, value); }
        }

        public int Y
        {
            get { return _y; }
            set { SetProperty<int>(ref _y, value); }
        }

        public int Width
        {
            get { return _width; }
            set { SetProperty<int>(ref _width, value); }
        }

        public int Height
        {
            get { return _height; }
            set { SetProperty<int>(ref _height, value); }
        }

        public int RotationAngle
        {
            get { return _rotation_angle; }
            set { SetProperty<int>(ref _rotation_angle, value); }
        }

        internal IslandCoordinates()
        {

        }


        
    }
}
