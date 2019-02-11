using MakeTxGui.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MakeTxGui.Views
{
    /// <summary>
    /// Interaction logic for MakeTxView.xaml
    /// </summary>
    public partial class MakeTxView : UserControl
    {
        MakeTxViewModel _vm;
        public MakeTxView()
        {
            InitializeComponent();
            _vm = main.DataContext as MakeTxViewModel;
        }

        private void AllFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object obj in e.RemovedItems)
            {
                _vm.AllFilesSelected.Remove((string)obj);
            }
            foreach (object obj in e.AddedItems)
            {
                _vm.AllFilesSelected.Add((string)obj);
            }
        }

        private void SelectedFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object obj in e.RemovedItems)
            {
                _vm.SelectedFilesSelected.Remove((string)obj);
            }
            foreach (object obj in e.AddedItems)
            {
                _vm.SelectedFilesSelected.Add((string)obj);
            }
        }
    }
}
