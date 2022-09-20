using Anno_World_Manager.viewmodel.baseclasses;
using Anno_World_Manager.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace Anno_World_Manager.viewmodel
{
    internal class WorldViewModel : ViewModelBase
    {
        ContinentalViewModel viewModelArctic;
        public ContinentalViewModel ViewModelArctic
        {
            get { return viewModelArctic; }
            set { SetProperty<ContinentalViewModel>(ref viewModelArctic, value); }
        }

        ContinentalViewModel viewModelNewWorld;
        public ContinentalViewModel ViewModelNewWorld
        {
            get { return viewModelNewWorld; }
            set { SetProperty<ContinentalViewModel>(ref viewModelNewWorld, value); }
        }

        ContinentalViewModel viewModelOldWorld;
        public ContinentalViewModel ViewModelOldWorld
        {
            get { return viewModelOldWorld; }
            set { SetProperty<ContinentalViewModel>(ref viewModelOldWorld, value); }
        }

        ContinentalViewModel viewModelCapTrelawney;
        public ContinentalViewModel ViewModelCapTrelawney
        {
            get { return viewModelCapTrelawney; }
            set { SetProperty<ContinentalViewModel>(ref viewModelCapTrelawney, value); }
        }

        ContinentalViewModel viewModelEnbesa;
        public ContinentalViewModel ViewModelEnbesa
        {
            get { return viewModelEnbesa; }
            set { SetProperty<ContinentalViewModel>(ref viewModelEnbesa, value); }
        }

        internal WorldViewModel()
        {
            viewModelArctic = new ContinentalViewModel(this, WorldRegion.Arctic);
            viewModelNewWorld = new ContinentalViewModel(this, WorldRegion.NewWorld);
            viewModelOldWorld = new ContinentalViewModel(this, WorldRegion.OldWorld);
            viewModelCapTrelawney = new ContinentalViewModel(this, WorldRegion.CapTrelawney);
            viewModelEnbesa = new ContinentalViewModel(this, WorldRegion.Enbesa);
        }
    }

    internal class ContinentalViewModel : ViewModelBase
    {
        private WorldViewModel _parentWorldViewModel;

        private String name = String.Empty;
        public String Name
        {
            get { return name; }
            set { SetProperty<String>(ref name, value); }
        }

        private bool isMissionDLC = true;
        public bool IsMissingDLC
        {
            get { return isMissionDLC; }
            set { SetProperty<bool>(ref isMissionDLC, value); }
        }

        private Color colorDarken;
        public Color ColorDarken
        {
            get { return colorDarken; }
            set { SetProperty<Color>(ref colorDarken, value); }
        }

        private Color colorLight;
        public Color ColorLight
        {
            get { return colorLight; }
            set { SetProperty<Color>(ref colorLight, value); }
        }

        private int rotationAngle = 0;
        public int RotationAngle
        {
            get { return rotationAngle; }
            set { SetProperty<int>(ref rotationAngle, value); }
        }

        private String dlcMissingMessage = String.Empty;
        public String DlcMissingMessage
        {
            get { return dlcMissingMessage; }
            set { SetProperty<String>(ref dlcMissingMessage, value); }
        }


        internal ContinentalViewModel(WorldViewModel p_parent, WorldRegion p_region)
        {
            //  store parent ViewModel instance
            this._parentWorldViewModel = p_parent;

            switch(p_region)
            {
                case WorldRegion.Arctic:
                    SetDefaultValuesRegionArctic();
                    break;
                case WorldRegion.OldWorld:
                    SetDefaultValuesRegionOldWorld();
                    break;
                case WorldRegion.NewWorld:
                    SetDefaultValuesRegionNewWorld();
                    break;
                case WorldRegion.CapTrelawney:
                    SetDefaultValuesRegionCapTrelawney();
                    break;
                case WorldRegion.Enbesa:
                    SetDefaultValuesRegionEnbesa();
                    break;
                default: throw new ArgumentException("Region not implemented: {0}", p_region.ToString());
            }
        }

        private void SetDefaultValuesRegionArctic()
        {
            this.Name = "Artic";
            this.IsMissingDLC = ! Runtime.Anno1800Dlcs.HasDLCThePassage;
            this.ColorLight = Color.FromArgb(255, 255, 255, 255);
            this.ColorDarken = Color.FromArgb(255, 222, 222, 222);
            this.DlcMissingMessage = "Sorry, you dont own the required DLC 'The Passage'";
            this.RotationAngle = 0;
        }

        private void SetDefaultValuesRegionNewWorld()
        {
            this.Name = "New World";
            this.IsMissingDLC = false;
            this.ColorLight = Color.FromArgb(255, 4, 146, 19);       
            this.ColorDarken = Color.FromArgb(255, 3, 105, 13);
            this.DlcMissingMessage = String.Empty;  //  not required
            //this.RotationAngle = -90;
        }

        private void SetDefaultValuesRegionOldWorld()
        {
            this.Name = "Old World";
            this.IsMissingDLC = false;
            this.ColorLight = Color.FromArgb(255, 14, 209, 76);
            this.ColorDarken = Color.FromArgb(255, 11, 164, 60);
            this.DlcMissingMessage = String.Empty;  // not required
            //this.RotationAngle = -90;
        }

        private void SetDefaultValuesRegionCapTrelawney()
        {
            this.Name = "Cap Trelawney";
            this.IsMissingDLC = !Runtime.Anno1800Dlcs.HasDLCSunkenTreasures;
            this.ColorLight = Color.FromArgb(255, 14, 209, 76);
            this.ColorDarken = Color.FromArgb(255, 11, 164, 60);
            this.DlcMissingMessage = "Sorry, you dont own the required DLC 'Sunken Treasures'";
            //this.RotationAngle = -90;
        }

        private void SetDefaultValuesRegionEnbesa()
        {
            this.Name = "Enbesa";
            this.IsMissingDLC = !Runtime.Anno1800Dlcs.HasDLCTheLandOfLions;
            this.ColorLight = Color.FromArgb(255, 248, 146, 18);
            this.ColorDarken = Color.FromArgb(255, 209, 123, 14);
            this.DlcMissingMessage = "Sorry, you dont own the required DLC 'The Lanf of Lions'";
            //this.RotationAngle = -90;
        }
    }
}
