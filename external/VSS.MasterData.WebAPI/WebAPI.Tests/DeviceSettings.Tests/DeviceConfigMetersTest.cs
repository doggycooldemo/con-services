﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSettings.Test
{
	class DeviceConfigMetersTest
	{
		private static ControllerContext GetMockHttpContext()
		{
			//TODO: Add Mock HTTP Request object for DeviceConfigAssetSecurity Request
			var controllerContext = new ControllerContext();
			controllerContext.HttpContext = new DefaultHttpContext();
			return controllerContext;
		}
	}
}
