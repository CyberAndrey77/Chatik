using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client.Views
{
    /// <summary>
    /// Логика взаимодействия для ChatControlView.xaml
    /// </summary>
    public partial class ChatControlView : UserControl
    {
        public ChatControlView()
        {
            InitializeComponent();
        }

        private void ListBox_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((ListBox)sender).Items.Count > 0)
            {
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
            }
        }

        private void ListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
            }
        }

        private void ListBox_OnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
        }

        private void ListBox_OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
        }

        private void ListBox_OnLayoutUpdated(object sender, EventArgs e)
        {
            
        }
    }
}
