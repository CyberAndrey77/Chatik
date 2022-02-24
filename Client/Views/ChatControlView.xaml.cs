using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Client.ViewModels;

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

        private void OnScroll(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
                //ScrollViewer.LineDown();
            }
            //ScrollViewer.LineDown();
        }

        private void ListBox_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = (ChatControlViewModel)DataContext;
            vm.MessageEvent += OnScroll;
        }
    }
}
