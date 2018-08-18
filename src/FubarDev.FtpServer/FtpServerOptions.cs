// <copyright file="FtpServerOptions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using JetBrains.Annotations;

namespace FubarDev.FtpServer
{
    /// <summary>
    /// The FTP server options.
    /// </summary>
    public class FtpServerOptions
    {
        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        public string ServerAddress { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        public int Port { get; set; } = 21;

        /// <summary>
        /// Gets or sets the available ports for passive FTP connections. If no ports are set or
        /// the property is <c>null</c>, the default behavior will be used
        /// </summary>
        [CanBeNull]
        public int[] AvailablePasvPorts { get; set; }
    }
}
