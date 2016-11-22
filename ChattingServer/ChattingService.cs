using ChattingInterfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ChattingServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChattingService : IChattingService
    {
        public ConcurrentDictionary<string, ConnectedClient> _connectedClients = new ConcurrentDictionary<string, ConnectedClient>();

        public int Login(string userName)
        {
            foreach(var client in _connectedClients)
            {
                if(client.Key.ToLower()== userName.ToLower())
                {
                    return 1;
                }
            }
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();

            ConnectedClient newClient = new ConnectedClient();
            newClient.connection = establishedUserConnection;
            newClient.Username = userName;

            _connectedClients.TryAdd(userName, newClient);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Client login: {0} at {1}", newClient.Username, System.DateTime.Now);
            Console.ResetColor();

            return 0;
        }

        public void Logout()
        {
            ConnectedClient client = GetMyClient();
            if (client != null)
            {
                ConnectedClient removedClient;
                _connectedClients.TryRemove(client.Username, out removedClient);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Client logoff: {0} at {1}", removedClient.Username, System.DateTime.Now);
                Console.ResetColor();
            }
           
        }
        public ConnectedClient GetMyClient()
        {
            var establishedUserConnection = OperationContext.Current.GetCallbackChannel<IClient>();
            foreach(var client in _connectedClients)
            {
                if(client.Value.connection == establishedUserConnection)
                {
                    return client.Value;
                }
                
            }
            return null;
        }

        public void sendMessageToAll(string message, string userName)
        {
            foreach(var client in _connectedClients)
            {
                if(client.Key.ToLower() != userName.ToLower())
                {
                    client.Value.connection.GetMessage(message, userName);
                }
            }
        }
    }
}
