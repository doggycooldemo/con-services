﻿using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace VSS.Raptor.Service.WebApiTests.ProductionData.Controllers
{
  [TestClass]
  public class CCAColorPaletteControllerTest
  {
    /// <summary>
    /// Full integration test that requires a raptor stack running for getting a list of CCA data colour palettes.
    /// </summary>
    /// 
    [TestMethod]
    //[Ignore]
    public void PD_GetCCAColorPaletteFullIntegration()
    {
      /*
      CCAColorPaletteRequest request = CCAColorPaletteRequest.CreateCCAColorPaletteRequest(111, 1);

      CCAColorPaletteController controller = new CCAColorPaletteController();

      CCAColorPaletteResult result = controller.Get(request.projectId, request.machineId);

      // Assert
      Assert.IsNotNull(result);
      Assert.IsTrue(result.Message == "success", result.Message);
      */
    }
  }
}
