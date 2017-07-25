﻿
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ProductionDataSvc.AcceptanceTests.Models;
using RaptorSvcAcceptTestsCommon.Models;
using RaptorSvcAcceptTestsCommon.Utils;
using TechTalk.SpecFlow;

namespace ProductionDataSvc.AcceptanceTests.StepDefinitions
{
  [Binding, Scope(Feature = "Compaction")]
  public sealed class CompactionSteps
  {
    private string url;

    private Getter<CompactionCmvSummaryResult> cmvSummaryRequester;
    private Getter<CompactionCmvDetailedResult> cmvDetailsRequester;
    private Getter<CompactionMdpSummaryResult> mdpSummaryRequester;
    private Getter<CompactionPassCountSummaryResult> passCountSummaryRequester;
    private Getter<CompactionPassCountDetailedResult> passCountDetailsRequester;
    private Getter<CompactionSpeedSummaryResult> speedSummaryRequester;
    private Getter<CompactionTemperatureSummaryResult> temperatureSummaryRequester;
    private Getter<CompactionCmvPercentChangeResult> cmvPercentChangeRequester;
    private Poster<StatisticsParameters, ProjectStatistics> projectStatisticsPoster;
    private Getter<ElevationStatisticsResult> elevationRangeRequester;
    private Getter<TileResult> tileRequester;
    private Getter<CompactionColorPalettesResult> paletteRequester;
    private Getter<CompactionElevationPaletteResult> elevPaletteRequester;
    private Getter<ProfileResult> profileRequester;

    private StatisticsParameters statsRequest;
    private string projectUid;
    private string queryParameters = string.Empty;


    [Given(@"the Compaction CMV Summary service URI ""(.*)""")]
    public void GivenTheCompactionCMVSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [Given(@"a projectUid ""(.*)""")]
    public void GivenAProjectUid(string projectUid)
    {
      this.projectUid = projectUid;
    }

    [Given(@"a startUtc ""(.*)"" and an EndUtc ""(.*)""")]
    public void GivenAStartUtcAndAnEndUtc(string startUtc, string endUtc)
    {
      queryParameters = string.Format("&startUtc={0}&endUtc={1}",
        startUtc, endUtc);
    }

    [When(@"I request CMV summary")]
    public void WhenIRequestCMVSummary()
    {
      cmvSummaryRequester = GetIt<CompactionCmvSummaryResult>();
    }

    [Then(@"the CMV summary result should be")]
    public void ThenTheCMVSummaryResultShouldBe(string multilineText)
    {
      CompareIt<CompactionCmvSummaryResult>(multilineText, cmvSummaryRequester);
    }

    [Given(@"the Compaction MDP Summary service URI ""(.*)""")]
    public void GivenTheCompactionMDPSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request MDP summary")]
    public void WhenIRequestMDPSummary()
    {
      mdpSummaryRequester = GetIt<CompactionMdpSummaryResult>();
    }

    [Then(@"the MDP result should be")]
    public void ThenTheMDPResultShouldBe(string multilineText)
    {
      CompareIt<CompactionMdpSummaryResult>(multilineText, mdpSummaryRequester);
    }

    [Given(@"the Compaction Passcount Summary service URI ""(.*)""")]
    public void GivenTheCompactionPasscountSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Passcount summary")]
    public void WhenIRequestPasscountSummary()
    {
      passCountSummaryRequester = GetIt<CompactionPassCountSummaryResult>();
    }

    [Then(@"the Passcount summary result should be")]
    public void ThenThePasscountResultShouldBe(string multilineText)
    {
      CompareIt<CompactionPassCountSummaryResult>(multilineText, passCountSummaryRequester);
    }

    [Given(@"the Compaction Passcount Details service URI ""(.*)""")]
    public void GivenTheCompactionPasscountDetailsServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Passcount details")]
    public void WhenIRequestPasscountDetails()
    {
      passCountDetailsRequester = GetIt<CompactionPassCountDetailedResult>();
    }

    [Then(@"the Passcount details result should be")]
    public void ThenThePasscountDetailsResultShouldBe(string multilineText)
    {
      CompareIt<CompactionPassCountDetailedResult>(multilineText, passCountDetailsRequester);
    }


    [Given(@"the Compaction Temperature Summary service URI ""(.*)""")]
    public void GivenTheCompactionTemperatureSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Temperature summary")]
    public void WhenIRequestTemperatureSummary()
    {
      temperatureSummaryRequester = GetIt<CompactionTemperatureSummaryResult>();
    }

    [Then(@"the Temperature result should be")]
    public void ThenTheTemperatureResultShouldBe(string multilineText)
    {
      CompareIt<CompactionTemperatureSummaryResult>(multilineText, temperatureSummaryRequester);
    }

    [Given(@"the Compaction Speed Summary service URI ""(.*)""")]
    public void GivenTheCompactionSpeedSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Speed summary")]
    public void WhenIRequestSpeedSummary()
    {
      speedSummaryRequester = GetIt<CompactionSpeedSummaryResult>();
    }

    [Then(@"the Speed result should be")]
    public void ThenTheSpeedResultShouldBe(string multilineText)
    {
      CompareIt<CompactionSpeedSummaryResult>(multilineText, speedSummaryRequester);
    }

    [Given(@"the Compaction CMV % Change Summary service URI ""(.*)""")]
    public void GivenTheCompactionCMVChangeSummaryServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request CMV % change")]
    public void WhenIRequestCMVChange()
    {
      cmvPercentChangeRequester = GetIt<CompactionCmvPercentChangeResult>();
    }

    [Then(@"the CMV % Change result should be")]
    public void ThenTheCMVChangeResultShouldBe(string multilineText)
    {
      CompareIt<CompactionCmvPercentChangeResult>(multilineText, cmvPercentChangeRequester);
    }

    [Given(@"the Compaction Elevation Range service URI ""(.*)""")]
    public void GivenTheCompactionElevationRangeServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Elevation Range")]
    public void WhenIRequestElevationRange()
    {
      elevationRangeRequester = GetIt<ElevationStatisticsResult>();
    }

    [Then(@"the Elevation Range result should be")]
    public void ThenTheElevationRangeResultShouldBe(string multilineText)
    {
      CompareIt<ElevationStatisticsResult>(multilineText, elevationRangeRequester);
    }

    [Given(@"the Compaction Project Statistics service URI ""(.*)""")]
    public void GivenTheCompactionProjectStatisticsServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Project Statistics")]
    public void WhenIRequestProjectStatistics()
    {
      statsRequest = new StatisticsParameters { projectUid = this.projectUid };
      projectStatisticsPoster = PostIt<StatisticsParameters, ProjectStatistics>(statsRequest);
    }

    [Then(@"the Project Statistics result should be")]
    public void ThenTheProjectStatisticsResultShouldBe(string multilineText)
    {
      CompareIt(multilineText, projectStatisticsPoster);
    }

    [Given(@"the Compaction Tiles service URI ""(.*)""")]
    public void GivenTheCompactionTilesServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [Given(@"a displayMode ""(.*)"" and a bbox ""(.*)"" and a width ""(.*)"" and a height ""(.*)""")]
    public void GivenADisplayModeAndABboxLLAndAWidthAndAHeight(int mode, string bbox, int width, int height)
    {
      queryParameters = string.Format("&mode={0}&BBOX={1}&WIDTH={2}&HEIGHT={3}", 
        mode, bbox, width, height);
    }

    [When(@"I request a Tile")]
    public void WhenIRequestATile()
    {
      tileRequester = GetIt<TileResult>();
    }

    [Then(@"the Tile result should be")]
    public void ThenTheTileResultShouldBe(string multilineText)
    {
      CompareIt<TileResult>(multilineText, tileRequester);
    }

    [Given(@"the Compaction Elevation Palette service URI ""(.*)""")]
    public void GivenTheCompactionElevationPaletteServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Elevation Palette")]
    public void WhenIRequestElevationPalette()
    {
      elevPaletteRequester = GetIt<CompactionElevationPaletteResult>();
    }

    [Then(@"the Elevation Palette result should be")]
    public void ThenTheElevationPaletteResultShouldBe(string multilineText)
    {
      CompareIt<CompactionElevationPaletteResult>(multilineText, elevPaletteRequester);
    }

    [Given(@"the Compaction Palettes service URI ""(.*)""")]
    public void GivenTheCompactionPalettesServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request Palettes")]
    public void WhenIRequestPalettes()
    {
      paletteRequester = GetIt<CompactionColorPalettesResult>();
    }

    [Then(@"the Palettes result should be")]
    public void ThenThePalettesResultShouldBe(string multilineText)
    {
      CompareIt<CompactionColorPalettesResult>(multilineText, paletteRequester);
    }

    [Given(@"the Compaction CMV Details service URI ""(.*)""")]
    public void GivenTheCompactionCMVDetailsServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [When(@"I request CMV details")]
    public void WhenIRequestCMVDetails()
    {
      cmvDetailsRequester = GetIt<CompactionCmvDetailedResult>();      
    }

    [Then(@"the CMV details result should be")]
    public void ThenTheCMVDetailsResultShouldBe(string multilineText)
    {
      CompareIt<CompactionCmvDetailedResult>(multilineText, cmvDetailsRequester);
    }

    [Given(@"the ProfileSlicer service URI ""(.*)""")]
    public void GivenTheProfileSlicerServiceURI(string url)
    {
      this.url = RaptorClientConfig.CompactionSvcBaseUri + url;
    }

    [Given(@"a startLatDegrees ""(.*)"" and a startLonDegrees ""(.*)"" and a endLatDegrees ""(.*)"" and a endLonDegrees ""(.*)""")]
    public void GivenAStartLatDegreesAndAStartLonDegreesAndAEndLatDegreesAndAEndLonDegrees(Decimal startLatDegrees, Decimal startLonDegrees, Decimal endLatDegrees, Decimal endLonDegrees)
    {
      queryParameters =
        $"&startLatDegrees={startLatDegrees}&startLonDegrees={startLonDegrees}&endLatDegrees={endLatDegrees}&endLonDegrees={endLonDegrees}";
    }

    [When(@"I request a ProductionData Slicer Profile")]
    public void WhenIRequestAProductionDataSlicerProfile()
    {
      profileRequester = GetIt<ProfileResult>();
    }

    [Then(@"the Profile response should be")]
    public void ThenTheProfileResponseShouldBe(string multilineText)
    {
      CompareIt<ProfileResult>(multilineText, profileRequester);
    }

    private Getter<T> GetIt<T>()
    {
      this.url = string.Format("{0}?projectUid={1}", this.url, projectUid);     
      this.url += this.queryParameters;
      Getter<T> getter = new Getter<T>(this.url);
      getter.DoValidRequest();
      return getter;
    }

    private Poster<T, U> PostIt<T, U>(T request)
    {
      Poster<T, U> poster = new Poster<T, U>(this.url, request);
      //poster.CurrentRequest = request;
      poster.DoValidRequest();
      return poster;
    }

    private void CompareIt<T>(string multilineText, Getter<T> requester)
    {
      T expected = JsonConvert.DeserializeObject<T>(multilineText);      
      Assert.AreEqual(expected, requester.CurrentResponse);
    }

    private void CompareIt<T, U>(string multilineText, Poster<T, U> poster)
    {
      U expected = JsonConvert.DeserializeObject<U>(multilineText);
      Assert.AreEqual(expected, poster.CurrentResponse);
    }

  }
}
