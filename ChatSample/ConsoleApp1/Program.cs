using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static bool loop;
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var chatservice = ChatSample.Container.Instance.ChatService;
            (chatservice.Messages as INotifyCollectionChanged).CollectionChanged += Program_CollectionChanged;
            chatservice.PropertyChanged += Client_PropertyChanged;
            chatservice.ErrorEvent += Chatservice_ErrorEvent;
                            
            chatservice.LoadRoomAsync().GetAwaiter().GetResult();

            ChatSample.RoomModel selectroom = null;
            string username = "";

            loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("参加するルームを選択してください。");
                foreach (var room in chatservice.Rooms)
                {
                    Console.WriteLine($"{room.Id} -- {room.Name}");
                }
                var key = Console.ReadLine();
                int id = 0;
                if (int.TryParse(key, out id))
                {
                    if (chatservice.Rooms.Any(x => x.Id == id))
                    {
                        selectroom = chatservice.Rooms.First(y => y.Id == id);
                        loop = false;
                    }
                }
            }

            loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("ユーザ名を入力してください。");
                username = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(username))
                {
                    loop = false;
                }
            }

            Console.Clear();

            chatservice.JoinRoomAsync(selectroom, username).GetAwaiter().GetResult();

            loop = true;
            while (loop)
            {
                var key = Console.ReadLine();
                switch (key)
                {
                    case "input":
                        Console.Clear();
                        var message = "";
                        while (string.IsNullOrWhiteSpace(message))
                        {
                            Console.WriteLine("メッセージを入力してください。");
                            message = Console.ReadLine();
                        }
                        chatservice.SendMessageAsync(message).GetAwaiter().GetResult();
                        Console.Clear();
                        foreach (var item in chatservice.Messages.Reverse().Take(10).Reverse())
                        {
                            Console.WriteLine($"{item.Message}   {item.UserName}");
                        }
                        break;
                    case "end":
                        Console.Clear();
                        Console.WriteLine("終了します。");
                        chatservice.ExitRoom().GetAwaiter().GetResult();
                        break;
                }
            }
            
        }

        private static void Chatservice_ErrorEvent(object sender, ChatSample.ErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            Console.Read();
        }

        private static void Client_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var service = (ChatSample.ChatService)sender;
            switch(e.PropertyName)
            {
                case "SelectedRoom":
                    if(service.SelectedRoom == null)
                    {
                        Console.Clear();
                        Console.WriteLine("チャットが終了しました。");
                        loop = false;
                    }
                    break;
            }
        }

        private static void Program_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems?.Count > 0)
            {
                foreach(ChatSample.ChatMessageModel item in e.NewItems)
                {
                    Console.Out.WriteLineAsync($"{item.Message}   {item.UserName}");
                }
            }
        }
    }
}