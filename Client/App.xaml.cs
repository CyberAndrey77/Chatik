using Client.Views;
using Prism.Ioc;
using System.Windows;
using Prism.Mvvm;
using Prism.Unity;
using Client.ViewModels;
using System;
using Client.NetWork;
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
            containerRegistry.RegisterDialog<CreateChatView, CreateChatViewModel>("CreateChat");
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.RegisterSingleton<IConnectionService, ConnectionService>();
            containerRegistry.RegisterSingleton<IChatService, ChatService>();
            containerRegistry.RegisterSingleton<ITransport, WsClient>();
            containerRegistry.RegisterSingleton<ILogService, LogService>();
            containerRegistry.Register<ChatControlViewModel>();
            containerRegistry.Register<LogControlViewModel>();
            containerRegistry.Register<MainWindowViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            BindViewModelToView<LogControlViewModel, LogControlView>();
            BindViewModelToView<ChatControlViewModel, ChatControlView>();
            BindViewModelToView<MainWindowViewModel, MainWindow>();
            BindViewModelToView<LoginViewModel, LoginView>();
        }


        private void BindViewModelToView<TViewModel, TView>()
        {
            ViewModelLocationProvider.Register(typeof(TView).ToString(), () => Container.Resolve<TViewModel>());
        }
    }
}