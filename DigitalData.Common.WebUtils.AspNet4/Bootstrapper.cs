// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.Bootstrapper
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Web;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class Bootstrapper
  {
    private static CompositionContainer CompositionContainer;
    private static bool IsLoaded;

    public static void Compose()
    {
      if (Bootstrapper.IsLoaded)
        return;
      Bootstrapper.CompositionContainer = new CompositionContainer((ComposablePartCatalog) new AggregateCatalog()
      {
        Catalogs = {
          (ComposablePartCatalog) new DirectoryCatalog(HttpContext.Current.Server.MapPath("~/Plugins"))
        }
      }, Array.Empty<ExportProvider>());
      Bootstrapper.CompositionContainer.ComposeParts();
      Bootstrapper.IsLoaded = true;
    }

    public static T GetInstance<T>(string contractName = null)
    {
      T obj = default (T);
      if (Bootstrapper.CompositionContainer == null)
        return obj;
      if (!Bootstrapper.CompositionContainer.Catalog.Parts.Any<ComposablePartDefinition>())
        return obj;
      try
      {
        obj = !string.IsNullOrWhiteSpace(contractName) ? Bootstrapper.CompositionContainer.GetExportedValue<T>(contractName) : Bootstrapper.CompositionContainer.GetExportedValue<T>();
      }
      catch (ImportCardinalityMismatchException ex)
      {
      }
      return obj;
    }
  }
}
