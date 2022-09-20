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

namespace Anno_World_Manager.viewmodel
{
    public class MapEditViewModel2 : ViewModelBase, IDisposable
    {
        /// <summary>
        /// The session with map, islands, etc. that is being edited here.
        /// </summary>
        public Session Session
        {
            get { return session; }
            set { 
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
                    CalculateMapCanvasHeightWidth();
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
        /// Rotation Angle of Map to Display
        /// </summary>
        public int RotationAngle
        {
            get { return rotationAngle; }
            set {  SetProperty<int>(ref rotationAngle, value); }
        }
        private int rotationAngle = -45;

        private int mapMargin = 50;






        private int mapCanvasHeight = 1000;
        private int mapCanvasWidth = 1000;
        public int MapCanvasHeight
        {
            get { return mapCanvasHeight; }
            set { SetProperty<int>(ref mapCanvasHeight, value); }
        }
        public int MapCanvasWidth
        {
            get { return mapCanvasWidth; }
            set { SetProperty<int>(ref mapCanvasWidth, value); }
        }



        //  TODO: Überlegen doch einfach ein rect daraus machen? w/h/x/y
        private int mapPlayableAreaW = 800;
        private int mapPlayableAreaH = 800;
        private int mapPlayableAreaX = 50;
        private int mapPlayableAreaY = 50;
        public int MapPlayableAreaWidth
        {
            get { return mapPlayableAreaW; }
            set { SetProperty<int> (ref mapPlayableAreaW, value); }
        }
        public int MapPlayableAreaHeight
        {
            get { return mapPlayableAreaH; }
            set { SetProperty<int>(ref mapPlayableAreaH, value); }
        }
        public int MapPlayableAreaX
        {
            get { return mapPlayableAreaX; }
            set { SetProperty<int>(ref mapPlayableAreaX, value); }
        }
        public int MapPlayableAreaY
        {
            get { return mapPlayableAreaY; }
            set { SetProperty<int>(ref mapPlayableAreaY, value); }
        }


        private const String propertyName_ShowIslandsFromRegionOldWorld = "ShowIslandsFromRegionOldWorld";
        private bool showIslandsFromRegionOldWold = false;
        public bool ShowIslandsFromRegionOldWorld
        {
            get { return showIslandsFromRegionOldWold; }
            set { SetProperty<bool>(ref showIslandsFromRegionOldWold, value); }
        }

        private const String propertyName_ShowIslandsFromRegionCapTrelawney = "ShowIslandsFromRegionCapTrelawney";
        private bool showIslandsFromRegionCapTrelawney = false;
        public bool ShowIslandsFromRegionCapTrelawney
        {
            get { return showIslandsFromRegionCapTrelawney; }
            set { SetProperty<bool>(ref showIslandsFromRegionCapTrelawney, value); }
        }

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


        /// <summary>
        /// Ctor - Defaults to World Region Old World
        /// </summary>
        /// <param name="region">Parameters in which region to apply for island selection</param>
        internal MapEditViewModel2(WorldRegion region = WorldRegion.OldWorld)
        {
            
            CalculateMapCanvasHeightWidth();

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
            switch(e.PropertyName)
            {
                case Session.propertyName_SizeMapHeight:
                case Session.propertyName_SizeMapWidth:
                    CalculateMapCanvasHeightWidth();
                    break;
                //  TODO: Playable Area has Changed
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
        /// Calculate Map Canvas Height and Width (Trigonomy)
        /// </summary>
        /// 
        private void CalculateMapCanvasHeightWidth()
        {

            int _mapRotationAngle = RotationAngle;

            int newMapCanvasWidth = MapCanvasWidth;
            int newMapCanvasHeight = MapCanvasHeight;
            
            switch(_mapRotationAngle)
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


        


        public void CommandAddIsland()
        {

        }

        public void CommandRemoveIsland()
        {

        }

        public void CommandIslandSetPosition()
        {

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
