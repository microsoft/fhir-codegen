// <copyright file="CGLoggerExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;


namespace fhir_codegen_app.Logging;

public static class CGLoggerExtensions
{
    public static ILoggingBuilder AddCGLogger(
            this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, CGLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <CGLoggerConfiguration, CGLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddCGLogger(
        this ILoggingBuilder builder,
        Action<CGLoggerConfiguration> configure)
    {
        builder.AddCGLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}
