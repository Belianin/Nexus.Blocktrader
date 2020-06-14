using Microsoft.AspNetCore.Mvc;
using Nexus.Prophecy.Notifications;

namespace Nexus.Prophecy.Controllers
{
    [Route("api/v1")]
    public class NotifyController : Controller
    {
        private readonly INotificationService service;

        public NotifyController(INotificationService service)
        {
            this.service = service;
        }

        [HttpPost("notify")]
        public IActionResult Error([FromBody] NotificationRequest request)
        {
            var notification = new Notification(request.LogLevel, request.Message);
            
            service.Notify(notification);

            return Ok();
        }
    }
}