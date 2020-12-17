using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketService.Hub
{
    public class Handel
    {
        private ConcurrentDictionary<string, WebSocket> _users = new ConcurrentDictionary<string, WebSocket>();

        public async Task AddUser(WebSocket socket, string name)
        {
            try
            {
                var userAddedSuccessfully = _users.TryAdd(name, socket);
                while (!userAddedSuccessfully)
                {
                    userAddedSuccessfully = _users.TryAdd(name, socket);
                }


                while (socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult socketResponse;
                    var package = new List<byte>();
                    do
                    {
                        socketResponse = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        package.AddRange(new ArraySegment<byte>(buffer, 0, socketResponse.Count));
                    } while (!socketResponse.EndOfMessage);
                    var bufferAsString = System.Text.Encoding.ASCII.GetString(package.ToArray());
                }
                //await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
            catch (Exception ex)
            { }
        }

        public ConcurrentDictionary<string, WebSocket> GetUser()
        {
            return _users;
        }

        public async Task<ResponceMessage> Closs(string Id)
        {

            try
            {
                var user = _users.Where(f => f.Key == Id).Select(f => f.Value).FirstOrDefault();
                if (user != null)
                {
                   await user.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                statusDescription: "Closed by the ConnectionManager",
                                cancellationToken: CancellationToken.None);
                    //await user.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "close connection 2", CancellationToken.None);
                    return new ResponceMessage { Status = true, Message = "OK" };
                }
                return new ResponceMessage { Status = false, Message = "Id" };

            }
            catch (Exception ex)
            {
                return new ResponceMessage { Status = false, Message = ex.Message };
            }
        }
        public async Task<ResponceMessage> Send(ConcurrentDictionary<string, WebSocket> user)
        {
            try
            {
                foreach (var theSocket in user.Where(s => s.Value.State == WebSocketState.Open).ToList())
                {
                    
                    var stringAsBytes = System.Text.Encoding.ASCII.GetBytes(theSocket.Key);
                    await theSocket.Value.SendAsync(new ArraySegment<byte>(stringAsBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                return new ResponceMessage { Status = true, Message = "OK" };
            }
            catch (Exception ex)
            {
                return new ResponceMessage { Status = false, Message = ex.Message };
            }
        }

        public async Task<ResponceMessage> Send(string Id)
        {
            try
            {
                var user = _users.Where(f => f.Key == Id).Select(f => f.Value).FirstOrDefault();
                if (user != null)
                {
                    var stringAsBytes = System.Text.Encoding.ASCII.GetBytes(Id);
                    await user.SendAsync(new ArraySegment<byte>(stringAsBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    return new ResponceMessage { Status = true, Message = "OK" };
                }
                return new ResponceMessage { Status = false, Message = "Id" };

            }
            catch (Exception ex)
            {
                return new ResponceMessage { Status = false, Message = ex.Message };
            }
        }
    }
}
