// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.Logging.LoggingSetup
// Assembly: DigitalData.Common.Logging, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 927D73D4-8C7A-4508-B977-95D2A9260E56
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.Logging.dll

using DigitalData.Common.Entities;
using DigitalData.Common.Reflection;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DigitalData.Common.Logging
{
  public class LoggingSetup
  {
    public static List<Type> GetLoggingTypes() => AssemblyScanner.GetAssemblies(searchPattern: "DigitalData.Common.Logging.*.dll").SelectMany<Assembly, Type>((Func<Assembly, IEnumerable<Type>>) (a => ((IEnumerable<Type>) a.GetExportedTypes()).Where<Type>((Func<Type, bool>) (t => t.Name.EndsWith("Logging"))))).ToList<Type>();

    public static void ValidateOptions(LoggingOptions options, IEnumerable<Type> loggingTypes)
    {
      List<MethodInfo> methodInfoList = new List<MethodInfo>();
      foreach (Type loggingType in loggingTypes)
        methodInfoList.Add(loggingType.GetMethod(nameof (ValidateOptions)));
      foreach (MethodBase methodBase in methodInfoList)
        methodBase.Invoke((object) null, new object[1]
        {
          (object) options
        });
    }

    public static LoggerConfiguration ConfigLogger(
      LoggingOptions options,
      List<Type> loggingTypes,
      LoggerConfiguration config = null)
    {
      LoggerConfiguration config1 = config ?? new LoggerConfiguration();
      config1.MinimumLevel.Is((LogEventLevel) Enum.Parse(typeof (LogEventLevel), options.LogLevel ?? LogEventLevel.Information.ToString())).MinimumLevel.Override("Microsoft", LogEventLevel.Warning).MinimumLevel.Override("System", LogEventLevel.Warning).Enrich.WithProperty("Application", (object) options.AppName).Enrich.FromLogContext().Enrich.WithMachineName().Enrich.WithEnvironmentUserName();
      if (options.LogExceptionDetails)
        config1.Enrich.WithExceptionDetails((IDestructuringOptions) new DestructuringOptionsBuilder().WithDefaultDestructurers());
      if (options.EnableSelfLog)
        SelfLog.Enable(TextWriter.Synchronized((TextWriter) File.AppendText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "selflog.log"))));
      LoggingSetup.SetupSinks(config1, options, loggingTypes);
      return config1;
    }

    private static void SetupSinks(
      LoggerConfiguration config,
      LoggingOptions options,
      List<Type> loggingTypes)
    {
      List<MethodInfo> methodInfoList = new List<MethodInfo>();
      foreach (Type loggingType in loggingTypes)
        methodInfoList.Add(loggingType.GetMethod("Setup"));
      foreach (MethodBase methodBase in methodInfoList)
        methodBase.Invoke((object) null, new object[2]
        {
          (object) config,
          (object) options
        });
    }
  }
}
