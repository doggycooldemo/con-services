﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34209
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace LandfillService.AcceptanceTests.Scenarios
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class ProjectsFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Projects.feature"
#line hidden
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Projects", "", ProgrammingLanguage.CSharp, ((string[])(null)));
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
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Projects")))
            {
                LandfillService.AcceptanceTests.Scenarios.ProjectsFeature.FeatureSetup(null);
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
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Project list")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void ProjectList()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Project list", ((string[])(null)));
#line 3
this.ScenarioSetup(scenarioInfo);
#line 4
 testRunner.When("I try to get a list of all projects", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "UID",
                        "TimezoneName",
                        "LegacyTimezoneName"});
            table1.AddRow(new string[] {
                        "06A92E4F-FAA2-E511-80E5-0050568821E6",
                        "America/Chicago",
                        "Central Standard Time"});
#line 5
 testRunner.Then("the project \'Casella-Stanley Landfill\' is in the list with details", ((string)(null)), table1, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Project data - one day")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void ProjectData_OneDay()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Project data - one day", ((string[])(null)));
#line 9
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "ProjectName",
                        "GeofenceUID",
                        "DateRange"});
            table2.AddRow(new string[] {
                        "Casella-Stanley Landfill",
                        "fb4f0e9d-12f4-11e5-b129-0050568838e5",
                        "OneDay"});
#line 10
 testRunner.When("I try to get data for", ((string)(null)), table2, "When ");
#line 13
 testRunner.Then("the response contains data for \'OneDay\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Project data - three days")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void ProjectData_ThreeDays()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Project data - three days", ((string[])(null)));
#line 15
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "ProjectName",
                        "GeofenceUID",
                        "DateRange"});
            table3.AddRow(new string[] {
                        "Casella-Stanley Landfill",
                        "fb4f0e9d-12f4-11e5-b129-0050568838e5",
                        "ThreeDays"});
#line 16
 testRunner.When("I try to get data for", ((string)(null)), table3, "When ");
#line 19
 testRunner.Then("the response contains data for \'ThreeDays\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Project data - two years")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void ProjectData_TwoYears()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Project data - two years", ((string[])(null)));
#line 21
this.ScenarioSetup(scenarioInfo);
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "ProjectName",
                        "GeofenceUID",
                        "DateRange"});
            table4.AddRow(new string[] {
                        "Casella-Stanley Landfill",
                        "fb4f0e9d-12f4-11e5-b129-0050568838e5",
                        "TwoYears"});
#line 22
 testRunner.When("I try to get data for", ((string)(null)), table4, "When ");
#line 25
 testRunner.Then("the response contains data for \'TwoYears\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Machine lifts - between specific dates")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void MachineLifts_BetweenSpecificDates()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Machine lifts - between specific dates", ((string[])(null)));
#line 27
this.ScenarioSetup(scenarioInfo);
#line 28
 testRunner.When("I try to get machine lift details for project \'PPG Track Wheel\' between dates \'20" +
                    "16-05-03&2015-05-03\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 29
 testRunner.Then("the following machine lift details are returned", "[\r\n  {\r\n\t\"assetId\": 1000001,\r\n\t\"machineName\": \"BLUE TRUCK\",\r\n\t\"isJohnDoe\": true,\r" +
                    "\n\t\"lifts\": [\r\n\t  {\r\n\t\t\"layerId\": 1,\r\n\t\t\"endTime\": \"2016-05-03T04:53:53.977\"\r\n\t  " +
                    "}\r\n\t]\r\n  }\r\n]", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Machine lifts - two years")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void MachineLifts_TwoYears()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Machine lifts - two years", ((string[])(null)));
#line 46
this.ScenarioSetup(scenarioInfo);
#line 47
 testRunner.When("I try to get machine lift details for project \'PPG Track Wheel\' between dates \'No" +
                    "tSpecified\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 48
 testRunner.Then("the following machine lift details are returned", "[\r\n  {\r\n\t\"assetId\": 1000001,\r\n\t\"machineName\": \"BLUE TRUCK\",\r\n\t\"isJohnDoe\": true,\r" +
                    "\n\t\"lifts\": [\r\n\t  {\r\n\t\t\"layerId\": 1,\r\n\t\t\"endTime\": \"2016-05-03T04:53:53.977\"\r\n\t  " +
                    "}\r\n\t]\r\n  }\r\n]", ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Gets volume and time summary for a landfill project")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Projects")]
        public virtual void GetsVolumeAndTimeSummaryForALandfillProject()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Gets volume and time summary for a landfill project", ((string[])(null)));
#line 65
this.ScenarioSetup(scenarioInfo);
#line 66
 testRunner.When("I try to get volume and time summary for project \'Casella-Stanley Landfill\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 67
 testRunner.Then("the remaining volume is 98748.471408972138 and the remaining time is 105.10249164" +
                    "632826", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
