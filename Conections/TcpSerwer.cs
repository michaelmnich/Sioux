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
    class TcpSerwer
    {
        private Dictionary<long, MyTcpClientCreditials> MyClients;
        private Dictionary<string, MyTcpClientCreditials> MyClients_byName;
        public TcpSerwer()
        {
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
                if (msg.Contains("@reg")) //example: @reg#miner12#dsafasfasfas£
                {
                    string[] msg_array = msg.Split('#');
                    if (msg_array.Length > 2)
                    {
                        if (!LIcenceValidate(msg_array[1], msg_array[2])) //
                        {

                            client.SendMessage(
                                new ScsTextMessage(
                                    "Invalid licence verufication" + System.Environment.NewLine
                                ));
                            Console.WriteLine("LOG: msg client: " + client.ClientId + ", msg: " + message.Text +
                                              " Alredy Exist Error");
                            client.Disconnect();
                        }
                        else
                        {
                            MyClients[client.ClientId].name = msg_array[1];
                            MyClients[client.ClientId].Ip = client.RemoteEndPoint.ToString();
                            // MyClients[client.ClientId].usserName = "user123"; //todo z bazy pobrac jak sie zaloguje koparka to sluzy do zbierania info o koparkach narazie nie uzywane 

                            MyClients_byName.Add(MyClients[client.ClientId].name, MyClients[client.ClientId]);
                            client.SendMessage(
                                new ScsTextMessage(
                                    "Miner: " + msg_array[1] + "Valdiation sucess" + System.Environment.NewLine
                                ));
                        }

                    }
                }
                else if (msg.Contains("@stat")) //example: @stat#Eth ore Not mining
                {
                    string[] msg_array = msg.Split('#');
                    if (msg_array.Length > 1)
                    {
                        MyClients[client.ClientId].MiningStatus = msg_array[1];
                    }
                    
                }

            }

            Console.WriteLine("LOG: msg client: " + client.ClientId + ", msg: " + message.Text);

            //Send reply message to the client

        }

        private bool LIcenceValidate(string name, string key)
        {
            //  string contents = File.ReadAllText(Environment.CurrentDirectory+"/keys.txt");
            //contents.Contains(key)
            if (MyClients_byName.ContainsKey(name)) return false;

            string DatabaseKey=  Dao.Instance().GetDao().select_miner(name);
            if (key == DatabaseKey )
            {
                return true;
            }
            return false;
        }


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


        public string GetStatus(string name)
        {
            if (MyClients_byName.ContainsKey(name))
            {
                return "Online;"+ MyClients_byName[name].MiningStatus;
            }
            return "Offline";
        }

        public string UpdateMiners()
        {
            string toreturn = "Aktualzicaja:<br>";
            string s =  "@<data>" +
                         " <StartMining>" +
                         "   <Curency>UPDATE</Curency>" +
                         " </StartMining>" +
                         "</data>";
            foreach (MyTcpClientCreditials clientCreditials in MyClients_byName.Values)
            {
                clientCreditials.client.SendMessage(
                    new ScsTextMessage(
                        s + System.Environment.NewLine
                    ));
                toreturn += "Aktualizacja: " + clientCreditials.name + "<br>";
            }
            return toreturn;
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
        public long KeyId { get; set; } //will be used to store licence key to check if it is not used twice
        public IScsServerClient client { get; set; }
       // public string usserName { get; set; }
        public string MiningStatus { get; set; }
    }
}
