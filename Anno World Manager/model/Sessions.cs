using Anno_World_Manager.viewmodel.baseclasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model
{
    public class Sessions : ViewModelBase
    {

        private static String repositoryFilename = "session.json";

        /// <summary>
        /// Directory of all sessions held in the editor
        /// </summary>
        public ObservableCollection<Session> Repository
        {
            get { return _sessionRepository; }
            set { SetProperty<ObservableCollection<Session>>(ref _sessionRepository, value); }
        }
        private ObservableCollection<Session> _sessionRepository = new ObservableCollection<Session>();

        public Sessions()
        {

        }

        public void Initialize()
        {
            Log.Logger.Trace("called");
            this.Read();
        }

        /// <summary>
        /// Read Session Repository from local File
        /// </summary>
        public void Read()
        {
            try
            {
                var filename = AppContext.BaseDirectory + repositoryFilename;
                using (System.IO.StreamReader file = File.OpenText(filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Repository = (ObservableCollection<Session>)serializer.Deserialize(file, typeof(ObservableCollection<Session>));
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }

            //  Fall-Back - If File is empty or Read failed
            if (Repository == null) { Repository = new ObservableCollection<Session>(); }
        }

        /// <summary>
        /// Write Session Repository to local File
        /// </summary>
        public void Write()
        {

        }
    }
}
