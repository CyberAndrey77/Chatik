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
using Common.EventArgs;

namespace Client.ViewModels
{
    public class ChatControlViewModel : BindableBase
    {
        private string _sendText;
        private readonly IMessageService _messageService;
        private IConnectionService _connectionService;
        private IDialogService _dialogService;
        private IChatService _chatService;
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

        public DelegateCommand CreateChatCommand { get; }
        
        public ChatControlViewModel(IMessageService messageService, IConnectionService connectionService, IChatService chatService, IDialogService dialogService)
        {
            _messageViewModel = new MessageViewModel();
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;
            _chatService = chatService;

            _connectionService.UserListEvent += OnGetUsers;
            _connectionService.UserEvent += OnUserConnectOrDisconnect;
            _connectionService.MessageStatusChangeEvent += OnMessageStatusChange;
            _connectionService.GetUserChats += GetChats;
            _messageService.MessageEvent += OnMessageReceived;
            _messageService.GetPrivateMessageEvent += OnPrivateMessage;
            _messageService.ChatMessageEvent += OnChatMessage;

            _chatService.ChatCreatedEvent += OnCreatedChat;
            _chatService.ChatIsCreatedEvent += OnChatIsCreated;
            IsButtonEnable = false;

            MessageViewModels = new ObservableCollection<MessageViewModel>();
            Users = new ObservableCollection<User>();
            ChatViewModels = new ObservableCollection<ChatViewModel>();

            _sendQueue = new ConcurrentQueue<MessageViewModel>();
            SendCommand = new DelegateCommand(ExecuteCommand);
            CreateDialog = new DelegateCommand(CreateDialogWithUser);
            CreateChatCommand = new DelegateCommand(CreateChat);
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
                        chat.Name = chat.Name.Replace(_connectionService.Name, "");
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

                if (SelectedChat.Name != chat.Name)
                {
                    return;
                }
                var messageViewModel = new MessageViewModel()
                {
                    Name = e.CreatorName,
                    Text = $"{e.CreatorName} создал беседу",
                    Time = DateTime.Now,
                    MessageType = MessageType.Outgoing,
                    MessageStatus = MessageStatus.Delivered
                };

                MessageViewModels.Add(messageViewModel);
            });
        }

        private void OnCreatedChat(object sender, ChatEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (!ChatViewModels.Any(x => x.Name == e.ChatName))
                {
                    var users = new ObservableCollection<User>();

                    foreach (var user in e.UserIds.Select(id => Users.First(x => x.Id == id)))
                    {
                        users.Add(user);
                    }

                    ChatViewModels.Add(new ChatViewModel(users, e.ChatName, false));
                }

                var chat = ChatViewModels.First(x => x.Name == e.ChatName);

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

        //private void OnPrivateMessage(object sender, PrivateMessageEventArgs e)
        //{
        //    App.Current.Dispatcher.Invoke(delegate
        //    {
        //        User senderUser = Users.First(x => x.Id == e.SenderId);
        //        //if (ChatViewModels.Where(x => x.SenderUserId == e.SenderUserId).Count() == 0)
        //        if (!ChatViewModels.Any(x => x.Name == senderUser.Name))
        //        {
        //            User receiverUser = Users.First(x => x.Id == e.ReceiverId);
        //            var users = new ObservableCollection<User>()
        //            {
        //                new User(senderUser.Id, senderUser.Name),
        //                new User(receiverUser.Id, receiverUser.Name)
        //            };
        //            ChatViewModels.Add(new ChatViewModel(users, senderUser.Name, true));
        //        }

        //        var chat = ChatViewModels.First(x => x.Name == senderUser.Name);

        //        if (SelectedChat.Name != chat.Name)
        //        {
        //            chat.CountUnreadMessages++;
        //            return;
        //        }

        //        var messageViewModel = new MessageViewModel()
        //        {
        //            Name = senderUser.Name,
        //            Text = e.Message,
        //            Time = e.Time,
        //            MessageType = MessageType.Ingoing,
        //            MessageStatus = MessageStatus.Delivered
        //        };

        //        MessageViewModels.Add(messageViewModel);
        //    });
        //}

        private void OnPrivateMessage(object sender, ChatMessageEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                User senderUser = Users.First(x => x.Id == e.SenderUserId);
                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == e.ChatId);
                if (chat == null)
                {
                    User receiverUser = Users.First(x => x.Id == e.UserIds.First(u => u != e.SenderUserId));
                    var users = new ObservableCollection<User>()
                    {
                        new User(senderUser.Id, senderUser.Name),
                        new User(receiverUser.Id, receiverUser.Name)
                    };
                    ChatViewModels.Add(new ChatViewModel(users, senderUser.Name, true));
                }


                if (SelectedChat.Name != chat.Name)
                {
                    chat.CountUnreadMessages++;
                    return;
                }

                var messageViewModel = new MessageViewModel()
                {
                    Name = senderUser.Name,
                    Text = e.Message,
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
                //if (ChatViewModels.Where(x => x.SenderUserId == e.SenderUserId).Count() == 0)
                var chat = ChatViewModels.FirstOrDefault(x => x.Chat.Id == e.ChatId);

                if (chat == null)
                {
                    var users = new ObservableCollection<User>();
                    foreach (var id in e.UserIds)
                    {
                        users.Add(new User(id, Users.First(x => x.Id == id).Name));
                    }
                    string chatName = string.Empty;
                    foreach (var user in users)
                    {
                        chatName += user.Name;
                    }

                    chat = new ChatViewModel(users, chatName, true)
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
                    new User(_connectionService.Id, _connectionService.Name),
                    selectedUser
                };

                var newDialog = new ChatViewModel(users, _connectionService.Name + selectedUser.Name, true);
                ChatViewModels.Add(newDialog);
                SelectedChat = newDialog;
            });
        }

        private void CreateChat()
        {
            var dialogParameters = new DialogParameters
            {
                { "users", Users }
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

                _chatService.CreateChat(chatName, _connectionService.Name, selectedUsersList, false);

                var newChat = new ChatViewModel(selectedUsers, chatName, false);
                ChatViewModels.Add(newChat);
                SelectedChat = newChat;
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
                RaisePropertyChanged(nameof(message));
            });
        }
    }
}
