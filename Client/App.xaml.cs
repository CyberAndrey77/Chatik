using Client.Views;
using Prism.Ioc;
using System.Windows;
using Prism.Mvvm;
using Prism.Unity;
using Client.ViewModels;
using System;
using Client.Services;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<CreateDialogView, CreateDialogViewModel>("CreateDialog");
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.RegisterSingleton<IConnectionService, ConnectionService>();
            containerRegistry.Register<ChatControlViewModel>();
            containerRegistry.Register<MainWindowViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            BindViewModelToView<ChatControlViewModel, ChatControlView>();
            BindViewModelToView<MainWindowViewModel, MainWindow>();
            BindViewModelToView<LoginView, LoginViewModel>();
        }


        private void BindViewModelToView<TViewModel, TView>()
        {
            ViewModelLocationProvider.Register(typeof(TView).ToString(), () => Container.Resolve<TViewModel>());
        }
    }
}