using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Hik.Communication.Scs.Client;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;

using System.Text;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols.BinarySerialization;
using Hik.Communication.Scs.Server;
using ManagingWebSerwer.dao;

namespace ManagingWebSerwer.Conections
{
   public class TcpSerwer
    {
        private Dictionary<long, MyTcpClientCreditials> MyClients;
        private Dictionary<string, MyTcpClientCreditials> MyClients_byName;

        private List<string> _logList;

        public List<string> Logs { get { return _logList; } }
        public TcpSerwer()
        {
            _logList = new List<string>();
            MyClients = new Dictionary<long, MyTcpClientCreditials>();
            MyClients_byName = new Dictionary<string, MyTcpClientCreditials>();
            Task t = Task.Run(() => {
                var server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(8586));
                server.WireProtocolFactory = new MyWireProtocolFactory(); //Set custom wire protocol factory!
                //Register events of the server to be informed about clients
                server.ClientConnected += Server_ClientConnected;
                server.ClientDisconnected += Server_ClientDisconnected;

                server.Start(); //Start the server
                //server.
                Console.WriteLine("Server is started successfully. Press enter to stop...");
                Console.ReadLine(); //Wait user to press enter

                server.Stop(); //Stop the server

            });


        }


        public void Server_ClientConnected(object sender, ServerClientEventArgs e)
        {
            Console.WriteLine("A new client is connected. Client Id = " + e.Client.ClientId);

            //Register to MessageReceived event to receive messages from new client
            e.Client.MessageReceived += Client_MessageReceived;
            MyTcpClientCreditials tempClient = new MyTcpClientCreditials();
            //tempClient.Id = e.Client.ClientId;
            tempClient.client = e.Client;
            MyClients.Add(e.Client.ClientId, tempClient);

        }

        public void Server_ClientDisconnected(object sender, ServerClientEventArgs e)
        {
            if (MyClients.ContainsKey(e.Client.ClientId))
            {
                if (MyClients[e.Client.ClientId].name!=null && MyClients_byName.ContainsKey(MyClients[e.Client.ClientId].name))
                {
                    MyClients_byName.Remove(MyClients[e.Client.ClientId].name);
                }
                MyClients.Remove(e.Client.ClientId);
            }
            Console.WriteLine("A client is disconnected! Client Id = " + e.Client.ClientId);
        }



        public void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message as ScsTextMessage; //Server only accepts text messages
            if (message == null)
            {
                return;
            }

            //Get a reference to the client
            var client = (IScsServerClient)sender;
            if (MyClients.ContainsKey(client.ClientId))
            {
                string msg = message.Text;
                Console.WriteLine("LOG: msg client: " + client.ClientId + ", msg: " + message.Text);
                _logList.Add("LOG: msg client: " + client.ClientId + ", msg: " + message.Text);

            }
            Console.WriteLine("LOG: UNKNOW client msg: " + client.ClientId + ", msg: " + message.Text);
            _logList.Add("LOG: UNKNOW  client msg: " + client.ClientId + ", msg: " + message.Text);

            //Send reply message to the client

        }



        /// <summary>
        ///  Senging messages to connected clients
        /// </summary>
        /// <param name="s"></param>
        /// <param name="name"></param>
        public void SendMessage_to_Client(string s, string name)
        {
            //byte[] bytes = Encoding.ASCII.GetBytes(s);

            if (MyClients_byName.ContainsKey(name))
            {
                MyClients_byName[name].client.SendMessage(
                    new ScsTextMessage(
                        s + System.Environment.NewLine
                    ));
            }


        }

        /// <summary>
        ///     Return status of courent client
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetStatus(string name)
        {
            if (MyClients_byName.ContainsKey(name))
            {
                return "Online;";
            }
            return "Offline";
        }

      
    }

    internal class MyWireProtocolFactory : IScsWireProtocolFactory
    {
        public IScsWireProtocol CreateWireProtocol()
        {
            return new MyWireProtocol();
        }
    }


    public class MyWireProtocol : BinarySerializationProtocol
    {
        protected override byte[] SerializeMessage(IScsMessage message)
        {
            return Encoding.UTF8.GetBytes(((ScsTextMessage)message).Text);
        }



        protected override IScsMessage DeserializeMessage(byte[] bytes)
        {

            byte[] b = bytes;
            //Decode UTF8 encoded text and create a ScsTextMessage object
            return new ScsTextMessage(Encoding.UTF8.GetString(b));
            // return new ScsTextMessage("sdadsa");
        }
    }

    public class MyTcpClientCreditials
    {
        public string Ip { get; set; }
        public string name { get; set; }
        public IScsServerClient client { get; set; }
      
    }
}
