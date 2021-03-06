﻿using Newtonsoft.Json.Linq;
using ProductionDataSvc.AcceptanceTests.Models;
using Xunit.Gherkin.Quick;

namespace ProductionDataSvc.AcceptanceTests.StepDefinitions
{
  [FeatureFile("CoordinateSystemPost.feature")]
  public class CoordinateSystemPostSteps : FeaturePostRequestBase<JObject, ResponseBase>
  { }
}
