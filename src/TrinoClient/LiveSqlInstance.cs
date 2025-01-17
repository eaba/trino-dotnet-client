﻿using System.Runtime.CompilerServices;
using Akka.Actor;
using SharpPulsar.Trino.Live;
using SharpPulsar.Trino.Message;
using SharpPulsar.Trino.Naming;

namespace TrinoClient
{
    public class LiveSqlInstance
    {
        private readonly IActorRef _queryActor;
        public LiveSqlInstance(ActorSystem system, ClientOptions clientOptions, string topic, TimeSpan queryInterval, DateTime startAtPublishTime)
        {
            if (string.IsNullOrWhiteSpace(clientOptions.Server))
                throw new ArgumentException("Trino Server cannot be empty");

            var hasQuery = !string.IsNullOrWhiteSpace(clientOptions.Execute) || !string.IsNullOrWhiteSpace(clientOptions.File);

            if (!hasQuery)
                throw new ArgumentException("Query cannot be empty");

            if (!string.IsNullOrWhiteSpace(clientOptions.Execute))
            {
                clientOptions.Execute.TrimEnd(';');
            }
            else
            {
                clientOptions.Execute = File.ReadAllText(clientOptions.File).TrimEnd(';');
            }
            if (!clientOptions.Execute.Contains("__publish_time__ > {time}"))
            {
                if (clientOptions.Execute.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("add '__publish_time__ > {time}' to where clause");
                }
                throw new ArgumentException("add 'where __publish_time__ > {time}' to '" + clientOptions.Execute + "'");
            }
            if (!TopicName.IsValid(topic))
                throw new ArgumentException($"Topic '{topic}' failed validation");

            var q = new LiveSqlSession(clientOptions.ToClientSession(), clientOptions, queryInterval, startAtPublishTime, TopicName.Get(topic).ToString());

            _queryActor = system.ActorOf(LiveQuery.Prop(q));
        }
        public async IAsyncEnumerable<LiveSqlData> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var data = await _queryActor.Ask<LiveSqlData>(Read.Instance).ConfigureAwait(false);
            while (data != null && !cancellationToken.IsCancellationRequested)
            {
                yield return data;
                data = await _queryActor.Ask<LiveSqlData>(Read.Instance);
            }
        }

    }
}