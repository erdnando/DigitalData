// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.NoGetterModelBinder
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using System.ComponentModel;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web
{
  public class NoGetterModelBinder : DefaultModelBinder
  {
    protected override void BindProperty(
      ControllerContext controllerContext,
      ModelBindingContext bindingContext,
      PropertyDescriptor propertyDescriptor)
    {
      string subPropertyName = DefaultModelBinder.CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
      if (!bindingContext.ValueProvider.ContainsPrefix(subPropertyName))
        return;
      IModelBinder binder = this.Binders.GetBinder(propertyDescriptor.PropertyType);
      ModelMetadata modelMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
      ModelBindingContext bindingContext1 = new ModelBindingContext()
      {
        ModelMetadata = modelMetadata,
        ModelName = subPropertyName,
        ModelState = bindingContext.ModelState,
        ValueProvider = bindingContext.ValueProvider
      };
      object propertyValue = this.GetPropertyValue(controllerContext, bindingContext1, propertyDescriptor, binder);
      modelMetadata.Model = propertyValue;
      ModelState modelState = bindingContext.ModelState[subPropertyName];
      if (modelState == null || modelState.Errors.Count == 0)
      {
        if (!this.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, propertyValue))
          return;
        this.SetProperty(controllerContext, bindingContext, propertyDescriptor, propertyValue);
        this.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, propertyValue);
      }
      else
        this.SetProperty(controllerContext, bindingContext, propertyDescriptor, propertyValue);
    }
  }
}
