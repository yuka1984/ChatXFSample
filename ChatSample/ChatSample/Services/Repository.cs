using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ChatSample
{
    internal class Repository
    {
        public Repository()
        {
            Rooms = new ObservableCollection<RoomModel>();
            Messages = new ObservableCollection<ChatMessageModel>();
        }
        public ObservableCollection<RoomModel> Rooms { get; }
        public ObservableCollection<ChatMessageModel> Messages { get; }
    }
}
