using System;
using System.ComponentModel;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClick_SelectLightTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"../Themes/LightTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void MenuItem_OnClick_SelectDarkTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"../Themes/DarkTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
