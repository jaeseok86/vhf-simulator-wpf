using STRRadar.Models;
using System;

namespace STRRadar.Services.SimulationInformationProviders
{
    public interface ISimulationInformationProvider
    {
        public event EventHandler<SimulationEventArgs>? SimulationInitialized;
        public event EventHandler<SimulationEventArgs>? SimulationRunning;
        public event EventHandler<SimulationEventArgs>? SimulationPaused;
        public event EventHandler<SimulationEventArgs>? SimulationTerminated;
        public SimulationInformation? GetSimulationInformation();
    }
}
