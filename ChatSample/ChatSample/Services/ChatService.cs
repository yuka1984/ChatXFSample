using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Net.WebSockets;

namespace ChatSample
{
    public class ChatService : BindableBase
    {
        private Repository _repository;
        private ClientWebSocket _socket;

        internal ChatService(Repository repository)
        {
            this._repository = repository;
            this.Rooms = new ReadOnlyObservableCollection<RoomModel>(repository.Rooms);
            this.Messages = new ReadOnlyObservableCollection<ChatMessageModel>(repository.Messages);         
        }
        
        public delegate void ErrorEventHandler(object sender, ErrorEventArgs args);
        public event ErrorEventHandler ErrorEvent;

        /// <summary>
        /// ルームリスト
        /// </summary>
        public ReadOnlyObservableCollection<RoomModel> Rooms { get; }
        /// <summary>
        /// メッセージリスト
        /// </summary>
        public ReadOnlyObservableCollection<ChatMessageModel> Messages { get; }

        private RoomModel selectedRoom;
        /// <summary>
        /// 選択されているチャットルーム
        /// </summary>
        public RoomModel SelectedRoom
        {
            get { return selectedRoom; }
            private set { this.SetProperty(ref this.selectedRoom, value); }
        }

        public bool IsJoined => SelectedRoom != null;

        private string username;
        public string UserName {
            get { return username; }
            private set { this.SetProperty(ref this.username, value); }
        }

        /// <summary>
        /// ルームリストを更新
        /// </summary>
        /// <returns></returns>
        public async Task LoadRoomAsync()
        {
            var httpclient = HttpClientExtentions.CreateHttpClient();
            try
            {
                var result = await httpclient.GetStringAsync("http://websocketscaletest1.azurewebsites.net/api/room");
                var models = JsonConvert.DeserializeObject<List<RoomModel>>(result);
                _repository.Rooms.Clear();
                models.ForEach(room => _repository.Rooms.Add(room));
            }
            catch (Exception exception)
            {
                ErrorEvent?.Invoke(this, new ErrorEventArgs(exception));
            }
            
        }

        /// <summary>
        /// チャットルームへの接続
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task JoinRoomAsync(RoomModel room, string username)
        {
            if(IsJoined)
            {
                await ExitRoom();
            }

            _socket = new ClientWebSocket();
            await _socket.ConnectAsync(new Uri("ws://websocketscaletest1.azurewebsites.net/ws"), CancellationToken.None);
            var jsonmessage = JsonConvert.SerializeObject(new JoinMessage { RoomId = room.Id, UserName = username });
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonmessage)), WebSocketMessageType.Text, true, CancellationToken.None);

            var binary = new ArraySegment<byte>(new byte[4096]);

            Task.Factory.StartNew(async () => await ReceiveAsync(), TaskCreationOptions.LongRunning);

            SelectedRoom = room;
            UserName = username;
            
        }
        /// <summary>
        /// チャットルームから離脱
        /// </summary>
        /// <returns></returns>
        public async Task ExitRoom()
        {
            if(IsJoined)
            {
                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                SelectedRoom = null;
            }
        }
        /// <summary>
        /// メッセージの送信
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            if(IsJoined)
            {
                await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);                
            }
        }

        /// <summary>
        /// メッセージの受信を行います。
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveAsync()
        {
            var resultCount = 0;
            var buffer = new byte[4096];
            while (true)
            {
                var segmentbuffer = new ArraySegment<byte>(buffer, resultCount, buffer.Length - resultCount);
                var result = await _socket.ReceiveAsync(segmentbuffer, CancellationToken.None);
                resultCount += result.Count;
                if (resultCount >= buffer.Length)
                {
                    Debug.WriteLine("Long Message!!!");
                    await _socket.CloseOutputAsync(WebSocketCloseStatus.PolicyViolation, "Long Message",
                        CancellationToken.None);
                    _socket.Dispose();
                    SelectedRoom = null;
                    UserName = "";
                }
                else if (result.EndOfMessage)
                {
                    if (result.MessageType == WebSocketMessageType.Close || resultCount == 0)
                    {
                        SelectedRoom = null;
                        UserName = "";
                        break;
                    }
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = JsonConvert.DeserializeObject<ChatMessageModel>(Encoding.UTF8.GetString(buffer, 0, resultCount));
                        _repository.Messages.Add(message);
                        resultCount = 0;
                    }
                    else
                    {
                        SelectedRoom = null;
                        UserName = "";
                        break;
                    }
                }
            }
        }
    }


}
