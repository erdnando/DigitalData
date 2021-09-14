// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.ReportsController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Extensions;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class ReportsController : DdocController
  {
    public ReportsController(
      IDdocService ddoc,
      IOptions<DiDaSettings> settings,
      ICustomReports customReports)
      : base(ddoc, settings.Value)
    {
      this.CustomReports = customReports;
    }

    public ICustomReports CustomReports { get; }

    public ActionResult Main()
    {
      this.ViewData["ShowDefaultReports"] = (object) this.Settings.GetSetting<bool>("ShowDefaultReports");
      if (this.CustomReports.ReportList != null)
        this.ViewData["CustomReportList"] = (object) this.CustomReports.ReportList;
      return (ActionResult) this.View();
    }

    public ViewResult Welcome() => this.View();

        public PartialViewResult DailyReport()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Páginas",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "Documentos",
                    Value = "2"
                }
            };
            ((dynamic)base.ViewBag).CollectionPage = selectListItems;
            return base.PartialView();
        }

        [HttpPost]
    public async Task<ActionResult> GetDailyReport(
      DateTime startDate,
      DateTime endDate,
      int reportType)
    {
      ReportsController reportsController = this;
      Response<object> response = new Response<object>();
      try
      {
        List<DateTime> labels = new List<DateTime>();
        for (DateTime dateTime = startDate; dateTime.Date <= endDate; dateTime = dateTime.AddDays(1.0))
          labels.Add(dateTime.Date);
        List<DdocReportRecord> dailyReport = await reportsController.Ddoc.GetDailyReport(startDate, endDate);
        List<int> intList = new List<int>();
        foreach (DateTime dateTime in labels)
        {
          DateTime date = dateTime;
          if (dailyReport.Any<DdocReportRecord>((Func<DdocReportRecord, bool>) (r => r.Date == date)))
          {
            switch (reportType)
            {
              case 1:
                intList.Add(dailyReport.Single<DdocReportRecord>((Func<DdocReportRecord, bool>) (r => r.Date == date)).PageCount);
                continue;
              case 2:
                intList.Add(dailyReport.Single<DdocReportRecord>((Func<DdocReportRecord, bool>) (r => r.Date == date)).DocumentCount);
                continue;
              default:
                continue;
            }
          }
          else
            intList.Add(0);
        }
        response.List = new List<object>()
        {
          (object) labels,
          (object) intList
        };
        response.Result = RequestResult.Success;
        labels = (List<DateTime>) null;
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message);
      }
      ActionResult actionResult = (ActionResult) reportsController.Json((object) response);
      response = (Response<object>) null;
      return actionResult;
    }

        public PartialViewResult DateRangeReport()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Páginas",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "Documentos",
                    Value = "2"
                }
            };
            ((dynamic)base.ViewBag).CollectionPage = selectListItems;
            return base.PartialView();
        }

        [HttpPost]
    public async Task<ActionResult> GetRangeDate(
      DateTime startDate,
      DateTime endDate,
      int typeReport)
    {
      ReportsController reportsController = this;
      Response<object> response = new Response<object>();
      try
      {
        List<int> values = new List<int>();
        List<DdocReportRecord> rangeDate = await reportsController.Ddoc.GetRangeDate(reportsController.CurrentUserSession.DdocGroups, startDate, endDate);
        List<string> list = rangeDate.Select<DdocReportRecord, string>((Func<DdocReportRecord, string>) (x => x.CollectionName)).ToList<string>();
        switch (typeReport)
        {
          case 1:
            values = rangeDate.Select<DdocReportRecord, int>((Func<DdocReportRecord, int>) (y => y.PageCount)).ToList<int>();
            break;
          case 2:
            values = rangeDate.Select<DdocReportRecord, int>((Func<DdocReportRecord, int>) (y => y.DocumentCount)).ToList<int>();
            break;
        }
        response.List = new List<object>()
        {
          (object) list,
          (object) values
        };
        response.Result = RequestResult.Success;
        values = (List<int>) null;
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message);
      }
      ActionResult actionResult = (ActionResult) reportsController.Json((object) response);
      response = (Response<object>) null;
      return actionResult;
    }

        public PartialViewResult TotalReport()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Páginas",
                    Value = "1"
                },
                new SelectListItem()
                {
                    Text = "Documentos",
                    Value = "2"
                }
            };
            ((dynamic)base.ViewBag).CollectionPage = selectListItems;
            return base.PartialView();
        }

        [HttpPost]
    public async Task<ActionResult> GetTotalReport(int typeReport)
    {
      ReportsController reportsController = this;
      Response<object> response = new Response<object>();
      try
      {
        List<int> values = new List<int>();
        List<DdocReportRecord> totalReport = await reportsController.Ddoc.GetTotalReport(reportsController.CurrentUserSession.DdocGroups);
        List<string> list = totalReport.Select<DdocReportRecord, string>((Func<DdocReportRecord, string>) (x => x.CollectionName)).ToList<string>();
        switch (typeReport)
        {
          case 1:
            values = totalReport.Select<DdocReportRecord, int>((Func<DdocReportRecord, int>) (y => y.PageCount)).ToList<int>();
            break;
          case 2:
            values = totalReport.Select<DdocReportRecord, int>((Func<DdocReportRecord, int>) (y => y.DocumentCount)).ToList<int>();
            break;
        }
        response.List = new List<object>()
        {
          (object) list,
          (object) values
        };
        response.Result = RequestResult.Success;
        values = (List<int>) null;
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ex.Message);
      }
      ActionResult actionResult = (ActionResult) reportsController.Json((object) response);
      response = (Response<object>) null;
      return actionResult;
    }
  }
}
