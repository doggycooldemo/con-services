﻿using System.Net;
using ProductionDataSvc.AcceptanceTests.Helpers;
using ProductionDataSvc.AcceptanceTests.Models;
using Xunit;
using Xunit.Gherkin.Quick;

namespace ProductionDataSvc.AcceptanceTests.StepDefinitions
{
  [FeatureFile("SummaryVolumes.feature")]
  public class SummaryVolumesSteps : FeaturePostRequestBase<SummaryVolumesParameters, ResponseBase>
  {
    [And(@"I require surveyed surface")]
    public async void AndIRequireSurveyedSurface()
    {
      var result = await BeforeAndAfter.CreateSurveyedSurface();

      Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
  }
}
