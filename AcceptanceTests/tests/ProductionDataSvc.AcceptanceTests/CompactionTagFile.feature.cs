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
    public partial class CompactionTagFileFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "CompactionTagFile.feature"
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CompactionTagFile", "  I should be able to POST tag files for compaction.", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        && (testRunner.FeatureContext.FeatureInfo.Title != "CompactionTagFile")))
            {
                global::ProductionDataSvc.AcceptanceTests.CompactionTagFileFeature.FeatureSetup(null);
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
        
        public virtual void TagFile_BadTagFile(string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Bad Tag File", exampleTags);
#line 4
this.ScenarioSetup(scenarioInfo);
#line 5
  testRunner.Given("the Tag file service URI \"/api/v2/tagfiles\" and request repo \"CompactionTagFileRe" +
                    "quest.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 6
  testRunner.When(string.Format("I POST a tag file with Code {0} from the repository expecting bad request return", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 7
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Tag File: 2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "2005")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Failed to process tagfile with error: The TAG file was found to be corrupted on " +
            "its pre-processing scan.\"")]
        public virtual void TagFile_BadTagFile_2005()
        {
#line 4
this.TagFile_BadTagFile("2005", "\"Failed to process tagfile with error: The TAG file was found to be corrupted on " +
                    "its pre-processing scan.\"", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Tag File: 2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "2008")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"Failed to process tagfile with error: OnChooseMachine. Machine Subscriptions Inv" +
            "alid.\"")]
        public virtual void TagFile_BadTagFile_2008()
        {
#line 4
this.TagFile_BadTagFile("2008", "\"Failed to process tagfile with error: OnChooseMachine. Machine Subscriptions Inv" +
                    "alid.\"", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFile_ArchivedProject(string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Archived Project", exampleTags);
#line 13
this.ScenarioSetup(scenarioInfo);
#line 14
  testRunner.Given("the Tag file service URI \"/api/v2/tagfiles\" and request repo \"CompactionTagFileRe" +
                    "quest.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 15
  testRunner.When(string.Format("I POST a tag file with Code {0} from the repository expecting unauthorized reques" +
                        "t return", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 16
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Archived Project: -1")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "-1")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "-1")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"The project has been archived and this function is not allowed.\"")]
        public virtual void TagFile_ArchivedProject_1()
        {
#line 13
this.TagFile_ArchivedProject("-1", "\"The project has been archived and this function is not allowed.\"", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFile_BadRequest(string paramName, string code, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile - Bad Request", exampleTags);
#line 21
this.ScenarioSetup(scenarioInfo);
#line 22
  testRunner.Given("the Tag file service URI \"/api/v2/tagfiles\" and request repo \"CompactionTagFileRe" +
                    "quest.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 23
  testRunner.When(string.Format("I POST a Tag file with name \"{0}\" from the repository expecting bad request retur" +
                        "n", paramName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 24
  testRunner.Then(string.Format("the Tag Process Service response should contain Error Code {0}", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_NullFileName()
        {
#line 21
this.TagFile_BadRequest("NullFileName", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_NullData()
        {
#line 21
this.TagFile_BadRequest("NullData", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFile_BadRequest_FilenameTooLong()
        {
#line 21
this.TagFile_BadRequest("FilenameTooLong", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile - Bad Request: NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "2008")]
        public virtual void TagFile_BadRequest_NullProjectUid()
        {
#line 21
this.TagFile_BadRequest("NullProjectUid", "2008", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFileDirectSubmission_BadTagFile(string code, string message, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile Direct Submission - Bad Tag File", exampleTags);
#line 32
this.ScenarioSetup(scenarioInfo);
#line 33
  testRunner.Given("the Tag file service URI \"/api/v2/tagfiles/direct\" and request repo \"CompactionTa" +
                    "gFileDirectSubmissionRequest.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 34
  testRunner.When(string.Format("I POST a tag file with Code {0} from the repository expecting bad request return", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 35
  testRunner.Then(string.Format("the Tag Process Service response should contain Code {0} and Message {1}", code, message), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Tag File: 5")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "5")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "5")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"The TAG file was found to be corrupted on its pre-processing scan.\"")]
        public virtual void TagFileDirectSubmission_BadTagFile_5()
        {
#line 32
this.TagFileDirectSubmission_BadTagFile("5", "\"The TAG file was found to be corrupted on its pre-processing scan.\"", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Tag File: 8")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "8")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Code", "8")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:Message", "\"OnChooseMachine. Machine Subscriptions Invalid.\"")]
        public virtual void TagFileDirectSubmission_BadTagFile_8()
        {
#line 32
this.TagFileDirectSubmission_BadTagFile("8", "\"OnChooseMachine. Machine Subscriptions Invalid.\"", ((string[])(null)));
#line hidden
        }
        
        public virtual void TagFileDirectSubmission_BadRequest(string paramName, string code, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("TagFile Direct Submission - Bad Request", exampleTags);
#line 41
this.ScenarioSetup(scenarioInfo);
#line 42
  testRunner.Given("the Tag file service URI \"/api/v2/tagfiles/direct\" and request repo \"CompactionTa" +
                    "gFileDirectSubmissionRequest.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 43
  testRunner.When(string.Format("I POST a Tag file with name \"{0}\" from the repository expecting bad request retur" +
                        "n", paramName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
  testRunner.Then(string.Format("the Tag Process Service response should contain Error Code {0}", code), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullFileName")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFileDirectSubmission_BadRequest_NullFileName()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("NullFileName", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullData")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFileDirectSubmission_BadRequest_NullData()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("NullData", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "FilenameTooLong")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "-1")]
        public virtual void TagFileDirectSubmission_BadRequest_FilenameTooLong()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("FilenameTooLong", "-1", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullProjectUid")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "8")]
        public virtual void TagFileDirectSubmission_BadRequest_NullProjectUid()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("NullProjectUid", "8", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: NullMachineId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullMachineId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullMachineId")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "8")]
        public virtual void TagFileDirectSubmission_BadRequest_NullMachineId()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("NullMachineId", "8", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("TagFile Direct Submission - Bad Request: NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionTagFile")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:paramName", "NullBoundary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:code", "8")]
        public virtual void TagFileDirectSubmission_BadRequest_NullBoundary()
        {
#line 41
this.TagFileDirectSubmission_BadRequest("NullBoundary", "8", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
