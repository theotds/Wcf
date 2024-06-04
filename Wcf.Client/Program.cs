using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Wcf.Client
{
    internal class Program
    {
        [ServiceContract]
        public interface IMessageService
        {
            [OperationContract]
            string[] GetMessages();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("press any key to continue...");
            Console.ReadLine();
            string uri = "net.tcp://localhost:6565/MessageService";
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            var channel = new ChannelFactory<IMessageService>(binding);
            var endpoint = new EndpointAddress(uri);
            var proxy = channel.CreateChannel(endpoint);
            var result = proxy?.GetMessages();
            if(result != null) { 
                result.ToList().ForEach(x => Console.WriteLine(x));
            }
            Console.ReadLine();
        }
    }
}
