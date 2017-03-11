using System;
using System.Collections.Generic;
using System.Text;
using Reactive.Bindings.Extensions;
using StatefulModel;
using System.Linq;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using ChatSample;
using System.Threading.Tasks;

namespace App1
{
    public class RoomSelectPageViewModel
    {
        private ChatService _chatservice;
        public RoomSelectPageViewModel(ChatService chatservice)
        {
            this._chatservice = chatservice;
            this.SelectedRoom = new ReactiveProperty<RoomModel>();
            this.SelectedRoom.Subscribe(SelectRoom);
            this.LoadedCommand = new AsyncReactiveCommand();
            this.LoadedCommand.Subscribe(async _ => await LoadedAsync());
        }

        public ReadOnlyObservableCollection<RoomModel> Rooms => _chatservice.Rooms;

        public ReactiveProperty<RoomModel> SelectedRoom { get; set; }

        public AsyncReactiveCommand LoadedCommand { get; set; }

        private void SelectRoom(RoomModel room)
        {
            if(room != null)
                Xamarin.Forms.MessagingCenter.Send<RoomSelectPageViewModel, RoomModel>(this, "SelectRoom", room);
        }

        private async Task LoadedAsync()
        {
            await this._chatservice.LoadRoomAsync();
        }

    }
}
