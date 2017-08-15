using System;
using System.Collections.Generic;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.ProviderState
{
    public interface IProviderStateManager
    {
        IDictionary<string, Action> GetStates();
        void AddState(string name, Action action);
        void ClearStates();
    }
}