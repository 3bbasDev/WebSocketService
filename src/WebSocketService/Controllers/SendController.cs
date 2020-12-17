using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketService.Hub;


namespace WebSocketService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        private readonly Handel _Handel;

        public SendController(Handel Handel)
        {
            _Handel = Handel;

        }

        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok(new { users = _Handel.GetUser(), Open = _Handel.GetUser().Where(f => f.Value.State == System.Net.WebSockets.WebSocketState.Open).ToList() }));
        }
        [HttpPost]
        public async Task<IActionResult> SendAll()
        {
            try
            {
                var Send = await _Handel.Send(_Handel.GetUser());
                if (!Send.Status) return await Task.FromResult(BadRequest(Send));
                return await Task.FromResult(Ok(Send));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(BadRequest(ex.Message));
            }
        }
        [HttpPost("{Id}")]
        public async Task<IActionResult> SendOne(string Id)
        {
            try
            {
                var Send = await _Handel.Send(Id);
                if (!Send.Status) return await Task.FromResult(BadRequest(Send));
                return await Task.FromResult(Ok(Send));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(BadRequest(ex.Message));
            }
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Close(string Id)
        {
            var Closs = await _Handel.Closs(Id);
            if (!Closs.Status) return await Task.FromResult(BadRequest(Closs));
            return await Task.FromResult(Ok(Closs));

        }
    }
}
