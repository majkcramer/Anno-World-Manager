using Anno_World_Manager.viewmodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Anno_World_Manager.helper.Geometrie;
using Vector2 = Anno_World_Manager.helper.Geometrie.Vector2;
using Anno_World_Manager.model;
using System.Diagnostics;

namespace Anno_World_Manager.view
{
    /// <summary>
    /// Interaktionslogik für MapEdit.xaml
    /// </summary>
    public partial class MapEdit : UserControl
    {
        /// <summary>
        /// Activate Logging on Debug Level for DragDrop Operations within this Code Behind Class
        /// </summary>
        private bool activateDragDropDebug = true;


        /// <summary>
        /// 
        /// </summary>
        private MapElementIsland _dragdropIsland_new = null;

        
        internal MapEditViewModel ViewModel
        {
            get { GuaranteeViewModelAllocation(); return _viewModel; }
            set { _viewModel = value; }
        }
        private MapEditViewModel _viewModel = null;



        public MapEdit(MapEditViewModel viewModel)
        {
            InitializeComponent();

            this.ViewModel = viewModel;
            this.DataContext = viewModel;
        }

        private void GuaranteeViewModelAllocation()
        {
            if (_viewModel == null)
            {
                if (this.DataContext != null)
                {
                    var dataObj = this.DataContext as DataObject;
                    var receivedViewModel = dataObj.GetData(typeof(MapEditViewModel)) as MapEditViewModel;
                    if (receivedViewModel == null)
                    {
                        throw new Exception("DataContext of MapEdit UserControl is null !");
                    }
                    _viewModel = receivedViewModel;
                }
                else
                {
                    Log.Logger.Debug("Is ViewModel not yet initialized ?!?");
                }
            }
        }


        

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void SliderMapSizeW_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SliderMapSizeH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SliderPlayableAreaW_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SliderPlayableAreaH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SliderPlayableAreaX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void SliderPlayableAreaY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }






        /// <summary>
        ///     Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        ///     Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        ///     Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
        }


        #region DragDrop - Helper Functions

        private MapElementIsland DragDropVisualAppereanceCreate(IslandOnMap viewmodel)
        {
            var element = new MapElementIsland();
            element.DataContext = viewmodel;
            return element;
        }

        private void DragDropVisualAppereanceAdd(MapElementIsland view)
        {
            //  Check if the function is applied correctly
            if (view.DataContext == null)
            {
                Log.Logger.Warn("Dear DEV, the view you provided doest not contain any viewmodel !");
            }
            //  Temporary caching of the DragDrop island and its properties
            _dragdropIsland_new = view;
            //  Let the ViewModel set the DragDrop state correctly.
            ViewModel.CommandDragDropIsInProgress();
            //  Add the visual appearance to the DragDrop layer for display
            DragAndDropOnlyLayer.Children.Add(view);
        }

        private void DragDropVisualAppereanceClear()
        {
            //  Clear temporary cache of the DragDrop Island and its properties
            _dragdropIsland_new = null;
            //  Let the ViewModel set the DragDrop state correctly.
            ViewModel.CommandDragDropHasStopped();
            //  Clear the visual appearence of the DragDrop layer
            DragAndDropOnlyLayer.Children.Clear();
        }


        private bool HasActiveDragDropIsland()
        {
            //  Island which is in drag/drop workprogress is stored in _dragdropIsland.
            //  Therefore: If _dragdropIsland == null : No Active Island Drag Drop in Progress
            return _dragdropIsland_new != null;
        }

        private IslandOnMap ConvertIslandViewModelToIslandOnMap(IslandViewModel modelToConvert)
        {
            IslandOnMap retval = ViewModel.CreateIslandOnMap(modelToConvert._island);
            retval.Png = modelToConvert.Png;
            return retval;
        }

        #endregion


        #region DragDrop - Start

        /// <summary>
        /// User initiated Island Add - DragDrop from Island Listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid) 
            {
                //  Convert sender into the concrete type
                var draggedItem = sender as Grid;
                //  Convert sender datacontext into the concrete viewmodel
                IslandViewModel draggedDataContext = draggedItem.DataContext as IslandViewModel;
                //  Build DataContext i want to drag
                IslandOnMap DataContextToDrag = ConvertIslandViewModelToIslandOnMap(draggedDataContext);
                //  Create new DataObject 
                DataObject dataObject = new DataObject(typeof(IslandOnMap), DataContextToDrag);
                //  Start DragDrop with sender and data to use
                DragDrop.DoDragDrop(draggedItem, dataObject, DragDropEffects.Copy);
            }
        }


        /// <summary>
        /// User initiated Island move/remove - DragDrop from Map Canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (activateDragDropDebug) { Log.Logger.Debug("User initiated DragDrop from Map Canvas object which hopefully contains Islands: {0}", sender); }

            var canvas = sender as Canvas;
            if (canvas == null) { return; }

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            DependencyObject element = hitTestResult.VisualHit;

            //  Convert DependencyObject into a FrameworkElement, to get its DataContext (viewmodel)
            var frameworkElement = (FrameworkElement)element;
            if (frameworkElement != null)
            {
                IslandOnMap viewModel = frameworkElement.DataContext as IslandOnMap;
                if (viewModel != null)
                {
                    //  Remove the selected Island from Map Canvas
                    ViewModel.CommandRemoveIsland(viewModel.InstanceID);
                    //  Create new DataObject 
                    DataObject dataObject = new DataObject(typeof(IslandOnMap), viewModel);
                    //  Start a Stopwatch to messure the Millisecons of the DragDrop Operation
                    //  Maybe it was only a short Click and no DragDrop Operation
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    //  Start DragDrop with sender and data to use
                    DragDrop.DoDragDrop(canvas, dataObject, DragDropEffects.Move);
                    //  Stop Stopwatch
                    stopWatch.Stop();
                    //  Check the elapsed Time since begin of DragDrop Operation.
                    //  * Longer then 300 Milliseconds = a valid DragDrop Operation or single long leftclick delete = nothing to do
                    //  * Shorter = only a valid left click to select Island = bring the island back
                    //  REMARKS: If we dont like the solution, maybe we better have to check the dragdrop cached island and his propertys (eg. Height=NaN ?)
                    if (stopWatch.ElapsedMilliseconds < 300)
                    {
                        Log.Logger.Debug("We assume it was only a single short click and bring the island back");
                        ViewModel.CommandAddIsland(viewModel, viewModel.PositionX, viewModel.ViewModelPositionY);
                        ViewModel.CommandSelectedIslandSet(viewModel.InstanceID);
                    }
                }
            }
        }

        #endregion

        #region DragDrop - Becomming visual

        /// <summary>
        /// Will be fired, when a active DragDrop from outside the MapGridWrapper enters the MapGridWrapper Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapGridWrapper_DragEnter(object sender, DragEventArgs e)
        {
            if (!HasActiveDragDropIsland())
            {
                Log.Logger.Debug("DragDropIsland created");

                DataObject? datacontext_old = e.Data as DataObject;
                IslandOnMap? viewmodel = datacontext_old.GetData(typeof(IslandOnMap)) as IslandOnMap;
                MapElementIsland visualAppereance = DragDropVisualAppereanceCreate(viewmodel);
                DragDropVisualAppereanceAdd(visualAppereance);
            }
            else
            {
                Log.Logger.Warn("Confused. How could another drag drop Operation enter MapGridWrapper while a Island Drag Drop is currently in progress?!?");
            }
        }

        #endregion


        #region DragDrop - Movement

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapGridWrapper_DragOver(object sender, DragEventArgs e)
        {
            if (HasActiveDragDropIsland())
            {
                //  Check: Is Island over Map?
                Log.Logger.Debug("DragDropIsland moved");
                Point p = e.GetPosition(MapCanvasContent2); 
                IslandOverMapCheckResults _isIslandOverMap = IsIslandOverMap(p);
                switch(_isIslandOverMap.IsIslandOverMap)
                {
                    case true:
                        
                        if (_dragdropIsland_new.crossOut.Visibility != Visibility.Hidden)
                        {
                            _dragdropIsland_new.crossOut.Visibility = Visibility.Hidden;
                        }
                        switch(_isIslandOverMap.RequiresNewXYIslandCoordinates)
                        {
                            case true:
                                Canvas.SetLeft(_dragdropIsland_new, _isIslandOverMap.NewX);
                                Canvas.SetTop(_dragdropIsland_new, _isIslandOverMap.NewY);
                                break;
                            case false:
                                Canvas.SetLeft(_dragdropIsland_new, p.X);
                                Canvas.SetTop(_dragdropIsland_new, p.Y);
                                break;
                        }
                        break;

                    case false:
                        if (_dragdropIsland_new.crossOut.Visibility != Visibility.Visible)
                        {
                            _dragdropIsland_new.crossOut.Visibility = Visibility.Visible;
                        }
                        Canvas.SetLeft(_dragdropIsland_new, p.X);
                        Canvas.SetTop(_dragdropIsland_new, p.Y);
                        break;
                }
                
            }
            else
            {
                Log.Logger.Warn("?!? _dragdropIsland is empty ?!?");
            }
        }

        #endregion

        #region DragDrop - End
        private void MapGridWrapper_DragLeave(object sender, DragEventArgs e)
        {
            //  Ignored, because event fires to often. Even while dragged Mouse is always above MapGridWrapper
        }

        private void MapGridWrapper_Drop(object sender, DragEventArgs e)
        {
            Log.Logger.Debug("MapGridWrapper_Drop");
            Point p = e.GetPosition(MapCanvasContent2);
            IslandDropReleases(p, e);
        }

        private void IslandDropReleases(Point pt, DragEventArgs e)
        {
            Log.Logger.Debug("IslandDropReleases");
            var result = IsIslandOverMap(pt);
            if (result.IsIslandOverMap)
            {
                IslandOnMap? droppedData = e.Data.GetData(typeof(IslandOnMap)) as IslandOnMap;

                var droppedPositionX = Canvas.GetLeft(_dragdropIsland_new);
                var droppedPositionY = Canvas.GetTop(_dragdropIsland_new);

                ViewModel.CommandAddIsland(droppedData, (int)droppedPositionX, (int)droppedPositionY);   //  TODO - Prio 2 - Build as ICommand. Then remove local _viewModel from Code Behind
            }
            DragDropVisualAppereanceClear();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private IslandOverMapCheckResults IsIslandOverMap(Point pt)
        {
            bool isIslandOverMap_Clearly = false;
            IslandOverMapCheckResults retval_isIslandOverMap = new IslandOverMapCheckResults(false);

            if ( ! HasActiveDragDropIsland())
            {
                Log.Logger.Debug("Should not Occour !!!");
                return retval_isIslandOverMap;
            }

            //  -----------------------------------------------------------------------------
            //  Old Variant with HitTest - Not working
            //  - when inside map: allway corret
            //  - when outside map: only ~60% correct
            //  - SLOW (!)
            //
            //  HitTestResult result = VisualTreeHelper.HitTest(this.MapCanvasContent, pt);
            //
            //  if (result != null)
            //  {
            //      if (result.VisualHit is FrameworkElement)
            //      {
            //          Perform action on hit visual object.
            //

            //  -----------------------------------------------------------------------------
            //  New Variant: Simple Geometric Calculation - Sucessfull working
            //
            //  Performe simple Calculation agains MapSize X/Y/Width/Height
            //  Fun Fact: A lot faster then HitTest
            //


            int mapContentAreaW = ViewModel.Session.SizeMapWidth;
            int mapContentAreaH = ViewModel.Session.SizeMapHeight;

            int tollerance = 0;

            switch(IsIslandSnappingAktivated.IsChecked)     //  TODO: Prio 3 - Move to Settings (enable / disbale9
            {
                case true:
                    //  Island should snap to Map Boarder
                    //
                    tollerance = 50;                    
                    break;
                case null:
                case false:
                    //  Island should not snap to Map Border
                    //
                    break;
            }

            

            //  Check 1: Is Island regular on Map (ignores if snapping actived or not)

            int accepted_leftx = 0;
            int accepted_topy = 0;
            int accepted_rightx = mapContentAreaW - (int)_dragdropIsland_new.Width;
            int accepted_bottomy = mapContentAreaH - (int)_dragdropIsland_new.Height;
            if (pt.X >= accepted_leftx && pt.Y >= accepted_topy && pt.X <= accepted_rightx && pt.Y <= accepted_bottomy)
            {
                isIslandOverMap_Clearly = true;
                retval_isIslandOverMap = new IslandOverMapCheckResults(true);
            }

            //  Check 2: If Island is without Snapping not on Map, but Snapping is actived: try to identify if Island.X / Island.Y Position should be applied

            if( ! isIslandOverMap_Clearly && tollerance > 0)
            {
                int tolleranced_leftx = accepted_leftx - tollerance;
                int tolleranced_topy = accepted_topy - tollerance;
                int tolleranced_rightx = accepted_rightx + tollerance;
                int tolleranced_bottomy = accepted_bottomy + tollerance;

                if (pt.X >= tolleranced_leftx && pt.Y >= tolleranced_topy && pt.X <= tolleranced_rightx && pt.Y <= tolleranced_bottomy)
                {
                    //  Island is only with Snapping Tollernace above Map
                    int new_Island_x = (int) pt.X;
                    int new_Island_y = (int) pt.Y;
                    bool island_changed_xy = false;
                    if (pt.X <= accepted_leftx && pt.X >= tolleranced_leftx) { new_Island_x = accepted_leftx; island_changed_xy = true; }
                    if (pt.Y <= accepted_topy && pt.Y >= tolleranced_topy) { new_Island_y = accepted_topy; island_changed_xy = true; }
                    if (pt.X >= accepted_rightx && pt.X <= tolleranced_rightx ) { new_Island_x = accepted_rightx; island_changed_xy = true; }
                    if (pt.Y >= accepted_bottomy && pt.Y <= tolleranced_bottomy) { new_Island_y = accepted_bottomy; island_changed_xy = true; }

                    if (!island_changed_xy) { Log.Logger.Debug("Here must be something wrong!!!"); }

                    retval_isIslandOverMap = new IslandOverMapCheckResults(true, new_Island_x, new_Island_y);
                }
            }

            //  Check 3: The position of an island must be divisible by 8 (Required by Anno 1800)
            //           For example 0,8,16,24,...
            switch(retval_isIslandOverMap.RequiresNewXYIslandCoordinates)
            {
                case true:
                    if (retval_isIslandOverMap.NewX % 8 != 0) 
                    {
                        retval_isIslandOverMap.NewX = (retval_isIslandOverMap.NewX / 8) * 8;
                    }
                    if (retval_isIslandOverMap.NewY % 8 != 0)
                    {
                        retval_isIslandOverMap.NewY = (retval_isIslandOverMap.NewY / 8) * 8;
                    }
                    break;
                case false:
                    if (pt.X % 8 != 0) 
                    { 
                        retval_isIslandOverMap.RequiresNewXYIslandCoordinates = true;
                        retval_isIslandOverMap.NewX = ((int) pt.X / 8) * 8;
                    }
                    if (pt.Y % 8 != 0)
                    {
                        retval_isIslandOverMap.RequiresNewXYIslandCoordinates = true;
                        retval_isIslandOverMap.NewY = ((int) pt.Y / 8) * 8;
                    }
                    //  Last check whether new values X and Y are set
                    if (retval_isIslandOverMap.RequiresNewXYIslandCoordinates)
                    {
                        if (retval_isIslandOverMap.NewX == IslandOverMapCheckResults.newXYdefaultValue) { retval_isIslandOverMap.NewX = (int) pt.X; }
                        if (retval_isIslandOverMap.NewY == IslandOverMapCheckResults.newXYdefaultValue) { retval_isIslandOverMap.NewY = (int) pt.Y; }
                    }
                    break;
            }

            return retval_isIslandOverMap;
        }


        /// <summary>
        /// Structured return value containing adjustment requirements for island positions
        /// </summary>
        internal struct IslandOverMapCheckResults
        {
            internal const int newXYdefaultValue = int.MinValue;
            internal bool IsIslandOverMap = false;
            internal bool HasActivedMapBorderSnapping = false;
            internal bool RequiresNewXYIslandCoordinates = false;
            internal int NewX = newXYdefaultValue;
            internal int NewY = newXYdefaultValue;

            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="isIslandOverMap">Is the checked island on the map</param>
            internal IslandOverMapCheckResults(bool isIslandOverMap)
            {
                this.IsIslandOverMap = isIslandOverMap;
            }

            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="isIslandOverMap">Is the checked island on the map</param>
            /// <param name="newX">Necessary adjustment resp. new Y position</param>
            /// <param name="newY">Necessary adjustment resp. new Y position</param>
            internal IslandOverMapCheckResults(bool isIslandOverMap, int newX, int newY)
            {
                this.IsIslandOverMap = isIslandOverMap;
                this.HasActivedMapBorderSnapping = true;
                this.RequiresNewXYIslandCoordinates = true;
                this.NewX = newX;
                this.NewY = newY;
            }
        }

        private void ListView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            switch(e.Action)
            {
                case DragAction.Continue:
                    ViewModel.CommandDragDropIsInProgress();
                    break;
                case DragAction.Drop:
                    ViewModel.CommandDragDropHasStopped();
                    break;
                case DragAction.Cancel:
                    ViewModel.CommandDragDropHasStopped();
                    break;
                default:
                    Log.Logger.Debug("Dear DEV, it seems there is a new c# DragDrop State you did not know before.");
                    break;
            }
        }

        private void MapCanvas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Log.Logger.Debug("MapCanvas_Selection Changed sender: {0}", sender);
            Log.Logger.Debug("MapCanvas_Selection Changed eventargs: {0}", e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Fires only if Canvas has a Background Color != null. Background Color "Transparent" is okay.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas == null)
                return;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            var element = hitTestResult.VisualHit;
            
            bool isLeftButtonPressed = e.LeftButton == MouseButtonState.Pressed;
            bool isRightButtonPressed = e.RightButton == MouseButtonState.Pressed;
            bool isMiddleButtonPressed = e.MiddleButton == MouseButtonState.Pressed;

            if (element != null)
            {
                //  WORK STAND: Liefert den TextBlock der als Parent das Grid aufweist (aus MapElementIsland)
                //  aber (!) im DataContext bekomme ich zugriff auf das Model (IslandOnMap) mit seiner InstanceID (!)
                Log.Logger.Debug("Selected Element: {0}", element);
                switch (element)
                {
                    case Canvas:
                        //  The Map Canvas itself (no Island/Element) is clicked
                        ViewModel.CommandSelectedIslandClear();
                        break;
                    default:
                        //  A Island/Element got clicked
                        if (isLeftButtonPressed)
                        {
                            //  Ignore, because handeled within "MapCanvas_PreviewMouseLeftButtonDown" for DragDrop / Click
                        }
                        if (isRightButtonPressed)
                        {
                            var control = (FrameworkElement) element as FrameworkElement;
                            IslandOnMap datacontext = (IslandOnMap) control.DataContext as IslandOnMap;
                            datacontext.Rotate();
                        }
                        if (isMiddleButtonPressed)
                        {
                            //  Dont know yet, what could be a good idea ?!?
                            //  Maybe another PNG ? Or remove Island/Element? Or replace with a random from Pool ?!?
                        }
                        break;
                }
            }
            else
            {
                Log.Logger.Debug("No Element selected");
            }

            // do something with element
        }

        private void MapCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Log.Logger.Debug("MapCanvas_PreviewMouseDown: {0}", e.ButtonState);
        }
    }
}