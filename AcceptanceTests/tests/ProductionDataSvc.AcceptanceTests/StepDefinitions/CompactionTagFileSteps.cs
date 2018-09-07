﻿using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionDataSvc.AcceptanceTests.Models;
using RaptorSvcAcceptTestsCommon.Utils;
using TechTalk.SpecFlow;

namespace ProductionDataSvc.AcceptanceTests.StepDefinitions
{
  [Binding, Scope(Feature = "CompactionTagFile")]
  public class CompactionTagFileSteps
  {
    private Poster<CompactionTagFilePostParameter, CompactionTagFilePostResult> tagPoster;

    [Given(@"the Tag file service URI ""(.*)"" and request repo ""(.*)""")]
    public void GivenTheTagFileServiceURIAndRequestRepo(string uri, string requestFile)
    {
      uri = RaptorClientConfig.TagSvcBaseUri + uri;
      tagPoster = new Poster<CompactionTagFilePostParameter, CompactionTagFilePostResult>(uri, requestFile);
    }

    [When(@"I POST a tag file with code (.*) from the repository")]
    public void WhenIPOSTATagFileWithCodeFromTheRepository(int code)
    {
      tagPoster.DoValidRequest(code.ToString());
    }

    [When(@"I POST a tag file with Code (.*) from the repository expecting bad request return")]
    public void WhenIPOSTATagFileWithCodeFromTheRepositoryExpectingBadRequestReturn(int code)
    {
      tagPoster.DoInvalidRequest(code.ToString());
    }

    [When(@"I POST a Tag file with name ""(.*)"" from the repository expecting bad request return")]
    public void WhenIPOSTATagFileWithNameFromTheRepositoryExpectingBadRequestReturn(string paramName)
    {
      tagPoster.DoInvalidRequest(paramName);
    }

    [When(@"I POST a tag file with Code (.*) from the repository expecting unauthorized request return")]
    public void WhenIPOSTATagFileWithCodeFromTheRepositoryExpectingUnauthorizedRequestReturn(int code)
    {
      tagPoster.DoInvalidRequest(code.ToString(), HttpStatusCode.Unauthorized);
    }

    [Then(@"the Tag Process Service response should contain Code (.*) and Message ""(.*)""")]
    public void ThenTheTagProcessServiceResponseShouldContainCodeAndMessage(int code, string message)
    {
      Assert.IsTrue(tagPoster.CurrentResponse.Code == code && tagPoster.CurrentResponse.Message == message,
        $"Expected Code {code} and Message {message}, but received {tagPoster.CurrentResponse.Code} and {tagPoster.CurrentResponse.Message} instead.");
    }

    [Then(@"the Tag Process Service response should contain Error Code (.*)")]
    public void ThenTheTagProcessServiceResponseShouldContainErrorCode(int code)
    {
      Assert.AreEqual(code, tagPoster.CurrentResponse.Code,
        $"Expected Code {code}, but received {tagPoster.CurrentResponse.Code} instead.");
    }
  }
}
