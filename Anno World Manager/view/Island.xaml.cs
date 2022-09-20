using Anno_World_Manager.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Anno_World_Manager.view
{
    /// <summary>
    /// Interaktionslogik für Island.xaml
    /// </summary>
    public partial class Island : UserControl
    {
        public Island()
        {
            InitializeComponent();
        }

        /// <summary>
        /// User initated Drag Drop of Map placed Island
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  only do Drag Drop if this Island is placed on Map
            UIElement parent = (UIElement) Parent;  //  Canvas      Canvas.Name = "MapCanvasContent"
            if (parent != null && parent is Canvas)
            {
                Island draggedItem = this;
                IslandViewModel draggedDataContext = this.DataContext as IslandViewModel;
                //  Create new DataObject with a Clone of the used viewmodel
                DataObject dataobject = new DataObject(typeof(IslandViewModel), draggedDataContext.Clone());

                //  Remove this Island from Parent Canvas (parent.Children)
                Canvas _parent = (Canvas)parent;
                _parent.Children.Remove(this);

                //  Start DragDrop with sender and viewmodel to use
                DragDrop.DoDragDrop(draggedItem, dataobject, DragDropEffects.Copy);     //  Check: Maybe Clone (before) or Copy (here) is redundant
            }
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IslandViewModel vm = this.DataContext as IslandViewModel;
            if (vm != null)
            {
                vm.RotateNextClockwise();
            }
        }
    }
}
