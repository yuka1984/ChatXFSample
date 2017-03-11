using ChatSample;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings.Extensions;

namespace App1
{
    public class ChatViewModel
    {
        private ChatService _service;
        public ChatViewModel(ChatService service)
        {
            this._service = service;
            this.SendCommand = this.Message.Select(x => !string.IsNullOrEmpty(x)).ToAsyncReactiveCommand();
            this.SendCommand.Subscribe(async _ => await Send());

            this.ExitCommand = new AsyncReactiveCommand();
            this.ExitCommand.Subscribe(async _ => await Exit());

            this.Title = service.ObserveProperty(x => x.SelectedRoom).Select(x => x?.Name)
                .CombineLatest(
                    service.ObserveProperty(x => x.UserName),
                    (roomname, username) => $"{username} - {roomname}"
                )
                .ToReadOnlyReactiveProperty();
        }

        public ReadOnlyReactiveProperty<string> Title { get; private set; }

        public ReactiveProperty<string> Message { get; private set; } = new ReactiveProperty<string>();
        public ReadOnlyObservableCollection<ChatMessageModel> Messages => this._service.Messages;
        public AsyncReactiveCommand SendCommand { get; private set; }

        public AsyncReactiveCommand ExitCommand { get; private set; }
        private async Task Send()
        {
            await this._service.SendMessageAsync(Message.Value);
            Message.Value = "";
        }
        private async Task Exit()
        {
            await this._service.ExitRoom();
        }
    }
}
