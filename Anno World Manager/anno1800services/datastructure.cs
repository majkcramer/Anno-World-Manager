using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Anno_World_Manager.anno1800services.gamedata.DataArchives;

namespace Anno_World_Manager.anno1800services
{
    internal class datastructure
    {
        private bool _isLoading = true;
        public bool IsLoading 
        { 
            get { return _isLoading; }
            private set { SetProperty<bool>(ref _isLoading, value); }
        }

        private IDataArchive _dataArchive = gamedata.DataArchives.DataArchive.Default;
        public IDataArchive DataArchive
        {
            get => _dataArchive;
            private set
            {
                if (_dataArchive is System.IDisposable disposable) { disposable.Dispose(); }
                SetProperty(ref _dataArchive, value);
                OnPropertyChanged("DataArchive");   //  TODO: Seems redundant, because is always part of SetProperty fn
            }
        }


        public void LoadDataPath(string path)
        {
            IsLoading = true;

            Task.Run(async () => {
                var archive = await gamedata.DataArchives.DataArchive.OpenAsync(path);
                IsLoading = false;  //  TODO: Fires to early. Maybe because the following Dispatcher Invoke takes too long time ?
                System.Windows.Application.Current.Dispatcher.Invoke(() => DataArchive = archive);
            });
        }



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
