// Decompiled with JetBrains decompiler

using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Extensions
{
  public interface IFolderPreProcessor
  {
    Func<List<DdocFolder>, Task<List<DdocFolder>>> PreProcessFolders { get; set; }
  }
}
