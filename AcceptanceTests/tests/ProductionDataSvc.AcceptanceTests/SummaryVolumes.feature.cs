﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.3.2.0
//      SpecFlow Generator Version:2.3.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace ProductionDataSvc.AcceptanceTests
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.3.2.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class SummaryVolumesFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "SummaryVolumes.feature"
#line hidden
        
        public virtual Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext
        {
            get
            {
                return this._testContext;
            }
            set
            {
                this._testContext = value;
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner(null, 0);
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SummaryVolumes", "\tI should be able to request Summary Volumes.", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Title != "SummaryVolumes")))
            {
                global::ProductionDataSvc.AcceptanceTests.SummaryVolumesFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>(TestContext);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line 5
 testRunner.Given("the Summary Volumes service URI \"/api/v1/volumes/summary\", request repo \"SummaryV" +
                    "olumeRequest.json\" and result repo \"SummaryVolumeResponse.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        public virtual void SummaryVolumes_GoodRequest(string parameterName, string resultName, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "requireSurveyedSurface"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("SummaryVolumes - Good Request", @__tags);
#line 8
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 9
 testRunner.When(string.Format("I request Summary Volumes supplying \"{0}\" paramters from the repository", parameterName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 10
 testRunner.Then(string.Format("the response should match \"{0}\" result from the repository", resultName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: FilterToFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterToFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "FilterToFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterToFilter")]
        public virtual void SummaryVolumes_GoodRequest_FilterToFilter()
        {
#line 8
this.SummaryVolumes_GoodRequest("FilterToFilter", "FilterToFilter", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: EarliestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "EarliestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "EarliestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "EarliestFilterToDesign")]
        public virtual void SummaryVolumes_GoodRequest_EarliestFilterToDesign()
        {
#line 8
this.SummaryVolumes_GoodRequest("EarliestFilterToDesign", "EarliestFilterToDesign", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: LatestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "LatestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "LatestFilterToDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "LatestFilterToDesign")]
        public virtual void SummaryVolumes_GoodRequest_LatestFilterToDesign()
        {
#line 8
this.SummaryVolumes_GoodRequest("LatestFilterToDesign", "LatestFilterToDesign", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: DesignToEarliestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "DesignToEarliestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "DesignToEarliestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "DesignToEarliestFilter")]
        public virtual void SummaryVolumes_GoodRequest_DesignToEarliestFilter()
        {
#line 8
this.SummaryVolumes_GoodRequest("DesignToEarliestFilter", "DesignToEarliestFilter", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: DesignToLatestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "DesignToLatestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "DesignToLatestFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "DesignToLatestFilter")]
        public virtual void SummaryVolumes_GoodRequest_DesignToLatestFilter()
        {
#line 8
this.SummaryVolumes_GoodRequest("DesignToLatestFilter", "DesignToLatestFilter", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: FilterToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "FilterToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterToCompositeNoSurveyedSurface")]
        public virtual void SummaryVolumes_GoodRequest_FilterToCompositeNoSurveyedSurface()
        {
#line 8
this.SummaryVolumes_GoodRequest("FilterToCompositeNoSurveyedSurface", "FilterToCompositeNoSurveyedSurface", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: CompositeToDesignNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "CompositeToDesignNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "CompositeToDesignNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "CompositeToDesignNoSurveyedSurface")]
        public virtual void SummaryVolumes_GoodRequest_CompositeToDesignNoSurveyedSurface()
        {
#line 8
this.SummaryVolumes_GoodRequest("CompositeToDesignNoSurveyedSurface", "CompositeToDesignNoSurveyedSurface", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: DesignToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "DesignToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "DesignToCompositeNoSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "DesignToCompositeNoSurveyedSurface")]
        public virtual void SummaryVolumes_GoodRequest_DesignToCompositeNoSurveyedSurface()
        {
#line 8
this.SummaryVolumes_GoodRequest("DesignToCompositeNoSurveyedSurface", "DesignToCompositeNoSurveyedSurface", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: SummationTestLotOneOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "SummationTestLotOneOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "SummationTestLotOneOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "SummationTestLotOneOfThree")]
        public virtual void SummaryVolumes_GoodRequest_SummationTestLotOneOfThree()
        {
#line 8
this.SummaryVolumes_GoodRequest("SummationTestLotOneOfThree", "SummationTestLotOneOfThree", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: SummationTestLotTwoOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "SummationTestLotTwoOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "SummationTestLotTwoOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "SummationTestLotTwoOfThree")]
        public virtual void SummaryVolumes_GoodRequest_SummationTestLotTwoOfThree()
        {
#line 8
this.SummaryVolumes_GoodRequest("SummationTestLotTwoOfThree", "SummationTestLotTwoOfThree", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: SummationTestLotThreeOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "SummationTestLotThreeOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "SummationTestLotThreeOfThree")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "SummationTestLotThreeOfThree")]
        public virtual void SummaryVolumes_GoodRequest_SummationTestLotThreeOfThree()
        {
#line 8
this.SummaryVolumes_GoodRequest("SummationTestLotThreeOfThree", "SummationTestLotThreeOfThree", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: SummationTestTheWholeLot")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "SummationTestTheWholeLot")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "SummationTestTheWholeLot")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "SummationTestTheWholeLot")]
        public virtual void SummaryVolumes_GoodRequest_SummationTestTheWholeLot()
        {
#line 8
this.SummaryVolumes_GoodRequest("SummationTestTheWholeLot", "SummationTestTheWholeLot", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: FilterToDesignWithFillTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterToDesignWithFillTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "FilterToDesignWithFillTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterToDesignWithFillTolerances")]
        public virtual void SummaryVolumes_GoodRequest_FilterToDesignWithFillTolerances()
        {
#line 8
this.SummaryVolumes_GoodRequest("FilterToDesignWithFillTolerances", "FilterToDesignWithFillTolerances", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: FilterToDesignWithCutTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterToDesignWithCutTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "FilterToDesignWithCutTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterToDesignWithCutTolerances")]
        public virtual void SummaryVolumes_GoodRequest_FilterToDesignWithCutTolerances()
        {
#line 8
this.SummaryVolumes_GoodRequest("FilterToDesignWithCutTolerances", "FilterToDesignWithCutTolerances", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Good Request: FilterToFilterWithBothTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("requireSurveyedSurface")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterToFilterWithBothTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "FilterToFilterWithBothTolerances")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterToFilterWithBothTolerances")]
        public virtual void SummaryVolumes_GoodRequest_FilterToFilterWithBothTolerances()
        {
#line 8
this.SummaryVolumes_GoodRequest("FilterToFilterWithBothTolerances", "FilterToFilterWithBothTolerances", ((string[])(null)));
#line hidden
        }
        
        public virtual void SummaryVolumes_BadRequest(string parameterName, string httpCode, string errorCode, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("SummaryVolumes - Bad Request", exampleTags);
#line 42
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 43
 testRunner.When(string.Format("I request Summary Volumes supplying \"{0}\" paramters from the repository expecting" +
                        " error http code {1}", parameterName, httpCode), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
 testRunner.Then(string.Format("the response body should contain Error Code {0}", errorCode), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Bad Request: NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:HttpCode", "400")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ErrorCode", "-1")]
        public virtual void SummaryVolumes_BadRequest_NullProjectId()
        {
#line 42
this.SummaryVolumes_BadRequest("NullProjectId", "400", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("SummaryVolumes - Bad Request: InvalidLatLon")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "SummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "InvalidLatLon")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParameterName", "InvalidLatLon")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:HttpCode", "400")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ErrorCode", "-1")]
        public virtual void SummaryVolumes_BadRequest_InvalidLatLon()
        {
#line 42
this.SummaryVolumes_BadRequest("InvalidLatLon", "400", "-1", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
