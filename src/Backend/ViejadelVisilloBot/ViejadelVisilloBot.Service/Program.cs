﻿// <copyright file="Program.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Authentication;
using ViejadelVisilloBot.Service.Settings;

namespace ViejadelVisilloBot.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel((ctx, opt) =>
                {
                    var config = new BotConfiguration();
                    ctx.Configuration.GetSection(nameof(BotConfiguration)).Bind(config);
                    config.Initialize();
                    opt.Configure()
                        .Endpoint("HTTPS", listenOptions =>
                        {
                            listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls12;
                        });
                    opt.ListenAnyIP(config.CallSignalingPort, o => o.UseHttps());
                    opt.ListenAnyIP(config.CallSignalingPort + 1);
                    opt.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                });
    }
}
