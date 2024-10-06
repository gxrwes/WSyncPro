using System;
using System.Collections.Generic;
using WSyncPro.Models.Content;
using WSyncPro.Models.State;

namespace WSyncPro.Core.Managers
{
    public sealed class StateManager
    {
        private static readonly Lazy<StateManager> lazy = new Lazy<StateManager>(() => new StateManager());

        public static StateManager Instance => lazy.Value;

        public Dictionary<SyncJob, SyncJobState> JobStates { get; set; }

        private StateManager()
        {
            JobStates = new Dictionary<SyncJob, SyncJobState>();
        }
    }
}
