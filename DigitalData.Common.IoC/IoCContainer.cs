// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.IoC.IoCContainer
// Assembly: DigitalData.Common.IoC, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4E4106D8-966E-4166-AF0F-F08976D90899
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.IoC.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DigitalData.Common.IoC
{
  public static class IoCContainer
  {
    public static IServiceCollection Services { get; set; }

    public static IServiceProvider ServiceProvider { get; set; }

    public static void Initialize(Action configureServices)
    {
          
           IoCContainer.Services = (IServiceCollection) new ServiceCollection();
           configureServices();
           IoCContainer.ServiceProvider = (IServiceProvider)IoCContainer.Services.BuildServiceProvider();
        }

    public static void Initialize(IHost host) => IoCContainer.ServiceProvider = host.Services;

    public static void Initialize(IServiceProvider serviceProvider) => IoCContainer.ServiceProvider = serviceProvider;

    public static IEnumerable<T> GetServices<T>() => IoCContainer.ServiceProvider.GetServices<T>();

    public static IEnumerable<object> GetServices(Type servicesType) => IoCContainer.ServiceProvider.GetServices(servicesType);

    public static T GetService<T>() => IoCContainer.ServiceProvider.GetRequiredService<T>();

    public static object GetService(Type serviceType) => IoCContainer.ServiceProvider.GetRequiredService(serviceType);

    public static void AddTransient(Type serviceType, Type implementationType) => IoCContainer.Services.AddTransient(serviceType, implementationType);

    public static void AddTransient(
      Type serviceType,
      Func<IServiceProvider, object> implementationFactory)
    {
      IoCContainer.Services.AddTransient(serviceType, implementationFactory);
    }

    public static void AddTransient<TService, TImplementation>()
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddTransient<TService, TImplementation>();
    }

    public static void AddTransient(Type serviceType) => IoCContainer.Services.AddTransient(serviceType);

    public static void AddTransient<TService>() where TService : class => IoCContainer.Services.AddTransient<TService>();

    public static void AddTransient<TService>(
      Func<IServiceProvider, TService> implementationFactory)
      where TService : class
    {
      IoCContainer.Services.AddTransient<TService>(implementationFactory);
    }

    public static void AddTransient<TService, TImplementation>(
      Func<IServiceProvider, TImplementation> implementationFactory)
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddTransient<TService, TImplementation>(implementationFactory);
    }

    public static void AddTransients(IEnumerable<Type> types) => IoCContainer.Services.AddTransients(types);

    public static void AddTransients(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddTransients(types, condition);

    public static void AddTransients<T>(IEnumerable<Type> types) => IoCContainer.Services.AddTransients<T>(types);

    public static void AddTransients<T>(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddTransients<T>(types, condition);

    public static void AddScoped(Type serviceType, Type implementationType) => IoCContainer.Services.AddScoped(serviceType, implementationType);

    public static void AddScoped(
      Type serviceType,
      Func<IServiceProvider, object> implementationFactory)
    {
      IoCContainer.Services.AddScoped(serviceType, implementationFactory);
    }

    public static void AddScoped<TService, TImplementation>()
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddScoped<TService, TImplementation>();
    }

    public static void AddScoped(Type serviceType) => IoCContainer.Services.AddScoped(serviceType);

    public static void AddScoped<TService>() where TService : class => IoCContainer.Services.AddScoped<TService>();

    public static void AddScoped<TService>(
      Func<IServiceProvider, TService> implementationFactory)
      where TService : class
    {
      IoCContainer.Services.AddScoped<TService>(implementationFactory);
    }

    public static void AddScoped<TService, TImplementation>(
      Func<IServiceProvider, TImplementation> implementationFactory)
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddScoped<TService, TImplementation>(implementationFactory);
    }

    public static void AddScoped(IEnumerable<Type> types) => IoCContainer.Services.AddScoped(types);

    public static void AddScoped(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddScoped(types, condition);

    public static void AddScoped<T>(IEnumerable<Type> types) => IoCContainer.Services.AddScoped<T>(types);

    public static void AddScoped<T>(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddScoped<T>(types, condition);

    public static void AddSingleton(Type serviceType, Type implementationType) => ServiceCollectionServiceExtensions.AddSingleton(IoCContainer.Services, serviceType, implementationType);

    public static void AddSingleton(
      Type serviceType,
      Func<IServiceProvider, object> implementationFactory)
    {
      ServiceCollectionServiceExtensions.AddSingleton(IoCContainer.Services, serviceType, implementationFactory);
    }

    public static void AddSingleton<TService, TImplementation>()
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddSingleton<TService, TImplementation>();
    }

    public static void AddSingleton(Type serviceType) => IoCContainer.Services.AddSingleton(serviceType);

    public static void AddSingleton<TService>() where TService : class => IoCContainer.Services.AddSingleton<TService>();

    public static void AddSingleton<TService>(
      Func<IServiceProvider, TService> implementationFactory)
      where TService : class
    {
      IoCContainer.Services.AddSingleton<TService>(implementationFactory);
    }

    public static void AddSingleton<TService, TImplementation>(
      Func<IServiceProvider, TImplementation> implementationFactory)
      where TService : class
      where TImplementation : class, TService
    {
      IoCContainer.Services.AddSingleton<TService, TImplementation>(implementationFactory);
    }

    public static void AddSingleton(Type serviceType, object implementationInstance) => IoCContainer.Services.AddSingleton(serviceType, implementationInstance);

    public static void AddSingleton<TService>(TService implementationInstance) where TService : class => IoCContainer.Services.AddSingleton<TService>(implementationInstance);

    public static void AddSingletons(IEnumerable<Type> types) => IoCContainer.Services.AddSingletons(types);

    public static void AddSingletons(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddSingletons(types, condition);

    public static void AddSingletons<T>(IEnumerable<Type> types) => IoCContainer.Services.AddSingletons<T>(types);

    public static void AddSingletons<T>(IEnumerable<Type> types, Predicate<Type> condition) => IoCContainer.Services.AddSingletons<T>(types, condition);

    public static void RegisterScoped<T>(Assembly assembly) => IoCContainer.Services.RegisterScoped<T>(assembly);

    public static void RegisterScoped<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterScoped<T>(assemblies);

    public static void RegisterScopedAs<T>(Assembly assembly, Type defaultImplementation = null) => IoCContainer.Services.RegisterScopedAs<T>(assembly, defaultImplementation);

    public static void RegisterScopedAs<T>(
      IEnumerable<Assembly> assemblies,
      Type defaultImplementation = null)
    {
      IoCContainer.Services.RegisterScopedAs<T>(assemblies, defaultImplementation);
    }

    public static void RegisterMultipleScoped<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleScoped<T>(assembly);

    public static void RegisterMultipleScoped<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleScoped<T>(assemblies);

    public static void RegisterMultipleScopedAs<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleScopedAs<T>(assembly);

    public static void RegisterMultipleScopedAs<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleScopedAs<T>(assemblies);

    public static void RegisterTransient<T>(Assembly assembly) => IoCContainer.Services.RegisterTransient<T>(assembly);

    public static void RegisterTransient<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterTransient<T>(assemblies);

    public static void RegisterTransientAs<T>(Assembly assembly, Type defaultImplementation = null) => IoCContainer.Services.RegisterTransientAs<T>(assembly, defaultImplementation);

    public static void RegisterTransientAs<T>(
      IEnumerable<Assembly> assemblies,
      Type defaultImplementation = null)
    {
      IoCContainer.Services.RegisterTransientAs<T>(assemblies, defaultImplementation);
    }

    public static void RegisterMultipleTransients<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleTransients<T>(assembly);

    public static void RegisterMultipleTransients<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleTransients<T>(assemblies);

    public static void RegisterMultipleTransientsAs<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleTransientsAs<T>(assembly);

    public static void RegisterMultipleTransientsAs<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleTransientsAs<T>(assemblies);

    public static void RegisterSingleton<T>(Assembly assembly) => IoCContainer.Services.RegisterSingleton<T>(assembly);

    public static void RegisterSingleton<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterSingleton<T>(assemblies);

    public static void RegisterSingletonAs<T>(Assembly assembly, Type defaultImplementation = null) => IoCContainer.Services.RegisterTransientAs<T>(assembly, defaultImplementation);

    public static void RegisterSingletonAs<T>(
      IEnumerable<Assembly> assemblies,
      Type defaultImplementation = null)
    {
      IoCContainer.Services.RegisterSingletonAs<T>(assemblies, defaultImplementation);
    }

    public static void RegisterMultipleSingletons<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleSingletons<T>(assembly);

    public static void RegisterMultipleSingletons<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleSingletons<T>(assemblies);

    public static void RegisterMultipleSingletonsAs<T>(Assembly assembly) => IoCContainer.Services.RegisterMultipleSingletonsAs<T>(assembly);

    public static void RegisterMultipleSingletonsAs<T>(IEnumerable<Assembly> assemblies) => IoCContainer.Services.RegisterMultipleSingletonsAs<T>(assemblies);
  }
}
