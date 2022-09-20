using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Anno_World_Manager.model;
using Anno_World_Manager.viewmodel.baseclasses;
using Anno_World_Manager.helper;
using Anno_World_Manager.view;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Markup;

namespace Anno_World_Manager.viewmodel
{
    public class MapEditViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// The session with map, islands, etc. that is being edited here.
        /// </summary>
        public Session Session
        {
            get { return session; }
            set
            {
                //  do nothing if old == new :-D
                if (value == session) { return; }
                //  unsubscribe from old Session
                if (session != null) { session.PropertyChanged -= Session_PropertyChanged; }
                //  set Property
                SetProperty<Session>(ref session, value);
                //  subribe to new Session
                if (session != null)
                {
                    session.PropertyChanged += Session_PropertyChanged;
                    //  as we have Map Size now, do a recalucation (which results in redraw because auf databinding from view)
                    CalculateHeightWidthOfMapCanvasContainer();
                    CalculatePlayerAreaMarginFromSessionData();
                }
            }
        }
        private Session session = new Session();


        /// <summary>
        /// Collection of all dragable Islands
        /// </summary>
        public ObservableCollection<IslandViewModel> Islands
        {
            get { return islands; }
            private set { SetProperty<ObservableCollection<IslandViewModel>>(ref islands, value); }
        }
        private ObservableCollection<IslandViewModel> islands = new ObservableCollection<IslandViewModel>();

        /// <summary>
        /// Determines whether drag-drop operations of islands on the map should be automatically aligned to the map border.
        /// </summary>
        public bool IsIslandSnappingAktivated
        {
            get { return isIslandSnappingAktivated; }
            set { SetProperty<bool>(ref isIslandSnappingAktivated, value); }
        }
        private bool isIslandSnappingAktivated = true;

        /// <summary>
        /// Rotation Angle of Map to Display within Anno World Manager
        /// </summary>
        public int RotationAngle
        {
            get { return rotationAngle; }
            set { SetProperty<int>(ref rotationAngle, value); }
        }
        private int rotationAngle = -45;



        private int mapMargin = 50;






        
        /// <summary>
        /// Height of the Canvas Container (!) which holds the Map as Canvas.
        /// </summary>
        public int MapCanvasHeight
        {
            get { return mapCanvasHeight; }
            private set { SetProperty<int>(ref mapCanvasHeight, value); }
        }
        private int mapCanvasHeight = 1000;

        /// <summary>
        /// Width of the Canvas Container (!) which holds the Map as Canvas
        /// </summary>
        public int MapCanvasWidth
        {
            get { return mapCanvasWidth; }
            private set { SetProperty<int>(ref mapCanvasWidth, value); }
        }
        private int mapCanvasWidth = 1000;


        #region Playable Area - Configuration by User - As Margins

        public int PlayableAreaMarginLeft
        {
            get { return _playablaAreaMarginLeft; }
            set
            {
                if (_playablaAreaMarginLeft != value)
                {
                    SetProperty<int>(ref _playablaAreaMarginLeft, value);
                    CalculatePlayerAreaCanvasTranlateAndDimension();
                }
            }
        }
        private int _playablaAreaMarginLeft = 0;
        private const String propertyName_PlayableAreaMarginLeft = "PlayableAreaMarginLeft";

        public int PlayableAreaMarginTop
        {
            get { return _playablaAreaMarginTop; }
            set
            {
                if (_playablaAreaMarginTop != value)
                {
                    SetProperty<int>(ref _playablaAreaMarginTop, value);
                    CalculatePlayerAreaCanvasTranlateAndDimension();
                }
            }
        }
        private int _playablaAreaMarginTop = 0;
        private const String propertyName_PlayableAreaMarginTop = "PlayableAreaMarginTop";

        public int PlayableAreaMarginRight
        {
            get { return _playablaAreaMarginRight; }
            set
            {
                if (_playablaAreaMarginRight != value)
                {
                    SetProperty<int>(ref _playablaAreaMarginRight, value);
                    CalculatePlayerAreaCanvasTranlateAndDimension();
                }
            }
        }
        private int _playablaAreaMarginRight = 0;
        private const String propertyName_PlayableAreaMarginRight = "PlayableAreaMarginRight";

        public int PlayableAreaMarginBottom
        {
            get { return _playablaAreaMarginBottom; }
            set
            {
                if (_playablaAreaMarginBottom != value)
                {
                    SetProperty<int>(ref _playablaAreaMarginBottom, value);
                    CalculatePlayerAreaCanvasTranlateAndDimension();
                }
            }
        }
        private int _playablaAreaMarginBottom = 0;
        private const String propertyName_PlayableAreaMarginBottom = "PlayableAreaMarginBottom";

        #endregion


        #region Playable Area - Ui Needs - As Translate & Width/Height
        /// <summary>
        /// TranslateX Transform Value to position the canvas correctly
        /// </summary>
        /// <remarks>
        /// The Canvas within the WPF UserControl <see cref="MapEdit"/> needs his Position 
        /// as TranslateTransform (x/y) and Size (Width/Height).
        /// </remarks>
        public int PlayableAreaCanvasTranslateX
        {
            get { return _playablaAreaCanvasTranslateX; }
            private set { SetProperty<int>(ref _playablaAreaCanvasTranslateX, value); }
        }
        private int _playablaAreaCanvasTranslateX = 0;
        private const String propertyName_PlayableAreaTranslateX = "PlayableAreaCanvasTranslateX";

        public int PlayableAreaCanvasTranslateY
        {
            get { return _playablaAreaCanvasTranslateY; }
            private set { SetProperty<int>(ref _playablaAreaCanvasTranslateY, value); }
        }
        private int _playablaAreaCanvasTranslateY = 0;
        private const String propertyName_PlayableAreaTranslateY = "PlayableAreaCanvasTranslateY";

        public int PlayableAreaCanvasWidth
        {
            get { return _playablaAreaCanvasWidth; }
            private set { SetProperty<int>(ref _playablaAreaCanvasWidth, value); }
        }
        private int _playablaAreaCanvasWidth = 0;
        private const String propertyName_PlayableAreaWidth = "PlayableAreaCanvasWidth";

        public int PlayableAreaCanvasHeight
        {
            get { return _playablaAreaCanvasHeight; }
            private set { SetProperty<int>(ref _playablaAreaCanvasHeight, value); }
        }
        private int _playablaAreaCanvasHeight = 0;
        private const String propertyName_PlayableAreaHeight = "PlayableAreaCanvasHeight";

        #endregion

        #region

        public bool ShowIslandsFromRegionOldWorld
        {
            get { return showIslandsFromRegionOldWold; }
            set { SetProperty<bool>(ref showIslandsFromRegionOldWold, value); }
        }
        private bool showIslandsFromRegionOldWold = false;
        private const String propertyName_ShowIslandsFromRegionOldWorld = "ShowIslandsFromRegionOldWorld";

        
        
        public bool ShowIslandsFromRegionCapTrelawney
        {
            get { return showIslandsFromRegionCapTrelawney; }
            set { SetProperty<bool>(ref showIslandsFromRegionCapTrelawney, value); }
        }
        private bool showIslandsFromRegionCapTrelawney = false;
        private const String propertyName_ShowIslandsFromRegionCapTrelawney = "ShowIslandsFromRegionCapTrelawney";

        private const String propertyName_ShowIslandsFromRegionArctic = "ShowIslandsFromRegionArctic";
        private bool showIslandsFromRegionArctic = false;
        public bool ShowIslandsFromRegionArctic
        {
            get { return showIslandsFromRegionArctic; }
            set { SetProperty<bool>(ref showIslandsFromRegionArctic, value); }
        }

        private const String propertyName_ShowIslandsFromRegionNewWorld = "ShowIslandsFromRegionNewWorld";
        private bool showIslandsFromRegionNewWorld = false;
        public bool ShowIslandsFromRegionNewWorld
        {
            get { return showIslandsFromRegionNewWorld; }
            set { SetProperty<bool>(ref showIslandsFromRegionNewWorld, value); }
        }

        private const String propertyName_ShowIslandsFromRegionEnbesa = "ShowIslandsFromRegionEnbesa";
        private bool showIslandsFromRegionEnbesa = false;
        public bool ShowIslandsFromRegionEnbesa
        {
            get { return showIslandsFromRegionEnbesa; }
            set { SetProperty<bool>(ref showIslandsFromRegionEnbesa, value); }
        }

        private const String propertyName_ShowIslandsFromScenarios = "ShowIslandsFromScenarios";
        private bool showIslandsFromScenarios = false;
        public bool ShowIslandsFromScenarios
        {
            get { return showIslandsFromScenarios; }
            set { SetProperty<bool>(ref showIslandsFromScenarios, value); }
        }

        private const String propertyName_ShowIslandsFromCommunity = "ShowIslandsFromCommunity";
        private bool showIslandsFromCommunity = false;
        public bool ShowIslandsFromCommunity
        {
            get { return showIslandsFromCommunity; }
            set { SetProperty<bool>(ref showIslandsFromCommunity, value); }
        }


        private const String propertyName_ShowIslandsAll = "ShowIslandsAll";
        private bool showIslandsAll = false;
        public bool ShowIslandsAll
        {
            get { return showIslandsAll; }
            set { SetProperty<bool>(ref showIslandsAll, value); }
        }

        private const String propertyName_ShowIslandIsSettleable = "ShowIslandIsSettleable";
        private bool showIslandsIsSettleable = true;
        public bool ShowIslandIsSettleable
        {
            get { return showIslandsIsSettleable; }
            set { SetProperty<bool>(ref showIslandsIsSettleable, value); }
        }

        private const String propertyName_ShowIslandIsDecorative = "ShowIslandIsDecorative";
        private bool showIslandsIsDecorative = false;
        public bool ShowIslandIsDecorative
        {
            get { return showIslandsIsDecorative; }
            set { SetProperty<bool>(ref showIslandsIsDecorative, value); }
        }

        private const String propertyName_ShowIslandIsThirdParty = "ShowIslandIsThirdParty";
        private bool showIslandsIsThirdParty = true;
        public bool ShowIslandIsThirdParty
        {
            get { return showIslandsIsThirdParty; }
            set { SetProperty<bool>(ref showIslandsIsThirdParty, value); }
        }

        private const String propertyName_ShowIslandsIsSmall = "ShowIslandsIsSmall";
        private bool showIslandsIsSmall = false;
        public bool ShowIslandsIsSmall
        {
            get { return showIslandsIsSmall; }
            set { SetProperty<bool>(ref showIslandsIsSmall, value); }
        }

        private const String propertyName_ShowIslandsIsMedium = "ShowIslandsIsMedium";
        private bool showIslandsIsMedium = false;
        public bool ShowIslandsIsMedium
        {
            get { return showIslandsIsMedium; }
            set { SetProperty<bool>(ref showIslandsIsMedium, value); }
        }

        private const String propertyName_ShowIslandsIsLarge = "ShowIslandsIsLarge";
        private bool showIslandsIsLarge = true;
        public bool ShowIslandsIsLarge
        {
            get { return showIslandsIsLarge; }
            set { SetProperty<bool>(ref showIslandsIsLarge, value); }
        }

        #endregion

        public bool DragDropIsInProgress
        {
            get { return dragDropIsInProgress; }
            set { if (value != dragDropIsInProgress) { SetProperty<bool>(ref dragDropIsInProgress, value); } }
        }
        private bool dragDropIsInProgress = false;

        

        /// <summary>
        /// Ctor - Defaults to World Region Old World
        /// </summary>
        /// <param name="region">Parameters in which region to apply for island selection</param>
        internal MapEditViewModel(WorldRegion region = WorldRegion.OldWorld)
        {

            CalculateHeightWidthOfMapCanvasContainer();


            CalculatePlayerAreaMarginFromSessionData();

            //  Remove every Selection by Region
            //  Goal: Save the developer from accidentally implausible preassignments
            ClearRegionSelection();

            //  Assign the filter by region according to parameter
            SetSelectionByRegion(region);



            //  subscribe Property Changes
            //  Remarks: subscribe Session Property Changes - solved in setter
            this.PropertyChanged += MapEditViewModel_PropertyChanged;

            //  Apply Check
            CheckSelectionIfIslandsAll();

            //  Build initial Island List
            FilterIslandList();
        }


        private void Session_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Session.propertyName_SizeMapHeight:
                case Session.propertyName_SizeMapWidth:
                    CalculateHeightWidthOfMapCanvasContainer();
                    CalculatePlayerAreaCanvasTranlateAndDimension();
                    break;
                    
            }
        }


        


        /// <summary>
        /// Handle Property Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapEditViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case propertyName_ShowIslandsAll:
                    CheckSelectionIfIslandsAll();
                    FilterIslandList();
                    break;
                case propertyName_ShowIslandsFromRegionOldWorld:
                case propertyName_ShowIslandsFromRegionCapTrelawney:
                case propertyName_ShowIslandsFromRegionArctic:
                case propertyName_ShowIslandsFromRegionNewWorld:
                case propertyName_ShowIslandsFromRegionEnbesa:
                case propertyName_ShowIslandsFromScenarios:
                case propertyName_ShowIslandsFromCommunity:
                case propertyName_ShowIslandIsSettleable:
                case propertyName_ShowIslandIsDecorative:
                case propertyName_ShowIslandIsThirdParty:
                case propertyName_ShowIslandsIsSmall:
                case propertyName_ShowIslandsIsMedium:
                case propertyName_ShowIslandsIsLarge:
                    FilterIslandList();
                    break;

            }
        }

        private void ClearRegionSelection()
        {
            ShowIslandsFromRegionOldWorld = false;
            ShowIslandsFromRegionCapTrelawney = false;
            ShowIslandsFromRegionArctic = false;
            ShowIslandsFromRegionNewWorld = false;
            ShowIslandsFromRegionEnbesa = false;
            ShowIslandsFromScenarios = false;
            ShowIslandsFromCommunity = false;
        }
        private void SetSelectionByRegion(WorldRegion region)
        {
            switch(region)
            {
                case WorldRegion.OldWorld:
                    ShowIslandsFromRegionOldWorld = true;
                    break;
                case WorldRegion.CapTrelawney:
                    ShowIslandsFromRegionCapTrelawney = true;
                    break;
                case WorldRegion.Arctic:
                    ShowIslandsFromRegionArctic = true;
                    break;
                case WorldRegion.NewWorld:
                    ShowIslandsFromRegionNewWorld = true;
                    break;
                case WorldRegion.Enbesa:
                    ShowIslandsFromRegionEnbesa = true;
                    break;
                case WorldRegion.ScenarioEdenBurning:
                case WorldRegion.ScenarioTheAnarchist:
                    ShowIslandsFromScenarios = true;
                    break;
                case WorldRegion.CommunityCreated:
                    ShowIslandsFromCommunity = true;
                    break;
                default:
                    Log.Logger.Debug("DEV: A WorldRegion was selected that is not handled in the code: {0}", region);
                    break;
            }
        }

        private void CheckSelectionIfIslandsAll()
        {
            if (ShowIslandsAll)
            {
                //  Reselect ShowIslands Propertys
                ShowIslandIsSettleable = false;
                ShowIslandIsDecorative = false;
                ShowIslandIsThirdParty = false;
                ShowIslandsIsSmall = false;
                ShowIslandsIsMedium = false;
                ShowIslandsIsLarge = false;
            }
        }

        private void FilterIslandList()
        {
            Log.Logger.Debug("rebuild Island List");

            /*
             * First Solution Linq Extension essentialiy did not work. Dont know why. Logic changed afterwards, commented Version did not work 
            //  Hmmm..... Vielleicht doch mit einem Iterator und Scoring ob passt ?!?

            var newlist = Runtime.IslandsKnown.KnownIslands
                .If(ShowIslandsWithRegionOldWorld, q => q.Where(x => x.Regions.Contains(WorldRegion.OldWorld)))
                .If(ShowIslandsWithRegionNewWorld, q => q.Where(x => x.Regions.Contains(WorldRegion.NewWorld)))
                .If(ShowIslandIsSettleable, q => q.Where(x => x.CanBeColonized == IslandCanBeColonized.Yes))
                .If(ShowIslandsIsLarge, q => q.Where(x => x.Size == IslandSize.Large))
                .If(ShowIslandsIsMedium, q => q.Where(x => x.Size == IslandSize.Medium))
                .If(ShowIslandsIsSmall, q => q.Where(x => x.Size == IslandSize.Small))
                .ToList();
            */


            //  Second Solution : Iterate and Score every entry

            islands.Clear();

            int index = 0;
            int count = Runtime.IslandsKnown.KnownIslands.Count();
            while (index < count)
            {
                bool IsCorrectRegion = false;
                

                if ( ShowIslandsFromRegionOldWorld && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.OldWorld)) { IsCorrectRegion = true; }
                if ( ShowIslandsFromRegionCapTrelawney && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.CapTrelawney)) { IsCorrectRegion = true; }
                if ( ShowIslandsFromRegionArctic && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.Arctic)) { IsCorrectRegion = true; }
                if ( ShowIslandsFromRegionNewWorld && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.NewWorld)) { IsCorrectRegion = true; }
                if ( ShowIslandsFromRegionEnbesa && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.Enbesa)) { IsCorrectRegion = true; }
                if ( ShowIslandsFromScenarios && (Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.ScenarioEdenBurning) || Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.ScenarioTheAnarchist))) { IsCorrectRegion = true; }
                if (ShowIslandsFromCommunity && Runtime.IslandsKnown.KnownIslands[index].Regions.Contains(WorldRegion.CommunityCreated)) { IsCorrectRegion = true; }

                bool doCopy = false;

                if (ShowIslandsAll)
                {
                    if (IsCorrectRegion) { doCopy = true; }
                }
                else
                {
                    if (IsCorrectRegion && ShowIslandIsSettleable && Runtime.IslandsKnown.KnownIslands[index].CanBeColonized == IslandCanBeColonized.Yes) { doCopy = true; }
                    if (IsCorrectRegion && ShowIslandIsDecorative && Runtime.IslandsKnown.KnownIslands[index].CanBeColonized == IslandCanBeColonized.No) { doCopy = true; }
                    if (IsCorrectRegion && ShowIslandIsThirdParty && Runtime.IslandsKnown.KnownIslands[index].Type == IslandType.ThirdParty) { doCopy = true; }
                    if (IsCorrectRegion && ShowIslandsIsSmall && Runtime.IslandsKnown.KnownIslands[index].Size == IslandSize.Small) { doCopy = true; }
                    if (IsCorrectRegion && ShowIslandsIsMedium && Runtime.IslandsKnown.KnownIslands[index].Size == IslandSize.Medium) { doCopy = true; }
                    if (IsCorrectRegion && ShowIslandsIsLarge && Runtime.IslandsKnown.KnownIslands[index].Size == IslandSize.Large) { doCopy = true; }
                }
                
                if (doCopy)
                {
                    IslandViewModel _island = new IslandViewModel(Runtime.IslandsKnown.KnownIslands[index]);
                    Islands.Add(_island);
                }
                index++;
            }
        }

        /// <summary>
        /// Calculate Canvas Map holding Container in Dimension Height and Width (Trigonomy)
        /// </summary>
        private void CalculateHeightWidthOfMapCanvasContainer()
        {

            int _mapRotationAngle = RotationAngle;

            int newMapCanvasWidth = MapCanvasWidth;
            int newMapCanvasHeight = MapCanvasHeight;

            switch (_mapRotationAngle)
            {

                case 0:
                case 180:
                    //  Angle 0° or 180°
                    Log.Logger.Debug("Map Rotationangle is 0° or 180°: {0}", _mapRotationAngle);
                    newMapCanvasWidth = session.SizeMapWidth;
                    newMapCanvasHeight = session.SizeMapHeight;
                    break;
                case 90:
                case 270:
                    //  Angle 90° or 270°
                    Log.Logger.Debug("Map Rotationangle is 90° or 270°: {0}", _mapRotationAngle);
                    newMapCanvasWidth = session.SizeMapHeight;
                    newMapCanvasHeight = session.SizeMapWidth;
                    break;
                default:
                    //  Angle is anything else
                    Log.Logger.Debug("Map Rotationangle is: {0}", _mapRotationAngle);
                    double angleInRadians = DegreeToRadian(_mapRotationAngle);
                    double widthA = Math.Abs(Math.Cos(angleInRadians) * session.SizeMapWidth);
                    double widthB = Math.Abs(Math.Sin(angleInRadians) * session.SizeMapWidth);
                    double heightA = Math.Abs(Math.Cos(angleInRadians) * session.SizeMapHeight);
                    double heightB = Math.Abs(Math.Sin(angleInRadians) * session.SizeMapHeight);
                    newMapCanvasWidth = (int)(widthA + heightB);
                    newMapCanvasHeight = (int)(widthB + heightA);
                    break;
            }

            newMapCanvasWidth += mapMargin;
            newMapCanvasHeight += mapMargin;

            if (MapCanvasWidth != newMapCanvasWidth) { MapCanvasWidth = newMapCanvasWidth; }
            if (MapCanvasHeight != newMapCanvasHeight) { MapCanvasHeight = newMapCanvasHeight; }

        }


        private void CalculatePlayerAreaMarginFromSessionData()
        {
            if (Session != null)
            {
                int margin_left = Session.PlayableAreaTopLeftX;
                int margin_top = Session.SizeMapHeight - Session.PlayableAreaBottomRightY;          //  Because ... Hmmm... Anno stored his Y-Positions mirrored ... whyever ...
                int margin_right = Session.SizeMapWidth - Session.PlayableAreaButtomRightX;
                int margin_bottom = Session.PlayableAreaTopLeftY; ;                                 //  Because ... Hmmm... Anno stored his Y-Positions mirrored ... whyever ...
                this.PlayableAreaMarginLeft = margin_left;
                this.PlayableAreaMarginTop = margin_top;
                this.PlayableAreaMarginRight = margin_right;
                this.PlayableAreaMarginBottom = margin_bottom;
            }
        }

        private void CalculatePlayerAreaCanvasTranlateAndDimension()
        {
            if (Session != null)
            {
                int map_height = Session.SizeMapHeight;
                int map_width = Session.SizeMapWidth;
                int pl_height = map_height - this.PlayableAreaMarginTop - this.PlayableAreaMarginBottom;
                int pl_width = map_width - this.PlayableAreaMarginLeft - this.PlayableAreaMarginRight;
                int pl_translate_y = (this.PlayableAreaMarginBottom - this.PlayableAreaMarginTop)/2 * -1;       //  /2 because Canvas is allready centered
                int pl_translate_x = (this.PlayableAreaMarginRight - this.PlayableAreaMarginLeft)/2 * -1;

                this.PlayableAreaCanvasTranslateX = pl_translate_x;
                this.PlayableAreaCanvasTranslateY = pl_translate_y;
                this.PlayableAreaCanvasWidth = pl_width;
                this.PlayableAreaCanvasHeight = pl_height;
            }
        }


        /// <summary>
        /// Add Island on Map
        /// </summary>
        /// <param name="islandViewModel"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        public void CommandAddIsland(IslandViewModel islandViewModel, int positionX, int positionY)
        {
            IslandOnMap island = CreateIslandOnMap(islandViewModel._island);
            island.Png = islandViewModel.Png;
            CommandAddIsland(islandViewModel, positionX, positionY);
        }

        /// <summary>
        /// Add Island on Map
        /// </summary>
        /// <param name="islandViewModel"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        public void CommandAddIsland(IslandOnMap islandViewModel, int positionX, int positionY)
        {
            islandViewModel.PositionX = positionX;
            islandViewModel.ViewModelPositionY = positionY;    
            //  Add Island
            Session.IslandOnMaps.Add(islandViewModel);
        }

        public IslandOnMap CreateIslandOnMap(model.Island islandModel)
        {
            IslandOnMap island = Session.CreateIslandOnMap();
            island.CopyIslandProperties(islandModel);
            return island;
        }

        public void CommandRemoveIsland(Guid instanceID)
        {
            var toRemove = Session.IslandOnMaps.Where(x => x.InstanceID == instanceID).ToList();
            if (toRemove != null)
            {
                int c = toRemove.Count();
                int i = 0;
                while (i < c)
                {
                    Session.IslandOnMaps.Remove(toRemove[i]);
                    i++;
                }
            }
            else
            {
                Log.Logger.Debug("This was unexpected. Could not found or more the 1 result Entrys for instanceID {0}", instanceID);
            }
        }

        public void CommandIslandSetPosition()
        {

        }

        public void CommandDragDropIsInProgress()
        {
            DragDropIsInProgress = true;
        }

        public void CommandDragDropHasStopped()
        {
            DragDropIsInProgress = false;
        }

        public void CommandSelectedIslandSet(Guid instanceId)
        {
            Session.SetSelectedIsland(instanceId);
        }

        public void CommandSelectedIslandClear()
        {
            Session.SelectedIsland = null;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
            //  TODO: Implement Dispose()
            throw new NotImplementedException();
        }
    }
}
