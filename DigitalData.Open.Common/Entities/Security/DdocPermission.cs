
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  public class DdocPermission
  {
    [DataMember]
    [DisplayName("Comentar: ")]
    public bool CommentPermission { get; set; }

    [DataMember]
    [DisplayName("Eliminar: ")]
    public bool DeletePermission { get; set; }

    [DataMember]
    [DisplayName("Exportar: ")]
    public bool ExportPermission { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Imprimir: ")]
    public bool PrintPermission { get; set; }

    [DataMember]
    [DisplayName("Ver: ")]
    public bool ReadPermission { get; set; }

    [DataMember]
    [DisplayName("Grupo de seguridad: ")]
    public int SecurityGroupId { get; set; }

    [DataMember]
    public string SecurityGroupName { get; set; }

    [DataMember]
    [DisplayName("Grupo de usuarios: ")]
    public int UserGroupId { get; set; }

    [DataMember]
    public string UserGroupName { get; set; }

    [DataMember]
    [DisplayName("Modificar: ")]
    public bool WritePermission { get; set; }

    public Permissions AsToken()
    {
      Permissions permissions = Permissions.None;
      if (this == null)
        return permissions;
      if (this.ReadPermission)
        permissions |= Permissions.Read;
      if (this.WritePermission)
        permissions |= Permissions.Write;
      if (this.CommentPermission)
        permissions |= Permissions.Comment;
      if (this.ExportPermission)
        permissions |= Permissions.Export;
      if (this.PrintPermission)
        permissions |= Permissions.Print;
      if (this.DeletePermission)
        permissions |= Permissions.Delete;
      return permissions;
    }
  }
}
