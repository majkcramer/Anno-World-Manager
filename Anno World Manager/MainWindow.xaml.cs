using Anno_World_Manager.helper;
using Anno_World_Manager.ImExPort2;
using Anno_World_Manager.model;
using Anno_World_Manager.view;
using Anno_World_Manager.viewmodel;
using Anno_World_Manager.viewmodel.baseclasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anno_World_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// Yes, i know. Navigation could be implemented MVVM way with RelayedCommand, Prism, ICommands and whatever. But this current Solution was much(!) faster to implement.
    /// </remarks>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //  Grid Navigation Menue Layer Element Names as const - for easy handling
        private const string menue_header = "LayerHeader";
        private const string menue_status = "LayerStatus";
        private const string menue_islands = "LayerIslands";
        private const string menue_maps = "LayerMaps";
        private const string menue_world = "LayerWorld";
        private const string menue_settings = "LayerSettings";
        private const string menue_exit = "LayerExit";

        
        

        /// <summary>
        /// 
        /// </summary>
        public bool IsApplicationReady  //  TODO: Not in use yet
        {
            get { return _isApplicationReady; }
            set { SetProperty<bool>(ref _isApplicationReady, value); }
        }
        private bool _isApplicationReady = false;   //  TODO: Not in use yet

        /// <summary>
        /// Property to Show/Hide the 'explanation' Text within Navigation Menue
        /// </summary>
        public bool HideNavigation
        {
            get { return _hideNavigation; }
            set { SetProperty<bool>(ref _hideNavigation, value); }
        }
        private bool _hideNavigation = false;

        /// <summary>
        /// Ctor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            try
            {

                //  Store Content Area in Navigation
                Navigation.MainWindowContent = (ContentControl)this.maincontent;

                //  Use this Window Code Behind as ViewModel. Yes, i know that's not MVVM. Read Class Remarks above :-D
                this.DataContext = this;

                //  Initialize Logger
                Log.Initialize();
                Log.Logger.Info("Anno World Manager - started");

                //  Show Wait Window - Until Application is ready
                PreparingApplication waitwindow = new PreparingApplication();
                waitwindow.DataContext = this;
                maincontent.Content = waitwindow;

                //  Initial Runtime (may take some time)
                DelayFactory.DelayAction(10, new Action(() => { Runtime.Initialize(); CheckInitialisation(); }));

                LoadNavigationIcons();

            }
            catch(Exception ex)
            {

            }
        }

        private void LoadNavigationIcons()
        {
            this.icon_navmenue_shownavigation.UriSource = new Uri("pack://application:,,,/Images/ionic.io/menu-outline.svg"); ;
            this.icon_navmenue_shownavigation.Width = 24;
            this.icon_navmenue_shownavigation.Height = 24;

            this.icon_navmenue_status.UriSource = new Uri("pack://application:,,,/Images/ionic.io/pulse-outline.svg");
            this.icon_navmenue_status.Width = 24;
            this.icon_navmenue_status.Height = 24;

            //  TODO: More DIFFERENT Icons

            this.icon_navmenue_islands.UriSource = new Uri("pack://application:,,,/Images/ionic.io/earth-outline.svg");
            this.icon_navmenue_islands.Width = 24;
            this.icon_navmenue_islands.Height = 24;

            this.icon_navmenue_maps.UriSource = new Uri("pack://application:,,,/Images/ionic.io/earth-outline.svg");
            this.icon_navmenue_maps.Width = 24;
            this.icon_navmenue_maps.Height = 24;

            this.icon_navmenue_world.UriSource = new Uri("pack://application:,,,/Images/ionic.io/earth-outline.svg");
            this.icon_navmenue_world.Width = 24;
            this.icon_navmenue_world.Height = 24;

            this.icon_navmenue_settings.UriSource = new Uri("pack://application:,,,/Images/ionic.io/settings-outline.svg");
            this.icon_navmenue_settings.Width = 24;
            this.icon_navmenue_settings.Height = 24;

            this.icon_navmenue_exit.UriSource = new Uri("pack://application:,,,/Images/ionic.io/exit-outline.svg");
            this.icon_navmenue_exit.Width = 24;
            this.icon_navmenue_exit.Height = 24;

        }

        private void MenueLayer_MouseEnter(object sender, MouseEventArgs e)
        {
            Log.Logger.Debug("Enter");
        }

        private void MenueLayer_MouseLeave(object sender, MouseEventArgs e)
        {
            Log.Logger.Debug("Leave");
        }

        private void CheckInitialisation()
        {
            //  TODO: Prio 2 - Check is Application Ready (Usability for Users)
            //  Check: Can i receive a Island PNG ?!?
            //  YES: Set IsApplicationReady = true;
            //  Dann freischalten von Funktionen


            

            bool _isinitialized = false;
            System.Windows.Media.Imaging.BitmapImage? dummy_png = new();

            //  Hypothesis: If the application can successfully obtain an island PNG from the Anno 1800 data stream, initialization is successfully completed.

            try
            {
                using Stream stream = Runtime.Anno1800GameData.DataArchive.OpenRead(@"data\dlc01\sessions\islands\pool\moderate_c_01\_gamedata\moderate_c_01\mapimage.png");
                if (stream != null)
                {
                    dummy_png.BeginInit();
                    dummy_png.StreamSource = stream;
                    dummy_png.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    dummy_png.EndInit();
                    dummy_png.Freeze();
                    if (dummy_png.Height > 0)
                    {
                        _isinitialized = true;

                        //  
                        Runtime.CheckAfterInitializationCompletet();
                    }
                    else
                    {
                        Log.Logger.Debug("Observe dear Dev: A PNG with a height of 0px tumbled out of the stream. Is tolerable if it happens only once in the opened stream.");
                    }
                }
            }
            catch (Exception ex)
            {

            }

            dummy_png = null;

            switch(_isinitialized)
            {
                case true:
                    //  Set VM Property
                    IsApplicationReady = true;
                    //  If Successfull show Status Page
                    DisplayPageStatus();
                    break;
                case false:
                    //  Start another Check in 0,5 seconds
                    DelayFactory.DelayAction(500, new Action(() => { CheckInitialisation(); }));
                    break;
            }
        }

        private void MenueLayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid? _sender = sender as Grid;
            if (_sender != null)
            {
                switch (_sender.Name)
                {
                    case menue_header:
                        //  Invert HideNavigation bool
                        HideNavigation = !HideNavigation;
                        break;
                    case menue_status:
                        DisplayPageStatus();
                        break;
                    case menue_islands:
                        //  TODO: Create
                        break;
                    case menue_maps:
                        DisplayPageMaps();
                        break;
                    case menue_world:
                        WorldMap map = new WorldMap();
                        map.DataContext = new WorldViewModel();
                        maincontent.Content = map;
                        break;
                    case menue_settings:
                        maincontent.Content = new view.Settings();  // TODO: Alles
                        break;
                    case menue_exit:
                        //  TODO: Inkl. Sicherheitsabfrage implementieren
                        break;
                    default:
                        Log.Logger.Warn("Unrecognized Menue Entry: {0}", _sender.Name);
                        break;
                }
            }
            
            //  TODO: Prio 9 - Nice WPF Animation for Show/Hide Navigation in WPF Layer?
            //  TODO: Prio 9 - Nice WPF Animation for View Change in WPF Layer?
        }

        private void DisplayPageStatus()
        {
            maincontent.Content = new view.Status();    // TODO: Create ViewModel
        }

        private void DisplayPageMaps()
        {
            //  Create view (here: Maps_Overview as UserControl)
            Maps_Overview view = new Maps_Overview();
            //  Create viewmodel and assign it to the DataContext of the view
            view.DataContext = new MapsOverviewModel();          //  TODO: Better Name to match the view
            //  Assign the view with viewmodel to display within the maincontent named area
            maincontent.Content = view;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, string[]? dependingPropertyNames = null, [CallerMemberName] string propertyName = "")
        {
            if (property is null && value is null)
                return;

            if (!(property?.Equals(value) ?? false))
            {
                property = value;
                OnPropertyChanged(propertyName);
                if (dependingPropertyNames is not null)
                    foreach (var name in dependingPropertyNames)
                        OnPropertyChanged(name);
            }
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  TODO: REMOVE this dirty Hack
            var viewModel = new MapEditViewModel();
            MapEdit view = new MapEdit(viewModel);
            view.DataContext = viewModel;
            maincontent.Content = view;
        }
    }
}
