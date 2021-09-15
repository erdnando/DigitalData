
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum CellType
  {
    [EnumMember] Normal,
    [EnumMember] Checkbox,
    [EnumMember] Radio,
    [EnumMember] Textbox,
    [EnumMember] Combo,
    [EnumMember] Button,
    [EnumMember] Image,
    [EnumMember] Action,
    [EnumMember] Textlink,
    [EnumMember] Iconlink,
    [EnumMember] Thumbnail,
    [EnumMember] Thumbnaillink,
  }
}
