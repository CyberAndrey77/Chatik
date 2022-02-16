using Client.Models;
using Common.Enums;
using Prism.Mvvm;
using System;

namespace Client.ViewModels
{
    public class MessageViewModel : BindableBase
    {
        private DateTime _time;
        private string _name;
        private string _text;
        private int _column;
        private MessageStatus _messageStatus;
        private MessageType _messageType;

        public Message Message { get; set; }

        public Guid Id { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                Message.Name = value;
            }
        }

        public DateTime Time
        {
            get => _time;
            set
            {
                SetProperty(ref _time, value);
                Message.Time = value;
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref _text, value);
                Message.Text = value;
            }
        }

        public MessageStatus MessageStatus
        {
            get => _messageStatus;
            set
            {
                SetProperty(ref _messageStatus, value);
                Message.Status = value;
            }
        }


        public MessageType MessageType
        {
            get => _messageType;
            set
            {
                SetProperty(ref _messageType, value);
                Column = value == MessageType.Ingoing ? 0 : 1;
            }
        }

        public int Column
        {
            get => _column;
            set => SetProperty(ref _column, value);
        }

        public MessageViewModel()
        {
            //Time = message.Time;
            //Text = message.Text;
            //SenderUserId = message.SenderUserId;

            //Message = message;
            Id = Guid.NewGuid();
            Message = new Message();
        }
    }
}
