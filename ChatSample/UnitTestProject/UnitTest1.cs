using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var chatservice = new ChatSample.ChatService(new ChatSample.Repository());

            chatservice.LoadLoomAsync().GetAwaiter().GetResult();

        }
    }
}
