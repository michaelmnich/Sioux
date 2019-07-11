using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace palyground
{
    class Program
    {
        static void Main(string[] args)
        {



            //try
            //{
            //    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    IPAddress ipAdd = System.Net.IPAddress.Parse("127.0.0.1");
            //    IPEndPoint remoteEP = new IPEndPoint(ipAdd, 8586);

            //    socket.Connect(remoteEP);
            //    string msg = "dupadupa";
            //   // msg.Text = "czesc";
            //    byte[] byData = GetBytes(msg);
            //    //socket.Send(byData);
            //    socket.Send(byData);

            //    socket.Disconnect(false);
            //    socket.Close();
            //}

            //catch (Exception e)
            //{
            //    Console.WriteLine("Error..... " + e.StackTrace);
            //}


            select_miner("miner12");
            Console.ReadLine();
        }


        private static void select_miner(string name)
        {
            string cs = @"server=localhost;userid=root;
            password=;database=cryptomineweb;SslMode=none";

            MySqlConnection conn = null;
            MySqlDataReader rdr = null;

            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();

                string stm = "SELECT * FROM miners WHERE  MinerName = '"+ name + "'";
                MySqlCommand cmd = new MySqlCommand(stm, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr.GetString(3));
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }

            }
        }


        private static void WriteInt32(byte[] buffer, int startIndex, int number)
        {
            buffer[startIndex] = (byte)((number >> 24) & 0xFF);
            buffer[startIndex + 1] = (byte)((number >> 16) & 0xFF);
            buffer[startIndex + 2] = (byte)((number >> 8) & 0xFF);
            buffer[startIndex + 3] = (byte)((number) & 0xFF);
        }

      
        public static byte[] GetBytes(string message)
        {
          
            byte[] serializedMessage = Encoding.UTF8.GetBytes(message);


           int messageLength = serializedMessage.Length;


            //Create a byte array including the length of the message (4 bytes) and serialized message content
            byte[] bytes = new byte[messageLength + 4];
            WriteInt32(bytes, 0, messageLength);
            Array.Copy(serializedMessage, 0, bytes, 4, messageLength);


            return bytes;
        }




    }
}




/*
 
     
       static void Main(string[] args)
        {









            var client = ScsClientFactory.CreateClient(new ScsTcpEndPoint("127.0.0.1", 8586));
            client.WireProtocol = new MyWireProtocolFactory().CreateWireProtocol(); //Set custom wire protocol factory!
            //Register to MessageReceived event to receive messages from server.
            client.MessageReceived += Client_MessageReceived;

            Console.WriteLine("Press enter to connect to the server...");
            Console.ReadLine(); //Wait user to press enter

            client.Connect(); //Connect to the server

            Console.Write("Write some message to be sent to server: ");
            var messageText = Console.ReadLine(); //Get a message from user

            //Send message to the server
            client.SendMessage(new ScsTextMessage(messageText));

            Console.WriteLine("Press enter to disconnect from server...");
            Console.ReadLine(); //Wait user to press enter

            client.Disconnect(); //Close connection to server








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


        static void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            //Client only accepts text messages
            var message = e.Message as ScsTextMessage;
            if (message == null)
            {
                return;
            }

            Console.WriteLine("Server sent a message: " + message.Text);
        }
    }
}

     
     
     */
