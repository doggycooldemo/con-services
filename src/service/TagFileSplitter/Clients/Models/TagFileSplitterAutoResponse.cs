﻿using System.Collections.Generic;
using VSS.MasterData.Models.ResultHandling.Abstractions;

namespace CCSS.TagFileSplitter.Models
{
  public class TagFileSplitterAutoResponse : ContractExecutionResult
  {
    public List<TargetServiceResponse> TargetServiceResponses;

    public TagFileSplitterAutoResponse()
    {
    }

    public TagFileSplitterAutoResponse(List<TargetServiceResponse> targetServiceResponses, int code = 0, string message = null)
    {
      TargetServiceResponses = targetServiceResponses;
      Code = code;
      Message = message ?? ContractExecutionResult.DefaultMessage;
    }
  }
}