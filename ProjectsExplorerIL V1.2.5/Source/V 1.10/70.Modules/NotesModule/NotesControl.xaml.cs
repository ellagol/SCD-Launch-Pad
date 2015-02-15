using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotesModule
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NotesControl : UserControl
    {
        public NotesControl()
        {
            InitializeComponent();
        }

        private void Expander_IsHitTestVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
