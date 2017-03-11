using ChatSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace App1
{
	public partial class App : Application
	{
		public App ()
		{

			InitializeComponent();
            var service = Container.Instance.ChatService;
            var roomselectpage = new RoomSelectPage();
            roomselectpage.BindingContext = new RoomSelectPageViewModel(service);
            var navpage = new NavigationPage(roomselectpage);            

            MainPage = navpage;

            MessagingCenter.Subscribe<RoomSelectPageViewModel, RoomModel>(this, "SelectRoom", (vm, room) => {
                var inputvm = new InputUserPageViewModel(service, room);
                navpage.PushAsync(new InputUserPage { BindingContext = inputvm });
            });

            MessagingCenter.Subscribe<InputUserPageViewModel>(this, "Join", vm => {
                var chatvm = new ChatViewModel(service);
                MainPage.Navigation.PushModalAsync(new ChatPage { BindingContext = chatvm });
                //navpage.PopToRootAsync().ContinueWith((task) => );
            });
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
