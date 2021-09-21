
using DigitalData.Common.Cfdi;
using DigitalData.Common.Conversions;
using DigitalData.Common.Cryptography.MD5;
using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.Common.Logging;
using DigitalData.Common.PdfUtils;
using DigitalData.Common.PdfUtils.Interop;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Config;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.Security;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalData.Open.Core.BLL
{
  public class DDocService : IDdocService, IDisposable
  {
    public async Task ActivityLog(DdocActionLogEntry entry)
    {
      try
      {
        if (!this.Settings.GetSetting<bool>(nameof (ActivityLog)))
          return;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.ActivityLog.WriteLogEntry(entry);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al escribir en el registro de actividades de d.doc", (object) new
        {
          entry = entry
        });
        throw;
      }
    }

    public async Task<DdocGroup> GetDdocGroup(int groupId, GroupType groupType)
    {
      DdocGroup ddocGroup1;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocGroup ddocGroup2;
          switch (groupType)
          {
            case GroupType.UserGroup:
              ddocGroup2 = await dataAccess.Admin.GetUserGroup(groupId);
              break;
            case GroupType.SecurityGroup:
              ddocGroup2 = await dataAccess.Admin.GetSecurityGroup(groupId);
              break;
            default:
              throw new DdocException("Tipo de grupos no definido.");
          }
          ddocGroup1 = ddocGroup2;
        }
        if (ddocGroup1 == null)
          throw new DdocException(string.Format("El grupo {0} no existe", (object) groupId));
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener grupo de seguridad", (object) new
        {
          groupId = groupId,
          groupType = groupType
        });
        throw;
      }
      return ddocGroup1;
    }

    public async Task<List<DdocGroup>> GetDdocGroups(GroupType groupType)
    {
      IEnumerable<DdocGroup> source;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocGroup> ddocGroups;
          switch (groupType)
          {
            case GroupType.UserGroup:
              ddocGroups = await dataAccess.Admin.GetUserGroups();
              break;
            case GroupType.SecurityGroup:
              ddocGroups = await dataAccess.Admin.GetSecurityGroups();
              break;
            default:
              throw new DdocException("Tipo de grupos no definido.");
          }
          source = ddocGroups;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener grupos de seguridad", (object) new
        {
          groupType = groupType
        });
        throw;
      }
      return source.ToList<DdocGroup>();
    }

    public async Task<int> SaveDdocGroup(DdocGroup group)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          switch (group.Type)
          {
            case GroupType.UserGroup:
              IEnumerable<DdocGroup> userGroups = await dataAccess.Admin.GetUserGroups();
              int num1;
              if (group.IsNew)
              {
                if (userGroups.Any<DdocGroup>((Func<DdocGroup, bool>) (g => g.Name.Trim() == group.Name.Trim())))
                  throw new DdocException("El grupo ya existe.");
                num1 = await dataAccess.Admin.CreateUserGroup(group);
              }
              else
                num1 = await dataAccess.Admin.UpdateUserGroup(group);
              return num1;
            case GroupType.SecurityGroup:
              IEnumerable<DdocGroup> securityGroups = await dataAccess.Admin.GetSecurityGroups();
              int num2;
              if (group.IsNew)
              {
                if (securityGroups.Any<DdocGroup>((Func<DdocGroup, bool>) (g => g.Name.Trim() == group.Name.Trim())))
                  throw new DdocException("El grupo ya existe.");
                num2 = await dataAccess.Admin.CreateSecurityGroup(group);
              }
              else
                num2 = await dataAccess.Admin.UpdateSecurityGroup(group);
              return num2;
            default:
              throw new DdocException("Tipo de grupo no definido.");
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar grupo de seguridad", (object) new
        {
          group = group
        });
        throw;
      }
    }

    public async Task DeleteDdocGroup(int groupId, GroupType groupType)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          switch (groupType)
          {
            case GroupType.UserGroup:
              await dataAccess.Admin.DeleteUserGroup(groupId);
              break;
            case GroupType.SecurityGroup:
              await dataAccess.Admin.DeleteSecurityGroup(groupId);
              break;
            default:
              throw new DdocException("Tipo de grupo no definido.");
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar grupo de seguridad", (object) new
        {
          groupId = groupId,
          groupType = groupType
        });
        throw;
      }
    }

    public async Task<List<DdocPermission>> GetDdocGroupPermissions(
      int groupId,
      GroupType groupType)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocGroup> userGroups = await dataAccess.Admin.GetUserGroups();
          IEnumerable<DdocGroup> securityGroups = await dataAccess.Admin.GetSecurityGroups();
          switch (groupType)
          {
            case GroupType.UserGroup:
              IEnumerable<DdocPermission> groupPermissions1 = await dataAccess.Admin.GetUserGroupPermissions(groupId);
              foreach (DdocPermission ddocPermission in groupPermissions1)
              {
                DdocPermission p = ddocPermission;
                p.UserGroupName = userGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.UserGroupId)).Name;
                p.SecurityGroupName = securityGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.SecurityGroupId)).Name;
              }
              return groupPermissions1.ToList<DdocPermission>();
            case GroupType.SecurityGroup:
              IEnumerable<DdocPermission> groupPermissions2 = await dataAccess.Admin.GetSecurityGroupPermissions(groupId);
              foreach (DdocPermission ddocPermission in groupPermissions2)
              {
                DdocPermission p = ddocPermission;
                p.UserGroupName = userGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.UserGroupId)).Name;
                p.SecurityGroupName = securityGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.SecurityGroupId)).Name;
              }
              return groupPermissions2.ToList<DdocPermission>();
            default:
              throw new DdocException("Tipo de grupo no definido.");
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Error al obtener permisos de grupo de seguridad", (object) new
        {
          groupId = groupId,
          groupType = groupType
        });
        throw;
      }
    }

    public async Task<List<DdocPermission>> GetAllPermissions()
    {
      List<DdocPermission> list;
      try
      {
        IEnumerable<DdocPermission> allPermissions;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocGroup> userGroups = await dataAccess.Admin.GetUserGroups();
          IEnumerable<DdocGroup> securityGroups = await dataAccess.Admin.GetSecurityGroups();
          allPermissions = await dataAccess.Admin.GetAllPermissions();
          foreach (DdocPermission ddocPermission in allPermissions)
          {
            DdocPermission p = ddocPermission;
            p.UserGroupName = userGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.UserGroupId)).Name;
            p.SecurityGroupName = securityGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == p.SecurityGroupId)).Name;
          }
          userGroups = (IEnumerable<DdocGroup>) null;
          securityGroups = (IEnumerable<DdocGroup>) null;
        }
        list = allPermissions.ToList<DdocPermission>();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener todos los permisos");
        throw;
      }
      return list;
    }

    public async Task<int> SavePermission(DdocPermission permission)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocPermission> allPermissions = await dataAccess.Admin.GetAllPermissions();
          if (permission.IsNew)
          {
            if (allPermissions.Any<DdocPermission>((Func<DdocPermission, bool>) (p => p.SecurityGroupId == permission.SecurityGroupId && p.UserGroupId == permission.UserGroupId)))
              throw new DdocException("Ya existe la regla para los grupos seleccionados.");
            num = await dataAccess.Admin.CreatePermission(permission);
          }
          else
            num = await dataAccess.Admin.UpdatePermission(permission);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar permiso", (object) new
        {
          permission = permission
        });
        throw;
      }
      return num;
    }

    public async Task DeletePermission(int permissionId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Admin.DeletePermission(permissionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar permiso", (object) new
        {
          permissionId = permissionId
        });
        throw;
      }
    }

    public async Task<DdocConfiguration> GetConfiguration()
    {
      DdocConfiguration configuration;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          configuration = await dataAccess.Admin.GetConfiguration();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener configuración de contraseñas");
        throw;
      }
      return configuration;
    }

    public async Task<int> SaveConfiguration(DdocConfiguration configuration)
    {
      int num;
      try
      {
        this.ValidateConfiguration(configuration);
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Admin.SaveConfiguration(configuration);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar configuración de contraseñas", (object) new
        {
          configuration = configuration
        });
        throw;
      }
      return num;
    }

    private void ValidateConfiguration(DdocConfiguration configuration)
    {
      if (configuration.PasswordMinLength < 6)
        throw new DdocException("La longitud mínima de la contraseña es 6 caracteres.");
    }

    public async Task<User> GetUser(string username)
    {
      User user;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          user = await dataAccess.Admin.GetUser(username);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener usuario", (object) new
        {
          username = username
        });
        throw;
      }
      return user;
    }

    public async Task<List<User>> GetUsers()
    {
      IEnumerable<User> users;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          users = await dataAccess.Admin.GetUsers();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener usuarios");
        throw;
      }
      return users.ToList<User>();
    }

    public async Task<int> SaveUser(User user)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (user.IsNew)
          {
            IEnumerable<User> users = await dataAccess.Admin.GetUsers();
            if (this.InstanceFeatures.MaxUsers > 0 && users.Count<User>() >= this.InstanceFeatures.MaxUsers)
              throw new DdocException("Se alcanzó el límite de usuarios para esta licencia.");
            await this.ValidatePassword(user);
            if (users.Any<User>((Func<User, bool>) (u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase))))
              throw new DdocException("El usuario ya existe.");
            if (!user.Profile.Any<DdocGroup>())
              throw new DdocException("Debe asignar al menos un grupo");
            num = await dataAccess.Admin.CreateUser(user);
            users = (IEnumerable<User>) null;
          }
          else
            num = await dataAccess.Admin.UpdateUser(user);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar usuario", (object) new
        {
          user = user
        });
        throw;
      }
      return num;
    }

    public async Task UpdateUserProfile(User user)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Admin.UpdateUserProfile(user.Username, string.Join<int>(",", user.Profile.Select<DdocGroup, int>((Func<DdocGroup, int>) (g => g.Id))));
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar perfil de usuario", (object) new
        {
          user = user
        });
        throw;
      }
    }

    public async Task<int> UpdateUserPassword(User user)
    {
      int num;
      try
      {
        await this.ValidatePassword(user);
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Admin.UpdatePassword(user.Username, user.Password);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar contraseña de usuario", (object) new
        {
          user = user
        });
        throw;
      }
      return num;
    }

    private async Task ValidatePassword(User user)
    {
      DdocConfiguration configuration;
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
      {
        if (!string.IsNullOrEmpty(user.OldPassword))
        {
          if ((await dataAccess.Security.GetUserLoginData(user.Username)).Password != BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(new ASCIIEncoding().GetBytes(user.OldPassword))).Replace("-", ""))
            throw new DdocException("La contraseña actual es incorrecta");
        }
        configuration = await dataAccess.Admin.GetConfiguration();
      }
      if (user.Password.Length < 1)
        throw new DdocException("Debe ingresar una contraseña");
      if (user.Password != user.PasswordConfirmation)
        throw new DdocException("Las contraseñas no coinciden");
      if (user.Password.Length > configuration.PasswordMaxLength || user.Password.Length < configuration.PasswordMinLength)
        throw new DdocException(string.Format("La contraseña debe tener entre {0} y {1} caracteres", (object) configuration.PasswordMinLength, (object) configuration.PasswordMaxLength));
      if (!new Regex("[\\d+]").IsMatch(user.Password))
        throw new DdocException("La contraseña debe contener al menos un dígito");
      if (!Regex.Match(user.Password, "[A-Z]+", RegexOptions.CultureInvariant).Success)
        throw new DdocException("La contraseña debe contener al menos una mayúscula");
      if (!Regex.Match(user.Password, "[!,@,#,$,%,&,/,?,*,+,-,_,(,)]+", RegexOptions.CultureInvariant).Success)
        throw new DdocException("La contraseñas debe contener al menos uno de los siguientes caracteres especiales: ! , @ , # , $ , % , & , / , ? , * , + , - , _ , ( , )");
      user.Password = Md5Hashing.GenerateMd5Hash(user.Password);
    }

    public async Task<List<DdocGroup>> GetUserProfile(string username)
    {
      IEnumerable<DdocGroup> userProfile;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          userProfile = await dataAccess.Admin.GetUserProfile(username);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener perfil de usuario", (object) new
        {
          username = username
        });
        throw;
      }
      return userProfile.ToList<DdocGroup>();
    }

    public async Task<int> UnlockUser(string username)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Admin.UnlockUser(username);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al desbloquear contraseña de usuario", (object) new
        {
          username = username
        });
        throw;
      }
      return num;
    }

    public async Task DeleteUser(string username)
    {
      try
      {
        if (username == "admin")
          throw new DdocException("Este usuario no se puede eliminar");
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if ((await dataAccess.Admin.GetUsers()).Count<User>() == 2)
            throw new DdocException("No se puede eliminar el único usuario disponible");
          await dataAccess.Admin.DeleteUser(username);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar usuario", (object) new
        {
          username = username
        });
        throw;
      }
    }

    public async Task<bool> FileServerExists(string fileServerId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Admin.FileServerExists(fileServerId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar servidor de archivos", (object) new
        {
          fileServerId = fileServerId
        });
        throw;
      }
      return flag;
    }

    public async Task<List<DdocFileServer>> GetFileServers()
    {
      IEnumerable<DdocFileServer> fileServers;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          fileServers = await dataAccess.Admin.GetFileServers();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener servidores");
        throw;
      }
      return fileServers.ToList<DdocFileServer>();
    }

    public async Task<int> SaveFileServer(DdocFileServer fileServer)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (fileServer.IsNew)
          {
            if ((await dataAccess.Admin.GetFileServers()).Any<DdocFileServer>((Func<DdocFileServer, bool>) (f => f.Name.Trim() == fileServer.Name)))
              throw new DdocException("El servidor de archivos ya existe");
            num = int.Parse(await dataAccess.Admin.CreateFileServer(fileServer));
          }
          else
            num = await dataAccess.Admin.UpdateFileServer(fileServer);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar servidor", (object) new
        {
          fileServer = fileServer
        });
        throw;
      }
      return num;
    }

    public async Task DeleteFileServer(string fileServerId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocFileServer> fileServers = await dataAccess.Admin.GetFileServers();
          if (fileServers.Count<DdocFileServer>() == 1 && fileServers.ElementAt<DdocFileServer>(0).Id == fileServerId)
            throw new DdocException("No se puede eliminar el único servidor de archivos disponible.");
          if ((await dataAccess.Admin.GetWarehouses()).Any<DdocWarehouse>((Func<DdocWarehouse, bool>) (w => w.ServerId == fileServerId)))
            throw new DdocException("No se puede eliminar un servidor que aun tiene almacenes.");
          await dataAccess.Admin.DeleteFileServer(fileServerId);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar servidor", (object) new
        {
          fileServerId = fileServerId
        });
        throw;
      }
    }

    public async Task<bool> WarehouseExists(int warehouseId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Admin.WarehouseExists(warehouseId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar almacen", (object) new
        {
          warehouseId = warehouseId
        });
        throw;
      }
      return flag;
    }

    public async Task<List<DdocWarehouse>> GetWarehouses()
    {
      List<DdocWarehouse> list;
      try
      {
        IEnumerable<DdocWarehouse> warehouses;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          warehouses = await dataAccess.Admin.GetWarehouses();
          foreach (DdocWarehouse ddocWarehouse1 in warehouses)
          {
            DdocWarehouse w = ddocWarehouse1;
            DdocWarehouse ddocWarehouse = w;
            ddocWarehouse.Paths = new List<DdocPath>(await dataAccess.Admin.GetWarehousePaths(w.Id));
            ddocWarehouse = (DdocWarehouse) null;
            w.ActivePath = w.Paths.SingleOrDefault<DdocPath>((Func<DdocPath, bool>) (p => p.Id == w.ActivePathId))?.RootPath;
            foreach (DdocPath path in w.Paths)
            {
              if (w.ActivePathId == path.Id)
                path.Active = true;
            }
          }
        }
        list = warehouses.ToList<DdocWarehouse>();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener almacenes");
        throw;
      }
      return list;
    }

    public async Task<bool> WarehousePathExists(int warehousePathId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Admin.WarehousePathExists(warehousePathId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar almacén", (object) new
        {
          warehousePathId = warehousePathId
        });
        throw;
      }
      return flag;
    }

    public async Task<List<DdocPath>> GetWarehousePaths(int warehouseId)
    {
      List<DdocPath> list;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocWarehouse warehouse = await dataAccess.Admin.GetWarehouse(warehouseId);
          IEnumerable<DdocPath> warehousePaths = await dataAccess.Admin.GetWarehousePaths(warehouseId);
          DdocPath ddocPath = warehousePaths.SingleOrDefault<DdocPath>((Func<DdocPath, bool>) (p => p.Id == warehouse.ActivePathId));
          if (ddocPath != null)
            ddocPath.Active = true;
          list = warehousePaths.ToList<DdocPath>();
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener rutas de almacén", (object) new
        {
          warehouseId = warehouseId
        });
        throw;
      }
      return list;
    }

    public async Task<int> SaveWarehouse(DdocWarehouse warehouse)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (warehouse.IsNew)
          {
            if ((await dataAccess.Admin.GetWarehouses()).Any<DdocWarehouse>((Func<DdocWarehouse, bool>) (w => w.Name.Trim() == warehouse.Name)))
              throw new DdocException("El almacén ya existe");
            num = await dataAccess.Admin.CreateWarehouse(warehouse);
          }
          else
            num = await dataAccess.Admin.UpdateWarehouse(warehouse);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar almacén", (object) new
        {
          warehouse = warehouse
        });
        throw;
      }
      return num;
    }

    public async Task<int> SaveWarehousePath(int warehouseId, DdocPath path)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (path.IsNew)
          {
            if ((await dataAccess.Admin.GetWarehousePaths(warehouseId)).Any<DdocPath>((Func<DdocPath, bool>) (p => p.RootPath.Trim() == path.RootPath)))
              throw new DdocException("La ruta ya existe para este almacén");
            num = await dataAccess.Admin.AddWarehousePath(warehouseId, path);
          }
          else
            num = await dataAccess.Admin.UpdateWarehousePath(path);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar ruta de almacén", (object) new
        {
          warehouseId = warehouseId,
          path = path
        });
        throw;
      }
      return num;
    }

    public async Task<int> DeleteWarehousePath(int pathId)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Admin.DeleteWarehousePath(pathId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar ruta de almacén", (object) new
        {
          pathId = pathId
        });
        throw;
      }
      return num;
    }

    public async Task<int> SetWarehouseActivePath(int warehouseId, int pathId)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Admin.SetWarehouseActivePath(warehouseId, pathId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al asignar ruta activa de almacén", (object) new
        {
          warehouseId = warehouseId,
          pathId = pathId
        });
        throw;
      }
      return num;
    }

    public async Task DeleteWarehouse(int warehouseId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocWarehouse> warehouses = await dataAccess.Admin.GetWarehouses();
          if (warehouses.Count<DdocWarehouse>() == 1 && warehouses.ElementAt<DdocWarehouse>(0).Id == warehouseId)
            throw new DdocException("No se puede eliminar el único almacen disponible.");
          if ((await dataAccess.Collections.GetCollections()).Any<DdocCollection>((Func<DdocCollection, bool>) (c =>
          {
            int? warehouseId1 = c.WarehouseId;
            int num = warehouseId;
            return warehouseId1.GetValueOrDefault() == num & warehouseId1.HasValue;
          })))
            throw new DdocException("No se puede eliminar un almacén que ya tiene colecciones.");
          if ((await dataAccess.Admin.GetWarehousePaths(warehouseId)).Any<DdocPath>())
            throw new DdocException("No se puede eliminar un almacén que ya tiene rutas asignadas.");
          await dataAccess.Admin.DeleteWarehouse(warehouseId);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar almacén", (object) new
        {
          warehouseId = warehouseId
        });
        throw;
      }
    }

    public async Task<bool> CollectionExists(string collectionId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Collections.CollectionExists(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar colección", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return flag;
    }

    public async Task<DdocCollection> GetCollection(string collectionId)
    {
      DdocCollection collection;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collection = await dataAccess.Collections.GetCollection(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener coleccion", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return collection;
    }

    public async Task<List<DdocCollection>> SearchCollection(string name)
    {
      IEnumerable<DdocCollection> source;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          source = await dataAccess.Collections.SearchCollection(name);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar coleccion por nombre", (object) new
        {
          name = name
        });
        throw;
      }
      return source.ToList<DdocCollection>();
    }

    public async Task<List<DdocCollection>> GetCollections()
    {
      IEnumerable<DdocCollection> collections;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collections = await dataAccess.Collections.GetCollections();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener colecciones");
        throw;
      }
      return collections.ToList<DdocCollection>();
    }

    public async Task<bool> CollectionFieldExists(int fieldId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Collections.CollectionFieldExists(fieldId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar campo de colección", (object) new
        {
          fieldId = fieldId
        });
        throw;
      }
      return flag;
    }

    public async Task<DdocField> GetCollectionField(int fieldId)
    {
      DdocField collectionField;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionField = await dataAccess.Collections.GetCollectionField(fieldId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener campo", (object) new
        {
          fieldId = fieldId
        });
        throw;
      }
      return collectionField;
    }

    public async Task<List<DdocField>> GetCollectionFields(string collectionId)
    {
      IEnumerable<DdocField> collectionFields;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionFields = await dataAccess.Collections.GetCollectionFields(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener campos de colección", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return collectionFields.ToList<DdocField>();
    }

    public async Task<string> GetCollectionType(string collectionId)
    {
      string collectionType;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionType = await dataAccess.Collections.GetCollectionType(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener tipo de colección", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return collectionType;
    }

    public async Task<List<DdocCollection>> GetSearchableCollections(
      string securityGroupsCsv,
      string collectionType = null)
    {
      IEnumerable<DdocCollection> searchableCollections;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          searchableCollections = await dataAccess.Collections.GetSearchableCollections(securityGroupsCsv, collectionType);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener colecciones para búsqueda", (object) new
        {
          securityGroupsCsv = securityGroupsCsv,
          collectionType = collectionType
        });
        throw;
      }
      return searchableCollections.ToList<DdocCollection>();
    }

    public async Task<List<DdocCollection>> GetRootCollections(
      string userGroupsCsv)
    {
      IEnumerable<DdocCollection> collections;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          collections = await dataAccess.Collections.GetRootCollections(userGroupsCsv);
          foreach (DdocCollection ddocCollection1 in collections)
          {
            DdocCollection ddocCollection = ddocCollection1;
            ddocCollection.Fields = new List<DdocField>(await dataAccess.Collections.GetCollectionFields(ddocCollection1.Id));
            ddocCollection = (DdocCollection) null;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener colecciones raíz", (object) new
        {
          userGroupsCsv = userGroupsCsv
        });
        throw;
      }
      List<DdocCollection> list = collections.ToList<DdocCollection>();
      collections = (IEnumerable<DdocCollection>) null;
      return list;
    }

    public async Task<List<DdocCollection>> GetParentCollections(
      string collectionId,
      string userGroupsCsv)
    {
      IEnumerable<DdocCollection> parentCollections;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          parentCollections = await dataAccess.Collections.GetParentCollections(collectionId, userGroupsCsv);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener colecciones padres", (object) new
        {
          collectionId = collectionId,
          userGroupsCsv = userGroupsCsv
        });
        throw;
      }
      return parentCollections.ToList<DdocCollection>();
    }

        public async Task<List<DdocCollection>> GetChildCollections(
         string collectionId,
         string userGroupsCsv)
        {
            IEnumerable<DdocCollection> childCollections;
            try
            {
                using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
                {
                    IEnumerable<DdocCollection> source;
                    if (collectionId.Equals("root"))
                        source = await dataAccess.Collections.GetRootCollections(userGroupsCsv);
                    else
                        source = await dataAccess.Collections.GetChildCollections(collectionId, userGroupsCsv);

                    childCollections = (IEnumerable<DdocCollection>)source.ToList<DdocCollection>();

                    //:TODO

                    //childCollections = (IEnumerable<DdocCollection>)childCollections.GroupBy(c => new
                    //{
                    //    Id = c.Id
                    //}).Select < IGrouping<<>f__AnonymousType23<string>, DdocCollection >, DdocCollection > (g => g.First<DdocCollection>()).ToList<DdocCollection>();
                    
                    
                    foreach (DdocCollection ddocCollection1 in childCollections)
                    {
                        DdocCollection ddocCollection = ddocCollection1;
                        ddocCollection.Fields = new List<DdocField>(await dataAccess.Collections.GetCollectionFields(ddocCollection1.Id));
                        ddocCollection = (DdocCollection)null;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogExError(ex, "Error al obtener colecciones hijas", (object)new
                {
                    collectionId = collectionId,
                    userGroupsCsv = userGroupsCsv
                });
                throw;
            }
            List<DdocCollection> list = childCollections.ToList<DdocCollection>();
            childCollections = (IEnumerable<DdocCollection>)null;
            return list;
        }

        public async Task<string> GetCollectionId(string collectionName, char collectionType)
    {
      string collectionId;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionId = await dataAccess.Collections.GetCollectionId(collectionName, collectionType);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener Id de colección", (object) new
        {
          collectionName = collectionName,
          collectionType = collectionType
        });
        throw;
      }
      return collectionId;
    }

    public async Task<int> GetSecurityGroupId(string collectionName, char collectionType)
    {
      int securityGroupId;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          securityGroupId = await dataAccess.Collections.GetSecurityGroupId(collectionName, collectionType);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener Id de grupo de seguridad", (object) new
        {
          collectionName = collectionName,
          collectionType = collectionType
        });
        throw;
      }
      return securityGroupId;
    }

    public async Task<bool> CollectionRuleExists(int ruleId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Collections.CollectionRuleExists(ruleId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar regla de colecciones", (object) new
        {
          ruleId = ruleId
        });
        throw;
      }
      return flag;
    }

    public async Task<List<DdocRule>> GetCollectionRules()
    {
      IEnumerable<DdocRule> collectionRules;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionRules = await dataAccess.Collections.GetCollectionRules();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener reglas de colección");
        throw;
      }
      return collectionRules.ToList<DdocRule>();
    }

    public async Task<List<DdocRule>> GetRulesForChildCollection(string collectionId)
    {
      IEnumerable<DdocRule> forChildCollection;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          forChildCollection = await dataAccess.Collections.GetRulesForChildCollection(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener regas para colección hija", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return forChildCollection.ToList<DdocRule>();
    }

    public async Task<List<DdocRule>> GetRulesForParentCollection(
      string collectionId)
    {
      IEnumerable<DdocRule> parentCollection;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          parentCollection = await dataAccess.Collections.GetRulesForParentCollection(collectionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener reglas para colección padre", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
      return parentCollection.ToList<DdocRule>();
    }

    public async Task<List<DdocRule>> GetRulesForChildField(int fieldId)
    {
      IEnumerable<DdocRule> rulesForChildField;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          rulesForChildField = await dataAccess.Collections.GetRulesForChildField(fieldId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener reglas para campo hijo", (object) new
        {
          fieldId = fieldId
        });
        throw;
      }
      return rulesForChildField.ToList<DdocRule>();
    }

    public async Task<List<DdocRule>> GetRulesForParentField(int fieldId)
    {
      IEnumerable<DdocRule> rulesForParentField;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          rulesForParentField = await dataAccess.Collections.GetRulesForParentField(fieldId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener reglas para campo padre", (object) new
        {
          fieldId = fieldId
        });
        throw;
      }
      return rulesForParentField.ToList<DdocRule>();
    }

    public async Task<int> SaveCollectionRule(DdocRule rule)
    {
      int num1;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocRule> collectionRules = await dataAccess.Collections.GetCollectionRules();
          if (rule.ParentId == rule.ChildId)
            throw new DdocException("Datos inválidos");
          if (collectionRules.Any<DdocRule>((Func<DdocRule, bool>) (r => r.ParentId == rule.ChildId && r.ChildId == rule.ParentId)))
            throw new DdocException("No se permiten referencias circulares");
          if (!collectionRules.Any<DdocRule>((Func<DdocRule, bool>) (r =>
          {
            if (r.ParentId == rule.ParentId && r.ChildId == rule.ChildId)
            {
              int? parentField1 = r.ParentField;
              int? parentField2 = rule.ParentField;
              if (parentField1.GetValueOrDefault() == parentField2.GetValueOrDefault() & parentField1.HasValue == parentField2.HasValue)
              {
                int? childField1 = r.ChildField;
                int? childField2 = rule.ChildField;
                return childField1.GetValueOrDefault() == childField2.GetValueOrDefault() & childField1.HasValue == childField2.HasValue;
              }
            }
            return false;
          })))
          {
            int? nullable = rule.ParentField;
            int num2 = 0;
            if (nullable.GetValueOrDefault() == num2 & nullable.HasValue)
            {
              nullable = rule.ChildField;
              int num3 = 0;
              if (nullable.GetValueOrDefault() == num3 & nullable.HasValue && collectionRules.Any<DdocRule>((Func<DdocRule, bool>) (r =>
              {
                if (r.ParentId == rule.ParentId && r.ChildId == rule.ChildId)
                {
                  int? parentField = r.ParentField;
                   num2 = 0;
                  if (parentField.GetValueOrDefault() == num2 & parentField.HasValue)
                  {
                    int? childField = r.ChildField;
                     num3 = 0;
                    return childField.GetValueOrDefault() == num3 & childField.HasValue;
                  }
                }
                return false;
              })))
                goto label_10;
            }
            int num6;
            if (rule.IsNew)
              num6 = await dataAccess.Collections.CreateCollectionRule(rule);
            else
              num6 = await dataAccess.Collections.UpdateCollectionRule(rule);
            num1 = num6;
            goto label_19;
          }
label_10:
          throw new DdocException("La regla ya existe");
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar regla", (object) new
        {
          rule = rule
        });
        throw;
      }
label_19:
      return num1;
    }

    public async Task DeleteCollectionRule(int ruleId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Collections.DeleteRule(ruleId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar regla de colección", (object) new
        {
          ruleId = ruleId
        });
        throw;
      }
    }

    public async Task<string> SaveCollection(DdocCollection collection)
    {
      string collectionId;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (collection.IsNew)
          {
            if (collection.Type == CollectionType.C)
            {
              if ((await dataAccess.Collections.GetRootCollections((string) null)).Any<DdocCollection>((Func<DdocCollection, bool>) (c => c.Name.Equals(collection.Name))))
                throw new DdocException("La colección raíz ya existe");
            }
            DdocCollection ddocCollection;
            int? nullable;
            if (collection.Type == CollectionType.D)
            {
              nullable = collection.WarehouseId;
              int num = 0;
              if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
              {
                nullable = collection.WarehouseId;
                if (nullable.HasValue)
                  goto label_14;
              }
              if (this.Storage.Name.Equals("FileSystem"))
              {
                ddocCollection = !string.IsNullOrEmpty(collection.NewWarehousePath) ? collection : throw new DdocException("Ingrese la nueva ruta del almacén.");
                IAdminDAL admin = dataAccess.Admin;
                ddocCollection.WarehouseId = new int?(await admin.CreateWarehouse(new DdocWarehouse()
                {
                  ServerId = "01",
                  Name = collection.Name,
                  ActivePath = collection.NewWarehousePath
                }));
                ddocCollection = (DdocCollection) null;
              }
            }
label_14:
            if (collection.SecurityGroupId == 0)
            {
              ddocCollection = collection;
              IAdminDAL admin = dataAccess.Admin;
              ddocCollection.SecurityGroupId = await admin.CreateSecurityGroup(new DdocGroup()
              {
                Name = collection.Name
              });
              ddocCollection = (DdocCollection) null;
            }
            collectionId = await dataAccess.Collections.CreateCollection(collection);
            if (collection.Type != CollectionType.C)
            {
              if (collection.Fields != null && collection.Fields.Count > 0)
              {
                foreach (DdocField field in collection.Fields)
                {
                  DdocField f = field;
                  int parentFieldId = f.Id;
                  f.IsNew = true;
                  DdocField ddocField = f;
                  ddocField.Id = await this.SaveCollectionField(collectionId, f, true);
                  ddocField = (DdocField) null;
                  if (f.CreateRule)
                  {
                    int collectionRule = await dataAccess.Collections.CreateCollectionRule(new DdocRule()
                    {
                      ParentId = collection.ParentId,
                      ChildId = collectionId,
                      ParentField = new int?(parentFieldId),
                      ChildField = new int?(f.Id)
                    });
                  }
                  f = (DdocField) null;
                }
              }
              if (!collection.ParentFieldIds.Any<int>())
              {
                ICollectionDAL collections = dataAccess.Collections;
                DdocRule rule = new DdocRule();
                rule.ParentId = collection.ParentId;
                rule.ChildId = collectionId;
                nullable = new int?();
                rule.ParentField = nullable;
                nullable = new int?();
                rule.ChildField = nullable;
                int collectionRule = await collections.CreateCollectionRule(rule);
              }
              if (await dataAccess.Collections.GetCollectionLockStatus())
                await dataAccess.Collections.CreateCollectionFieldsTable(collectionId);
              if (collection.Type == CollectionType.D)
              {
                if (this.InstanceFeatures.TextIndexing)
                  this.Indexing.CreateIndex(collectionId).Wait();
              }
            }
          }
          else
          {
            if (!string.IsNullOrEmpty(collection.FileDownloadTemplate))
            {
              int num1 = await dataAccess.Collections.UpdateCollectionFilenameTemplate(collection.Id, collection.FileDownloadTemplate);
            }
            int num2 = await dataAccess.Collections.UpdateCollection(collection);
            collectionId = collection.Id;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar colección", (object) new
        {
          collection = collection
        });
        throw;
      }
      string str = collectionId;
      collectionId = (string) null;
      return str;
    }

    public async Task<int> SaveCollectionFilenameTemplate(
      string collectionId,
      string filenameTemplate)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Collections.UpdateCollectionFilenameTemplate(collectionId, filenameTemplate);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar plantilla de nombre de archivo para colección", (object) new
        {
          collectionId = collectionId,
          filenameTemplate = filenameTemplate
        });
        throw;
      }
      return num;
    }

    public async Task DeleteCollection(string collectionId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if ((await dataAccess.Collections.GetCollection(collectionId)).HasDataTable)
          {
            if (await dataAccess.Collections.CollectionHasData(collectionId))
              throw new DdocException("La colección ya tiene datos. No se puede eliminar la colección.");
          }
          dataAccess.BeginTransaction();
          foreach (DdocRule rulesForChild in await dataAccess.Collections.GetRulesForChildCollection(collectionId))
            await dataAccess.Collections.DeleteRule(rulesForChild.Id);
          foreach (DdocField collectionField in await dataAccess.Collections.GetCollectionFields(collectionId))
            await dataAccess.Collections.DeleteCollectionField(collectionField.Id);
          await dataAccess.Collections.DeleteCollection(collectionId);
          dataAccess.CommitTransaction();
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar colección", (object) new
        {
          collectionId = collectionId
        });
        throw;
      }
    }

    public async Task<int> SaveCollectionField(
      string collectionId,
      DdocField field,
      bool newCollection = false)
    {
      int result;
      try
      {
        field.AllowedValuesString = field.AllowedValues == null || !field.AllowedValues.Any<string>() ? string.Empty : string.Join(",", (IEnumerable<string>) field.AllowedValues);
        DdocField ddocField1 = field;
        string empty;
        if (ddocField1.InMask == null)
          ddocField1.InMask = empty = string.Empty;
        DdocField ddocField2 = field;
        if (ddocField2.OutMask == null)
          ddocField2.OutMask = empty = string.Empty;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (field.IsNew)
          {
            result = await dataAccess.Collections.AddCollectionField(collectionId, field);
            if (field.ForceStructChange)
              await dataAccess.Collections.CreateCollectionField(result);
          }
          else
          {
            dataAccess.BeginTransaction();
            result = await dataAccess.Collections.UpdateCollectionField(field);
            if (field.ForceStructChange)
            {
              if (await dataAccess.Collections.CollectionHasData(collectionId))
                throw new DdocException("La colección ya tiene datos. No se puede modificar el campo.");
              await dataAccess.Collections.AlterCollectionField(field);
            }
            dataAccess.CommitTransaction();
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar campo", (object) new
        {
          collectionId = collectionId,
          field = field,
          newCollection = newCollection
        });
        throw;
      }
      return result;
    }

    public async Task MoveCollectionField(int fieldId, bool direction)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Collections.MoveCollectionField(fieldId, direction);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al mover campo", (object) new
        {
          fieldId = fieldId,
          direction = direction
        });
        throw;
      }
    }

    public async Task DeleteCollectionField(int fieldId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if ((await dataAccess.Collections.GetRulesForChildField(fieldId)).Any<DdocRule>())
            throw new DdocException("No se puede eliminar un campo heradado");
          ICollectionDAL collectionDal = dataAccess.Collections;
          DdocCollection collection = await collectionDal.GetCollection(await dataAccess.Collections.GetCollectionId(fieldId));
          collectionDal = (ICollectionDAL) null;
          DdocCollection ddocCollection = collection;
          if (ddocCollection.HasDataTable)
          {
            if (await dataAccess.Collections.CollectionHasData(ddocCollection.Id))
              throw new DdocException("La colección ya tiene datos. No se puede eliminar el campo.");
            await dataAccess.Collections.DropCollectionField(fieldId);
          }
          await dataAccess.Collections.DeleteCollectionField(fieldId);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar campo", (object) new
        {
          fieldId = fieldId
        });
        throw;
      }
    }

    public async Task FinalizeCollections()
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Collections.FinalizeCollections();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al finalizar estructura de colecciones");
        throw;
      }
    }

    public async Task<bool> GetCollectionLockStatus()
    {
      bool collectionLockStatus;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          collectionLockStatus = await dataAccess.Collections.GetCollectionLockStatus();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener estado de colecciones");
        throw;
      }
      return collectionLockStatus;
    }

    public DDocService(
      IOptions<DiDaSettings> settings,
      DdocFeatures features,
      IDdocAuthentication authentication,
      IDdocStorage storage,
      ILogger<DDocService> logger)
    {
      this.Settings = settings.Value;
      this.Logger = logger;
      this.Storage = storage;
      this.InstanceFeatures = features;
      this.Authentication = authentication;

      //new
      this.InstanceFeatures.TextIndexing = true;
      this.InstanceFeatures.Ocr = true;

      if (this.InstanceFeatures.TextIndexing)
        this.Indexing = IoCContainer.GetService<IDdocTextSearch>();
      if (!this.InstanceFeatures.Ocr)
        return;
      this.Ocr = IoCContainer.GetService<IDdocOcr>();
    }

    public DiDaSettings Settings { get; }

    public ILogger<DDocService> Logger { get; }

    public IDdocStorage Storage { get; }

    public IDdocAuthentication Authentication { get; }

    public IDdocTextSearch Indexing { get; }

    public IDdocOcr Ocr { get; }

    public DdocFeatures InstanceFeatures { get; }

    public void Dispose()
    {
      GC.WaitForPendingFinalizers();
      GC.Collect();
    }

    public async Task<bool> DocumentIdExists(string documentId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Documents.DocumentExists(documentId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al comprobar existencia de documento", (object) new
        {
          documentId = documentId
        });
        throw;
      }
      return flag;
    }

    public async Task<bool> DocumentExists(
      DdocDocument document,
      List<int> includedFields = null,
      List<int> excludedFields = null)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Documents.DocumentExists(document, includedFields, excludedFields);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al comprobar existencia de documento por sus datos", (object) new
        {
          document = document,
          includedFields = includedFields,
          excludedFields = excludedFields
        });
        throw;
      }
      return flag;
    }

    public async Task<bool> PageIdExists(string pageId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Documents.PageExists(pageId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al comprobar existencia de página", (object) new
        {
          pageId = pageId
        });
        throw;
      }
      return flag;
    }

    public async Task<DdocDocument> GetDocument(string documentId)
    {
      DdocDocument document;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          document = await dataAccess.Documents.GetDocument(documentId);
          DdocDocument ddocDocument = document != null ? document : throw new DdocException("El documento no existe");
          ddocDocument.Pages = new List<DdocPage>(await dataAccess.Documents.GetPages(documentId));
          ddocDocument = (DdocDocument) null;
          IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(document.CollectionId);
          ddocDocument = document;
          ddocDocument.Data = new List<DdocField>(await dataAccess.Documents.GetDocumentData(document.Id, document.CollectionId, collectionFields.ToList<DdocField>()));
          ddocDocument = (DdocDocument) null;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener documento", (object) new
        {
          documentId = documentId
        });
        throw;
      }
      DdocDocument ddocDocument1 = document;
      document = (DdocDocument) null;
      return ddocDocument1;
    }

    public async Task<int> CountDocuments(string collectionId, string securityGroupsCsv)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Documents.CountDocuments(collectionId, ((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)));
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener conteo de documentos de colección", (object) new
        {
          collectionId = collectionId,
          securityGroupsCsv = securityGroupsCsv
        });
        throw;
      }
      return num;
    }

    public async Task<List<DdocDocument>> GetDocuments(
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0)
    {
      List<DdocDocument> results = new List<DdocDocument>();
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocDocument> collectionDocuments = await dataAccess.Documents.GetDocuments(collectionId, ((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)), page, pageSize, sortBy, sortDirection);
          IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(collectionId);
          foreach (DdocDocument ddocDocument1 in collectionDocuments)
          {
            DdocDocument document = ddocDocument1;
            DdocDocument ddocDocument = document;
            List<DdocPage> ddocPageList1 = new List<DdocPage>();
            List<DdocPage> ddocPageList2 = ddocPageList1;
            ddocPageList2.Add((await dataAccess.Documents.GetPages(document.Id)).FirstOrDefault<DdocPage>());
            ddocDocument.Pages = ddocPageList1;
            ddocDocument = (DdocDocument) null;
            ddocPageList2 = (List<DdocPage>) null;
            ddocPageList1 = (List<DdocPage>) null;
            ddocDocument = document;
            ddocDocument.Data = new List<DdocField>(await dataAccess.Documents.GetDocumentData(document.Id, collectionId, collectionFields.ToList<DdocField>()));
            ddocDocument = (DdocDocument) null;
            results.Add(document);
            document = (DdocDocument) null;
          }
          collectionDocuments = (IEnumerable<DdocDocument>) null;
          collectionFields = (IEnumerable<DdocField>) null;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener documentos de colección", (object) new
        {
          collectionId = collectionId,
          securityGroupsCsv = securityGroupsCsv,
          page = page,
          pageSize = pageSize,
          sortBy = sortBy,
          sortDirection = sortDirection
        });
        throw;
      }
      List<DdocDocument> ddocDocumentList = results;
      results = (List<DdocDocument>) null;
      return ddocDocumentList;
    }

    public async Task<List<DdocPage>> GetPages(string documentId)
    {
      IEnumerable<DdocPage> pages;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          pages = await dataAccess.Documents.GetPages(documentId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener páginas de documento", (object) new
        {
          documentId = documentId
        });
        throw;
      }
      return pages.ToList<DdocPage>();
    }

    public async Task<string> CreatePage(
      string documentId,
      string extension,
      int sequence = 0,
      bool replaced = false)
    {
      string page;
      try
      {
        int? pathId = new int?();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (!await dataAccess.Documents.DocumentExists(documentId))
            throw new DdocException("El documento no existe");
          if (this.Storage.Name.Equals("FileSystem"))
            pathId = new int?(await dataAccess.Documents.GetWarehousePathId(documentId));
          page = await dataAccess.Documents.CreatePage(documentId, extension, sequence, pathId, replaced);
        }
        pathId = new int?();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al agregar página a documento", (object) new
        {
          documentId = documentId,
          extension = extension,
          sequence = sequence,
          replaced = replaced
        });
        throw;
      }
      return page;
    }

    public Task<(string newPageId, int imageCount)> UploadLocalPage(
      string documentId,
      string fileExt,
      string filePath,
      string pageId = null,
      int sequence = 0,
      string user = null)
    {
      return this.UploadPage(documentId, fileExt, File.ReadAllBytes(filePath), pageId, sequence, user);
    }

    public async Task<(string newPageId, int imageCount)> UploadPage(string documentId, string fileExt, byte[] byteSource, string pageId = null, int sequence = 0, string user = null)
    {
      string newPageId;
      int imageCount;

      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocDocument document = await dataAccess.Documents.GetDocument(documentId);
          if (document == null) throw new DdocException("El documento no existe");

          if (!string.IsNullOrEmpty(pageId))
          {
            sequence = await dataAccess.Documents.GetPageSequence(pageId);
            int num = await dataAccess.Documents.DeletePage(pageId, user);
            if (this.InstanceFeatures.TextIndexing)
              await this.Indexing.RemoveFromIndex(document.CollectionId, document.Id, pageId);
          }
          int? pathId = new int?();
          if (this.Storage.Name.Equals("FileSystem"))
            pathId = new int?(await dataAccess.Documents.GetWarehousePathId(documentId));
          if (this.InstanceFeatures.Ocr)
          {
            byteSource = await this.Ocr.PerformOcr(byteSource, fileExt);
            fileExt = "pdf";
          }
          newPageId = await dataAccess.Documents.CreatePage(documentId, fileExt.ToUpperInvariant(), sequence, pathId, !string.IsNullOrEmpty(pageId));
          await this.Storage.SaveBytes(documentId, newPageId, fileExt, byteSource);
          if (this.InstanceFeatures.TextIndexing)
            await this.Indexing.AddToIndex(document.CollectionId, documentId, newPageId, fileExt, (Stream) new MemoryStream(byteSource));
          if (fileExt.Equals("pdf", StringComparison.OrdinalIgnoreCase))
          {
            imageCount = PdfFileUtils.GetPageCount(byteSource);
            await dataAccess.Documents.UpdatePageImageCount(newPageId, imageCount);
          }
          else
            imageCount = 1;
          document = (DdocDocument) null;
          pathId = new int?();
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al agregar archivo via streaming al documento", (object) new
        {
          documentId = documentId,
          fileExt = fileExt,
          pageId = pageId,
          sequence = sequence,
          user = user
        });
        throw;
      }
      (string, int) valueTuple = (newPageId, imageCount);
      newPageId = (string) null;
      return valueTuple;
    }

    public async Task PopulateParents(string collectionId, List<DdocField> childFields)
    {
      IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>();
      IEnumerable<DdocCollection> parentCollections;
      try
      {
        parentCollections = await dataAccess.Collections.GetParentCollections(collectionId);
      }
      finally
      {
        dataAccess?.Dispose();
      }
      dataAccess = (IDdocDAL) null;
      if (!parentCollections.Any<DdocCollection>())
        ;
      else
      {
        foreach (DdocCollection ddocCollection in parentCollections)
        {
          DdocCollection parentCollection = ddocCollection;
          this.Logger.LogDebug("ParentCollection: " + parentCollection.Id + " => ChildCollection: " + collectionId);
          if (parentCollection.Type == CollectionType.F)
          {
            DdocFolder ddocFolder = new DdocFolder();
            ddocFolder.CollectionId = parentCollection.Id;
            ddocFolder.IsNew = true;
            ddocFolder.SecurityGroupId = parentCollection.SecurityGroupId;
            ddocFolder.Name = parentCollection.Name;
            DdocFolder folder = ddocFolder;
            dataAccess = IoCContainer.GetService<IDdocDAL>();
            try
            {
              IEnumerable<DdocField> parentFolderFields = (await dataAccess.Collections.GetCollectionFields(parentCollection.Id)).Where<DdocField>((Func<DdocField, bool>) (f => f.Inheritable));
              List<DdocRule> parentRules = (await dataAccess.Collections.GetRulesForParentCollection(parentCollection.Id)).Where<DdocRule>((Func<DdocRule, bool>) (r => r.ChildId == collectionId)).ToList<DdocRule>();
              List<DdocField> list1 = parentFolderFields.Where<DdocField>((Func<DdocField, bool>) (f => parentRules.Select<DdocRule, int?>((Func<DdocRule, int?>) (r => r.ParentField)).Contains<int?>(new int?(f.Id)))).ToList<DdocField>();
              this.Logger.LogDebug(string.Format("ParentFolderFields: {0} - ParentRules: {1} - Effective Fields: {2} ", (object) parentFolderFields.Count<DdocField>(), (object) parentRules.Count<DdocRule>(), (object) list1.Count));
              foreach (DdocRule ddocRule in parentRules)
              {
                DdocRule rule = ddocRule;
                this.Logger.LogDebug(string.Format("ParentField: {0} => ChildField: {1}", (object) rule.ParentField, (object) rule.ChildField));
                list1.Single<DdocField>((Func<DdocField, bool>) (f => f.Id.Equals((object) rule.ParentField))).Value = childFields.Single<DdocField>((Func<DdocField, bool>) (f => f.Id.Equals((object) rule.ChildField))).Value;
              }
              folder.Data = list1;
              List<DdocField> list2 = childFields.Join<DdocField, DdocField, string, DdocField>((IEnumerable<DdocField>) parentFolderFields.Except<DdocField>((IEnumerable<DdocField>) list1).ToList<DdocField>(), (Func<DdocField, string>) (field => field.Name), (Func<DdocField, string>) (field => field.Name), (Func<DdocField, DdocField, DdocField>) ((documentField, folderField) =>
              {
                folderField.Value = documentField.Value;
                return folderField;
              })).ToList<DdocField>();
              list2.RemoveAll((Predicate<DdocField>) (f => f.Value == null));
              folder.Data.AddRange((IEnumerable<DdocField>) list2);
              folder.DataKey = string.Join("/", folder.Data.Select<DdocField, string>((Func<DdocField, string>) (f => f.Value)));
              if (!await dataAccess.Folders.FolderExists(folder))
              {
                this.Logger.LogDebug("Creando folder");
                string folderId = await dataAccess.Folders.CreateFolder(folder);
                await dataAccess.Folders.CreateFolderData(folderId, folder.CollectionId, (IEnumerable<DdocField>) folder.Data);
                this.Logger.LogInformation("Creado folder " + folderId);
                folderId = (string) null;
              }
              else
                this.Logger.LogDebug("Folder ya existe");
              parentFolderFields = (IEnumerable<DdocField>) null;
            }
            finally
            {
              dataAccess?.Dispose();
            }
            dataAccess = (IDdocDAL) null;
            await this.PopulateParents(parentCollection.Id, folder.Data);
            folder = (DdocFolder) null;
          }
          parentCollection = (DdocCollection) null;
        }
      }
    }

    public async Task<string> RegisterNewDocument(string collectionId, int securityGroupId)
    {
      string str = (string) null;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          str = await dataAccess.Documents.RegisterNewDocument(collectionId, securityGroupId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al registrar nuevo documento");
        throw;
      }
      return str;
    }

    public async Task CommitDocument(
      string documentId,
      string name,
      List<DdocField> documentData,
      bool updateDate = true,
      DateTime? creationDate = null)
    {
      try
      {
        IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>();
        DdocDocument document;
        try
        {
          document = await dataAccess.Documents.GetDocument(documentId);
        }
        finally
        {
          dataAccess?.Dispose();
        }
        dataAccess = (IDdocDAL) null;
        await this.PopulateParents(document.CollectionId, documentData);
        string datakey = string.Join("/", documentData.Select<DdocField, string>((Func<DdocField, string>) (f => f.Value)));
        dataAccess = IoCContainer.GetService<IDdocDAL>();
        try
        {
          await dataAccess.Documents.CommitDocument(documentId, name, datakey);
          if (updateDate)
            await dataAccess.Documents.UpdateCreationDate(documentId, creationDate.HasValue ? creationDate.Value : DateTime.Now);
          await dataAccess.Documents.CreateDocumentData(documentId, document.CollectionId, (IEnumerable<DdocField>) documentData);
        }
        finally
        {
          dataAccess?.Dispose();
        }
        dataAccess = (IDdocDAL) null;
        document = (DdocDocument) null;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar documento", (object) new
        {
          documentId = documentId,
          name = name,
          documentData = documentData,
          updateDate = updateDate,
          creationDate = creationDate
        });
        throw;
      }
    }

    public async Task<string> SaveDocument(DdocDocument document, bool updateDate = true)
    {
      string documentId = (string) null;
      object obj;
      IDdocDAL dataAccess;
      int num1;
      try
      {
        await this.PopulateParents(document.CollectionId, document.Data);
        if (string.IsNullOrEmpty(document.DataKey))
          document.DataKey = string.Join("/", document.Data.Select<DdocField, string>((Func<DdocField, string>) (f => f.Value)));
        if (document.IsNew)
        {
          dataAccess = IoCContainer.GetService<IDdocDAL>();
          try
          {
            documentId = await dataAccess.Documents.CreateDocument(document);
            await dataAccess.Documents.CreateDocumentData(documentId, document.CollectionId, (IEnumerable<DdocField>) document.Data);
            await dataAccess.Documents.CommitDocument(documentId, document.Name, document.DataKey);
          }
          finally
          {
            dataAccess?.Dispose();
          }
          dataAccess = (IDdocDAL) null;
        }
        else
        {
          dataAccess = IoCContainer.GetService<IDdocDAL>();
          try
          {
            documentId = document.Id;
            await dataAccess.Documents.UpdateDocumentDataKey(document.Id, document.DataKey);
            await dataAccess.Documents.UpdateDocumentSecurity(document.Id, document.SecurityGroupId);
            await dataAccess.Documents.UpdateDocumentData(document.Id, document.CollectionId, (IEnumerable<DdocField>) document.Data, updateDate);
          }
          finally
          {
            dataAccess?.Dispose();
          }
          dataAccess = (IDdocDAL) null;
        }
        return documentId;
      }
      catch (Exception ex)
      {
        obj = (object) ex;
        num1 = 1;
      }
      if (num1 == 1)
      {
        this.Logger.LogExError((Exception) obj, "Error al guardar documento", (object) new
        {
          document = document,
          updateDate = updateDate
        });
        if (document.IsNew && !string.IsNullOrEmpty(documentId))
        {
          dataAccess = IoCContainer.GetService<IDdocDAL>();
          try
          {
            int num2 = await dataAccess.Documents.DeleteDocument(documentId, "system");
          }
          finally
          {
            dataAccess?.Dispose();
          }
          dataAccess = (IDdocDAL) null;
        }
        if (!(obj is Exception source2))
          throw new Exception("Error obj source2");
        ExceptionDispatchInfo.Capture(source2).Throw();
      }
      obj = (object) null;
      documentId = (string) null;
      //string str;
      return "";
    }

    public async Task UpdateDocumentData(
      string documentId,
      List<DdocField> documentData,
      bool updateDate = true)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Documents.UpdateDocumentData(documentId, (await dataAccess.Documents.GetDocument(documentId)).CollectionId, (IEnumerable<DdocField>) documentData, updateDate);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar datos de documento", (object) new
        {
          documentId = documentId,
          documentData = documentData,
          updateDate = updateDate
        });
        throw;
      }
    }

    public async Task UpdateDocumentSecurity(string documentId, int securityGroupId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Documents.UpdateDocumentSecurity(documentId, securityGroupId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar grupo de seguridad de documento", (object) new
        {
          documentId = documentId,
          securityGroupId = securityGroupId
        });
        throw;
      }
    }

    public async Task<int> DeleteDocument(string documentId, string user = null)
    {
      int num1;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocDocument document = await dataAccess.Documents.GetDocument(documentId);
          foreach (DdocPage page1 in await dataAccess.Documents.GetPages(documentId))
          {
            DdocPage page = page1;
            int num2 = await dataAccess.Documents.DeletePage(page.Id, user);
            if (this.InstanceFeatures.TextIndexing)
              await this.Indexing.RemoveFromIndex(document.CollectionId, documentId, page.Id);
            page = (DdocPage) null;
          }
          num1 = await dataAccess.Documents.DeleteDocument(documentId, user);
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar documento", (object) new
        {
          documentId = documentId,
          user = user
        });
        throw;
      }
      return num1;
    }

    public async Task UpdatePagesSequence(DdocDocument document)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          foreach (DdocPage page in document.Pages)
          {
            ++page.Sequence;
            await dataAccess.Documents.UpdatePageSequence(page.Id, page.Sequence);
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar páginas de documento", (object) new
        {
          document = document
        });
        throw;
      }
    }

    public async Task ChangeDocumentCollection(
      string documentId,
      CollectionChangeRequest request,
      int? securityGroupId = null)
    {
      try
      {
        List<DdocField> finalDocumentData;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          dataAccess.BeginTransaction();
          DdocDocument document = await dataAccess.Documents.GetDocument(documentId);
          if (document == null)
            throw new DdocException("El documento no existe");
          IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(document.CollectionId);
          DdocDocument ddocDocument = document;
          ddocDocument.Data = new List<DdocField>(await dataAccess.Documents.GetDocumentData(document.Id, document.CollectionId, collectionFields.ToList<DdocField>()));
          ddocDocument = (DdocDocument) null;
          finalDocumentData = new List<DdocField>(await dataAccess.Collections.GetCollectionFields(request.CollectionId));
          foreach (FieldCorrespondence fieldLink in request.FieldLinks)
          {
            FieldCorrespondence link = fieldLink;
            finalDocumentData.Single<DdocField>((Func<DdocField, bool>) (f => f.Id == link.NewFieldId)).Value = document.Data.Single<DdocField>((Func<DdocField, bool>) (f => f.Id == link.OldFieldId)).Value;
          }
          finalDocumentData.RemoveAll((Predicate<DdocField>) (f => string.IsNullOrEmpty(f.Value)));
          if (request.NewFields != null)
            finalDocumentData.AddRange((IEnumerable<DdocField>) request.NewFields);
          finalDocumentData.RemoveAll((Predicate<DdocField>) (f => string.IsNullOrEmpty(f.Value)));
          await dataAccess.Documents.CreateDocumentData(documentId, request.CollectionId, (IEnumerable<DdocField>) finalDocumentData);
          await dataAccess.Documents.UpdateDocumentCollection(documentId, request.CollectionId);
          if (securityGroupId.HasValue)
            await dataAccess.Documents.UpdateDocumentSecurity(document.Id, securityGroupId.Value);
          await dataAccess.Documents.DeleteDocumentData(documentId, document.CollectionId);
          IEnumerable<DdocPage> pages = await dataAccess.Documents.GetPages(documentId);
          Dictionary<DdocPage, byte[]> files = new Dictionary<DdocPage, byte[]>();
          foreach (DdocPage ddocPage1 in pages)
          {
            DdocPage ddocPage = ddocPage1;
            Dictionary<DdocPage, byte[]> dictionary = files;
            DdocPage key = ddocPage;
            dictionary.Add(key, await this.Storage.GetFileBytes(ddocPage.Id, ddocPage.Type));
            dictionary = (Dictionary<DdocPage, byte[]>) null;
            key = (DdocPage) null;
            await this.Storage.DeleteFile(ddocPage.Id, ddocPage.Type);
            if (this.Storage.Name.Equals("FileSystem"))
              await dataAccess.Documents.UpdatePageWarehousePathId(ddocPage.Id, await dataAccess.Documents.GetWarehousePathId(documentId));
            if (this.InstanceFeatures.TextIndexing)
              await this.Indexing.RemoveFromIndex(document.CollectionId, documentId, ddocPage.Id);
            ddocPage = (DdocPage) null;
          }
          dataAccess.CommitTransaction();
          foreach (KeyValuePair<DdocPage, byte[]> keyValuePair in files)
          {
            KeyValuePair<DdocPage, byte[]> ddocPage = keyValuePair;
            await this.Storage.SaveBytes(documentId, ddocPage.Key.Id, ddocPage.Key.Type, ddocPage.Value);
            if (this.InstanceFeatures.TextIndexing)
              await this.Indexing.AddToIndex(document.CollectionId, documentId, ddocPage.Key.Id, ddocPage.Key.Type, (Stream) new MemoryStream(ddocPage.Value));
            ddocPage = new KeyValuePair<DdocPage, byte[]>();
          }
          document = (DdocDocument) null;
          files = (Dictionary<DdocPage, byte[]>) null;
        }
        await this.PopulateParents(request.CollectionId, finalDocumentData);
        finalDocumentData = (List<DdocField>) null;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al mover documento de colección", (object) new
        {
          documentId = documentId,
          request = request,
          securityGroupId = securityGroupId
        });
        throw;
      }
    }

    public async Task<int> DeletePage(string pageId, string user = null)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocDocument document = await dataAccess.Documents.GetDocument(await dataAccess.Documents.GetDocumentId(pageId));
          int result = await dataAccess.Documents.DeletePage(pageId, user);
          if (this.InstanceFeatures.TextIndexing)
            await this.Indexing.RemoveFromIndex(document.CollectionId, document.Id, pageId);
          num = result;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar archivo", (object) new
        {
          pageId = pageId,
          user = user
        });
        throw;
      }
      return num;
    }

    public async Task<List<DdocDocument>> GetOrphanDocuments()
    {
      IEnumerable<DdocDocument> orphanDocuments;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          orphanDocuments = await dataAccess.Documents.GetOrphanDocuments(await dataAccess.Documents.GetCommitTimeout());
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar documentos sin datos");
        throw;
      }
      return orphanDocuments.ToList<DdocDocument>();
    }

    public async Task<List<DdocDocument>> GetExpiredDocuments()
    {
      IEnumerable<DdocDocument> expiredDocuments;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          expiredDocuments = await dataAccess.Documents.GetExpiredDocuments();
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar documentos sin datos");
        throw;
      }
      return expiredDocuments.ToList<DdocDocument>();
    }

    public async Task<bool> FolderIdExists(string folderId)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Folders.FolderExists(folderId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al comprobar existencia de folder", (object) new
        {
          folderId = folderId
        });
        throw;
      }
      return flag;
    }

    public async Task<bool> FolderExists(
      DdocFolder folder,
      List<int> includedFields = null,
      List<int> excludedFields = null)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Folders.FolderExists(folder, includedFields, excludedFields);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al comprobar existencia de folder por sus datos", (object) new
        {
          folder = folder,
          includedFields = includedFields,
          excludedFields = excludedFields
        });
        throw;
      }
      return flag;
    }

    public async Task<DdocFolder> GetFolder(string folderId)
    {
      DdocFolder ddocFolder1;
      try
      {
        DdocFolder folder;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          folder = await dataAccess.Folders.GetFolder(folderId);
          IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(folder.CollectionId);
          DdocFolder ddocFolder = folder;
          ddocFolder.Data = new List<DdocField>(await dataAccess.Folders.GetFolderData(folder.Id, folder.CollectionId, collectionFields.ToList<DdocField>()));
          ddocFolder = (DdocFolder) null;
        }
        ddocFolder1 = folder;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener folder", (object) new
        {
          folderId = folderId
        });
        throw;
      }
      return ddocFolder1;
    }

    public async Task<int> CountFolders(string collectionId, string securityGroupsCsv)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Folders.CountFolders(collectionId, ((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)));
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener conteo de folders de colección", (object) new
        {
          collectionId = collectionId,
          securityGroupsCsv = securityGroupsCsv
        });
        throw;
      }
      return num;
    }

    public async Task<List<DdocFolder>> GetFolders(
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0)
    {
      List<DdocFolder> ddocFolderList;
      try
      {
        List<DdocFolder> results = new List<DdocFolder>();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocFolder> collectionFolders = await dataAccess.Folders.GetFolders(collectionId, ((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)), page, pageSize, sortBy, sortDirection);
          IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(collectionId);
          foreach (DdocFolder ddocFolder1 in collectionFolders)
          {
            DdocFolder folder = ddocFolder1;
            DdocFolder ddocFolder = folder;
            ddocFolder.Data = new List<DdocField>(await dataAccess.Folders.GetFolderData(folder.Id, collectionId, collectionFields.ToList<DdocField>()));
            ddocFolder = (DdocFolder) null;
            results.Add(folder);
            folder = (DdocFolder) null;
          }
          collectionFolders = (IEnumerable<DdocFolder>) null;
          collectionFields = (IEnumerable<DdocField>) null;
        }
        ddocFolderList = results;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener folders de colección", (object) new
        {
          collectionId = collectionId,
          securityGroupsCsv = securityGroupsCsv,
          page = page,
          pageSize = pageSize,
          sortBy = sortBy,
          sortDirection = sortDirection
        });
        throw;
      }
      return ddocFolderList;
    }

    public async Task<string> SaveFolder(DdocFolder folder)
    {
      string id;
      try
      {
        folder.DataKey = string.Join("/", folder.Data.Select<DdocField, string>((Func<DdocField, string>) (f => f.Value)));
        IDdocDAL dataAccess;
        if (folder.IsNew)
        {
          dataAccess = IoCContainer.GetService<IDdocDAL>();
          try
          {
            DdocFolder ddocFolder = folder;
            ddocFolder.Id = await dataAccess.Folders.CreateFolder(folder);
            ddocFolder = (DdocFolder) null;
            await dataAccess.Folders.CreateFolderData(folder.Id, folder.CollectionId, (IEnumerable<DdocField>) folder.Data);
          }
          finally
          {
            dataAccess?.Dispose();
          }
          dataAccess = (IDdocDAL) null;
        }
        else
        {
          dataAccess = IoCContainer.GetService<IDdocDAL>();
          try
          {
            await dataAccess.Folders.UpdateFoldertDataKey(folder.Id, folder.DataKey);
            await dataAccess.Folders.UpdateFolderSecurity(folder.Id, folder.SecurityGroupId);
            await dataAccess.Folders.UpdateFolderData(folder.Id, folder.CollectionId, (IEnumerable<DdocField>) folder.Data);
          }
          finally
          {
            dataAccess?.Dispose();
          }
          dataAccess = (IDdocDAL) null;
        }
        id = folder.Id;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar folder", (object) new
        {
          folder = folder
        });
        throw;
      }
      return id;
    }

    public async Task UpdateFolderData(string folderId, List<DdocField> documentData)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Folders.UpdateFolderData(folderId, (await dataAccess.Folders.GetFolder(folderId)).CollectionId, (IEnumerable<DdocField>) documentData);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar datos de folder", (object) new
        {
          folderId = folderId,
          documentData = documentData
        });
        throw;
      }
    }

    public async Task UpdateFolderSecurity(string folderId, int securityGroupId)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          await dataAccess.Folders.UpdateFolderSecurity(folderId, securityGroupId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al actualizar grupo de seguridad de folder", (object) new
        {
          folderId = folderId,
          securityGroupId = securityGroupId
        });
        throw;
      }
    }

    public async Task<int> DeleteFolder(string folderId, string user)
    {
      int num;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          num = await dataAccess.Folders.DeleteFolder(folderId, user);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al eliminar folder", (object) new
        {
          folderId = folderId,
          user = user
        });
        throw;
      }
      return num;
    }

    public async Task<List<DdocReportRecord>> GetDailyReport(
      DateTime fromDate,
      DateTime toDate)
    {
      IEnumerable<DdocReportRecord> dailyReport;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          dailyReport = await dataAccess.Reports.GetDailyReport(fromDate, toDate);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener datos de reporte diario", (object) new
        {
          fromDate = fromDate,
          toDate = toDate
        });
        throw;
      }
      return dailyReport.ToList<DdocReportRecord>();
    }

    public async Task<List<DdocReportRecord>> GetRangeDate(
      string securityGroupsCsv,
      DateTime fromDate,
      DateTime toDate)
    {
      IEnumerable<DdocReportRecord> rangeDate;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          rangeDate = await dataAccess.Reports.GetRangeDate(((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)), fromDate, toDate);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener datos de reporte por rango de fechas", (object) new
        {
          securityGroupsCsv = securityGroupsCsv,
          fromDate = fromDate,
          toDate = toDate
        });
        throw;
      }
      return rangeDate.ToList<DdocReportRecord>();
    }

    public async Task<List<DdocReportRecord>> GetTotalReport(
      string securityGroupsCsv)
    {
      IEnumerable<DdocReportRecord> totalReport;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          totalReport = await dataAccess.Reports.GetTotalReport(((IEnumerable<string>) securityGroupsCsv.Split(',')).Select<string, int>(new Func<string, int>(int.Parse)));
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener datos de reporte de totales en d.doc", (object) new
        {
          securityGroupsCsv = securityGroupsCsv
        });
        throw;
      }
      return totalReport.ToList<DdocReportRecord>();
    }

    public async Task<(int totalRecords, List<DdocDocument> results)> SearchDocuments(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17)
    {
      List<DdocDocument> resultDocuments = new List<DdocDocument>();
      int totalRecords;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          (int num4, IEnumerable<DdocDocument> ddocDocuments4) = await dataAccess.Documents.SearchDocuments(searchParams, (await dataAccess.Collections.GetCollectionFields(searchParams.CollectionId)).ToList<DdocField>(), page, pageSize);
          totalRecords = num4;
          foreach (DdocDocument ddocDocument1 in ddocDocuments4)
          {
            DdocDocument document = ddocDocument1;
            DdocDocument fullDocument = await dataAccess.Documents.GetDocument(document.Id);
            DdocDocument ddocDocument = fullDocument;
            List<DdocPage> ddocPageList1 = new List<DdocPage>();
            List<DdocPage> ddocPageList2 = ddocPageList1;
            ddocPageList2.Add((await dataAccess.Documents.GetPages(document.Id)).FirstOrDefault<DdocPage>());
            ddocDocument.Pages = ddocPageList1;
            ddocDocument = (DdocDocument) null;
            ddocPageList2 = (List<DdocPage>) null;
            ddocPageList1 = (List<DdocPage>) null;
            fullDocument.Data = new List<DdocField>((IEnumerable<DdocField>) document.Data);
            resultDocuments.Add(fullDocument);
            fullDocument = (DdocDocument) null;
            document = (DdocDocument) null;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar documentos", (object) new
        {
          searchParams = searchParams,
          page = page,
          pageSize = pageSize
        });
        throw;
      }
      (int, List<DdocDocument>) valueTuple = (totalRecords, resultDocuments);
      resultDocuments = (List<DdocDocument>) null;
      return valueTuple;
    }

    public async Task<string> PrintDocumentSearchResults(DdocSearchParameters searchParams)
    {
      string str;
      try
      {
        List<DdocDocument> resultDocuments = new List<DdocDocument>();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          foreach (DdocDocument printSearchDocument in await dataAccess.Documents.PrintSearchDocuments(searchParams, (await dataAccess.Collections.GetCollectionFields(searchParams.CollectionId)).ToList<DdocField>()))
          {
            DdocDocument printDocument = printSearchDocument;
            DdocDocument fullDocument = await dataAccess.Documents.GetDocument(printDocument.Id);
            DdocDocument ddocDocument = fullDocument;
            List<DdocPage> ddocPageList1 = new List<DdocPage>();
            List<DdocPage> ddocPageList2 = ddocPageList1;
            ddocPageList2.Add((await dataAccess.Documents.GetPages(printDocument.Id)).FirstOrDefault<DdocPage>());
            ddocDocument.Pages = ddocPageList1;
            ddocDocument = (DdocDocument) null;
            ddocPageList2 = (List<DdocPage>) null;
            ddocPageList1 = (List<DdocPage>) null;
            fullDocument.Data = printDocument.Data;
            resultDocuments.Add(fullDocument);
            fullDocument = (DdocDocument) null;
            printDocument = (DdocDocument) null;
          }
        }
        str = this.PrintResults((IEnumerable<DdocChildEntity>) resultDocuments);
        resultDocuments = (List<DdocDocument>) null;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar documentos", (object) new
        {
          searchParams = searchParams
        });
        throw;
      }
      return str;
    }

    public async Task<string> DocumentMassDownload(DdocSearchParameters searchParams)
    {
      string str;
      try
      {
        List<DdocDocument> resultDocuments;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          resultDocuments = new List<DdocDocument>(await dataAccess.Documents.PrintSearchDocuments(searchParams, (await dataAccess.Collections.GetCollectionFields(searchParams.CollectionId)).ToList<DdocField>()));
          foreach (DdocDocument ddocDocument1 in resultDocuments)
          {
            DdocDocument ddocDocument = ddocDocument1;
            ddocDocument.Pages = new List<DdocPage>(await dataAccess.Documents.GetPages(ddocDocument1.Id));
            ddocDocument = (DdocDocument) null;
          }
        }
        str = await this.CompressResults(searchParams.CollectionId, resultDocuments);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error en descarga masiva de documentos", (object) new
        {
          searchParams = searchParams
        });
        throw;
      }
      return str;
    }

    private async Task<string> CompressResults(
      string collectionId,
      List<DdocDocument> documents)
    {
      string resultsFile = Path.Combine(Path.GetTempPath(), string.Format("{0:N}.zip", (object) Guid.NewGuid()));
      using (ZipArchive zip = new ZipArchive((Stream) new FileStream(resultsFile, FileMode.CreateNew), ZipArchiveMode.Create, false))
      {
        resultsFile = Path.GetFileNameWithoutExtension(resultsFile);
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocCollection collection = await dataAccess.Collections.GetCollection(collectionId);
          foreach (DdocDocument document1 in documents)
          {
            DdocDocument document = document1;
            string str1;
            if (collection.FileDownloadTemplate != null)
              str1 = await this.GetCustomFilename(dataAccess, document.Id, collection.Id, collection.FileDownloadTemplate);
            else
              str1 = (string) null;
            string str2 = str1;
            ZipArchiveEntry zipItem;
            if (document.Pages.Count == 1)
            {
              DdocPage page = document.Pages[0];
              zipItem = zip.CreateEntry(str2 != null ? str2 + "." + page.Type : document.Id.Substring(3, 7) + "." + page.Type, System.IO.Compression.CompressionLevel.Optimal);
              using (MemoryStream fileStream = await this.Storage.GetFileStream(page.Id, page.Type))
              {
                using (Stream destination = zipItem.Open())
                  fileStream.CopyTo(destination);
              }
              zipItem = (ZipArchiveEntry) null;
            }
            else
            {
              string folderName = str2 ?? document.Id.Substring(3, 7);
              foreach (DdocPage page in document.Pages)
              {
                zipItem = zip.CreateEntry(folderName + "/" + string.Format("Page-{0}.{1}", (object) page.Sequence, (object) page.Type), System.IO.Compression.CompressionLevel.Optimal);
                using (MemoryStream fileStream = await this.Storage.GetFileStream(page.Id, page.Type))
                {
                  using (Stream destination = zipItem.Open())
                    fileStream.CopyTo(destination);
                }
                zipItem = (ZipArchiveEntry) null;
              }
              folderName = (string) null;
            }
            document = (DdocDocument) null;
          }
          collection = (DdocCollection) null;
        }
      }
      string str = resultsFile;
      resultsFile = (string) null;
      return str;
    }

    public async Task<List<DdocDocument>> DocumentTextSearch(
      string collectionId,
      string searchText)
    {
      List<DdocDocument> ddocDocumentList;
      try
      {
        List<DdocDocument> resultDocuments = new List<DdocDocument>();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          foreach (DdocDocument ddocDocument1 in await dataAccess.Documents.DocumentTextSearch(collectionId, (await dataAccess.Collections.GetCollectionFields(collectionId)).ToList<DdocField>(), searchText))
          {
            DdocDocument document = ddocDocument1;
            DdocDocument fullDocument = await dataAccess.Documents.GetDocument(document.Id);
            DdocDocument ddocDocument = fullDocument;
            List<DdocPage> ddocPageList1 = new List<DdocPage>();
            List<DdocPage> ddocPageList2 = ddocPageList1;
            ddocPageList2.Add((await dataAccess.Documents.GetPages(document.Id)).FirstOrDefault<DdocPage>());
            ddocDocument.Pages = ddocPageList1;
            ddocDocument = (DdocDocument) null;
            ddocPageList2 = (List<DdocPage>) null;
            ddocPageList1 = (List<DdocPage>) null;
            fullDocument.Data = document.Data;
            resultDocuments.Add(fullDocument);
            fullDocument = (DdocDocument) null;
            document = (DdocDocument) null;
          }
        }
        ddocDocumentList = resultDocuments;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar documentos por texto", (object) new
        {
          collectionId = collectionId,
          searchText = searchText
        });
        throw;
      }
      return ddocDocumentList;
    }

    public async Task<(int totalRecords, List<DdocFolder> results)> SearchFolders(
      DdocSearchParameters searchParams,
      int page = 1,
      int pageSize = 17)
    {
      List<DdocFolder> resultFolders = new List<DdocFolder>();
      int totalRecords;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          (int num4, IEnumerable<DdocFolder> ddocFolders4) = await dataAccess.Folders.SearchFolders(searchParams, (await dataAccess.Collections.GetCollectionFields(searchParams.CollectionId)).ToList<DdocField>(), page, pageSize);
          totalRecords = num4;
          foreach (DdocFolder ddocFolder in ddocFolders4)
          {
            DdocFolder folder = ddocFolder;
            DdocFolder folder1 = await dataAccess.Folders.GetFolder(folder.Id);
            folder1.Data = folder.Data;
            resultFolders.Add(folder1);
            folder = (DdocFolder) null;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar folders", (object) new
        {
          searchParams = searchParams,
          page = page,
          pageSize = pageSize
        });
        throw;
      }
      (int, List<DdocFolder>) valueTuple = (totalRecords, resultFolders);
      resultFolders = (List<DdocFolder>) null;
      return valueTuple;
    }

    public async Task<string> PrintFolderSearchResults(DdocSearchParameters searchParams)
    {
      string str;
      try
      {
        List<DdocFolder> resultFolders = new List<DdocFolder>();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          foreach (DdocFolder printSearchFolder in await dataAccess.Folders.PrintSearchFolders(searchParams, (await dataAccess.Collections.GetCollectionFields(searchParams.CollectionId)).ToList<DdocField>()))
          {
            DdocFolder folder = printSearchFolder;
            DdocFolder folder1 = await dataAccess.Folders.GetFolder(folder.Id);
            folder1.Data = folder.Data;
            resultFolders.Add(folder1);
            folder = (DdocFolder) null;
          }
        }
        str = this.PrintResults((IEnumerable<DdocChildEntity>) resultFolders);
        resultFolders = (List<DdocFolder>) null;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar folders", (object) new
        {
          searchParams = searchParams
        });
        throw;
      }
      return str;
    }

    public async Task<List<DdocFolder>> FolderTextSearch(
      string collectionId,
      string searchText)
    {
      List<DdocFolder> resultFolders = new List<DdocFolder>();
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          foreach (DdocFolder ddocFolder in await dataAccess.Folders.FolderTextSearch(collectionId, (await dataAccess.Collections.GetCollectionFields(collectionId)).ToList<DdocField>(), searchText))
          {
            DdocFolder folder = ddocFolder;
            DdocFolder folder1 = await dataAccess.Folders.GetFolder(folder.Id);
            folder1.Data = folder.Data;
            resultFolders.Add(folder1);
            folder = (DdocFolder) null;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar folders por texto", (object) new
        {
          collectionId = collectionId,
          searchText = searchText
        });
        throw;
      }
      List<DdocFolder> ddocFolderList = resultFolders;
      resultFolders = (List<DdocFolder>) null;
      return ddocFolderList;
    }

    public async Task<List<DdocCollection>> NavigateFolder(string folderId)
    {
      List<DdocCollection> resultCollections = new List<DdocCollection>();
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
      {
        DdocFolder parentFolder = await dataAccess.Folders.GetFolder(folderId);
        IEnumerable<DdocField> parentFolderData = await dataAccess.Folders.GetFolderData(folderId, parentFolder.CollectionId, (await dataAccess.Collections.GetCollectionFields(parentFolder.CollectionId)).ToList<DdocField>());
        List<IGrouping<string, DdocRule>> list = (await dataAccess.Collections.GetRulesForParentCollection(parentFolder.CollectionId)).GroupBy<DdocRule, string>((Func<DdocRule, string>) (r => r.ChildId)).ToList<IGrouping<string, DdocRule>>();
        int fieldLimit = this.Settings.GetSetting<int>("FolderViewFieldLimit");
        foreach (IGrouping<string, DdocRule> grouping in list)
        {
          IGrouping<string, DdocRule> group = grouping;
          DdocCollection childCollection = await dataAccess.Collections.GetCollection(group.ToList<DdocRule>()[0].ChildId);
          childCollection.Rules = group.ToList<DdocRule>();
          DdocCollection ddocCollection = childCollection;
          ddocCollection.Fields = new List<DdocField>(await dataAccess.Collections.GetCollectionFields(childCollection.Id));
          ddocCollection = (DdocCollection) null;
          if (fieldLimit > 0)
            childCollection.Fields = childCollection.Fields.Take<DdocField>(fieldLimit).ToList<DdocField>();
          try
          {
            IEnumerable<DdocChildEntity> folderContents = await dataAccess.Folders.GetFolderContents(childCollection.Id, childCollection.Type, childCollection.Rules.Select<DdocRule, KeyValuePair<string, string>>((Func<DdocRule, KeyValuePair<string, string>>) (r => new KeyValuePair<string, string>(string.Format("C{0}", (object) r.ChildField.Value), parentFolderData.Single<DdocField>((Func<DdocField, bool>) (f =>
            {
              int id = f.Id;
              int? parentField = r.ParentField;
              int valueOrDefault = parentField.GetValueOrDefault();
              return id == valueOrDefault & parentField.HasValue;
            })).Value))).ToDictionary<string, string>(), childCollection.Fields);
            childCollection.TotalSearchResults = (long) folderContents.Count<DdocChildEntity>();
            childCollection.SearchResults = folderContents.ToList<DdocChildEntity>();
          }
          catch (Exception ex)
          {
            this.Logger.LogExError(ex, "Error al obtener contenido de folder", (object) new
            {
              folderId = folderId
            });
            throw;
          }
          if (childCollection.SearchResults.Any<DdocChildEntity>())
            resultCollections.Add(childCollection);
          childCollection = (DdocCollection) null;
          group = (IGrouping<string, DdocRule>) null;
        }
        parentFolder = (DdocFolder) null;
      }
      List<DdocCollection> ddocCollectionList = resultCollections;
      resultCollections = (List<DdocCollection>) null;
      return ddocCollectionList;
    }

    public async Task<List<DdocCollection>> GlobalSearch(
      DdocSearchParameters parameters)
    {
      List<DdocCollection> resultCollections = new List<DdocCollection>();
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          IEnumerable<DdocChildEntity> folderResults = await dataAccess.Search.GlobalSearch(parameters.TextQuery, CollectionType.F, parameters.SecurityGroupsCsv);
          List<DdocChildEntity> searchResults = folderResults.Union<DdocChildEntity>(await dataAccess.Search.GlobalSearch(parameters.TextQuery, CollectionType.D, parameters.SecurityGroupsCsv)).ToList<DdocChildEntity>();
          IEnumerable<string> strings = searchResults.Select<DdocChildEntity, string>((Func<DdocChildEntity, string>) (i => i.CollectionId)).Distinct<string>();
          int maxHits = this.Settings.GetSetting<int>("GlobalSearchMaxHits");
          int fieldLimit = this.Settings.GetSetting<int>("GlobalSearchFieldLimit");
          foreach (string str in strings)
          {
            string collectionId = str;
            DdocCollection col = await dataAccess.Collections.GetCollection(collectionId);
            DdocCollection ddocCollection = col;
            ddocCollection.Fields = (await this.GetCollectionFields(collectionId)).Take<DdocField>(fieldLimit).ToList<DdocField>();
            ddocCollection = (DdocCollection) null;
            foreach (DdocChildEntity ddocChildEntity1 in searchResults.Where<DdocChildEntity>((Func<DdocChildEntity, bool>) (i => i.CollectionId == collectionId)).Take<DdocChildEntity>(maxHits))
            {
              DdocChildEntity result = ddocChildEntity1;
              DdocChildEntity item = (DdocChildEntity) null;
              DdocChildEntity ddocChildEntity;
              switch (col.Type)
              {
                case CollectionType.F:
                  item = (DdocChildEntity) await dataAccess.Folders.GetFolder(result.Id);
                  ddocChildEntity = item;
                  ddocChildEntity.Data = new List<DdocField>(await dataAccess.Folders.GetFolderData(result.Id, col.Id, col.Fields));
                  ddocChildEntity = (DdocChildEntity) null;
                  break;
                case CollectionType.D:
                  item = (DdocChildEntity) await dataAccess.Documents.GetDocument(result.Id);
                  ddocChildEntity = item;
                  ddocChildEntity.Data = new List<DdocField>(await dataAccess.Documents.GetDocumentData(result.Id, col.Id, col.Fields));
                  ddocChildEntity = (DdocChildEntity) null;
                  break;
              }
              if (fieldLimit > 0)
                item.Data = item.Data.Take<DdocField>(fieldLimit).ToList<DdocField>();
              col.SearchResults.Add(item);
              item = (DdocChildEntity) null;
              result = (DdocChildEntity) null;
            }
            col.TotalSearchResults = (long) col.SearchResults.Count;
            resultCollections.Add(col);
            col = (DdocCollection) null;
          }
          folderResults = (IEnumerable<DdocChildEntity>) null;
          searchResults = (List<DdocChildEntity>) null;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al realizar búsqueda global", (object) new
        {
          parameters = parameters
        });
        throw;
      }
      List<DdocCollection> ddocCollectionList = resultCollections;
      resultCollections = (List<DdocCollection>) null;
      return ddocCollectionList;
    }

    public async Task<List<DdocChildEntity>> GlobalSearch2(
      string textQuery,
      CollectionType collectionType,
      string securityGroupsCsv)
    {
      IEnumerable<DdocChildEntity> searchResults;
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
      {
        searchResults = await dataAccess.Search.GlobalSearch2(textQuery, collectionType, securityGroupsCsv, this.Settings.GetSetting<int>("GlobalSearchMaxHits"));
        Dictionary<string, List<DdocField>> collectionsFields = new Dictionary<string, List<DdocField>>();
        foreach (DdocChildEntity ddocChildEntity1 in searchResults)
        {
          DdocChildEntity result = ddocChildEntity1;
          if (!collectionsFields.ContainsKey(result.CollectionId))
          {
            Dictionary<string, List<DdocField>> dictionary = collectionsFields;
            string key = result.CollectionId;
            dictionary.Add(key, await this.GetCollectionFields(result.CollectionId));
            dictionary = (Dictionary<string, List<DdocField>>) null;
            key = (string) null;
          }
          List<DdocField> fields = new List<DdocField>((IEnumerable<DdocField>) collectionsFields[result.CollectionId]);
          DdocChildEntity ddocChildEntity;
          switch (collectionType)
          {
            case CollectionType.F:
              ddocChildEntity = result;
              ddocChildEntity.Data = new List<DdocField>(await dataAccess.Folders.GetFolderData(result.Id, result.CollectionId, fields));
              ddocChildEntity = (DdocChildEntity) null;
              break;
            case CollectionType.D:
              ddocChildEntity = result;
              ddocChildEntity.Data = new List<DdocField>(await dataAccess.Documents.GetDocumentData(result.Id, result.CollectionId, fields));
              ddocChildEntity = (DdocChildEntity) null;
              break;
          }
          result = (DdocChildEntity) null;
        }
        collectionsFields = (Dictionary<string, List<DdocField>>) null;
      }
      List<DdocChildEntity> list = searchResults.ToList<DdocChildEntity>();
      searchResults = (IEnumerable<DdocChildEntity>) null;
      return list;
    }

    public async Task<(long totalRecords, List<DdocChildEntity> results)> AllFieldsSearch(
      string searchText,
      string collectionId,
      string securityGroupsCsv,
      int page = 1,
      int pageSize = 17,
      string sortBy = null,
      int sortDirection = 0)
    {
      long num;
      IEnumerable<DdocChildEntity> source;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocCollection collection = await dataAccess.Collections.GetCollection(collectionId);
          (num, source) = await dataAccess.Search.AllFieldsSearch(searchText, collection.Id, collection.Type, (await dataAccess.Collections.GetCollectionFields(collectionId)).ToList<DdocField>(), securityGroupsCsv, page, pageSize, sortBy, sortDirection);
          collection = (DdocCollection) null;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al realizar búsqueda por todos los campos", (object) new
        {
          searchText = searchText,
          collectionId = collectionId,
          securityGroupsCsv = securityGroupsCsv,
          page = page,
          pageSize = pageSize,
          sortBy = sortBy,
          sortDirection = sortDirection
        });
        throw;
      }
      return (num, source.ToList<DdocChildEntity>());
    }

    public async Task<(int totalItems, List<DdocChildEntity> results)> TextSearch(
      DdocSearchParameters parameters)
    {
      (int, List<DdocChildEntity>) valueTuple;
      try
      {
        HashSet<string> includedCollections = new HashSet<string>();
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          if (string.IsNullOrEmpty(parameters.CollectionId))
          {
            foreach (DdocEntity searchableCollection in await dataAccess.Collections.GetSearchableCollections(parameters.SecurityGroupsCsv, "D"))
              includedCollections.Add(searchableCollection.Id);
          }
          else
            includedCollections.Add((await dataAccess.Collections.GetCollection(parameters.CollectionId)).Id);
        }
        parameters.IncludedCollections = includedCollections.ToList<string>();
        List<DdocChildEntity> source = await this.BuildResultItems(this.Indexing.SearchIndex(parameters).GetAwaiter().GetResult(), parameters.TextQuery, parameters.SortResultsBy);
        source.RemoveAll((Predicate<DdocChildEntity>) (doc => !((IEnumerable<string>) parameters.SecurityGroupsCsv.Split(',')).Contains<string>(doc.SecurityGroupId.ToString())));
        if (!string.IsNullOrEmpty(parameters.CollectionId))
          source.RemoveAll((Predicate<DdocChildEntity>) (doc => doc.CollectionId != parameters.CollectionId));
        if (!string.IsNullOrEmpty(parameters.SortResultsBy))
        {
          switch (parameters.SortDirection)
          {
            case -1:
              source = source.OrderByDescending<DdocChildEntity, string>((Func<DdocChildEntity, string>) (item => item["OrderKey"].Value)).ToList<DdocChildEntity>();
              break;
            case 1:
              source = source.OrderBy<DdocChildEntity, string>((Func<DdocChildEntity, string>) (item => item["OrderKey"].Value)).ToList<DdocChildEntity>();
              break;
          }
        }
        valueTuple = (source.Count, source);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error en búsqueda por contenido", (object) new
        {
          parameters = parameters
        });
        throw;
      }
      return valueTuple;
    }

    private async Task<List<DdocChildEntity>> BuildResultItems(
      List<DDocTextSearchResult> searchResults,
      string query,
      string sortBy = null)
    {
      List<DdocChildEntity> items = new List<DdocChildEntity>();
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
      {
        foreach (DDocTextSearchResult searchResult in searchResults)
        {
          DDocTextSearchResult result = searchResult;
          string documentId = await dataAccess.Documents.GetDocumentId(result.PageId);
          if (documentId != null)
          {
            Decimal? hitPage = new Decimal?();
            if (Regex.IsMatch(result.Synopsis, "\\[\\-([\\d\\,]+)\\-\\]"))
              hitPage = new Decimal?(Decimal.Parse(Regex.Match(result.Synopsis, "\\[\\-([\\d\\,]+)\\-\\]").Groups[1].Captures[0].ToString()));
            string collectionId = await dataAccess.Documents.GetCollectionId(documentId);
            IEnumerable<DdocField> collectionFields = await dataAccess.Collections.GetCollectionFields(collectionId);
            DdocDocument resultingDocument = await dataAccess.Documents.GetDocument(documentId);
            resultingDocument.Id = resultingDocument.Id + "?search=" + query.Trim() + (hitPage.HasValue ? string.Format("&pn={0}", (object) hitPage) : string.Empty);
            IEnumerable<DdocField> documentData = await dataAccess.Documents.GetDocumentData(documentId, collectionId, collectionFields.ToList<DdocField>());
            resultingDocument.Data = new List<DdocField>()
            {
              new DdocField()
              {
                Name = "Datos",
                TypeString = "VARCHAR",
                Value = "<table>" + string.Join(string.Empty, documentData.Select<DdocField, string>((Func<DdocField, string>) (field => "<tr><td style=\"text-align:right;\"><b>" + field.Name + "<b></td><td>" + field.Value + "</td></tr>"))) + "</table>"
              },
              new DdocField()
              {
                Name = "Sinopsis",
                TypeString = "VARCHAR",
                Value = result.Synopsis.Replace("'", "")
              },
              new DdocField()
              {
                Name = "OrderKey",
                TypeString = "VARCHAR",
                Value = documentData.SingleOrDefault<DdocField>((Func<DdocField, bool>) (f => string.Format("C{0}", (object) f.Id) == sortBy))?.Value ?? documentId
              }
            };
            DdocDocument ddocDocument = resultingDocument;
            List<DdocPage> ddocPageList1 = new List<DdocPage>();
            List<DdocPage> ddocPageList2 = ddocPageList1;
            ddocPageList2.Add((await dataAccess.Documents.GetPages(resultingDocument.Id)).FirstOrDefault<DdocPage>());
            ddocDocument.Pages = ddocPageList1;
            ddocDocument = (DdocDocument) null;
            ddocPageList2 = (List<DdocPage>) null;
            ddocPageList1 = (List<DdocPage>) null;
            items.Add((DdocChildEntity) resultingDocument);
            documentId = (string) null;
            hitPage = new Decimal?();
            collectionId = (string) null;
            collectionFields = (IEnumerable<DdocField>) null;
            resultingDocument = (DdocDocument) null;
            result = (DDocTextSearchResult) null;
          }
        }
      }
      List<DdocChildEntity> ddocChildEntityList = items;
      items = (List<DdocChildEntity>) null;
      return ddocChildEntityList;
    }

    public async Task<List<string>> SearchFieldValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string query)
    {
      IEnumerable<string> source;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          source = await dataAccess.Search.SearchFieldValues(collectionId, fieldId, op, query);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar valor para campo", (object) new
        {
          collectionId = collectionId,
          fieldId = fieldId,
          op = op,
          query = query
        });
        throw;
      }
      return source.ToList<string>();
    }

    public async Task<List<Dictionary<string, object>>> SearchCollectionValues(
      string collectionId,
      int fieldId,
      Comparison op,
      string query)
    {
      IEnumerable<object> source;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          source = await dataAccess.Search.SearchCollectionValues(collectionId, fieldId, op, query);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar valores por campo", (object) new
        {
          collectionId = collectionId,
          fieldId = fieldId,
          op = op,
          query = query
        });
        throw;
      }
      return source.Cast<IDictionary<string, object>>().Select<IDictionary<string, object>, Dictionary<string, object>>((Func<IDictionary<string, object>, Dictionary<string, object>>) (d => new Dictionary<string, object>(d))).ToList<Dictionary<string, object>>();
    }

    public async Task<string> SaveSearchFilters(DdocSearchParameters searchParameters)
    {
      string withoutExtension;
      try
      {
        string str = Path.Combine(Path.GetTempPath(), string.Format("{0:N}.dfd", (object) Guid.NewGuid()));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          ConformanceLevel = ConformanceLevel.Auto,
          Indent = true
        };
        using (XmlWriter xmlWriter = XmlWriter.Create(str, settings))
          new XmlSerializer(typeof (DdocSearchParameters), string.Empty).Serialize(xmlWriter, (object) searchParameters, namespaces);
        withoutExtension = Path.GetFileNameWithoutExtension(str);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al guardar filtros de búsqeuda avanzada", (object) new
        {
          searchParameters = searchParameters
        });
        throw;
      }
      return withoutExtension;
    }

    private string PrintResults(IEnumerable<DdocChildEntity> resultItems)
    {
      List<DdocChildEntity> list = resultItems.ToList<DdocChildEntity>();
      string str = Path.Combine(Path.GetTempPath(), string.Format("{0:N}.xlsx", (object) Guid.NewGuid()));
      FileInfo file = new FileInfo(str);
      string withoutExtension = Path.GetFileNameWithoutExtension(str);
      using (ExcelPackage excelPackage = new ExcelPackage((Stream) new MemoryStream()))
      {
        ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Reporte Resultados de " + (list[0] is DdocDocument ? "Documentos" : "Folders") + " Ddoc.");
        List<DdocField> data1 = list[0].Data;
        int index1 = 0;
        int Col1 = 1;
        while (index1 < data1.Count)
        {
          if (!data1[index1].Hidden)
          {
            excelWorksheet.Cells[1, Col1].Value = (object) data1[index1].Name;
            excelWorksheet.Cells[1, Col1].Style.Font.Color.SetColor(Color.White);
            excelWorksheet.Cells[1, Col1].Style.Font.SetFromFont(new Font("Calibri", 11f));
            excelWorksheet.Cells[1, Col1].Style.Font.Bold = true;
            excelWorksheet.Cells[1, Col1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            excelWorksheet.Cells[1, Col1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
          }
          else
            --Col1;
          ++index1;
          ++Col1;
        }
        int index2 = 0;
        int Row = 2;
        while (index2 < list.Count)
        {
          List<DdocField> data2 = list[index2].Data;
          int index3 = 0;
          int Col2 = 1;
          while (index3 < data2.Count)
          {
            if (!data2[index3].Hidden)
            {
              if (data2[index3].Type == FieldType.Boolean && !string.IsNullOrEmpty(data2[index3].Value))
                excelWorksheet.Cells[Row, Col2].Value = data2[index3].Value.Equals("False") ? (object) "NO" : (object) "SI";
              else if (data2[index3].Type == FieldType.Money)
              {
                excelWorksheet.Cells[Row, Col2].Style.Numberformat.Format = "$ #,##0.00";
                excelWorksheet.Cells[Row, Col2].Value = (object) Decimal.Parse(data2[index3].Value);
              }
              else
                excelWorksheet.Cells[Row, Col2].Value = (object) data2[index3].Value;
              excelWorksheet.Cells[Row, Col2].AutoFitColumns();
            }
            else
              --Col2;
            ++index3;
            ++Col2;
          }
          ++index2;
          ++Row;
        }
        excelPackage.SaveAs(file);
      }
      return withoutExtension;
    }

    public async Task<bool> UserExists(string username)
    {
      bool flag;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          flag = await dataAccess.Security.UserExists(username);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al buscar usuario", (object) new
        {
          username = username
        });
        throw;
      }
      return flag;
    }

    public async Task<UserSession> Login(User userLogin)
    {
      UserSession userSession = new UserSession();
      User user;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          user = await this.Authentication.AuthenticateUser(userLogin);
          if (!user.Profile.Any<DdocGroup>())
            throw new DdocException("El usuario no pertenece a ningún grupo");
          string csvUserProfile = string.Join<int>(",", user.Profile.Select<DdocGroup, int>((Func<DdocGroup, int>) (g => g.Id)));
          Permissions ddocToken = await dataAccess.Security.GetUserGroupPermissions(user.Profile);
          if (!userLogin.Username.Equals("Admin", StringComparison.OrdinalIgnoreCase) && ddocToken == Permissions.None)
            throw new DdocException("El usuario no tiene permisos asignados");
          string groupsByUserGroups = await dataAccess.Security.GetSecurityGroupsByUserGroups(csvUserProfile);
          userSession.Username = user.Username;
          userSession.Name = user.Name;
          userSession.Email = user.Email;
          userSession.DdocToken = ddocToken;
          userSession.LoginOk = true;
          userSession.ServerId = 1;
          userSession.UserGroups = csvUserProfile;
          userSession.DdocGroups = groupsByUserGroups;
          userSession.Roles = new List<string>() { "User" };
          if (user.Profile.Select<DdocGroup, string>((Func<DdocGroup, string>) (group => group.Name.Trim())).Contains<string>("ddocAdmin"))
            userSession.Roles.Add("Admin");
          csvUserProfile = (string) null;
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al iniciar sesión", (object) new
        {
          userLogin = userLogin
        });
        userSession.LoginError = new LoginError()
        {
          ErrorMessage = ex.Message
        };
      }
      UserSession userSession1 = userSession;
      userSession = (UserSession) null;
      user = (User) null;
      return userSession1;
    }

    public async Task<DdocPermission> GetPermission(int permissionId)
    {
      DdocPermission permission;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          permission = await dataAccess.Security.GetPermission(permissionId);
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener permiso", (object) new
        {
          permissionId = permissionId
        });
        throw;
      }
      return permission;
    }

    public async Task<Permissions> GetElementPermissions(
      CollectionType type,
      string itemId,
      GroupFilters filters)
    {
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          switch (type)
          {
            case CollectionType.R:
              return Permissions.All;
            case CollectionType.C:
              return string.IsNullOrEmpty(filters.DdocGroupList) ? (await dataAccess.Security.GetCollectionEffectivePermissions(itemId, filters.UserGroupList)).AsToken() : (await dataAccess.Security.GetCollectionEffectivePermissions(itemId, filters.UserGroupList, filters.DdocGroupList)).AsToken();
            case CollectionType.F:
              return string.IsNullOrEmpty(filters.DdocGroupList) ? (await dataAccess.Security.GetFolderEffectivePermissions(itemId, filters.UserGroupList)).AsToken() : (await dataAccess.Security.GetFolderEffectivePermissions(itemId, filters.UserGroupList, filters.DdocGroupList)).AsToken();
            case CollectionType.D:
              return string.IsNullOrEmpty(filters.DdocGroupList) ? (await dataAccess.Security.GetDocumentEffectivePermissions(itemId, filters.UserGroupList)).AsToken() : (await dataAccess.Security.GetDocumentEffectivePermissions(itemId, filters.UserGroupList, filters.DdocGroupList)).AsToken();
            default:
              return Permissions.None;
          }
        }
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener permisos de objeto", (object) new
        {
          itemId = itemId
        });
        throw;
      }
    }

    public async Task<List<DdocGroup>> GetSecurityGroups(string securityGroupsCsv)
    {
      IEnumerable<DdocGroup> securityGroups;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          securityGroups = await dataAccess.Security.GetSecurityGroups(securityGroupsCsv);
        foreach (DdocGroup ddocGroup in securityGroups)
          ddocGroup.Type = GroupType.SecurityGroup;
      }
      catch (Exception ex)
      {
        this.Logger.LogExError(ex, "Error al obtener grupos de seguridad", (object) new
        {
          securityGroupsCsv = securityGroupsCsv
        });
        throw;
      }
      return securityGroups.ToList<DdocGroup>();
    }

    public async Task<Stream> GetDocumentStream(string pageId, int pdfPage = 0)
    {
      Stream documentStream = (Stream) null;
      try
      {
        string lowerInvariant;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          lowerInvariant = (await dataAccess.Viewer.GetPageType(pageId)).Trim().ToLowerInvariant();
        if (!(lowerInvariant == "tif") && !(lowerInvariant == "bmp") && (!(lowerInvariant == "jpg") && !(lowerInvariant == "jpeg")) && !(lowerInvariant == "png"))
        {
          if (lowerInvariant == "pdf")
          {
            if (pdfPage == 0)
              documentStream = (Stream) await this.Storage.GetFileStream(pageId, "pdf");
            else
              documentStream = await this.GetPdfPage(pageId, pdfPage);
          }
        }
        else
          documentStream = await this.GetImageAsPdf(pageId, lowerInvariant);
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al obtener documento");
        throw;
      }
      Stream stream = documentStream;
      documentStream = (Stream) null;
      return stream;
    }

    public async Task<Stream> GetFile(string pageId, string pageType = "pdf") => (Stream) await this.Storage.GetFileStream(pageId, pageType);

    public async Task<Stream> GetPageThumbnail(string pageId, int width = 0, int height = 0)
    {
      try
      {
        Stream imageStream = (Stream) null;
        string lowerInvariant;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
          lowerInvariant = (await dataAccess.Viewer.GetPageType(pageId)).Trim().ToLowerInvariant();
        switch (lowerInvariant)
        {
          case "bmp":
          case "jpeg":
          case "jpg":
          case "png":
          case "tif":
            imageStream = await this.GetImageThumbnail(pageId, lowerInvariant, width, height);
            break;
          case "pdf":
            imageStream = await this.GetPdfPageThumbnail(pageId, width, height);
            break;
          case "xml":
            imageStream = await this.GetXmlThumbnail();
            break;
        }
        return imageStream;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al obtener thumbnail de pagina");
      }
      return (Stream) new MemoryStream(new byte[0]);
    }

    public async Task<int> GetPdfPageCount(string pageId)
    {
      int pageCount;
      try
      {
        pageCount = PdfFileUtils.GetPageCount(await this.Storage.GetFileBytes(pageId, "pdf"));
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al obtener conteo de páginas de PDF");
        throw;
      }
      return pageCount;
    }

    public async Task<string> GetPath(string pageId, string pageType = null)
    {
      string str1;
      try
      {
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          string str2 = pageType;
          if (str2 == null)
            str2 = (await dataAccess.Viewer.GetPageType(pageId)).Trim().ToLowerInvariant();
          string str3 = str2;
          string filename = Path.Combine(pageId.Substring(3).Insert(5, "\\").Insert(3, "\\").Insert(1, "\\").Substring(0, 7), pageId.Substring(3, 7) + "." + str3);
          str1 = Path.Combine(await dataAccess.Viewer.GetWarehousePath(pageId), filename);
        }
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al obtener ruta de archivo");
        throw;
      }
      return str1;
    }

    public async Task<DDocFileResponse> DocumentDownload(
      string documentId,
      string downloadFormat)
    {
      DDocFileResponse ddocFileResponse1;
      try
      {
        string filename = "document";
        DDocFileResponse ddocFile = new DDocFileResponse();
        List<DdocPage> documentPages;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          DdocCollection collection = await dataAccess.Collections.GetCollection((await dataAccess.Documents.GetDocument(documentId)).CollectionId);
          if (collection.FileDownloadTemplate != null)
            filename = await this.GetCustomFilename(dataAccess, documentId, collection.Id, collection.FileDownloadTemplate);
          documentPages = new List<DdocPage>(await dataAccess.Documents.GetPages(documentId));
        }
        ddocFile.Filename = filename + "." + downloadFormat;
        string lowerInvariant = downloadFormat.Trim().ToLowerInvariant();
        DDocFileResponse ddocFileResponse;
        if (!(lowerInvariant == "tif"))
        {
          if (!(lowerInvariant == "pdf"))
          {
            if (lowerInvariant == "zip")
            {
              ddocFile.ContentType = "application/zip";
              ddocFileResponse = ddocFile;
              ddocFileResponse.FileByteStream = await this.GetDocumentAsZip(documentPages);
              ddocFileResponse = (DDocFileResponse) null;
            }
          }
          else
          {
            ddocFile.ContentType = "application/pdf";
            ddocFileResponse = ddocFile;
            ddocFileResponse.FileByteStream = await this.GetDocumentAsPdf(documentPages);
            ddocFileResponse = (DDocFileResponse) null;
          }
        }
        else
        {
          ddocFile.ContentType = "image/tiff";
          ddocFileResponse = ddocFile;
          ddocFileResponse.FileByteStream = await this.GetDocumentAsMultiTiff(documentPages);
          ddocFileResponse = (DDocFileResponse) null;
        }
        ddocFileResponse1 = ddocFile;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al descragar documento completo");
        throw;
      }
      return ddocFileResponse1;
    }

    public async Task<DDocFileResponse> PagesDownload(
      string pageId,
      string downloadFormat,
      int startPage,
      int endPage)
    {
      DDocFileResponse ddocFileResponse;
      try
      {
        string filename = string.Format("pages-{0}-{1}", (object) startPage, (object) endPage);
        DDocFileResponse ddocFile = new DDocFileResponse();
        string lowerInvariant;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          string documentId = await dataAccess.Documents.GetDocumentId(pageId);
          DdocCollection collection = await dataAccess.Collections.GetCollection((await dataAccess.Documents.GetDocument(documentId)).CollectionId);
          if (collection.FileDownloadTemplate != null)
            filename = await this.GetCustomFilename(dataAccess, documentId, collection.Id, collection.FileDownloadTemplate);
          lowerInvariant = (await dataAccess.Viewer.GetPageType(pageId)).Trim().ToLowerInvariant();
          documentId = (string) null;
        }
        MemoryStream fileStream = await this.Storage.GetFileStream(pageId, lowerInvariant);
        ddocFile.Filename = filename + "." + downloadFormat;
        MemoryStream memoryStream1 = new MemoryStream();
        MemoryStream memoryStream2 = memoryStream1;
        int start = startPage;
        int end = endPage;
        PdfFileSplitter.ExtractPages((Stream) fileStream, (Stream) memoryStream2, start, end);
        memoryStream1.Position = 0L;
        string str = downloadFormat;
        if (!(str == "tif"))
        {
          if (str == "pdf")
          {
            ddocFile.FileByteStream = (Stream) memoryStream1;
            ddocFile.ContentType = "application/pdf";
          }
        }
        else
        {
          ddocFile.FileByteStream = (Stream) new MemoryStream(ImageExtractor.ConvertPdfToTiff(memoryStream1.GetBuffer(), 300f, RenderType.Rgb, false, false, 1500, (string) null));
          ddocFile.ContentType = "image/tiff";
        }
        ddocFileResponse = ddocFile;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al extraer páginas de documento");
        throw;
      }
      return ddocFileResponse;
    }

    public async Task<DDocFileResponse> FileDownload(
      string pageId,
      string downloadFormat)
    {
      DDocFileResponse ddocFileResponse1;
      try
      {
        string filename = "page";
        DDocFileResponse ddocFile = new DDocFileResponse();
        string originalFileType;
        using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        {
          string documentId = await dataAccess.Documents.GetDocumentId(pageId);
          DdocCollection collection = await dataAccess.Collections.GetCollection((await dataAccess.Documents.GetDocument(documentId)).CollectionId);
          if (collection.FileDownloadTemplate != null)
            filename = await this.GetCustomFilename(dataAccess, documentId, collection.Id, collection.FileDownloadTemplate);
          originalFileType = (await dataAccess.Viewer.GetPageType(pageId)).Trim().ToLowerInvariant();
          documentId = (string) null;
        }
        MemoryStream fileStream = await this.Storage.GetFileStream(pageId, originalFileType);
        ddocFile.Filename = filename + "." + downloadFormat;
        switch (downloadFormat)
        {
          case "bmp":
            ddocFile.FileByteStream = (Stream) fileStream;
            ddocFile.ContentType = "image/bmp";
            break;
          case "jpeg":
          case "jpg":
            ddocFile.FileByteStream = (Stream) fileStream;
            ddocFile.ContentType = "image/jpg";
            break;
          case "pdf":
            DDocFileResponse ddocFileResponse = ddocFile;
            Stream stream;
            if (originalFileType == "pdf")
              stream = (Stream) fileStream;
            else
              stream = await this.GetImageAsPdf(pageId, originalFileType);
            ddocFileResponse.FileByteStream = stream;
            ddocFileResponse = (DDocFileResponse) null;
            ddocFile.ContentType = "application/pdf";
            break;
          case "png":
            ddocFile.FileByteStream = (Stream) fileStream;
            ddocFile.ContentType = "image/png";
            break;
          case "tif":
            ddocFile.FileByteStream = originalFileType == "tif" ? (Stream) fileStream : (Stream) new MemoryStream(ImageExtractor.ConvertPdfToTiff(fileStream.GetBuffer(), 300f, RenderType.Rgb, false, false, 1500, (string) null));
            ddocFile.ContentType = "image/tiff";
            break;
          case "xml":
            ddocFile.FileByteStream = (Stream) fileStream;
            ddocFile.ContentType = "application/xml";
            break;
        }
        ddocFileResponse1 = ddocFile;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al descargar archivo");
        throw;
      }
      return ddocFileResponse1;
    }

    private async Task<string> GetCustomFilename(
      IDdocDAL dataAccess,
      string documentId,
      string collectionId,
      string template)
    {
      string str;
      try
      {
        IEnumerable<DdocField> documentData = await dataAccess.Documents.GetDocumentData(documentId, collectionId, (await dataAccess.Collections.GetCollectionFields(collectionId)).ToList<DdocField>());
        string input = template;
        foreach (Match match in Regex.Matches(input, "{\\d+}").Cast<Match>().ToList<Match>())
        {
          int fieldId = int.Parse(match.Value.Trim('{', '}'));
          input = input.Replace(match.Value, documentData.Single<DdocField>((Func<DdocField, bool>) (d => d.Id == fieldId)).Value);
        }
        str = input;
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "ErrorId", (object) Guid.NewGuid().ToString("D"));
        this.Logger.LogError(ex, "Error al obtener nombre personalizado de documento");
        throw;
      }
      return str;
    }

    public async Task<Stream> GetImageAsPdf(string pageId, string fileExt)
    {
      Stream pdfAsStream;
      using (MemoryStream fileStream = await this.Storage.GetFileStream(pageId, fileExt))
        pdfAsStream = (Stream) new PdfImageConverter((Stream) fileStream).GetPdfAsStream(fileExt.Equals("tif", StringComparison.OrdinalIgnoreCase) || fileExt.Equals("tiff", StringComparison.OrdinalIgnoreCase));
      return pdfAsStream;
    }

    public async Task<Stream> GetPdfPage(string pageId, int pageNumber)
    {
      MemoryStream fileStream = await this.Storage.GetFileStream(pageId, "pdf");
      Stream stream;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (PdfDocument toDocument = new PdfDocument(new PdfWriter((Stream) memoryStream)))
        {
          using (PdfDocument pdfDocument = new PdfDocument(new PdfReader((Stream) fileStream)))
          {
            pdfDocument.CopyPagesTo(pageNumber, pageNumber, toDocument);
            stream = (Stream) new MemoryStream(memoryStream.GetBuffer());
          }
        }
      }
      return stream;
    }

    public async Task<Stream> GetImageThumbnail(
      string pageId,
      string fileExt,
      int width = 0,
      int height = 0)
    {
      System.Drawing.Image image = System.Drawing.Image.FromStream((Stream) await this.Storage.GetFileStream(pageId, fileExt));
      if (width == 0 && height == 0)
      {
        width = image.Width;
        height = image.Height;
      }
      System.Drawing.Image thumbnailImage = image.GetThumbnailImage(width, height, (System.Drawing.Image.GetThumbnailImageAbort) (() => false), IntPtr.Zero);
      MemoryStream memoryStream1 = new MemoryStream();
      MemoryStream memoryStream2 = memoryStream1;
      ImageFormat gif = ImageFormat.Gif;
      thumbnailImage.Save((Stream) memoryStream2, gif);
      memoryStream1.Position = 0L;
      return (Stream) memoryStream1;
    }

    public async Task<Stream> GetPdfPageThumbnail(string pageId, int width = 0, int height = 0)
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      byte[] pdfPageImage = ImageExtractor.ExtractPdfPageImage((await this.Storage.GetFileStream(pageId, "pdf")).GetBuffer(), RenderType.Rgb, new int?(1), ImageFormat.Gif, width != 0 ? new int?(width) : new int?(), height != 0 ? new int?(height) : new int?());
      GC.Collect();
      GC.WaitForPendingFinalizers();
      return (Stream) new MemoryStream(pdfPageImage);
    }

    public async Task<Stream> GetXmlThumbnail() => (Stream) File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/xmlThumb.gif"));

    public async Task<Stream> GetDocumentAsMultiTiff(List<DdocPage> documentPages)
    {
      ImageCodecInfo codecInfo = (ImageCodecInfo) null;
      MemoryStream mergedFileStream = new MemoryStream();
      foreach (ImageCodecInfo imageEncoder in ImageCodecInfo.GetImageEncoders())
      {
        if (imageEncoder.MimeType == "image/tiff")
          codecInfo = imageEncoder;
      }
      System.Drawing.Imaging.Encoder enc = System.Drawing.Imaging.Encoder.SaveFlag;
      EncoderParameters ep = new EncoderParameters(1);
      Bitmap pages = (Bitmap) null;
      int frame = 0;
      foreach (DdocPage documentPage in documentPages)
      {
        DdocPage page = documentPage;
        MemoryStream fileStream = await this.Storage.GetFileStream(page.Id, page.Type);
        if (page.Type.Trim().Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
        {
          byte[] buffer = fileStream.GetBuffer();
          int pageCount = PdfFileUtils.GetPageCount(buffer);
          for (int index = 0; index < pageCount; ++index)
          {
            using (MemoryStream memoryStream = new MemoryStream(ImageExtractor.ExtractPdfPageImage(buffer, RenderType.Rgb, new int?(index + 1), ImageFormat.Jpeg)))
            {
              if (frame == 0)
              {
                pages = (Bitmap) System.Drawing.Image.FromStream((Stream) memoryStream);
                ep.Param[0] = new EncoderParameter(enc, 18L);
                pages.Save((Stream) mergedFileStream, codecInfo, ep);
              }
              else
              {
                ep.Param[0] = new EncoderParameter(enc, 23L);
                pages.SaveAdd(System.Drawing.Image.FromStream((Stream) memoryStream), ep);
              }
            }
            ++frame;
          }
        }
        else if (page.Type.Trim().Equals("tif", StringComparison.InvariantCultureIgnoreCase))
        {
          using (System.Drawing.Image image = System.Drawing.Image.FromStream((Stream) fileStream))
          {
            foreach (Guid frameDimensions in image.FrameDimensionsList)
            {
              FrameDimension dimension1 = new FrameDimension(frameDimensions);
              int frameCount = image.GetFrameCount(dimension1);
              for (int frameIndex = 0; frameIndex < frameCount; ++frameIndex)
              {
                FrameDimension dimension2 = new FrameDimension(frameDimensions);
                image.SelectActiveFrame(dimension2, frameIndex);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                  image.Save((Stream) memoryStream, ImageFormat.Tiff);
                  if (frame == 0)
                  {
                    pages = (Bitmap) System.Drawing.Image.FromStream((Stream) memoryStream);
                    ep.Param[0] = new EncoderParameter(enc, 18L);
                    pages.Save((Stream) mergedFileStream, codecInfo, ep);
                  }
                  else
                  {
                    ep.Param[0] = new EncoderParameter(enc, 23L);
                    pages.SaveAdd(System.Drawing.Image.FromStream((Stream) memoryStream), ep);
                  }
                }
                ++frame;
              }
            }
          }
        }
        else
        {
          using (System.Drawing.Image image = System.Drawing.Image.FromStream((Stream) fileStream))
          {
            if (frame == 0)
            {
              pages = (Bitmap) image;
              ep.Param[0] = new EncoderParameter(enc, 18L);
              pages.Save((Stream) mergedFileStream, codecInfo, ep);
            }
            else
            {
              ep.Param[0] = new EncoderParameter(enc, 23L);
              pages.SaveAdd(image, ep);
            }
          }
          ++frame;
        }
        page = (DdocPage) null;
      }
      if (frame > 0)
      {
        ep.Param[0] = new EncoderParameter(enc, 20L);
        pages.SaveAdd(ep);
      }
      mergedFileStream.Position = 0L;
      Stream stream = (Stream) mergedFileStream;
      codecInfo = (ImageCodecInfo) null;
      mergedFileStream = (MemoryStream) null;
      enc = (System.Drawing.Imaging.Encoder) null;
      ep = (EncoderParameters) null;
      pages = (Bitmap) null;
      return stream;
    }

    public async Task<Stream> GetDocumentAsPdf(List<DdocPage> documentPages)
    {
      if (documentPages.Count == 1 && documentPages[0].Type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        return (Stream) await this.Storage.GetFileStream(documentPages[0].Id, documentPages[0].Type);
      using (MemoryStream tempStream = new MemoryStream())
      {
        using (PdfDocument finalPdf = new PdfDocument(new PdfWriter((Stream) tempStream)))
        {
          using (Document finalPdfDocument = new Document(finalPdf))
          {
            foreach (DdocPage documentPage in documentPages)
            {
              string lower = documentPage.Type.Trim().ToLower();
              if (!(lower == "pdf"))
              {
                if (lower == "jpg" || lower == "jpeg" || (lower == "png" || lower == "tif"))
                {
                  iText.Layout.Element.Image image;
                  using (MemoryStream fileStream = await this.Storage.GetFileStream(documentPage.Id, documentPage.Type.Trim().ToLower()))
                    image = new iText.Layout.Element.Image(ImageDataFactory.Create(fileStream.GetBuffer()));
                  image.SetWidth(finalPdf.GetDefaultPageSize().GetWidth() - 50f);
                  image.SetAutoScaleHeight(true);
                  finalPdfDocument.Add(image);
                }
              }
              else
              {
                using (MemoryStream fileStream = await this.Storage.GetFileStream(documentPage.Id, "pdf"))
                {
                  using (PdfDocument pdfDocument = new PdfDocument(new PdfReader((Stream) fileStream)))
                  {
                    int numberOfPages = pdfDocument.GetNumberOfPages();
                    pdfDocument.CopyPagesTo(1, numberOfPages, finalPdf);
                  }
                }
              }
            }
            finalPdfDocument.Close();
            return (Stream) new MemoryStream(tempStream.GetBuffer());
          }
        }
      }
    }

    public async Task<Stream> GetDocumentAsZip(List<DdocPage> documentPages)
    {
      MemoryStream memStream = new MemoryStream();
      using (ZipArchive zip = new ZipArchive((Stream) memStream, ZipArchiveMode.Create, true))
      {
        int pageCount = 1;
        foreach (DdocPage documentPage in documentPages)
        {
          ZipArchiveEntry zipItem = zip.CreateEntry(string.Format("Page {0}.{1}", (object) pageCount++, (object) documentPage.Type), System.IO.Compression.CompressionLevel.Optimal);
          using (MemoryStream fileStream = await this.Storage.GetFileStream(documentPage.Id, documentPage.Type))
          {
            using (Stream destination = zipItem.Open())
              fileStream.CopyTo(destination);
          }
          zipItem = (ZipArchiveEntry) null;
        }
        memStream.Flush();
      }
      memStream.Position = 0L;
      Stream stream = (Stream) memStream;
      memStream = (MemoryStream) null;
      return stream;
    }

    public async Task<CfdiData> GetCfdData(string pageId)
    {
      MemoryStream fileStream = await this.Storage.GetFileStream(pageId, "xml");
      return CfdiUtils.GetCfdiData(new CfdiValidator(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content")).ValidateCfdi((Stream) fileStream, CfdiValidationOptions.Schema));
    }
  }
}
