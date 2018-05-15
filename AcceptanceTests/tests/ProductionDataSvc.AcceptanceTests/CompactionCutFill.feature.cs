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
    public partial class CompactionCutFillFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "CompactionCutFill.feature"
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "CompactionCutFill", "I should be able to request Cut-Fill compaction data", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        && (testRunner.FeatureContext.FeatureInfo.Title != "CompactionCutFill")))
            {
                global::ProductionDataSvc.AcceptanceTests.CompactionCutFillFeature.FeatureSetup(null);
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
        
        public virtual void CompactionGetCut_FillDetails_NoDesignFilter(string requestName, string projectUID, string cutFillDesignUID, string resultName, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Compaction Get Cut-Fill Details - No Design Filter", exampleTags);
#line 4
this.ScenarioSetup(scenarioInfo);
#line 5
testRunner.Given("the Compaction service URI \"/api/v2/cutfill/details\" for operation \"CutFillDetail" +
                    "s\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 6
testRunner.And("the result file \"CompactionGetCutFillDataResponse.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 7
testRunner.And(string.Format("projectUid \"{0}\"", projectUID), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 8
testRunner.And(string.Format("a cutfillDesignUid \"{0}\"", cutFillDesignUID), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 9
testRunner.When("I request result", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 10
testRunner.Then(string.Format("the result should match the \"{0}\" from the repository", resultName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details - No Design Filter: ")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "NoDesignFilter_Details")]
        public virtual void CompactionGetCut_FillDetails_NoDesignFilter_()
        {
#line 4
this.CompactionGetCut_FillDetails_NoDesignFilter("", "ff91dd40-1569-4765-a2bc-014321f76ace", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "NoDesignFilter_Details", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details - No Design Filter: ProjectSettings")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "ProjectSettings")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "ProjectSettings")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "7925f179-013d-4aaf-aff4-7b9833bb06d6")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "NoDesignFilter_Details_PS")]
        public virtual void CompactionGetCut_FillDetails_NoDesignFilter_ProjectSettings()
        {
#line 4
this.CompactionGetCut_FillDetails_NoDesignFilter("ProjectSettings", "7925f179-013d-4aaf-aff4-7b9833bb06d6", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "NoDesignFilter_Details_PS", ((string[])(null)));
#line hidden
        }
        
        public virtual void CompactionGetCut_FillDetails(string requestName, string projectUID, string cutFillDesignUID, string filterUID, string resultName, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Compaction Get Cut-Fill Details", exampleTags);
#line 17
this.ScenarioSetup(scenarioInfo);
#line 18
testRunner.Given("the Compaction service URI \"/api/v2/cutfill/details\" for operation \"CutFillDetail" +
                    "s\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 19
testRunner.And("the result file \"CompactionGetCutFillDataResponse.json\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 20
testRunner.And(string.Format("projectUid \"{0}\"", projectUID), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
testRunner.And(string.Format("a cutfillDesignUid \"{0}\"", cutFillDesignUID), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
testRunner.And(string.Format("filterUid \"{0}\"", filterUID), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
testRunner.When("I request result", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 24
testRunner.Then(string.Format("the result should match the \"{0}\" from the repository", resultName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details: DesignOutside")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "DesignOutside")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "DesignOutside")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "7925f179-013d-4aaf-aff4-7b9833bb06d6")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUID", "1cf81668-1739-42d5-b068-ea025588796a")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "DesignOutside_Details")]
        public virtual void CompactionGetCut_FillDetails_DesignOutside()
        {
#line 17
this.CompactionGetCut_FillDetails("DesignOutside", "7925f179-013d-4aaf-aff4-7b9833bb06d6", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "1cf81668-1739-42d5-b068-ea025588796a", "DesignOutside_Details", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details: DesignIntersects")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "DesignIntersects")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "DesignIntersects")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "7925f179-013d-4aaf-aff4-7b9833bb06d6")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "220e12e5-ce92-4645-8f01-1942a2d5a57f")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUID", "3d9086f2-3c04-4d92-9141-5134932b1523")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "DesignIntersects_Details")]
        public virtual void CompactionGetCut_FillDetails_DesignIntersects()
        {
#line 17
this.CompactionGetCut_FillDetails("DesignIntersects", "7925f179-013d-4aaf-aff4-7b9833bb06d6", "220e12e5-ce92-4645-8f01-1942a2d5a57f", "3d9086f2-3c04-4d92-9141-5134932b1523", "DesignIntersects_Details", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details: FilterAreaMachine")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "FilterAreaMachine")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "FilterAreaMachine")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUID", "9c27697f-ea6d-478a-a168-ed20d6cd9a20")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "BoundaryMachineFilterCFDetails")]
        public virtual void CompactionGetCut_FillDetails_FilterAreaMachine()
        {
#line 17
this.CompactionGetCut_FillDetails("FilterAreaMachine", "ff91dd40-1569-4765-a2bc-014321f76ace", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "9c27697f-ea6d-478a-a168-ed20d6cd9a20", "BoundaryMachineFilterCFDetails", ((string[])(null)));
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Compaction Get Cut-Fill Details: AlignmentFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "CompactionCutFill")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("VariantName", "AlignmentFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:RequestName", "AlignmentFilter")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ProjectUID", "ff91dd40-1569-4765-a2bc-014321f76ace")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:CutFillDesignUID", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:FilterUID", "2811c7c3-d270-4d63-97e2-fc3340bf6c7a")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("Parameter:ResultName", "AlignmentFilter_Details")]
        public virtual void CompactionGetCut_FillDetails_AlignmentFilter()
        {
#line 17
this.CompactionGetCut_FillDetails("AlignmentFilter", "ff91dd40-1569-4765-a2bc-014321f76ace", "dd64fe2e-6f27-4a78-82a3-0c0e8a5e84ff", "2811c7c3-d270-4d63-97e2-fc3340bf6c7a", "AlignmentFilter_Details", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
