// <copyright file="ServiceCollectionExtensions.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;

using FubarDev.FtpServer;
using FubarDev.FtpServer.BackgroundTransfer;
using FubarDev.FtpServer.CommandExtensions;
using FubarDev.FtpServer.CommandHandlers;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the FTP server services to the collection.
        /// </summary>
        /// <param name="services">The service collection to add the FTP server services to.</param>
        /// <param name="configure">Configuration of the FTP server services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddFtpServer(
            this IServiceCollection services,
            [NotNull] Action<IFtpServerBuilder> configure)
        {
            services.AddOptions();

            services.AddSingleton<IFtpServer, FtpServer>();
            services.AddSingleton<ITemporaryDataFactory, TemporaryDataFactory>();
            services.AddSingleton<IFtpPasvPortMananger, FtpPasvPortMananger>();

            services.AddScoped<IFtpConnectionAccessor, FtpConnectionAccessor>();
            services.AddScoped<TcpSocketClientAccessor>();
            services.AddScoped(sp => sp.GetRequiredService<TcpSocketClientAccessor>().TcpSocketClient);

            services.AddScoped<IFtpConnection, FtpConnection>();

            services.Scan(
                sel => sel.FromAssemblyOf<PassCommandHandler>()
                    .AddClasses(filter => filter.AssignableTo<IFtpCommandHandler>()).As<IFtpCommandHandler>().WithScopedLifetime()
                    .AddClasses(filter => filter.AssignableTo<IFtpCommandHandlerExtension>().Where(t => t != typeof(GenericFtpCommandHandlerExtension))).As<IFtpCommandHandlerExtension>().WithScopedLifetime());

            configure(new FtpServerBuilder(services));

            return services;
        }

        private class FtpServerBuilder : IFtpServerBuilder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FtpServerBuilder"/> class.
            /// </summary>
            /// <param name="services">The service collection.</param>
            public FtpServerBuilder(IServiceCollection services)
            {
                Services = services;
            }

            /// <inheritdoc />
            public IServiceCollection Services { get; }
        }
    }
}
