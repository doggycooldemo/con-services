﻿using Microsoft.AspNetCore.Mvc;
using VSS.Raptor.Service.Common.Models;

namespace VSS.Raptor.Service.WebApiModels.Report.Contracts
{
  public interface IReportSvc
  {
    ProjectStatisticsResult PostProjectStatistics([FromBody] ProjectStatisticsRequest request);

  }
}
