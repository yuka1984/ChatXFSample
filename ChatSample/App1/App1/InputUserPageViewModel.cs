using ChatSample;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public class InputUserPageViewModel
    {
        private ChatService _chatservice;

        public InputUserPageViewModel(ChatService chatservice, RoomModel room)
        {
            this._chatservice = chatservice;
            this.SelectedRoom = room;
            this.UserName = new ReactiveProperty<string>();
            this.JoinCommand = UserName.Select(x => !string.IsNullOrWhiteSpace(x)).ToAsyncReactiveCommand();
            this.JoinCommand.Subscribe(async _ => await Join());
        }

        public RoomModel SelectedRoom { get; private set; }

        public ReactiveProperty<string> UserName { get; set; }

        public AsyncReactiveCommand JoinCommand { get; set; }

        private async Task Join()
        {
            await _chatservice.JoinRoomAsync(SelectedRoom, UserName.Value);
            MessagingCenter.Send<InputUserPageViewModel>(this, "Join");
        }
    }
}
