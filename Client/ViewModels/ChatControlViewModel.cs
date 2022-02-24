using Client.Models;
using Client.NetWork;
using Client.Services;
using Client.Services.EventArgs;
using Common.Enums;
using Common.EventArgs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Client.ViewModels
{
    public class ChatControlViewModel : BindableBase
    {
        private string _sendText;
        private readonly IMessageService _messageService;
        private readonly IConnectionService _connectionService;
        private readonly IDialogService _dialogService;
        private readonly IChatService _chatService;
        private ObservableCollection<MessageViewModel> _messageViewModels;
        private ObservableCollection<User> _onlineUsers;
        private ObservableCollection<User> _offlineUsers;
        private bool _isButtonEnable;
        private string _chatName;
        private ChatViewModel _selectedChat;
        private string _name;

        public EventHandler MessageEvent;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

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
                if (_selectedChat == value || value == null)
                {
                    return;
                }
                SetProperty(ref _selectedChat, value);
                SelectedChat.CountUnreadMessages = 0;
                MessageViewModels.Clear();
                if (_selectedChat.Chat.Id != 0)
                {
                    _messageService.GetMessages(SelectedChat.Chat.Id);
                }
                ChatName = value.Name;
            }
        }

        public ObservableCollection<MessageViewModel> MessageViewModels
        {
            get => _messageViewModels;
            set => SetProperty(ref _messageViewModels, value);
        }

        public List<User> Users
        {
            get;
            set;
        }

        public ObservableCollection<User> OnlineUsers
        {
            get => _onlineUsers;
            set => SetProperty(ref _onlineUsers, value);
        }
        public ObservableCollection<User> OfflineUsers
        {
            get => _offlineUsers;
            set => SetProperty(ref _offlineUsers, value);
        }

        public ObservableCollection<ChatViewModel> ChatViewModels { get; set; }

        public bool IsButtonEnable
        {
            get => _isButtonEnable;
            set => SetProperty(ref _isButtonEnable, value);
        }

        public DelegateCommand SendCommand { get; }

        public DelegateCommand CreateDialog { get; }

        public DelegateCommand CreateChatCommand { get; }

        public string CreateDialogName(string originalName)
        {
            var name = originalName.Split('|');
            return name[0] == _connectionService.Name ? name[1] : name[0];
        }

        public ChatControlViewModel(IMessageService messageService, IConnectionService connectionService, IChatService chatService, IDialogService dialogService)
        {
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;
            _chatService = chatService;

            _connectionService.GetOnlineUsers += OnGetOnlineUsers;
            _connectionService.UserEvent += OnUserConnectOrDisconnect;
            _connectionService.ConnectStatusChangeEvent += OnConnection;
            _connectionService.AllUsersEvent += OnAllUsers;
            _messageService.MessageStatusChangeEvent += OnMessageStatusChange;
            _chatService.GetUserChats += GetChats;
            _messageService.MessageEvent += OnMessageReceived;
            _messageService.ChatMessageEvent += OnChatMessage;
            _messageService.GetMessagesEvent += OnGetMessages;

            _chatService.CreatedChat += OnCreatedChat;
            IsButtonEnable = false;

            MessageViewModels = new ObservableCollection<MessageViewModel>();
            Users = new List<User>();
            OnlineUsers = new ObservableCollection<User>();
            OfflineUsers = new ObservableCollection<User>();
            ChatViewModels = new ObservableCollection<ChatViewModel>();

            SendCommand = new DelegateCommand(ExecuteCommand);
            CreateDialog = new DelegateCommand(CreateDialogWithUser);
            CreateChatCommand = new DelegateCommand(CreateChat);
        }

        private void OnAllUsers(object sender, GetUsersEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var user in e.Users)
                {
                    Users.Add(new User(user.Key, user.Value));
                }

                if (OnlineUsers.Count == 0)
                {
                    foreach (var user in e.Users)
                    {
                        OfflineUsers.Add(new User(user.Key, user.Value));
                    }
                }
                else
                {
                    foreach (var user in Users.Where(user => !OnlineUsers.Contains(user)))
                    {
                        OfflineUsers.Add(new User(user.Id, user.Name));
                    }
                }
            });
        }

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            switch (e.ConnectionRequestCode)
            {
                case ConnectionRequestCode.Connect:
                    Name = e.Name;
                    _messageService.Subscribe();
                    _chatService.Subscribe();
                    break;
                default:
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        Users.Clear();
                        MessageViewModels.Clear();
                        ChatViewModels.Clear();
                        OfflineUsers.Clear();
                        OnlineUsers.Clear();
                        //this = new ChatControlViewModel(_messageService, _connectionService, _chatService, _dialogService);
                    });
                    break;
            }
        }

        private void OnGetMessages(object sender, GetMessagesEventArgs<Message> e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var message in e.Messages)
                {
                    MessageViewModels.Add(new MessageViewModel()
                    {
                        Name = e.Users.First(x => x.Key == message.SenderId).Value,
                        Text = message.Text,
                        Time = message.Time,
                        MessageStatus = MessageStatus.Delivered,
                        MessageType = message.SenderId == _connectionService.Id ? MessageType.Outgoing : MessageType.Ingoing,
                        Message = message
                    });
                }
                MessageEvent?.Invoke(this, EventArgs.Empty);
            });
        }

        private void GetChats(object sender, UserChatEventArgs<Chat> e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var chat in e.Chats)
                {
                    if (chat.IsDialog)
                    {
                        chat.Name = CreateDialogName(chat.Name);
                    }

                    ChatViewModels.Add(new ChatViewModel
                        (chat.Users, chat.Name,
                        chat.IsDialog)
                    {
                        Chat =
                        {
                            Id = chat.Id
                        }
                    });
                }
                SelectedChat = ChatViewModels[0];
            });

        }

        private void OnCreatedChat(object sender, ChatEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {

                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == e.ChatId);
                if (chat == null)
                {
                    var users = e.UserIds.Select(id => Users.First(x => x.Id == id)).ToList();

                    chat = new ChatViewModel(users, e.ChatName, e.IsDialog) { Chat = { Id = e.ChatId } };

                    ChatViewModels.Add(chat);
                }

                if (chat.Chat.Id == 0)
                {
                    chat.Chat.Id = e.ChatId;
                }

                if (e.CreatorName == _connectionService.Name)
                {
                    SelectedChat = chat;
                }

                if (SelectedChat.Name != chat.Name)
                {
                    chat.CountUnreadMessages++;
                }
            });
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == e.ChatId);

                if (chat == null)
                {
                    var users = new List<User>();
                    foreach (var id in e.UserIds)
                    {
                        users.Add(new User(id, Users.First(x => x.Id == id).Name));
                    }
                    string chatName = $"{users[0].Name}|{users[1].Name}";

                    chat = new ChatViewModel(users, CreateDialogName(chatName), true)
                    {
                        Chat =
                        {
                            Id = e.ChatId
                        }
                    };
                    ChatViewModels.Add(chat);
                }

                if (SelectedChat.Chat.Id != e.ChatId)
                {
                    chat.CountUnreadMessages++;
                    return;
                }

                var messageViewModel = new MessageViewModel()
                {
                    Name = Users.First(x => x.Id == e.SenderUserId).Name,
                    Text = e.Message,
                    Time = e.Time,
                    MessageType = MessageType.Ingoing,
                    MessageStatus = MessageStatus.Delivered
                };

                MessageViewModels.Add(messageViewModel);
                MessageEvent?.Invoke(this, EventArgs.Empty);
            });
        }


        private void CreateDialogWithUser()
        {
            var users = Users.Where(user => user.Id != _connectionService.Id).ToList();
            var dialogParameters = new DialogParameters
            {
                { "users", users}
            };
            _dialogService.ShowDialog("CreateDialog", dialogParameters, r =>
            {
                if (r.Result != ButtonResult.OK)
                {
                    return;
                }

                r.Parameters.TryGetValue("user", out User selectedUser);
                var selectedUsers = new List<User>()
                {
                    new User(_connectionService.Id, _connectionService.Name),
                    selectedUser
                };

                ChatViewModel oldChat = null;
                foreach (var chat in ChatViewModels)
                {
                    if (chat.Name == selectedUser.Name)
                    {
                        oldChat = chat;
                    }
                }

                if (oldChat != null)
                {
                    SelectedChat = oldChat;
                }
                else
                {
                    var newDialog = new ChatViewModel(selectedUsers, selectedUser.Name, true) { Chat = { Name = _connectionService.Name + '|' + selectedUser.Name } };
                    ChatViewModels.Add(newDialog);
                    SelectedChat = newDialog;
                }
            });
        }

        private void CreateChat()
        {
            var users = Users.Where(o => o.Id != _connectionService.Id).ToList();

            var dialogParameters = new DialogParameters
            {
                { "users", users }
            };
            _dialogService.ShowDialog("MyStaleButton", dialogParameters, r =>
            {
                if (r.Result != ButtonResult.OK)
                {
                    return;
                }

                r.Parameters.TryGetValue("users", out List<User> selectedUsers);
                r.Parameters.TryGetValue("chatName", out string chatName);

                selectedUsers.Insert(0, new User(_connectionService.Id, _connectionService.Name));

                var selectedUsersList = selectedUsers.Select(user => user.Id).ToList();
                if (chatName == null)
                {
                    var name = new StringBuilder();
                    foreach (var user in selectedUsersList)
                    {
                        name.Append(Users.First(x => x.Id == user).Name);
                    }

                    chatName = name.ToString();
                }

                _chatService.CreateChat(chatName, 0, _connectionService.Name, selectedUsersList, false);
            });
        }

        private void OnUserConnectOrDisconnect(object sender, GetUserEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (e.IsConnect)
                {
                    OnlineUsers.Add(new User(e.Id, e.Login));
                    OfflineUsers.Remove(OfflineUsers.FirstOrDefault(x => x.Id == e.Id));
                }
                else
                {
                    OnlineUsers.Remove(OnlineUsers.FirstOrDefault(x => x.Id == e.Id));
                    OfflineUsers.Add(new User(e.Id, e.Login));
                }

                OnlineUsers = new ObservableCollection<User>(OnlineUsers.OrderBy(x => x.Name));
                OfflineUsers = new ObservableCollection<User>(OfflineUsers.OrderBy(x => x.Name));

                if (Users.FindIndex(x => x.Id == e.Id) == -1)
                {
                    Users.Add(new User(e.Id, e.Login));
                    Users = Users.OrderBy(x => x.Name).ToList();
                }
            });
        }

        private void OnGetOnlineUsers(object sender, GetUsersEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (OfflineUsers.Count == 0)
                {
                    foreach (var user in e.Users)
                    {
                        OnlineUsers.Add(new User(user.Key, user.Value));
                    }
                }
                else
                {
                    foreach (var user in e.Users)
                    {
                        OnlineUsers.Add(new User(user.Key, user.Value));
                        OfflineUsers.Remove(OfflineUsers.First(x => x.Id == user.Key));
                    }
                    OfflineUsers = new ObservableCollection<User>(OfflineUsers.OrderBy(x => x.Name));
                }

                OnlineUsers = new ObservableCollection<User>(OnlineUsers.OrderBy(x => x.Name));
            });
        }

        private void ExecuteCommand()
        {
            var message = new MessageViewModel()
            {
                Text = SendText.Trim(),
                Name = _connectionService.Name,
                Time = DateTime.Now,
                MessageStatus = MessageStatus.Sending,
                MessageType = MessageType.Outgoing
            };

            MessageViewModels.Add(message);

            var users = SelectedChat.Users.Select(user => user.Id).ToList();

            _messageService.SendChatMessage(_connectionService.Id, message.Text, SelectedChat.Chat.Id, users, SelectedChat.IsDialog, message.Id);
            SendText = string.Empty;
            MessageEvent?.Invoke(this, EventArgs.Empty);
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
            Thread.Sleep(100000);
        }
        private void OnMessageStatusChange(object sender, MessageRequestEvent e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                MessageViewModel message = MessageViewModels.First(x => x.Id == e.Id);
                switch (e.Status)
                {
                    case MessageStatus.Delivered:
                        message.MessageStatus = MessageStatus.Delivered;
                        break;
                    case MessageStatus.Sending:
                        message.MessageStatus = MessageStatus.Sending;
                        break;
                    case MessageStatus.NotDelivered:
                        message.MessageStatus = MessageStatus.NotDelivered;
                        break;
                }

                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == 0);
                if (chat != null)
                {
                    chat.Chat.Id = e.ChatId;
                }
                RaisePropertyChanged(nameof(message));
            });
        }
    }
}
