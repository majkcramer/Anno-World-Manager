using Anno_World_Manager.viewmodel.baseclasses;
using Anno_World_Manager.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno_World_Manager.model;
using System.Collections.ObjectModel;
using Anno_World_Manager.ImExPort2;
using Anno_World_Manager.view;

namespace Anno_World_Manager.viewmodel
{
    internal class MapsOverviewModel : ViewModelBase
    {
        

        #region View: Create New World

        private MapType _mapType = MapType.Session;
        public MapType MapType
        {
            get { return _mapType; }
            set { SetProperty<MapType>(ref _mapType, value); }
        }

        private const String mapRegionPropertyString = "MapRegion";
        private WorldRegion _mapRegion = WorldRegion.OldWorld;
        public WorldRegion MapRegion
        {
            get { return _mapRegion; }
            set { SetProperty<WorldRegion>(ref _mapRegion, value); }
        }

        private bool _hasMapTemplate;
        public bool HasMapTemplate
        {
            get { return _hasMapTemplate; }
            set { SetProperty<bool>(ref _hasMapTemplate, value); }
        }

        private ObservableCollection<MapTemplate> _listOfMapTemplates = new ObservableCollection<MapTemplate>();
        public ObservableCollection<MapTemplate> ListOfMapTemplates
        {
            get { return _listOfMapTemplates; }
            private set { SetProperty<ObservableCollection<MapTemplate>>(ref _listOfMapTemplates, value); }
        }

        private MapTemplate _selectedMapTemplate;
        public MapTemplate SelectedMapTemplate
        {
            get { return _selectedMapTemplate; }
            set { SetProperty<MapTemplate>(ref _selectedMapTemplate, value); }
        }

        private UserExperience _userExperience = UserExperience.Rookie;
        public UserExperience UserExperience
        {
            get { return _userExperience; }
            set { SetProperty<UserExperience>(ref _userExperience, value); }
        }

        #endregion

        internal MapsOverviewModel()
        {
            BuildICommands();

            //  Subscribe to the PropertyChanged event to be able to make resulting changes to data.
            this.PropertyChanged += MapViewModel_PropertyChanged;

            //  Fill the list for the first time using the preassigned region.
            BuildRebuildListOfMapTemplates();
        }

        internal void BuildICommands()
        {
            //  ICommand: CreateSession
            CreateSession = new CustomCommand();
            CreateSession.CanExecuteFunc = obj => true;
            CreateSession.ExecuteFunc = CreateSessionFunc;
        }


        /// <summary>
        /// Handle PropertyChanged Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Log.Logger.Debug("Property has changed : {0}", e.PropertyName);
            switch (e.PropertyName)
            {
                case mapRegionPropertyString:
                    //  
                    BuildRebuildListOfMapTemplates();
                    break;
                
            }
        }

        /// <summary>
        /// Build or Rebuild List of MapTemplates (depends on Region)
        /// </summary>
        private void BuildRebuildListOfMapTemplates()
        {
            //  Clear List
            ListOfMapTemplates.Clear();
            //  Fill the list based on the selected / preassigned region.
            int i = 0;
            while(i < Runtime.MapTemplatesKnows.KnownMapTemplates.Count)
            {
                if (Runtime.MapTemplatesKnows.KnownMapTemplates[i].Region == MapRegion)
                {
                    ListOfMapTemplates.Add(Runtime.MapTemplatesKnows.KnownMapTemplates[i]);
                }
                i++;
            }

            if (ListOfMapTemplates.Count > 0)
            {
                SelectedMapTemplate = ListOfMapTemplates[0];
            }
        }


        public CustomCommand CreateSession { get; set; }
        /// <summary>
        /// ActionFunction
        /// </summary>
        /// <param name="parameter">optionaler Parameter</param>
        public void CreateSessionFunc(object parameter)
        {
            //CreateSessionDo((int)parameter);
            CreateSessionDo();
        }

        /// <summary>
        /// Funktion, die bei Klick auf den Button ausgeführt werden soll.
        /// </summary>
        public void CreateSessionDo()
        {
            if (SelectedMapTemplate != null)
            {
                if (SelectedMapTemplate.IsVanilla)
                {
                    FluentResults.Result<Session> result = Service.GetSessionFromVanillaMapTemplate(SelectedMapTemplate.MapPath);
                    if (result.IsSuccess)
                    {
                        Session session = result.Value;
                        session.Name = SelectedMapTemplate.Name;
                        session.Region = SelectedMapTemplate.Region;
                        session.Initialize();

                        MapEditViewModel viewmodel = new MapEditViewModel(session.Region);
                        viewmodel.Session = session;

                        MapEdit view = new MapEdit(viewmodel);
                        view.DataContext = viewmodel;

                        Navigation.MainWindowContent.Content = view;
                    }
                }
                else
                {
                    Log.Logger.Error("Dear Dev, you have missed to implement loading a7tinfo from Filesystem! ");
                    //  TODO: Implement loading from File :-D
                }
            }
        }
    }
}
