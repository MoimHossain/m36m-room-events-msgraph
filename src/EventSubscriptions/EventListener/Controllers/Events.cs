using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EventListener.Controllers
{
    [ApiController]
    [Route("")]
    public class Events : ControllerBase
    {
        private static List<string> notifications = new List<string>();
        private readonly ILogger<Events> _logger;

        public Events(ILogger<Events> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Produces("text/plain")]
        public IActionResult Post()
        {
            if (Request.QueryString.HasValue && Request.Query.ContainsKey("validationToken"))
            {
                var token = WebUtility.UrlDecode(Request.Query["validationToken"]);
                return new ContentResult() { Content = token, ContentType = "text/plain", StatusCode = 200 };
            }
            else
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var json = reader.ReadToEndAsync().Result;
                    notifications.Add(json);
                }
                notifications.Add("============================================================");
                return new ContentResult() { Content = string.Empty, ContentType = "application/json", StatusCode = 202 };
            } 
        }





        [HttpGet]
        public string Get()
        {
            var sb = new StringBuilder();

            foreach (var item in notifications) sb.AppendLine(item);

            return sb.ToString();
        }

        [HttpPost("clear")]
        public string Clean()
        {
            notifications.Clear();
            return string.Empty;
        }
    }
}
