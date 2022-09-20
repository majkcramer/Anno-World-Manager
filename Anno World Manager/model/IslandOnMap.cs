using Anno_World_Manager.helper;
using Anno_World_Manager.ImExPort2.model;
using Anno_World_Manager.model.helper;
using FluentResults;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Anno_World_Manager.model
{
    /// <summary>
    /// 
    /// </summary>
    public class IslandOnMap : Island
    {
        /// <summary>
        /// 
        /// </summary>
        private VariableReference<int> _SizeMapWidth;
        

        /// <summary>
        /// Unique Assigned (!) Island Instance ID of an Island within this Session
        /// </summary>
        public Guid InstanceID { get; set; } = Guid.NewGuid();

        #region Prepared data for use in the ViewModel

        public int ViewModelRotation
        {
            get { return _viewModelRotation; }
            private set { SetProperty<int>(ref _viewModelRotation, value); }
        }
        private int _viewModelRotation = 0;
        private const String propertyName_RotationDataView = "ViewModelRotation";

        /// <summary>
        /// Position of Island within ViewModel (Y-Flipped against internal Data)
        /// </summary>
        /// <remarks>
        /// In the data structure of Anno, Y-positions are stored flipped.
        /// 
        /// Position on Map = MapSizeY - EntitySizeY - Y-Position
        /// </remarks>
        public int ViewModelPositionY
        {
            get { return _viewModelPositionY; }
            set { SetProperty<int>(ref _viewModelPositionY, value); }
        }
        private int _viewModelPositionY = 0;
        private const String PROPERTYNAME_VIEWMODELPOSITIONY = "ViewModelPositionY";
        #endregion

        public ElementType ElementType { get; set; }


        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;         //  WICHTIG ! Zur Darstellung muss die Y Position auf der Karte geflippt werden !!!!

        /// <summary>
        /// Rotation - Anno Data structure only
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public int Rotation
        {
            get { return _rotation; }
            private set { SetProperty<int>(ref _rotation, value); }
        }
        private int _rotation = 0;
        private const String propertyName_RotationData = "Rotation";


        

        public ObservableCollection<int> FertilityGuids
        {
            get { return fertilityGuids; }
            private set { SetProperty<ObservableCollection<int>>(ref fertilityGuids, value); }
        }
        private ObservableCollection<int> fertilityGuids = new ObservableCollection<int>();


        public bool RandomizeFertilities
        {
            get { return randomizeFertilities; }
            private set { SetProperty<bool>(ref randomizeFertilities, value); }
        }
        private bool randomizeFertilities = true;


        
        public BitmapSource Png
        {
            get { return _png; }
            set { SetProperty<BitmapSource>(ref _png, value); }
        }
        private BitmapSource _png = Pngs.CreateEmptyBitmapImage();


        public IslandOnMap(VariableReference<int> pMapSizeY)
        {
            //  save Variable Reference to parent Session.sizeMapWidth (for later use at Island Y Positioning)
            this._SizeMapWidth = pMapSizeY;
            this.PropertyChanged += IslandOnMap_PropertyChanged;
        }

        private void IslandOnMap_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case propertyName_RotationData:
                    UpdateRotionViewData();
                    break;
                case PROPERTYNAME_VIEWMODELPOSITIONY:
                    //  Maybe a Drag-Drop from View has changed ViewModelPositionY. Write changes y-flipped back to data
                    this.PositionY = CalcPosition.FlipYMapEntity(_SizeMapWidth.Value, this.IslandSizeY, ViewModelPositionY);
                    break;
                default:
                    break;
            }
        }

        private void UpdateRotionViewData()
        {
            int angle = 0;
            switch(this.Rotation)
            {
                case 0: angle = 0; break;
                case 1: angle = 1; break;
                case 2: angle = 2; break;
                case 3: angle = 3; break;
                default:
                    Log.Logger.Warn("DEV: Rotation Data seems to be wrong (expected 0-3): {0}", angle);
                    break;
            }
            //  Adopted from AME : angle * -90
            angle = angle * -90;
            //  Assign to ViewModel Property
            ViewModelRotation = angle;
        }

        public void Rotate()
        {
            switch(this.Rotation)
            {
                case 0:
                case 1:
                case 2:
                    this.Rotation += 1;
                    break;
                case 3:
                    this.Rotation = 0;
                    break;
                default:
                    Log.Logger.Warn("DEV: Rotation Data seems to be wrong (expected 0-3): {0}", this.Rotation);
                    break;
            }
        }

        public void AdoptPropertiesFromA7tinfo(TemplateElement a7tinfoModel_TemplateElement)
        {
            #region //MapTemplate/TemplateElement/ElementType
            if (a7tinfoModel_TemplateElement.ElementType != null)
            {
                switch (a7tinfoModel_TemplateElement.ElementType)
                {
                    case 0:
                        this.ElementType = ElementType.IsSpecificIsland; break;
                    case 1:
                        this.ElementType = ElementType.IsRandomIsland; break;
                    case 2:
                        this.ElementType = ElementType.IsShipPosition; break;
                    default:
                        Log.Logger.Warn("DEV: New Property Value found - a7tinfo ElementType: {0}", a7tinfoModel_TemplateElement.ElementType);
                        break;
                }
            }
            #endregion

            if (a7tinfoModel_TemplateElement.Element == null)
            {
                Log.Logger.Error("Important XPath //MapTemplate/TemplateElement/Element/ is empty !");
                return;
            }

            #region //MapTemplate/TemplateElement/Element/Position
            if (a7tinfoModel_TemplateElement.Element.Position != null)
            {
                if (a7tinfoModel_TemplateElement.Element.Position.Length == 2)
                {
                    this.PositionX = a7tinfoModel_TemplateElement.Element.Position[0];
                    this.PositionY = a7tinfoModel_TemplateElement.Element.Position[1];
                }
                else { Log.Logger.Warn("DEV: //MapTemplate/TemplateElement/Element/Position Lenght != 2. It is: {0}", a7tinfoModel_TemplateElement.Element.Position.Length); }
            }
            else { Log.Logger.Warn("Strange. //MapTemplate/TemplateElement/Element/Position is null"); }
            #endregion

            #region MapFilePath
            if (a7tinfoModel_TemplateElement.Element.MapFilePath != null)
            {
                //  TODO: Die Zuweisung ist nur für VANILLA Maps richtig !!!!
                this.VanillaMapPath = a7tinfoModel_TemplateElement.Element.MapFilePath;
            }
            #endregion

            #region //MapTemplate/TemplateElement/Element/Rotation90
            if (a7tinfoModel_TemplateElement.Element.Rotation90 != null)
            {
                switch (a7tinfoModel_TemplateElement.Element.Rotation90)
                {
                    case 0:
                        this.Rotation = 0; break;
                    case 1:
                        this.Rotation = 1; break;
                    case 2:
                        this.Rotation = 2; break;
                    case 3:
                        this.Rotation = 3; break;
                    default:
                        Log.Logger.Warn("DEV: New Property Value found - a7tinfo Rotation: {0}", a7tinfoModel_TemplateElement.Element.Rotation90);
                        break;
                }
            }
            else { Log.Logger.Warn("Strange. //MapTemplate/TemplateElement/Element/Rotation90 is null"); }
            #endregion

            #region //MapTemplate/TemplateElement/Element/IslandLabel
            if (a7tinfoModel_TemplateElement.Element.IslandLabel != null)
            {
                this.Label = a7tinfoModel_TemplateElement.Element.IslandLabel;
            }
            else { Log.Logger.Info("//MapTemplate/TemplateElement/Element/IslandLabel is null"); }
            #endregion

            #region //MapTemplate/TemplateElement/Element/FertilityGuids
            if (a7tinfoModel_TemplateElement.Element.FertilityGuids != null)
            {
                this.FertilityGuids.Clear();
                foreach (int e in a7tinfoModel_TemplateElement.Element.FertilityGuids)
                {
                    this.FertilityGuids.Add(e);
                }
            }
            #endregion

            #region //MapTemplate/TemplateElement/Element/RandomizeFertilities
            if (a7tinfoModel_TemplateElement.Element.RandomizeFertilities != null)
            {
                switch (a7tinfoModel_TemplateElement.Element.RandomizeFertilities)
                {
                    case false:
                        RandomizeFertilities = true; break;
                    case true:
                        RandomizeFertilities = false; break;
                    default:
                        DirivePropertyRandomizeFertilities();
                        Log.Logger.Warn("DEV: New Property Value found - a7tinfo RandomizeFertilities: {0}", a7tinfoModel_TemplateElement.Element.RandomizeFertilities);
                        break;
                }
            }
            else
            {
                DirivePropertyRandomizeFertilities();
            }
            #endregion

            #region MineSlotMapping - A Mystery for me :-D
            //  public int[][]? MineSlotMapping { get; set; }
            #endregion

            #region
            //public RandomIslandConfig? RandomIslandConfig { get; set; }
            #endregion

            #region //MapTemplate/TemplateElement/Element/Size
            if (a7tinfoModel_TemplateElement.Element.Size != null)
            {
                switch (a7tinfoModel_TemplateElement.Element.Size)
                {
                    case 0:
                        Size = IslandSize.Small;
                        break;
                    case 1:
                        Size = IslandSize.Medium;
                        break;
                    case 2:
                        Size = IslandSize.Large;
                        break;
                    default:
                        Log.Logger.Warn("DEV: New Property Value found - a7tinfo Size: {0}", a7tinfoModel_TemplateElement.Element.Size);
                        break;
                }
            }
            #endregion

            #region
            //public Difficulty? Difficulty { get; set; }
            #endregion

            #region
            //public Config? Config { get; set; }
            #endregion

            //  TODO: Prio 1 - Hier weiter fortsetzen
        }

        public void AdoptPropertiesFromKnownIslands(WorldRegion region)
        {
            bool success = false;   
            switch(this.ElementType)
            {
                case ElementType.IsSpecificIsland:
                    //  Adopt properties of the specified island
                    var result1 = Runtime.IslandsKnown.GetIsland(this.VanillaMapPath);
                    switch (result1.IsSuccess)
                    {
                        case true:
                            CopyIslandProperties(result1.Value);
                            success = true;
                            break;
                        case false:
                            break;
                    }
                    break;
                case ElementType.IsRandomIsland:
                    Result<Island> result = Runtime.IslandsKnown.GetRandomIslandFromPool(region, this.Size);
                    switch (result.IsSuccess)
                    {
                        case true:
                            CopyIslandProperties(result.Value);
                            //  TODO - Muss ich jetzt auf spezifische Insel umstellen oder lasse ich es
                            //  Spezifische Insel wäre nicht mehr Randomized Änderbar + würde Poolkonzept verletzten
                            //  Random Island würde den Nutzer irritieren und ggf. doch zu einer andere Insel im Spiel führen
                            success = true;
                            break;
                        case false:
                            break;
                    }
                    break;
                case ElementType.IsShipPosition:
                    //  TODO: Noch offen
                    break;
                default:
                    Log.Logger.Warn("DEV: You have missed something to implement here :-D");
                    break;
            }

            if (success)
            {
                this.ViewModelPositionY = CalcPosition.FlipYMapEntity(_SizeMapWidth.Value, this.IslandSizeY, this.PositionY);
            }
        }

        public void LoadPngs_VanillaImagePathMapImage()
        {
            //
            if (this.VanillaImagePathMapimage != null && this.VanillaImagePathMapimage != String.Empty)
            {
                var result =Pngs.LoadVanillaPng(this.VanillaImagePathMapimage);
                if (result.IsSuccess) { this.Png = result.Value; }
            }
        }

        public void CopyIslandProperties(Island tocopy)
        {
            //   Ignore Name
            //   Ignore Label
            //   Ignore Regions
            if (this.Size != tocopy.Size) { this.Size = tocopy.Size; }
            if (this.IslandSizeX != tocopy.IslandSizeX) { this.IslandSizeX = tocopy.IslandSizeX; }
            if (this.IslandSizeY != tocopy.IslandSizeY) { this.IslandSizeY = tocopy.IslandSizeY; }
            if (this.CountedWaterTiles != tocopy.CountedWaterTiles) { this.CountedWaterTiles = tocopy.CountedWaterTiles; }
            if (this.CountedNonBuildableTiles != tocopy.CountedNonBuildableTiles) { this.CountedNonBuildableTiles = tocopy.CountedNonBuildableTiles; }
            if (this.CountedBuildableTiles != tocopy.CountedBuildableTiles) { this.CountedBuildableTiles = tocopy.CountedBuildableTiles; }
            if (this.Type != tocopy.Type) { this.Type = tocopy.Type; }
            if (this.ThirdPartyType != tocopy.ThirdPartyType) { this.ThirdPartyType = tocopy.ThirdPartyType; }
            if (this.CanBeColonized != tocopy.CanBeColonized) { this.CanBeColonized = tocopy.CanBeColonized; }
            if (this.HasRiver != tocopy.HasRiver) { this.HasRiver = tocopy.HasRiver; }
            if (this.IsVanillaIsland != tocopy.IsVanillaIsland) { this.IsVanillaIsland = tocopy.IsVanillaIsland; }
            if (this.VanillaMapPath != tocopy.VanillaMapPath) { this.VanillaMapPath = tocopy.VanillaMapPath; }
            if (this.VanillaOpositeRiveredIslandName != tocopy.VanillaOpositeRiveredIslandName) { this.VanillaOpositeRiveredIslandName = tocopy.VanillaOpositeRiveredIslandName; }
            if (this.VanillaImagePathMapimage != tocopy.VanillaImagePathMapimage) { this.VanillaImagePathMapimage = tocopy.VanillaImagePathMapimage; }
            if (this.VanillaImagePathActivemapimage != tocopy.VanillaImagePathActivemapimage) { this.VanillaImagePathActivemapimage = tocopy.VanillaImagePathActivemapimage; }
            if (this.VanillaImagePathGamemapimage != tocopy.VanillaImagePathGamemapimage) { this.VanillaImagePathGamemapimage = tocopy.VanillaImagePathGamemapimage; }
            if (this.VanillaIsPool != tocopy.VanillaIsPool) { this.VanillaIsPool = tocopy.VanillaIsPool; }
        }



        



        /// <summary>
        /// Derives the property plausibly from existing information situation
        /// </summary>
        private void DirivePropertyRandomizeFertilities()
        {
            if (this.FertilityGuids.Count == 0) { RandomizeFertilities = true; } else { RandomizeFertilities = false; }
        }
    }

    public enum ElementType
    {
        IsSpecificIsland = 0,
        IsRandomIsland = 1,
        IsShipPosition = 2,
    }
}
