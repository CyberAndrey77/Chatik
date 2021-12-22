using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using Common.Enums;

namespace Client.Models
{
    public class Message
    {
        private string _text;
        private string _name;
        private DateTime _time;
        private MessageStatus _status;
        
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public DateTime Time
        {
            get => _time;
            set => _time = value;
        }

        public MessageStatus Status
        {
            get => _status;
            set => _status = value;
        }
    }
}