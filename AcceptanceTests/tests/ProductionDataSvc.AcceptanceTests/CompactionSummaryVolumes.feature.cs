﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class CompactionSummaryVolumesFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "CompactionSummaryVolumes.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CompactionSummaryVolumes", "I should be able to request Summary Volumes.", ProgrammingLanguage.CSharp, ((string[])(null)));
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
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "CompactionSummaryVolumes")))
            {
                ProductionDataSvc.AcceptanceTests.CompactionSummaryVolumesFeature.FeatureSetup(null);
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
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void CompactionGetSummaryVolumes(string requestName, string projectUid, string designUid, string filterUid, string filterUid2, string resultName, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Compaction Get Summary volumes", exampleTags);
#line 4
this.ScenarioSetup(scenarioInfo);
#line 5
testRunner.Given("the Compaction service URI \"/api/v2/compaction/volumes/summary\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 6
testRunner.And("the result file \"CompactionSummaryVolumeResponse.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 7
testRunner.And(string.Format("project \"{0}\"", projectUid), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 8
testRunner.And(string.Format("filter \"{0}\"", filterUid), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
testRunner.And(string.Format("design \"{0}\"", designUid), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
testRunner.And(string.Format("to filter \"{0}\"", filterUid2), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
testRunner.When("I request result", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 12
testRunner.Then(string.Format("the result should match the \"{0}\" from the repository", resultName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Summary volumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionSummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "SimpleVolumeSummary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "SimpleVolumeSummary")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUid", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:DesignUid", "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid2", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "SimpleVolumeSummary")]
        public virtual void CompactionGetSummaryVolumes_SimpleVolumeSummary()
        {
            this.CompactionGetSummaryVolumes("SimpleVolumeSummary", "ff91dd40-1569-4765-a2bc-014321f76ace", "", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B", "SimpleVolumeSummary", ((string[])(null)));
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Summary volumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionSummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "GroundToGround")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "GroundToGround")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUid", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:DesignUid", "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid", "a37f3008-65e5-44a8-b406-9a078ec62ece")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid2", "a37f3008-65e5-44a8-b406-9a078ec62ece")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "GroundToGround")]
        public virtual void CompactionGetSummaryVolumes_GroundToGround()
        {
            this.CompactionGetSummaryVolumes("GroundToGround", "ff91dd40-1569-4765-a2bc-014321f76ace", "", "a37f3008-65e5-44a8-b406-9a078ec62ece", "a37f3008-65e5-44a8-b406-9a078ec62ece", "GroundToGround", ((string[])(null)));
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Summary volumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionSummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterAndDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "FilterAndDesign")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUid", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:DesignUid", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid", "9c27697f-ea6d-478a-a168-ed20d6cd9a20")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid2", "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "FilterAndDesign")]
        public virtual void CompactionGetSummaryVolumes_FilterAndDesign()
        {
            this.CompactionGetSummaryVolumes("FilterAndDesign", "ff91dd40-1569-4765-a2bc-014321f76ace", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "9c27697f-ea6d-478a-a168-ed20d6cd9a20", "", "FilterAndDesign", ((string[])(null)));
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Summary volumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionSummaryVolumes")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "CustomBulkingAndShrinkage")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "CustomBulkingAndShrinkage")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUid", "3335311a-f0e2-4dbe-8acd-f21135bafee4")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:DesignUid", "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUid2", "A40814AA-9CDB-4981-9A21-96EA30FFECDD")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "CustomBulkingAndShrinkage")]
        public virtual void CompactionGetSummaryVolumes_CustomBulkingAndShrinkage()
        {
            this.CompactionGetSummaryVolumes("CustomBulkingAndShrinkage", "3335311a-f0e2-4dbe-8acd-f21135bafee4", "", "F07ED071-F8A1-42C3-804A-1BDE7A78BE5B", "A40814AA-9CDB-4981-9A21-96EA30FFECDD", "CustomBulkingAndShrinkage", ((string[])(null)));
        }
    }
}
#pragma warning restore
#endregion
