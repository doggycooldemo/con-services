﻿using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionDataSvc.AcceptanceTests.Models;
using RaptorSvcAcceptTestsCommon.Utils;
using TechTalk.SpecFlow;

namespace ProductionDataSvc.AcceptanceTests.StepDefinitions
{
  [Binding, Scope(Feature = "CompactionSummaryVolumes")]
  public class CompactionSummaryVolumesSteps
  {
    private Getter<CompactionVolumesSummaryResult> volumesSummaryRequester;
    private string url;

    [Given(@"the Compaction service URI ""(.*)""")]
    public void GivenTheCompactionServiceURI(string uri)
    {
      url = RaptorClientConfig.CompactionSvcBaseUri + uri;
    }

    [Given(@"the result file ""(.*)""")]
    public void GivenTheResultFile(string resultFileName)
    {
      volumesSummaryRequester = new Getter<CompactionVolumesSummaryResult>(url, resultFileName);
    }

    [Given(@"projectUid ""(.*)""")]
    public void GivenProjectUid(string projectUid)
    {
      volumesSummaryRequester.QueryString.Add("ProjectUid", projectUid);
    }

    [When(@"I request result")]
    public void WhenIRequestResult()
    {
      volumesSummaryRequester.DoValidRequest(url);
    }

    [Then(@"the result should match the ""(.*)"" from the repository")]
    public void ThenTheResultShouldMatchTheFromTheRepository(string resultName)
    {
      Assert.AreEqual(volumesSummaryRequester.ResponseRepo[resultName], volumesSummaryRequester.CurrentResponse);
    }

    [Given(@"filterUid ""(.*)""")]
    public void GivenFilterUid(string filterUid)
    {
      if (!string.IsNullOrEmpty(filterUid))
        { volumesSummaryRequester.QueryString.Add("filterUid", filterUid);}
    }

    [Given(@"volumeCalcType ""(.*)"" and baseFilterUid ""(.*)"" and topFilterUid ""(.*)""")]
    public void GivenVolumeCalcTypeAndBaseFilterUidAndTopFilterUid(int volumeCalcType, string baseFilterUid, string topFilterUid)
    {
      volumesSummaryRequester.QueryString.Add("volumeCalcType", volumeCalcType.ToString());
      if (!string.IsNullOrEmpty(baseFilterUid))
      { volumesSummaryRequester.QueryString.Add("baseFilterUid", baseFilterUid); }
      if (!string.IsNullOrEmpty(topFilterUid))
      { volumesSummaryRequester.QueryString.Add("topFilterUid", topFilterUid); }
    }

  }
}