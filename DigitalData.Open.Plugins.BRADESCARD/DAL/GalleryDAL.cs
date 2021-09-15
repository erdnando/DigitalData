// Decompiled with JetBrains decompiler
// Type: DigitalData.Ddoc.Plugins.GalleryDAL
// Assembly: DigitalData.Ddoc.Plugins.BRADESCARD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 76226255-9B51-4DC8-9719-6E02C225B847
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.Ddoc.Plugins.BRADESCARD.dll

using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Common.Entities;
using Microsoft.Extensions.Options;
using SqlKata;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DigitalData.Ddoc.Plugins
{
  public class GalleryDAL : SqlServerDAL
  {
    public GalleryDAL(IOptions<DiDaSettings> settings): base(settings.Value.GetConnectionSettings("DDocConnection"))
    {
    }

    public Task<string> GetFolderId(string expNumber) => base.db.Query("G_FOLDERS as F").WhereLike("DATAKEY", (object) (expNumber + "/%")).Select("F.GID").SingleOrDefaultAsync<string>();

    public async Task<IEnumerable<string>> GetFolderContents(
      string childCollectionId,
      IEnumerable<KeyValuePair<string, string>> filters)
    {
      Query query = base.db.Query("G_CAMPOS_" + childCollectionId + " as C").Join("G_DOCUMENTOS as D", "D.GID", "C.GID");
      foreach (KeyValuePair<string, string> filter in filters)
        query.Where(filter.Key, (object) filter.Value);
      query.Select("D.GID");
      return await query.GetAsync<string>();
    }
  }
}
