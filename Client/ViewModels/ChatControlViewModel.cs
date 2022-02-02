using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using Common.EventArgs;

namespace Client.ViewModels
{
    public class ChatControlViewModel : BindableBase
    {
        private string _sendText;
        private readonly IMessageService _messageService;
        private readonly IConnectionService _connectionService;
        private readonly IDialogService _dialogService;
        private readonly IChatService _chatService;
        private MessageViewModel _messageViewModel;
        private ObservableCollection<MessageViewModel> _messageViewModels;
        private ObservableCollection<User> _users;
        private bool _isButtonEnable;
        private readonly ConcurrentQueue<MessageViewModel> _sendQueue;
        private string _chatName;
        private ChatViewModel _selectedChat;
        private string _name;

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
                if (_selectedChat == value)
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

        public DelegateCommand CreateChatCommand { get; }

        public string CreateDialogName(string originalName)
        {
            var name = originalName.Split('|');
            return name[0] == _connectionService.Name ? name[1] : name[0];
        }
        
        public ChatControlViewModel(IMessageService messageService, IConnectionService connectionService, IChatService chatService, IDialogService dialogService)
        {
            _messageViewModel = new MessageViewModel();
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;
            _chatService = chatService;

            _connectionService.UserListEvent += OnGetUsers;
            _connectionService.UserEvent += OnUserConnectOrDisconnect;
            _messageService.MessageStatusChangeEvent += OnMessageStatusChange;
            _chatService.GetUserChats += GetChats;
            _messageService.MessageEvent += OnMessageReceived;
            _messageService.ChatMessageEvent += OnChatMessage;
            _messageService.GetMessagesEvent += OnGetMessages;

            _chatService.ChatCreatedEvent += OnCreatedChat;
            _chatService.ChatIsCreatedEvent += OnChatIsCreated;
            IsButtonEnable = false;
            Name = _connectionService.Name;

            MessageViewModels = new ObservableCollection<MessageViewModel>();
            Users = new ObservableCollection<User>();
            ChatViewModels = new ObservableCollection<ChatViewModel>();

            _sendQueue = new ConcurrentQueue<MessageViewModel>();
            SendCommand = new DelegateCommand(ExecuteCommand);
            CreateDialog = new DelegateCommand(CreateDialogWithUser);
            CreateChatCommand = new DelegateCommand(CreateChat);
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
            });
        }

        private void GetChats(object sender, UserChatEventArgs<Chat> e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                //ChatViewModels.Add(new ChatViewModel(Users, "Главный чат", false));
                foreach (var chat in e.Chats)
                {
                    if (chat.IsDialog)
                    {
                        chat.Name = CreateDialogName(chat.Name);
                    }

                    ChatViewModels.Add(new ChatViewModel(
                        (ObservableCollection<User>) new ObservableCollection<User>().AddRange(chat.Users), chat.Name,
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

        private void OnChatIsCreated(object sender, ChatEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                var chat = ChatViewModels.First(x => x.Name == e.ChatName);

                chat.Chat.Id = e.ChatId;
                SelectedChat = chat;

                //var messageViewModel = new MessageViewModel()
                //{
                //    Name = e.CreatorName,
                //    Text = $"{e.CreatorName} создал беседу",
                //    Time = DateTime.Now,
                //    MessageType = MessageType.Outgoing,
                //    MessageStatus = MessageStatus.Delivered
                //};

                //MessageViewModels.Add(messageViewModel);
            });
        }

        private void OnCreatedChat(object sender, ChatEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {

                var chat = ChatViewModels.FirstOrDefault(x => x.Name == e.ChatName);
                if (chat == null)
                {
                    var users = new ObservableCollection<User>();

                    foreach (var user in e.UserIds.Select(id => Users.First(x => x.Id == id)))
                    {
                        users.Add(user);
                    }

                    chat = new ChatViewModel(users, e.ChatName, e.IsDialog) {Chat = {Id = e.ChatId}};

                    ChatViewModels.Add(chat);
                }


                if (SelectedChat.Name != chat.Name)
                {
                    chat.CountUnreadMessages++;
                    return;
                }

                var messageViewModel = new MessageViewModel()
                {
                    Name = e.CreatorName,
                    Text = $"{e.CreatorName} создал беседу",
                    Time = e.Time,
                    MessageType = MessageType.Ingoing,
                    MessageStatus = MessageStatus.Delivered
                };

                MessageViewModels.Add(messageViewModel);
            });
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == e.ChatId);

                if (chat == null)
                {
                    var users = new ObservableCollection<User>();
                    foreach (var id in e.UserIds)
                    {
                        users.Add(new User(id, Users.First(x => x.Id == id).Name));
                    }
                    string chatName = $"{users[0].Name}|{users[1].Name}";
                    //foreach (var user in users)
                    //{
                    //    chatName += user.Name;
                    //}


                    chat = new ChatViewModel(users, CreateDialogName(chatName), true)
                    {
                        Chat =
                        {
                            Id = e.ChatId
                        }
                    };
                    ChatViewModels.Add(chat);
                }

                

                if (SelectedChat.Name != chat.Name)
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
            });
        }


        private void CreateDialogWithUser()
        {
            //_dialogService
            var users = new ObservableCollection<User>();
            foreach (var user in Users)
            {
                if (user.Id == _connectionService.Id)
                {
                    continue;
                }
                users.Add(user);
            }
            var dialogParameters = new DialogParameters
            {
                //{ "users", Users }
                { "users", users}
            };
            _dialogService.ShowDialog("CreateDialog", dialogParameters, r =>
            {
                if (r.Result != ButtonResult.OK)
                {
                    return;
                }

                r.Parameters.TryGetValue("user", out User selectedUser);
                var selectedUsers = new ObservableCollection<User>()
                {
                    new User(_connectionService.Id, _connectionService.Name),
                    selectedUser
                };

                ChatViewModel oldChat = null;

                //if (expr)
                //{
                //    foreach (var chatVM in ChatViewModels)
                //    {
                //        if (chatVM.Users.Count != selectedUsers.Count) continue;
                //        int matchingUsers = 0;
                //        foreach (var user in chatVM.Users)
                //        {
                //            if (selectedUsers.Any(x => x.Id == user.Id))
                //            {
                //                matchingUsers++;
                //            }

                //            if (matchingUsers == selectedUsers.Count)
                //            {
                //                oldChat = chatVM;
                //            }
                //        }
                //    }
                //}
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
            var users = new ObservableCollection<User>();
            foreach (var user in Users)
            {
                if (user.Id == _connectionService.Id)
                {
                    continue;
                }
                users.Add(user);
            }
            var dialogParameters = new DialogParameters
            {
                { "users", users }
            };
            _dialogService.ShowDialog("CreateChat", dialogParameters, r =>
            {
                if (r.Result != ButtonResult.OK)
                {
                    return;
                }

                r.Parameters.TryGetValue("users", out ObservableCollection<User> selectedUsers);
                r.Parameters.TryGetValue("chatName", out string chatName);

                selectedUsers.Insert(0, new User(_connectionService.Id, _connectionService.Name));

                var selectedUsersList = selectedUsers.Select(user => user.Id).ToList();
                if (chatName == null)
                {
                    StringBuilder name = new StringBuilder();
                    foreach (var user in selectedUsersList)
                    {
                        name.Append(Users.First(x => x.Id == user).Name);
                    }

                    chatName = name.ToString();
                }

                _chatService.CreateChat(chatName, 0, _connectionService.Name, selectedUsersList, false);

                var newChat = new ChatViewModel(selectedUsers, chatName, false);
                ChatViewModels.Add(newChat);
                //SelectedChat = newChat;
            });
        }

        private void OnUserConnectOrDisconnect(object sender, GetUserEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (e.IsConnect)
                {
                    Users.Add(new User(e.Id, e.Login));

                   // ChatViewModels[0].UserIds.Add(new User(e.Login));
                }
                else
                {

                    //ChatViewModels[0].UserIds.Remove(UserIds.First(x => x.Name == e.Login));
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
                    Users.Add(new User(user.Key, user.Value));
                }
                
                Users = new ObservableCollection<User>(Users.OrderBy(x => x.Name));
            });
        }

        private void ExecuteCommand()
        {
            //Message message = new Message();

            //message.SenderUserId = _connectionService.SenderUserId;
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

            //_messageService.SendMessage(message.SenderUserId, message.Text);

            var users = SelectedChat.Users.Select(user => user.Id).ToList();
            
            _messageService.SendChatMessage(_connectionService.Id, message.Text, SelectedChat.Chat.Id, users, SelectedChat.IsDialog);

            //if (SelectedChat.IsDialog)
            //{
            //    var users = SelectedChat.Users.Select(user => user.Id).ToList();
            //    _messageService.SendPrivateMessage(_connectionService.Id, message.Text, SelectedChat.Chat.Id, users);
            //}
            //else
            //{
            //    var users = SelectedChat.Users.Select(user => user.Id).ToList();
            //    _messageService.SendChatMessage(_connectionService.Id, message.Text, SelectedChat.Chat.Id, users, SelectedChat.IsDialog);
            //}
            
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
