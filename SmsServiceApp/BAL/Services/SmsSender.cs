using Model.DTOs;
using smscc;
using smscc.SMPP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Model.Interfaces;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Models;
using BAL.Managers;
using BAL.Interfaces;
using BAL.Exceptions;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.AspNet.Core;

namespace WebApp.Services
{
	/// <summary>
	/// Sets the connection with emulator for sending messages
	/// You should connect with SMPP, open session, send message(s)
	/// And after that close session and disconnect from service
	/// </summary>
	public class SmsSender : ISmsSender 
	{
		private readonly TwilioAccountDetails twilioAccountDetails;
		private readonly ICollection<MessageDTO> messagesForSend = new List<MessageDTO>();
		private readonly IServiceScopeFactory serviceScopeFactory;

		/// <summary>
		/// Create SMPP client, add event handlers 
		/// for getting information about received and sended messages,
		/// create connection and initialize session
		/// </summary>
		/// <param name="KeepAliveInterval">Property for changing connection time(seconds) with server</param>
		/// <param name="serviceScopeFactory">instance of static service</param>
		public SmsSender(IServiceScopeFactory serviceScopeFactory, IOptions<TwilioAccountDetails> _twilioAccountDetails)
		{
			this.twilioAccountDetails = _twilioAccountDetails.Value ?? throw new ArgumentException(nameof(_twilioAccountDetails));
			this.serviceScopeFactory = serviceScopeFactory;
		}

		#region Connection methods
		/// <summary>
		/// Sets the connection with SMPP Server,
		/// If you want connect to server, you should change remote host
		/// Ends when establishes the connection
		/// </summary>
		private async Task Connect()
		{
			string accountSid = twilioAccountDetails.AccountSid;
			string authToken = twilioAccountDetails.AuthToken;

			TwilioClient.Init(accountSid, authToken);
		}
		#endregion

		#region Message sending
		/// <summary>
		/// Send collection of messages 
		/// </summary>
		/// <param name="messages">Collection of messages for send</param>
		public async Task SendMessages(IEnumerable<MessageDTO> messages)
		{
			await Connect();

			foreach (MessageDTO message in messages)
				await SendMessage(message);
		}

		public async Task SendMessage(MessageDTO message)
		{
			var responseMessage = MessageResource.Create(
			body: message.MessageText,
			from: new Twilio.Types.PhoneNumber(message.SenderPhone),
			to: new Twilio.Types.PhoneNumber(message.RecepientPhone)
			);

			ChangeMessageState(2, responseMessage.Sid);
		}

		public async Task ReceiveMessage(TwiMLResult e)
		{
			//string report = $"Message From: {e.Data.}, To: {e.Destination}, Text: {e.Content}";

			//using (StreamWriter sw = new StreamWriter(@"Received messages.txt", true, Encoding.UTF8))
			//{
			//	sw.WriteLine(report);
			//}

			RecievedMessageDTO recievedMessage = new RecievedMessageDTO();
			//recievedMessage.SenderPhone = e.Originator;
			//recievedMessage.RecipientPhone = e.Destination;
			//recievedMessage.MessageText = e.Content;
			recievedMessage.TimeOfRecieve = DateTime.UtcNow;
			using (var scope = serviceScopeFactory.CreateScope())
			{
				scope.ServiceProvider.GetService<IRecievedMessageManager>().Insert(recievedMessage);
				scope.ServiceProvider.GetService<IRecievedMessageManager>().SearchStopWordInMessages(recievedMessage);
				scope.ServiceProvider.GetService<IRecievedMessageManager>().SearchSubscribeWordInMessages(recievedMessage);
			}
		}
        #endregion

        #region Support functions
        /// <summary>
        /// Change message state in database
        /// throw exception when couldn`t find information about report object
        /// in current message collection
        /// </summary>
        /// <param name="messageStateCode">message state code from report object</param>
        /// /// <param name="messageId">message id from SMPP server</param>
        private void ChangeMessageState(int messageStateCode, string messageId)
		{
			MessageState messageState;
			switch (messageStateCode)
			{
				case 2:
					messageState = MessageState.Delivered;
					break;
				case 6:
					messageState = MessageState.Accepted;
					break;
				case 5:
					messageState = MessageState.Undeliverable;
					break;
				case 8:
					messageState = MessageState.Rejected;
					break;
				default:
					return;
			}
			MessageDTO temp = messagesForSend.FirstOrDefault(m => m.ServerId == messageId);
			if (temp != null)
			{
				using (var scope = serviceScopeFactory.CreateScope())
				{
					scope.ServiceProvider.GetService<IMailingManager>().MarkAs(temp, messageState);
				}
				messagesForSend.Remove(temp);
			}
		}
		#endregion
	}
}
