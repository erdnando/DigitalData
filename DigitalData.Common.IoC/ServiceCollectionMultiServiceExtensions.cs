// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.IoC.ServiceCollectionMultiServiceExtensions
// Assembly: DigitalData.Common.IoC, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4E4106D8-966E-4166-AF0F-F08976D90899
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.IoC.dll

using DigitalData.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DigitalData.Common.IoC
{
  public static class ServiceCollectionMultiServiceExtensions
  {
    public static void SetupType(this Type type, IServiceCollection services) => type.GetMethod("ExtensionSetup", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.Invoke((object) "", new object[1]
    {
      (object) services
    });

    public static IServiceCollection AddScoped(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddScoped(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddScoped(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        services.AddScoped(type);
        type.SetupType(services);
      }
      return services;
    }

    public static IServiceCollection AddScoped<T>(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddScoped<T>(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddScoped<T>(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        services.AddScoped(typeof (T), type);
        type.SetupType(services);
      }
      return services;
    }

    public static IServiceCollection AddTransients(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddTransients(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddTransients(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        services.AddTransient(type);
        type.SetupType(services);
      }
      return services;
    }

    public static IServiceCollection AddTransients<T>(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddTransients<T>(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddTransients<T>(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        services.AddTransient(typeof (T), type);
        type.SetupType(services);
      }
      return services;
    }

    public static IServiceCollection AddSingletons(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddSingletons(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddSingletons(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        services.AddSingleton(type);
        type.SetupType(services);
      }
      return services;
    }

    public static IServiceCollection AddSingletons<T>(
      this IServiceCollection services,
      IEnumerable<Type> types)
    {
      return services.AddSingletons<T>(types, (Predicate<Type>) (type => true));
    }

    public static IServiceCollection AddSingletons<T>(
      this IServiceCollection services,
      IEnumerable<Type> types,
      Predicate<Type> condition)
    {
      foreach (Type type in types.Where<Type>((Func<Type, bool>) (t => condition(t))))
      {
        ServiceCollectionServiceExtensions.AddSingleton(services, typeof (T), type);
        type.SetupType(services);
      }
      return services;
    }

    public static void RegisterScoped<T>(this IServiceCollection services, Assembly assembly) => services.RegisterScoped<T>((IEnumerable<Assembly>) new Assembly[1]
    {
      assembly
    });

    public static void RegisterScoped<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      Type serviceType = assemblies.GetServiceType<T>();
      if (serviceType == (Type) null)
        return;
      services.AddScoped(serviceType);
      serviceType.SetupType(services);
    }

    public static void RegisterScopedAs<T>(
      this IServiceCollection services,
      Assembly assembly,
      Type defaultImplementation = null)
    {
      services.RegisterScopedAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      }, defaultImplementation);
    }

    public static void RegisterScopedAs<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies,
      Type defaultImplementation = null)
    {
      Type serviceType = assemblies.GetServiceType<T>();
      if (serviceType != (Type) null)
      {
        services.AddScoped(typeof (T), serviceType);
        serviceType.SetupType(services);
      }
      else
      {
        if (!(defaultImplementation != (Type) null))
          return;
        services.AddScoped(typeof (T), defaultImplementation);
      }
    }

    public static void RegisterMultipleScoped<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleScoped<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleScoped<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddScoped(serviceTypes);
    }

    public static void RegisterMultipleScopedAs<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleScopedAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleScopedAs<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddScoped<T>(serviceTypes);
    }

    public static void RegisterTransient<T>(this IServiceCollection services, Assembly assembly) => services.RegisterTransient<T>((IEnumerable<Assembly>) new Assembly[1]
    {
      assembly
    });

    public static void RegisterTransient<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      Type serviceType = assemblies.GetServiceType<T>();
      if (serviceType == (Type) null)
        return;
      services.AddTransient(serviceType);
      serviceType.SetupType(services);
    }

    public static void RegisterTransientAs<T>(
      this IServiceCollection services,
      Assembly assembly,
      Type defaultImplementation = null)
    {
      services.RegisterTransientAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      }, defaultImplementation);
    }

    public static void RegisterTransientAs<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies, Type defaultImplementation = null)
    {

      



            try
            {
                //The code that causes the error goes here.
                //---------------------------------------------------------
                Type serviceType = assemblies.GetServiceType<T>();
                if (serviceType != (Type)null)
                {
                    services.AddTransient(typeof(T), serviceType);
                    serviceType.SetupType(services);
                }
                else
                {
                    if (!(defaultImplementation != (Type)null))
                        return;
                    services.AddTransient(typeof(T), defaultImplementation);
                }
                //-------------------------------------------------------------
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //Display or log the error based on your application.
            }



        }

    public static void RegisterMultipleTransients<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleTransients<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleTransients<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddTransients(serviceTypes);
    }

    public static void RegisterMultipleTransientsAs<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleTransientsAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleTransientsAs<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddTransients<T>(serviceTypes);
    }

    public static void RegisterSingleton<T>(this IServiceCollection services, Assembly assembly) => services.RegisterSingleton<T>((IEnumerable<Assembly>) new Assembly[1]
    {
      assembly
    });

    public static void RegisterSingleton<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      Type serviceType = assemblies.GetServiceType<T>();
      if (serviceType == (Type) null)
        return;
      services.AddSingleton(serviceType);
      serviceType.SetupType(services);
    }

    public static void RegisterSingletonAs<T>(
      this IServiceCollection services,
      Assembly assembly,
      Type defaultImplementation = null)
    {
      services.RegisterTransientAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      }, defaultImplementation);
    }

    public static void RegisterSingletonAs<T>(this IServiceCollection services,IEnumerable<Assembly> assemblies,Type defaultImplementation = null)
    {
            try
            {
                Type serviceType = assemblies.GetServiceType<T>();
                if (serviceType != (Type) null)
                  {
               
                                ServiceCollectionServiceExtensions.AddSingleton(services, typeof(T), serviceType);
                                serviceType.SetupType(services);
               
        
                  }
                  else
                  {
                    if (!(defaultImplementation != (Type) null))
                      return;
                    ServiceCollectionServiceExtensions.AddSingleton(services, typeof (T), defaultImplementation);
                  }

            }
            catch (Exception ex)
            {
                string xxx;
                xxx = "error";
            }
        }

    public static void RegisterMultipleSingletons<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleSingletons<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleSingletons<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddSingletons(serviceTypes);
    }

    public static void RegisterMultipleSingletonsAs<T>(
      this IServiceCollection services,
      Assembly assembly)
    {
      services.RegisterMultipleSingletonsAs<T>((IEnumerable<Assembly>) new Assembly[1]
      {
        assembly
      });
    }

    public static void RegisterMultipleSingletonsAs<T>(
      this IServiceCollection services,
      IEnumerable<Assembly> assemblies)
    {
      IEnumerable<Type> serviceTypes = assemblies.GetServiceTypes<T>();
      services.AddSingletons<T>(serviceTypes);
    }
  }
}
