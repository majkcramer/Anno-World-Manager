using Anno_World_Manager.ImExPort2.model;
using Anno_World_Manager.model.helper;
using Anno_World_Manager.view;
using Anno_World_Manager.viewmodel.baseclasses;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    /// <summary>
    /// Data Class of a Session within Anno 1800
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class Session : ViewModelBase
    {
        /// <summary>
        /// Determines whether the maps of an existing session in the game should be overwritten (Vanilla), or an independently sailable session should be created (Custom).
        /// </summary>
        /// <remarks>
        /// Required for internal processing. The property decides which data must be generated against the game.
        /// </remarks>
        public SessionType SessionType
        {
            get { return _sessionType; }
            set { SetProperty<SessionType>(ref _sessionType, value); }
        }
        private SessionType _sessionType = SessionType.SessionVanilla;
        public const String propertyName_SessionType = "SessionType";

        /// <summary>
        /// Name of the Session (in Editor & Game)
        /// </summary>
        /// <remarks>
        /// Needed for the display in the editor, as well as the display in the game at Custom Session <see cref="SessionType.SessionCustom"/>.
        /// </remarks>
        public String Name
        {
            get { return _name; }
            set { SetProperty<String>(ref _name, value); }
        }
        private String _name = String.Empty;
        public const String propertyName_Name = "Name";

        /// <summary>
        /// 
        /// </summary>
        public WorldRegion Region
        {
            get { return _region; }
            set { SetProperty<WorldRegion>(ref _region, value); }
        }
        private WorldRegion _region = WorldRegion.NotSetYet;
        public const String propertyName_Region = "Region";


        #region ViewModel Propertys - Map Size


        private static int ConfigWarningMapDimension = 2500 * 2550;

        /// <summary>
        /// Should we ensure a square map?
        /// </summary>
        public bool VieModelSizeMapEnsureSquare
        {
            get { return _vieModelSizeMapEnsureSquare; }
            set
            {
                if (value != VieModelSizeMapEnsureSquare) // Adopt data only if they are really different
                {
                    SetProperty<bool>(ref _vieModelSizeMapEnsureSquare, value);
                    if (value)
                    {
                        //  Assign the Size of the larger property to the other Property
                        if (ViewModelSizeMapHeight > ViewModelSizeMapWidth) { ViewModelSizeMapHeight = ViewModelSizeMapWidth; }
                        if (ViewModelSizeMapWidth > ViewModelSizeMapHeight) { ViewModelSizeMapWidth = ViewModelSizeMapHeight; }
                    }
                }
            }
        }
        private bool _vieModelSizeMapEnsureSquare = true;

        /// <summary>
        /// Use this (!) Property for View Bindings - Map Width
        /// </summary>
        public int ViewModelSizeMapWidth
        {
            get { return _viewModelSizeMapWidth; }
            set {
                if (value != ViewModelSizeMapWidth) // Adopt data only if they are really different
                {
                    value = RoundDividableBy4(value);
                    SetProperty<int>(ref _viewModelSizeMapWidth, value);
                    SizeMapWidth = value;
                    if (VieModelSizeMapEnsureSquare)
                    {
                        ViewModelSizeMapHeight = value;
                    }
                    CheckWarningLargeMapSize();
                }
            }
        }
        private int _viewModelSizeMapWidth = 1000;
        public const string propertyName_ViewModelSizeMapWidth = "ViewModelSizeMapWidth";

        /// <summary>
        /// Use this (!) Property for View Bindings - Map Height
        /// </summary>
        public int ViewModelSizeMapHeight
        {
            get { return _viewModelSizeMapHeight; }
            set
            {
                if (value != ViewModelSizeMapHeight) // Adopt data only if they are really different
                {
                    value = RoundDividableBy4(value);
                    SetProperty<int>(ref _viewModelSizeMapHeight, value);
                    SizeMapHeight = value;
                    if (VieModelSizeMapEnsureSquare)
                    {
                        ViewModelSizeMapWidth = value;
                    }
                    CheckWarningLargeMapSize();
                }
            }
        }
        private int _viewModelSizeMapHeight = 1000;
        public const string propertyName_ViewModelSizeMapHeight = "ViewModelSizeMapHeight";


        public bool ViewModelShowWarningAboutMapSize
        {
            get { return _viewModelShowWarningAboutMapSize; }
            set { SetProperty<bool>(ref _viewModelShowWarningAboutMapSize, value); }
        }
        private bool _viewModelShowWarningAboutMapSize = false;

        #endregion



        #region DATA - Map Size
        /// <summary>
        /// Width of the map on which islands can be placed.  <seealso cref="SizeMapHeight"/>
        /// </summary>
        /// <remarks>
        /// * As far as we know at present, the width of a card must be divisible by 4 without any remainder.
        /// * The size of the map is not the playable area. For this purpose the PlayableArea within the map size is used 
        /// </remarks>
        public int SizeMapWidth
        {
            get { return sizeMapWidth; }
            private set
            {
                if (value != SizeMapWidth) // Adopt data only if they are really different
                {
                    value = RoundDividableBy4(value);
                    SetProperty<int>(ref sizeMapWidth, value);
                    ViewModelSizeMapWidth= value;
                }
            }
        }
        private int sizeMapWidth = 1000;
        public const string propertyName_SizeMapWidth = "SizeMapWidth";

        /// <summary>
        /// Height of the map on which islands can be placed. <seealso cref="SizeMapWidth"/> 
        /// </summary>
        /// <remarks>
        /// * As far as we know at present, the height of a card must be divisible by 4 without any remainder.
        /// * The size of the map is not the playable area. For this purpose the PlayableArea within the map size is used 
        /// </remarks>
        public int SizeMapHeight
        {
            get { return sizeMapHeight; }
            private set
            {
                if (value != SizeMapHeight) // Adopt data only if they are really different
                {
                    value = RoundDividableBy4(value);
                    SetProperty<int>(ref sizeMapHeight, value);
                    ViewModelSizeMapHeight = value;
                }
            }
        }
        private int sizeMapHeight = 1000;
        public const string propertyName_SizeMapHeight = "SizeMapHeight";
        #endregion

        #region DATA - PlayableArea Size
        /// <summary>
        /// X-Position of the Top Left Corner of the Playable Area within Size of the Map
        /// </summary>
        public int PlayableAreaTopLeftX
        {
            get { return mapPlayableAreaW; }
            set { if (value >= 0 && value <= SizeMapWidth) { SetProperty<int>(ref mapPlayableAreaW, value); } }
        }
        private int mapPlayableAreaW = 800;
        private const string propertyName_PlayableAreaTopLeftX = "PlayableAreaTopLeftX";

        /// <summary>
        /// Y-Position of the Top Left Corner of the Playable Area within Size of the Map
        /// </summary>
        public int PlayableAreaTopLeftY
        {
            get { return mapPlayableAreaH; }
            set { if (value >= 0 && value <= SizeMapHeight) { SetProperty<int>(ref mapPlayableAreaH, value); } }
        }
        private int mapPlayableAreaH = 800;
        private const string propertyName_PlayableAreaTopLeftY = "PlayableAreaTopLeftY";

        /// <summary>
        /// X-Position of the Bottom Right Corner of the Playable Area within Size of the Map
        /// </summary>
        public int PlayableAreaButtomRightX
        {
            get { return mapPlayableAreaX; }
            set { if (value >= 0 && value <= SizeMapWidth) { SetProperty<int>(ref mapPlayableAreaX, value); } }
        }
        private int mapPlayableAreaX = 50;
        private const string propertyName_PlayableAreaButtomRightX = "PlayableAreaButtomRightX";

        /// <summary>
        /// Y-Position of the Bottom Right Corner of the Playable Area within Size of the Map
        /// </summary>
        public int PlayableAreaBottomRightY
        {
            get { return mapPlayableAreaY; }
            set { if (value >= 0 && value <= SizeMapHeight) { SetProperty<int>(ref mapPlayableAreaY, value); } }
        }
        private int mapPlayableAreaY = 50;
        private const string propertyName_PlayableAreaBottomRightY = "PlayableAreaBottomRightY";
        #endregion

        //  Dont know if i need that   
        public Empty? RandomlyPlacedThirdParties { get; set; }
        public int? ElementCount { get; set; }



        /// <summary>
        /// Collection of all Islands/Elements on the Map
        /// </summary>
        public ObservableCollection<IslandOnMap> IslandOnMaps
        {
            get { return islandOnMaps; }
            set { SetProperty<ObservableCollection<IslandOnMap>>(ref islandOnMaps, value); }
        }
        private ObservableCollection<IslandOnMap> islandOnMaps = new ObservableCollection<IslandOnMap>();

        /// <summary>
        /// Is Any Island/Element on the Map selected?
        /// </summary>
        public bool HasSelectedIsland
        {
            get { return hasSelectedIsland; }
            set { if (value != hasSelectedIsland) { SetProperty<bool>(ref hasSelectedIsland, value); } }
        }
        private bool hasSelectedIsland = false;

        /// <summary>
        /// Maybe selected Island/Element on the Map
        /// </summary>
        public IslandOnMap? SelectedIsland
        {
            get { return selectedIsland; }
            set { if (value != selectedIsland) { SetProperty<IslandOnMap>(ref selectedIsland, value); if (selectedIsland == null) { HasSelectedIsland = false; } else { HasSelectedIsland = true; } } }
        }
        private IslandOnMap? selectedIsland = null;

        public Session()
        {

        }

        public Session(a7tinfoModel a7tinfo)
        {
            AdoptPropertiesFromA7tinfo(a7tinfo);
        }


        /// <summary>
        /// Adopt Informations/Data from a7tinfo to current Session
        /// </summary>
        /// <param name="a7Tinfo"></param>
        private void AdoptPropertiesFromA7tinfo(a7tinfoModel a7Tinfo)
        {
            //  The user has not yet made a decision.
            SessionType = SessionType.NotSetYet;

            if (a7Tinfo != null && a7Tinfo.MapTemplate != null)
            {
                #region Map Size
                if (a7Tinfo.MapTemplate.Size != null)
                {
                    if (a7Tinfo.MapTemplate.Size.Length == 2)
                    {
                        SizeMapHeight = a7Tinfo.MapTemplate.Size[0];
                        SizeMapWidth = a7Tinfo.MapTemplate.Size[1];
                    }
                    else { Log.Logger.Error("a7tinfo Size contains != 2 Entrys: {0}", a7Tinfo.MapTemplate.Size); }
                }
                else { Log.Logger.Error("a7tinfoModel Size is null"); }
                #endregion

                #region PlayableArea
                if (a7Tinfo.MapTemplate.PlayableArea != null)
                {
                    if (a7Tinfo.MapTemplate.PlayableArea.Length == 4)
                    {
                        PlayableAreaTopLeftX = a7Tinfo.MapTemplate.PlayableArea[0];
                        PlayableAreaTopLeftY = a7Tinfo.MapTemplate.PlayableArea[1];
                        PlayableAreaButtomRightX = a7Tinfo.MapTemplate.PlayableArea[2];
                        PlayableAreaBottomRightY = a7Tinfo.MapTemplate.PlayableArea[3];
                    }
                    else { Log.Logger.Error("a7tinfo PlayableArea contains != 4 Entrys: {0}", a7Tinfo.MapTemplate.Size); }
                }
                else { Log.Logger.Error("a7tinfoModel PlayableArea is null"); }
                #endregion

                #region RandomlyPlacedThirdParties
                RandomlyPlacedThirdParties = a7Tinfo.MapTemplate.RandomlyPlacedThirdParties;
                #endregion

                if (a7Tinfo.MapTemplate.TemplateElement != null)
                {
                    //  Clear Elements in Sessions (primary Islands)
                    islandOnMaps.Clear();
                    //  Create Element from Template
                    foreach (TemplateElement mapElement in a7Tinfo.MapTemplate.TemplateElement)
                    {
                        //  Create Element (primary Islands)
                        var island = CreateIslandOnMap();
                        //  Adopt map Element properties from a7tinfo
                        island.AdoptPropertiesFromA7tinfo(mapElement);
                        //  Add to List
                        islandOnMaps.Add(island);
                        //  Hint: The Islands are not ready to use yet. <see="Initialize">
                    }
                }
                else { Log.Logger.Info("a7tinfo does not contain any TemplateElement (Island)"); }
            }
            else { Log.Logger.Error("a7tinfoModel is null");}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// After creating 
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public void Initialize()
        {
            if (this.Region == WorldRegion.NotSetYet)
            {
                throw new ArgumentException("Dear Dev, Property WorldRegion must be set correct before calling this function.");
            }

            foreach(var island in islandOnMaps)
            {
                island.AdoptPropertiesFromKnownIslands(this.Region);
                
                island.LoadPngs_VanillaImagePathMapImage();     //  TODO: Prio 3 - Andere Bilder laden lassen
            }
        }

        public IslandOnMap CreateIslandOnMap()
        {
            //  Create a Reference to Session MapWidth. A up-to-date value is required to flipped Element y-Position. Therefore as Reference
            var referenceToSizeMapWidth = new VariableReference<int>(() => this.sizeMapWidth, x => { this.sizeMapWidth = x; });
            //  Create Element (primary Islands)
            var island = new IslandOnMap(referenceToSizeMapWidth);

            return island;
        }

        public void SetSelectedIsland(Guid instanceId)
        {
            var result = GetIslandByInstanceId(instanceId);
            if (result.IsSuccess)
            {
                SelectedIsland = result.Value;
            }
            else
            {
                SelectedIsland = null;
            }
        }

        public Result<IslandOnMap> GetIslandByInstanceId(Guid instanceId)
        {
            var list = IslandOnMaps.Where(x => x.InstanceID == instanceId).ToList();
            if (list != null)
            {
                if(list.Count > 1)
                {
                    Log.Logger.Error("Strange. InstanceId should be unique wihin a map. But i found {0} entrys of InstanceId {1}", list.Count, instanceId);
                }
                return Result.Ok(list[0]);
            }
            else
            {
                return Result.Fail(String.Empty);
            }
        }

        private static int RoundDividableBy4(int value)
        {
            if (value % 4 != 0)
            {
                value = (value / 4) * 4;
            };
            return value;
        }

        private void CheckWarningLargeMapSize()
        {
            if ((this.ViewModelSizeMapHeight * this.ViewModelSizeMapWidth) > ConfigWarningMapDimension)
            {
                this.ViewModelShowWarningAboutMapSize = true;
            }
            else
            {
                this.ViewModelShowWarningAboutMapSize = false;
            }
        }
    }

    
}
