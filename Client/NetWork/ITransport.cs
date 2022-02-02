﻿using System;
using System.Collections.Generic;
using System.Text;
using Client.Enums;
using Common;

namespace Client.NetWork
{

    public delegate void MethodHandler(MessageContainer messageContainer);

    public interface ITransport
    {
        void Connect(string address, int port);

        void Disconnect();

        void Login(string login);

        void Subscribe(EnumKey key, Action<MessageContainer> method);
        void SendChatMessage(int senderUserId, string text, int chatId, List<int> users, bool isDialog);
        void GetMessages(int chatId);
        void GetLogs(int selectType, DateTime starTime, DateTime endTime);
        void CreateChat(string chatName, int chatId, string creator, List<int> users, bool isDialog);
    }
}