using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Models;
using Client.Services;
using Client.Services.EventArgs;
using Client.NetWork;
using Common;
using Common.Enums;
using Prism.Interactivity;
using Prism.Services.Dialogs;
using Client.NetWork.EventArgs;

namespace Client.ViewModels
{
    public class ChatControlViewModel : BindableBase
    {
        private string _sendText;
        private readonly IMessageService _messageService;
        private IConnectionService _connectionService;
        private IDialogService _dialogService;
        private MessageViewModel _messageViewModel;
        private ObservableCollection<MessageViewModel> _messageViewModels;
        private ObservableCollection<User> _users;
        private bool _isButtonEnable;
        private readonly ConcurrentQueue<MessageViewModel> _sendQueue;
        private string _chatName;
        private ChatViewModel _selectedChat;

        public string SendText
        {
            get => _sendText;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(value);
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _sendText, value);
            }
        }

        public string ChatName
        {
            get => _chatName;
            set => SetProperty(ref _chatName, value);
        }

        public ChatViewModel SelectedChat
        {
            get => _selectedChat;
            set
            {
                if (_selectedChat == value)
                {
                    return;
                }
                SetProperty(ref _selectedChat, value);
                SelectedChat.CountUnreadMessages = 0;
                MessageViewModels.Clear();
                ChatName = value.Name;
            } 
        }
        
        public ObservableCollection<MessageViewModel> MessageViewModels
        {
            get => _messageViewModels;
            set => SetProperty(ref _messageViewModels, value);
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                SetProperty(ref _users, value);
            }
        }

        public ObservableCollection<ChatViewModel> ChatViewModels { get; set; }

        public bool IsButtonEnable
        {
            get => _isButtonEnable;
            set => SetProperty(ref _isButtonEnable, value);
        }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand CreateDialog { get;}
        
        public ChatControlViewModel(IMessageService messageService, IConnectionService connectionService, IDialogService dialogService)
        {
            _messageViewModel = new MessageViewModel();
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;


            _connectionService.UserListEvent += OnGetUsers;
            _connectionService.UserEvent += OnUserConnectOrDisconnect;
            _connectionService.MessageStatusChangeEvent += OnMessageStatusChange;
            _messageService.MessageEvent += OnMessageReceived;
            _messageService.GetPrivateMessageEvent += OnPrivateMessage;
            IsButtonEnable = false;

            MessageViewModels = new ObservableCollection<MessageViewModel>();
            Users = new ObservableCollection<User>();
            ChatViewModels = new ObservableCollection<ChatViewModel>();

            _sendQueue = new ConcurrentQueue<MessageViewModel>();
            SendCommand = new DelegateCommand(ExecuteCommand);
            CreateDialog = new DelegateCommand(CreateDialogWithUser);
        }

        private void OnPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                //if (ChatViewModels.Where(x => x.SenderName == e.SenderName).Count() == 0)
                if (!ChatViewModels.Any(x => x.Name == e.SenderName))
                {
                    var users = new ObservableCollection<User>()
                    {
                        new User(e.SenderName),
                        new User(e.ReceiverName)
                    };
                    ChatViewModels.Add(new ChatViewModel(users, e.SenderName, true));
                }

                var chat = ChatViewModels.First(x => x.Name == e.SenderName);

                if (SelectedChat.Name != chat.Name)
                {
                    chat.CountUnreadMessages++;
                    return;
                }

                var messageViewModel = new MessageViewModel()
                {
                    Name = e.SenderName,
                    Text = e.Message,
                    Time = e.Time,
                    MessageType = MessageType.Ingoing,
                    MessageStatus = MessageStatus.Delivered
                };

                MessageViewModels.Add(messageViewModel);
            });
        }

        private void CreateDialogWithUser()
        {
            //_dialogService
            var dialogParameters = new DialogParameters
            {
                { "users", Users }
            };
            _dialogService.ShowDialog("CreateDialog", dialogParameters, r =>
            {
                if (r.Result != ButtonResult.OK)
                {
                    return;
                }

                r.Parameters.TryGetValue("user", out User selectedUser);
                var users = new ObservableCollection<User>()
                {
                    new User(_messageViewModel.Name),
                    selectedUser
                };

                var newDialog = new ChatViewModel(users, selectedUser.Name, true);
                ChatViewModels.Add(newDialog);
                SelectedChat = newDialog;
            });
        }

        private void OnUserConnectOrDisconnect(object sender, GetUserEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (e.IsConnect)
                {
                    Users.Add(new User(e.Login));

                    ChatViewModels[0].Users.Add(new User(e.Login));
                }
                else
                {

                    ChatViewModels[0].Users.Remove(Users.First(x => x.Name == e.Login));
                    Users.Remove(Users.First(x => x.Name == e.Login));
                }
                
                Users = new ObservableCollection<User>(Users.OrderBy(x => x.Name));
            });
        }

        private void OnGetUsers(object sender, GetUsersEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var user in e.Users)
                {
                    Users.Add(new User(user));
                }
                
                Users = new ObservableCollection<User>(Users.OrderBy(x => x.Name));

                //TODO после создания базы данных убрать отсюда этот кусок.
                ChatViewModels.Add(new ChatViewModel(Users, "Главный чат", false));
                SelectedChat = ChatViewModels[0];
            });
        }

        private void ExecuteCommand()
        {
            //Message message = new Message();

            //message.SenderName = _connectionService.SenderName;
            //message.Text = SendText;
            //message.Time = DateTime.Now;

            var message = new MessageViewModel()
            {
                Text = SendText,
                Name = _connectionService.Name,
                Time = DateTime.Now,
                MessageStatus = MessageStatus.Sending,
                MessageType = MessageType.Outgoing
            };

            MessageViewModels.Add(message);

            _sendQueue.Enqueue(message);

            //_messageService.SendMessage(message.SenderName, message.Text);
            if (SelectedChat.IsDialog)
            {
                _messageService.SendPrivateMessage(message.Name, message.Text, ChatName);
            }
            

            SendText = string.Empty;
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (SelectedChat.Name != ChatViewModels[0].Name)
                {
                    ChatViewModels[0].CountUnreadMessages++;
                    return;
                }
                MessageViewModels.Add(new MessageViewModel()
                {
                    Text = e.Message,
                    Name = e.Name,
                    Time = e.Time,
                    MessageStatus = MessageStatus.Delivered,
                    MessageType = MessageType.Ingoing
                });
            });
        }
        private void OnMessageStatusChange(object sender, MessageRequestEvent e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                MessageViewModel message;
                switch (e.Status)
                {
                    case MessageStatus.Delivered:
                        if (_sendQueue.TryDequeue(out message))
                        {
                            message = MessageViewModels.First(x => x.Id == message.Id);
                            message.MessageStatus = MessageStatus.Delivered;
                        }
                        break;
                    case MessageStatus.Sending:
                        if (_sendQueue.TryPeek(out message))
                        {
                            message = MessageViewModels.First(x => x.Id == message.Id);
                            message.MessageStatus = MessageStatus.Sending;
                        }
                        break;
                    case MessageStatus.NotDelivered:
                        if (_sendQueue.TryPeek(out message))
                        {
                            message = MessageViewModels.First(x => x.Id == message.Id);
                            message.MessageStatus = MessageStatus.NotDelivered;
                        }
                        break;
                }
                RaisePropertyChanged(nameof(message));
            });
        }
    }
}
