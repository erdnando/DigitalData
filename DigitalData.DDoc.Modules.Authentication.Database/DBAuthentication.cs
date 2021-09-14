// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.Authentication.DatabaseAuthentication
// Assembly: DigitalData.DDoc.Modules.Authentication.Database, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2EBE4B08-80CE-4570-9F4B-8F878054AB02
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.Authentication.Database.dll

using DigitalData.Common.Cryptography.MD5;
using DigitalData.Common.IoC;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Entities.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Modules.Authentication
{
  public class DatabaseAuthentication : IDdocAuthentication
  {
    public string Name { get; } = "DatabaseAuth";

    public async Task<User> AuthenticateUser(User userLogin)
    {
      if (!this.SqlLoginEnabled())
        throw new DdocException("El modo nativo de autentificación no se encuentra configurado en el servidor");

      User user;
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
      {
        DdocLoginData userLoginData = await dataAccess.Security.GetUserLoginData(userLogin.Username);

        if (userLoginData == null)
          throw new DdocException("Los datos de autentificación no son correctos o el usuario no se encuentra registrado.");
        if (!userLoginData.Active)
          throw new DdocException("El usuario se encuentra bloqueado. Contacte al administrador.");

        DdocLoginConfig loginConfig = await dataAccess.Security.GetLoginConfig();
        if (!Md5Hashing.ValidateMd5Hash(userLogin.Password, userLoginData.Password))
        {
          if ((long) await dataAccess.Security.IncrementFailedLogon(userLogin.Username) == loginConfig.Tries)
          {
            await dataAccess.Security.DisableLogon(userLogin.Username);
            throw new DdocException("El usuario ha sido bloqueado por intentos fallidos de acceso, contacte al administrador.");
          }
          throw new DdocException("Los datos de autentificación no son correctos o el usuario no se encuentra registrado.");
        }

        if (!userLogin.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) && loginConfig.LoginExpirationEnabled)
        {
          if ((DateTime.Now - userLoginData.LastLogin).TotalDays >= (double) loginConfig.InactivityDays)
          {
            await dataAccess.Security.DisableLogon(userLogin.Username);
            throw new DdocException(string.Format("La contraseña se encuentra inactiva debido a que no se ha utilizado en {0} días.", (object) loginConfig.InactivityDays));
          }
          if (loginConfig.LoginExpiryDays > 0L && (DateTime.Now - userLoginData.CreationDate).TotalDays > (double) loginConfig.LoginExpiryDays)
          {
            await dataAccess.Security.DisableLogon(userLogin.Username);
            throw new DdocException("La contraseña se encuentra inactiva debido a que la cotraseña no se actualizó en la fecha requerida");
          }
        }

        await dataAccess.Security.SetLogonInfo(userLogin.Username);
        User user1 = await dataAccess.Admin.GetUser(userLogin.Username);
        User user2 = user1;
        user2.Profile = new List<DdocGroup>(await dataAccess.Security.GetUserProfile(userLogin.Username));
        user2 = (User) null;
        user = user1;
      }
      return user;
    }

    private bool SqlLoginEnabled()
    {
      using (IDdocDAL service = IoCContainer.GetService<IDdocDAL>())
        return service.TableExists("G_CONFIGURACION") & service.TableExists("G_CONTRASENA");
    }
  }
}
