using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using WebApp.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    public class SMSController  : TwilioController
    {
        private readonly ISmsSender _smsSender;

        public SMSController(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        // GET: /<controller>/
        public void Index(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message(incomingMessage.Body);

            _smsSender.ReceiveMessage(TwiML(messagingResponse));
        }
    }
}
