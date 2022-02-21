using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Client.File;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IConfig _config;
        public MainWindow(IConfig config)
        {
            InitializeComponent();
            _config = config;
            var uri = new Uri(_config.PathToTheme, UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void MenuItem_OnClick_SelectLightTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"../Themes/LightTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            _config.PathToTheme = @"../Themes/LightTheme.xaml";
        }

        private void MenuItem_OnClick_SelectDarkTheme(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"../Themes/DarkTheme.xaml", UriKind.Relative);
            ResourceDictionary resourceDictionary = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            _config.PathToTheme = @"../Themes/DarkTheme.xaml";
        }

        private void Button_OnClick_MinimizeWindowButton(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_OnClick_MaximizeWindowButton(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void TexBlock_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
