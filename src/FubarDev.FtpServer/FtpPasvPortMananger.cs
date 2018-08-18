using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FubarDev.FtpServer
{
    /// <summary>
    /// Manages the available ports for passive FTP connections
    /// </summary>
    public class FtpPasvPortMananger : IFtpPasvPortMananger
    {
        [CanBeNull] private readonly ILogger<FtpPasvPortMananger> _log;

        /// <summary>
        /// The available ports used for passive connections
        /// </summary>
        [CanBeNull]
        private readonly Queue<int> _pasvPorts;

        /// <summary>
        /// Gets a value indicating whether the PASV mode should use only  .
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has limited ports; otherwise, <c>false</c>.
        /// </value>
        public bool HasLimitedPorts { get; }


        public FtpPasvPortMananger(
            [NotNull] IOptions<FtpServerOptions> serverOptions,
            [CanBeNull] ILogger<FtpPasvPortMananger> log = null)
        {
            _pasvPorts = serverOptions.Value.AvailablePasvPorts == null || serverOptions.Value.AvailablePasvPorts.Length == 0 ? null : new Queue<int>(serverOptions.Value.AvailablePasvPorts);
            HasLimitedPorts = _pasvPorts != null;
            _log = log;
        }

        public int PeekPasvPort(TimeSpan timeout)
        {
            if (_pasvPorts == null)
                return 0;

            lock (_pasvPorts)
            {
                if (_pasvPorts.Count == 0)
                {
                    do
                    {
                        if (Monitor.Wait(_pasvPorts, timeout) == false) {
                            _log?.LogError("No free port available.");
                            return -1;
                        }
                    } while (_pasvPorts.Count == 0);
                }

                var port = _pasvPorts.Dequeue();
                _log?.LogInformation($"Allocating port {port} for connection.");
                return port;
            }
        }

        public void ReleasePasvPort(int port)
        {
            if (_pasvPorts != null)
            {
                lock (_pasvPorts)
                {
                    _pasvPorts.Enqueue(port);
                    _log?.LogInformation($"Port {port} available again.");
                    if (_pasvPorts.Count == 1)
                    {
                        // wake up any blocked dequeue
                        Monitor.PulseAll(_pasvPorts);
                    }
                }
            }
        }

    }
}
