﻿using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using VSS.Velociraptor.DesignProfiling.Servers.Client;

namespace VSS.TRex.Server.DesignElevation
{
  class Program
  {
    private static ILog Log;
    static void Main(string[] args)
    {
      // Initialise the Log4Net logging system
      var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
      string s = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "log4net.xml");
      XmlConfigurator.Configure(logRepository, new FileInfo(s));
      Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      var server = new CalculateDesignElevationsServer();
      Console.WriteLine("Press anykey to exit");
      Console.ReadLine();
    }
  }
}
