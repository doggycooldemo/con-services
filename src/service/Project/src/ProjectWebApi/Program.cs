﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using VSS.WebApi.Common;

namespace VSS.MasterData.Project.WebAPI
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build()
                             .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseLibuv(opts => opts.ThreadCount = 32)
                      .BuildKestrelWebHost()
                      .UseStartup<Startup>();
          });
  }
}
