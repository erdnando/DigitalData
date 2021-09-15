// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.ReportsDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Modules.DataAccess
{
  public class ReportsDAL : SqlServerDAL, IReportsDAL, ICommonDAL
  {
    public ReportsDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

        public async Task<bool> ShowDefaultReports()
        {
            Query query = base.db.Query("G_PARAMETROS").Where("PARAMETRO_NOMBRE", "ShowDefaultReports");
            String[] strArrays = new String[] { "PARAMETRO_VALOR" };
            int? nullable = null;
            int num = await QueryExtensionsAsync.SingleAsync<int>(query.Select(strArrays), nullable);
            return num == 1;
        }

        public Task<IEnumerable<DdocReportRecord>> GetDailyReport(DateTime fromDate, DateTime toDate)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("startDate", fromDate, new DbType?(DbType.DateTime), nullable, nullable1, nullable3, nullable2);
            DateTime dateTime = toDate.AddHours(23);
            dateTime = dateTime.AddMinutes(59);
            dateTime = dateTime.AddSeconds(59);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable4 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("endDate", dateTime.AddMilliseconds(999), new DbType?(DbType.DateTime), nullable, nullable1, nullable4, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable5 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<IEnumerable<DdocReportRecord>> task = connection.QueryAsync<DdocReportRecord>("ddoc.sp_DailyReport", dynamicParameter, transaction, nullable1, nullable5);
            return task;
        }

        public Task<IEnumerable<DdocReportRecord>> GetRangeDate(IEnumerable<int> securityGroups, DateTime fromDate, DateTime toDate)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            //DbType? nullable = null;
            ParameterDirection? nullable1 = null;
            int? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
           // dynamicParameter.Add("secGroups", securityGroups.ToTableValuedParameter<int>(), new DbType?(DbType.Int64), nullable1, nullable2, nullable4, nullable3);

            nullable1 = null;
            nullable2 = null;
            nullable3 = null;
            byte? nullable5 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("startDate", fromDate, new DbType?(DbType.DateTime), nullable1, nullable2, nullable5, nullable3);

            DateTime dateTime = toDate.AddHours(23);
            dateTime = dateTime.AddMinutes(59);
            dateTime = dateTime.AddSeconds(59);
            nullable1 = null;
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("endDate", dateTime.AddMilliseconds(999), new DbType?(DbType.DateTime), nullable1, nullable2, nullable6, nullable3);

            nullable1 = null;
            nullable2 = null;
            nullable3 = null;
            byte? nullable7 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("totals", false, new DbType?(DbType.Boolean), nullable1, nullable2, nullable7, nullable3);

            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable8 = new CommandType?(CommandType.StoredProcedure);
            nullable2 = null;
           
            Task<IEnumerable<DdocReportRecord>> task = connection.QueryAsync<DdocReportRecord>("ddoc.sp_CollectionReport", dynamicParameter, transaction, nullable2, nullable8);
            return task;
        }

        public Task<IEnumerable<DdocReportRecord>> GetTotalReport(IEnumerable<int> securityGroups)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
           // DbType? nullable = null;
            ParameterDirection? nullable1 = null;
            int? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
           // dynamicParameter.Add("secGroups", securityGroups.ToTableValuedParameter<int>(), new DbType?(DbType.DateTime), nullable1, nullable2, nullable4, nullable3);
            nullable1 = null;
            nullable2 = null;
            nullable3 = null;
            byte? nullable5 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("totals", true, new DbType?(DbType.Boolean), nullable1, nullable2, nullable5, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable6 = new CommandType?(CommandType.StoredProcedure);
            nullable2 = null;
            Task<IEnumerable<DdocReportRecord>> task = connection.QueryAsync<DdocReportRecord>("ddoc.sp_CollectionReport", dynamicParameter, transaction, nullable2, nullable6);
            return task;
        }

    //    [SpecialName]
    //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

    //[SpecialName]
    //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
  }
}
