// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.MefControllersSupport
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Web.Mvc;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public static class MefControllersSupport
  {
    public static void Enable()
    {
      Bootstrapper.Compose();
      ControllerBuilder.Current.SetControllerFactory((IControllerFactory) new MefControllerFactory());
    }
  }
}
