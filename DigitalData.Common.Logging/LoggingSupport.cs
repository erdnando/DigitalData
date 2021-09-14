// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.Logging.LoggingSupport
// Assembly: DigitalData.Common.Logging, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 927D73D4-8C7A-4508-B977-95D2A9260E56
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.Logging.dll

using DigitalData.Common.Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;

namespace DigitalData.Common.Logging
{
  public static class LoggingSupport
  {
    public static Microsoft.Extensions.Logging.ILogger Enable(LoggingOptions options)
    {
      List<Type> loggingTypes = LoggingSetup.GetLoggingTypes();
      LoggingSetup.ValidateOptions(options, (IEnumerable<Type>) loggingTypes);
      Log.Logger = (Serilog.ILogger) LoggingSetup.ConfigLogger(options, loggingTypes).CreateLogger();
      LoggerFactory factory = new LoggerFactory();
      factory.AddSerilog(dispose: true);
      return factory.CreateLogger(options.AppName);
    }
  }
}
