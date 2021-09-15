// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.Storage.FileSystemStorage
// Assembly: DigitalData.DDoc.Modules.Storage.FileSystem, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 20594B71-4B3D-42F1-BDE2-783FC29D85FB
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.Storage.FileSystem.dll

using DigitalData.Common.IoC;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Modules.Storage
{
  public class FileSystemStorage : IDdocStorage
  {
    public FileSystemStorage(ILogger<FileSystemStorage> logger) => this.Logger = logger;

    private ILogger<FileSystemStorage> Logger { get; }

    public string Name { get; } = "FileSystem";

    public async Task SaveBytes(
      string documentId,
      string fileId,
      string fileExt,
      byte[] byteSource)
    {
      string path = await FileSystemStorage.GetPath(fileId, fileExt);
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      MemoryStream memoryStream = new MemoryStream(byteSource);
      using (FileStream fileStream = File.Create(path))
      {
        memoryStream.CopyTo((Stream) fileStream);
        memoryStream.Close();
      }
    }

    public async Task SaveFile(
      string documentId,
      string fileId,
      string fileExt,
      string filePath)
    {
      string path = await FileSystemStorage.GetPath(fileId, fileExt);
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      File.Copy(filePath, path, true);
    }

    public async Task<MemoryStream> GetFileStream(string fileId, string fileExt)
    {
      string path = await FileSystemStorage.GetPath(fileId, fileExt);
      MemoryStream memoryStream = new MemoryStream();
      using (FileStream fileStream = File.OpenRead(path))
      {
        fileStream.CopyTo((Stream) memoryStream);
        memoryStream.Position = 0L;
      }
      return memoryStream;
    }

    public async Task<byte[]> GetFileBytes(string fileId, string fileExt)
    {
      byte[] buffer;
      using (FileStream fileStream = new FileStream(await FileSystemStorage.GetPath(fileId, fileExt), FileMode.Open, FileAccess.Read))
      {
        buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, Convert.ToInt32(fileStream.Length));
      }
      return buffer;
    }

    public async Task DeleteFile(string fileId, string type) => File.Delete(await FileSystemStorage.GetPath(fileId, type));

    private static async Task<string> GetPath(string pageId, string fileExt)
    {
      string level1 = pageId.Substring(3, 1);
      string level2 = pageId.Substring(4, 2);
      string level3 = pageId.Substring(6, 2);
      string filename = pageId.Substring(3, 7);
      string path1;
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        path1 = Path.Combine(await dataAccess.Viewer.GetWarehousePath(pageId), level1, level2, level3);
      string str = Path.Combine(path1, filename + "." + fileExt);
      level1 = (string) null;
      level2 = (string) null;
      level3 = (string) null;
      filename = (string) null;
      return str;
    }

    private static async Task<DdocCollection> GetCollection(string documentId)
    {
      DdocCollection collection;
      using (IDdocDAL dataAccess = IoCContainer.GetService<IDdocDAL>())
        collection = await dataAccess.Collections.GetCollection((await dataAccess.Documents.GetDocument(documentId)).CollectionId);
      return collection;
    }
  }
}
