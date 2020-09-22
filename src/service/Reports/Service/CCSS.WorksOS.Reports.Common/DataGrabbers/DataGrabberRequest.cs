﻿using Microsoft.AspNetCore.Http;

namespace CCSS.WorksOS.Reports.Common.DataGrabbers
{
  public class DataGrabberRequest
  {
    public IHeaderDictionary CustomHeaders { get; set; }
    public string QueryURL { get; set; }
    public string SvcMethod { get; set; }
  }
}
