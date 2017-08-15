using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.ProviderState
{
    public class ProviderStateManager : IProviderStateManager
    {
        private IDictionary<string, Action> _providerStates = new ConcurrentDictionary<string, Action>();

        public IDictionary<string, Action> GetStates()
        {
            return _providerStates;
        }

        public void AddState(string name, Action action)
        {         
            _providerStates.Add(name, action);
        }

        public void ClearStates()
        {
            _providerStates.Clear();
        }
    }
}