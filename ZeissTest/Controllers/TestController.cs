using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using ZeissTest.Model;

namespace ZeissTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        HubConnection connection;
        ConcurrentQueue<SocketMsg> socketMsgs = new ConcurrentQueue<SocketMsg>(); //消息队列

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
            //string uri = "ws://machinestream.herokuapp.com/ws";
            string uri = "http://localhost:7133/myhub";
            try
            {
                connection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl(uri).Build();//websocket连接
                ReceiveMsg();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        [HttpGet(Name ="GetMsg")]
        public async Task<ActionResult<SocketMsg?>> Get()
        {
            
            try
            {
                SocketMsg msg;
                socketMsgs.TryDequeue(out msg);//获取队列头个信息
                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        private async void ReceiveMsg()
        {
            try
            {
                SocketMsg result = null;
                connection.On<SocketMsg>("ReceiveMessage", msg =>
                {
                    result = msg;
                });
                await connection.StartAsync();
                if (result != null) 
                {
                    socketMsgs.Enqueue(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
