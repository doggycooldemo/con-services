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
    public partial class TagFileFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "TagFile.feature"
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "TagFile", "  I should be able to POST tag files.", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        && (testRunner.FeatureContext.FeatureInfo.Title != "TagFile")))
            {
                global::ProductionDataSvc.AcceptanceTests.TagFileFeature.FeatureSetup(null);
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
  testRunner.Given("the Tag file service URI \"/api/v1/tagfiles\" and request repo \"TagFileRequest.json" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        public virtual void TagFile_GoodRequest(string paramName, string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Good request", exampleTags);
#line 7
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 8
  testRunner.When(string.Format("I POST a tag file with name \"{0}\" from the repository", paramName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 9
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Good request: 100")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "100")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "100")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "0")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"success\"")]
        public virtual void TagFile_GoodRequest_100()
        {
#line 7
this.TagFile_GoodRequest("100", "0", "\"success\"", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Good request: NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "0")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"success\"")]
        public virtual void TagFile_GoodRequest_NullProjectId()
        {
#line 7
this.TagFile_GoodRequest("NullProjectId", "0", "\"success\"", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFile_BadTagFile(string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Bad Tag File", exampleTags);
#line 15
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 16
  testRunner.When(string.Format("I POST a tag file with Code {0} from the repository expecting bad request return", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 17
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Tag File: 2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Failed to process tagfile with error: The TAG file was found to be corrupted on " +
            "its pre-processing scan.\"")]
        public virtual void TagFile_BadTagFile_2005()
        {
#line 15
this.TagFile_BadTagFile("2005", "\"Failed to process tagfile with error: The TAG file was found to be corrupted on " +
                    "its pre-processing scan.\"", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Tag File: 2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Failed to process tagfile with error: OnChooseMachine. Machine Subscriptions Inv" +
            "alid.\"")]
        public virtual void TagFile_BadTagFile_2008()
        {
#line 15
this.TagFile_BadTagFile("2008", "\"Failed to process tagfile with error: OnChooseMachine. Machine Subscriptions Inv" +
                    "alid.\"", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Tag File: 2014")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "2014")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "2014")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Failed to process tagfile with error: OnChooseDataModel. First Epoch Blade Posit" +
            "ion Does Not Lie Within Project Boundary.\"")]
        public virtual void TagFile_BadTagFile_2014()
        {
#line 15
this.TagFile_BadTagFile("2014", "\"Failed to process tagfile with error: OnChooseDataModel. First Epoch Blade Posit" +
                    "ion Does Not Lie Within Project Boundary.\"", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFile_BadRequest(string paramName, string code, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Bad Request", exampleTags);
#line 24
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 25
  testRunner.When(string.Format("I POST a Tag file with name \"{0}\" from the repository expecting bad request retur" +
                        "n", paramName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 26
  testRunner.Then(string.Format("the Tag Process Service response should contain Error Code {0}", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_NullFileName()
        {
#line 24
this.TagFile_BadRequest("NullFileName", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_NullData()
        {
#line 24
this.TagFile_BadRequest("NullData", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_NullBoundary()
        {
#line 24
this.TagFile_BadRequest("NullBoundary", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_FilenameTooLong()
        {
#line 24
this.TagFile_BadRequest("FilenameTooLong", "-1", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFile_ArchivedProject(string paramName, string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Archived Project", exampleTags);
#line 34
this.ScenarioSetup(scenarioInfo);
#line 4
this.FeatureBackground();
#line 35
  testRunner.When(string.Format("I POST a Tag file with name \"{0}\" from the repository expecting bad request retur" +
                        "n", paramName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 36
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Archived Project: InvalidProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "TagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "InvalidProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ParamName", "InvalidProjectId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "-5")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Missing Project or project does not belong to specified customer or don\'t have a" +
            "ccess to the project 0\"")]
        public virtual void TagFile_ArchivedProject_InvalidProjectId()
        {
#line 34
this.TagFile_ArchivedProject("InvalidProjectId", "-5", "\"Missing Project or project does not belong to specified customer or don\'t have a" +
                    "ccess to the project 0\"", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
