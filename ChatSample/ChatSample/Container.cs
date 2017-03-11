using System;
using System.Collections.Generic;
using System.Text;

namespace ChatSample
{
    public class Container
    {
        private static Lazy<Container> lazyinstance = new Lazy<Container>(() => new Container());

        public static Container Instance => lazyinstance.Value;
        private Container()
        {
            this.Repocitory = new Repository();
            this.ChatService = new ChatService(this.Repocitory);
        }
        public ChatService ChatService { get; }

        private  Repository Repocitory { get; set; }
    }
}
