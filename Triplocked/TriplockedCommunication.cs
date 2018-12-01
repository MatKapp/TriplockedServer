using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;
using Triplocked.TriplockedEngine;

namespace Triplocked
{
    public class TriplockedCommunication : WebSocketHandler
    {
        public global::TriplockedEngine.TriplockedEngine TriplockedEngine  { get; set; }
        public TriplockedCommunication(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager, new ControllerMethodInvocationStrategy())
        {
            ((ControllerMethodInvocationStrategy)MethodInvocationStrategy).Controller = this;
            TriplockedEngine = new global::TriplockedEngine.TriplockedEngine();
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = WebSocketConnectionManager.GetId(socket);
            TriplockedEngine.AddPlayer(socketId);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} is now connected"
            };

            await SendMessageToAllAsync(message);
        }

        // this method can be called from a client, doesn't return anything.
        public async Task SendMessage(WebSocket socket, string message)
        {
           await InvokeClientMethodToAllAsync("receiveMessage", WebSocketConnectionManager.GetId(socket), message);
        }

        // this method can be called from a client, sequence of player actions.
        public async Task PlayerAction(WebSocket socket, string message)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var response = TriplockedEngine.Action(socketId, message);
            await InvokeClientMethodToAllAsync("receiveMessage", WebSocketConnectionManager.GetId(socket), response);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            TriplockedEngine.RemovePlayer(socketId);

            await base.OnDisconnected(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} disconnected"
            };
            await SendMessageToAllAsync(message);
        }
    }
}
