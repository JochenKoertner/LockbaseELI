using System;
using System.Reactive.Linq;
using Lockbase.CoreDomain.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ui.Common
{

    /// <summary>
    /// Reicht Nachrichten vom Driver per SignalR an die Browser App weiter
    /// </summary>
    public class BrowserChannel
    {
        private readonly IHubContext<SignalrHub, IHubClient> target;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ILogger<BrowserChannel> logger;

        private readonly IDisposable subscription;

        public BrowserChannel(
            IObservable<Message> source,
            IHubContext<SignalrHub, IHubClient> target,
            ILoggerFactory loggerFactory,
            IDateTimeProvider dateTimeProvider)
        {
            this.target = target;
            this.dateTimeProvider = dateTimeProvider;
            this.logger = loggerFactory.CreateLogger<BrowserChannel>();

            this.subscription = source
                .Select(Map)
                .Select(ToObservable)
                .Concat()
                .Subscribe(msg => this.logger.LogInformation(msg.Message));
        }

        /// <summary>
        /// Map Message from driver to SignalR message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
		private MessageInstance Map(Message msg) => new MessageInstance()
            {
                Timestamp = this.dateTimeProvider.Now.ToString(),
                From = msg.session_id,
                Message = msg.text
            };

		/// <summary>
		/// Send Message to all SignalR Clients 
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
        private IObservable<MessageInstance> ToObservable(MessageInstance msg)
            => Observable.FromAsync(async () =>
            {
                await this.target.Clients.All.BroadcastMessage(msg);
                return msg;
            });
    }
}