﻿// -----------------------------------------------------------------------
//  <copyright file="InMemoryProviderState.cs" company="Asynkron HB">
//      Copyright (C) 2015-2017 Asynkron HB All rights reserved
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proto.Persistence
{
    internal class InMemoryProviderState : IProviderState
    {
        private readonly ConcurrentDictionary<string, List<object>> _events = new ConcurrentDictionary<string, List<object>>();

        private readonly IDictionary<string, Tuple<object, long>> _snapshots =
            new Dictionary<string, Tuple<object, long>>();

        public Task<Tuple<object, long>> GetSnapshotAsync(string actorName)
        {
            Tuple<object, long> snapshot;
            _snapshots.TryGetValue(actorName, out snapshot);
            return Task.FromResult(snapshot);
        }

        public Task GetEventsAsync(string actorName, long indexStart, Action<object> callback)
        {
            List<object> events;
            if (_events.TryGetValue(actorName, out events))
            {
                foreach (var e in events)
                {
                    callback(e);
                }
            }
            return Task.FromResult(0);
        }

        public Task PersistEventAsync(string actorName, long index, object data)
        {
            var events = _events.GetOrAdd(actorName, new List<object>());
            events.Add(data);

            return Task.FromResult(0);
        }

        public Task PersistSnapshotAsync(string actorName, long index, object data)
        {
            _snapshots[actorName] = Tuple.Create((object) data, index);
            return Task.FromResult(0);
        }

        public Task DeleteEventsAsync(string actorName, long fromIndex)
        {
            return Task.FromResult(0);
        }

        public Task DeleteSnapshotsAsync(string actorName, long fromIndex)
        {
            return Task.FromResult(0);
        }
    }
}