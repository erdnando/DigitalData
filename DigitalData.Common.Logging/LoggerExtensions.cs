// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.Logging.LoggerExtensions
// Assembly: DigitalData.Common.Logging, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 927D73D4-8C7A-4508-B977-95D2A9260E56
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.Logging.dll

using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;

namespace DigitalData.Common.Logging
{
  public static class LoggerExtensions
  {
    public static void LogDebugParams(this ILogger logger, string message, object eventProperties = null)
    {
      IDisposable disposable = (IDisposable) null;
      if (eventProperties != null)
        disposable = LogContext.PushProperty("Parameters", eventProperties, true);
      logger.LogDebug(message);
      disposable?.Dispose();
    }

    public static void LogInfoParams(this ILogger logger, string message, object eventProperties = null)
    {
      IDisposable disposable = (IDisposable) null;
      if (eventProperties != null)
        disposable = LogContext.PushProperty("Parameters", eventProperties, true);
      logger.LogInformation(message);
      disposable?.Dispose();
    }

    public static void LogWarnParams(this ILogger logger, string message, object eventProperties = null)
    {
      IDisposable disposable = (IDisposable) null;
      if (eventProperties != null)
        disposable = LogContext.PushProperty("Parameters", eventProperties, true);
      logger.LogInformation(message);
      disposable?.Dispose();
    }

    public static void LogErrorParams(this ILogger logger, string message, object eventProperties = null)
    {
      IDisposable disposable = (IDisposable) null;
      if (eventProperties != null)
        disposable = LogContext.PushProperty("Parameters", eventProperties, true);
      logger.LogError(message);
      disposable?.Dispose();
    }

    public static string LogExWarning(
      this ILogger logger,
      Exception ex,
      string message,
      object eventProperties = null,
      bool includeErrorId = false)
    {
      IDisposable disposable1 = (IDisposable) null;
      IDisposable disposable2 = (IDisposable) null;
      string str = (string) null;
      if (eventProperties != null)
        disposable1 = LogContext.PushProperty("Parameters", eventProperties, true);
      if (includeErrorId)
      {
        if (ex.Data.Contains((object) "ErrorId"))
        {
          str = ex.Data[(object) "ErrorId"].ToString();
        }
        else
        {
          str = Guid.NewGuid().ToString("D");
          ex.Data.Add((object) "ErrorId", (object) str);
        }
        disposable2 = LogContext.PushProperty("ErrorId", (object) str);
      }
      logger.LogWarning(ex, message);
      disposable2?.Dispose();
      disposable1?.Dispose();
      return str;
    }

    public static string LogExError(
      this ILogger logger,
      Exception ex,
      string message,
      object eventProperties = null,
      bool includeErrorId = true)
    {
      IDisposable disposable1 = (IDisposable) null;
      IDisposable disposable2 = (IDisposable) null;
      string str = (string) null;
      if (eventProperties != null)
        disposable1 = LogContext.PushProperty("Parameters", eventProperties, true);
      if (includeErrorId)
      {
        if (ex.Data.Contains((object) "ErrorId"))
        {
          str = ex.Data[(object) "ErrorId"].ToString();
        }
        else
        {
          str = Guid.NewGuid().ToString("D");
          ex.Data.Add((object) "ErrorId", (object) str);
        }
        disposable2 = LogContext.PushProperty("ErrorId", (object) str);
      }
      logger.LogError(ex, message);
      disposable2?.Dispose();
      disposable1?.Dispose();
      return str;
    }
  }
}
