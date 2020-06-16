using Model.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twilio.AspNet.Core;

namespace WebApp.Services
{
	public interface ISmsSender
	{
		Task SendMessage(MessageDTO message);
		Task SendMessages(IEnumerable<MessageDTO> messages);
		Task ReceiveMessage(TwiMLResult receivedMessage);
	}
}
