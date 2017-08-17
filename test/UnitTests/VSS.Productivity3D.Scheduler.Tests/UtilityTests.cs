﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VSS.Productivity3D.Scheduler.Tests
{
  [TestClass]
  public class UtilityTests
  {
/*
    [TestMethod]
    public void TestJWTKey()
    {
      const string jwt =
        "eyJhbGciOiJSUzI1NiIsIng1dCI6IlltRTNNelE0TVRZNE5EVTJaRFptT0RkbU5UUm1OMlpsWVRrd01XRXpZbU5qTVRrek1ERXpaZyJ9.eyJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9hcHBsaWNhdGlvbm5hbWUiOiJDb21wYWN0aW9uLURldmVsb3AtQ0kiLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9wYXNzd29yZFBvbGljeURldGFpbHMiOiJleUoxY0dSaGRHVmtWR2x0WlNJNk1UUTVNVEUzTURFNE5qazNNaXdpYUdsemRHOXllU0k2V3lJMk5UTmlaakl5T0RnMk5qYzVOV1V3TkRFNU1qQTJOekUwWTJVek1EWmxNRE15WW1ReU1qWmlaRFUwWmpRek5qZzFOREkwTlRkbFpUSXhNRGcxTlRBd0lpd2lNakUyTnpkbU56bGlOVFZtWmpjek5qbGxNV1ZtT0RCaE5XRXdZVEZpWldJNE1qZzBaR0kwTXpZNU16QTNPVGt4WlRsalpEVTNORGcyTXpWallUZGxNaUlzSW1NNU5UQXdNRFpqTlRJelpXSTFPRGRoWkdFek1EVTFNakkwWVdSbFptRTNOMkl4TURjMllXUmxPVGcyTWpFMFpqSmpPREl6TWpZNE1HWXlOemsyTURVaVhYMD0iLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9rZXl0eXBlIjoiUFJPRFVDVElPTiIsInNjb3BlcyI6Im9wZW5pZCIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2VtYWlsVmVyaWZpZWQiOiJ0cnVlIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvc3Vic2NyaWJlciI6ImRldi12c3NhZG1pbkB0cmltYmxlLmNvbSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL3VzZXJ0eXBlIjoiQVBQTElDQVRJT05fVVNFUiIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL3JvbGUiOiJwdWJsaXNoZXIiLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9sYXN0VXBkYXRlVGltZVN0YW1wIjoiMTQ5NzI3ODIwNDkyMiIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2FjY291bnR1c2VybmFtZSI6IkRhdmlkX0dsYXNzZW5idXJ5IiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvaWRlbnRpdHlcL3VubG9ja1RpbWUiOiIwIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvYWNjb3VudG5hbWUiOiJ0cmltYmxlLmNvbSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2ZpcnN0bmFtZSI6IkRhdmUiLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9wYXNzd29yZFBvbGljeSI6IkhJR0giLCJpc3MiOiJ3c28yLm9yZ1wvcHJvZHVjdHNcL2FtIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvbGFzdG5hbWUiOiJHbGFzc2VuYnVyeSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2FwcGxpY2F0aW9uaWQiOiIzNzQzIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvdmVyc2lvbiI6IjEuNCIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2VuZHVzZXIiOiJkYXZpZF9nbGFzc2VuYnVyeUB0cmltYmxlLmNvbSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL3V1aWQiOiJjZTc5YjRiNy0yYTZmLTQ3NTUtOWNhOS0zZTQ5Yzg3ZWI0YjciLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9lbmR1c2VyVGVuYW50SWQiOiIxIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvZ2l2ZW5uYW1lIjoiRGF2ZSIsImV4cCI6MTQ5ODE3NjM2MiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvaWRlbnRpdHlcL2ZhaWxlZExvZ2luQXR0ZW1wdHMiOiIwIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvaWRlbnRpdHlcL2FjY291bnRMb2NrZWQiOiJmYWxzZSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2FwaWNvbnRleHQiOiJcL3RcL3RyaW1ibGUuY29tXC92c3MtZGV2LXByb2plY3RzIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvbGFzdExvZ2luVGltZVN0YW1wIjoiMTQ5ODE2NTAxOTM3MCIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL3RpZXIiOiJVbmxpbWl0ZWQiLCJodHRwOlwvXC93c28yLm9yZ1wvY2xhaW1zXC9zdGF0dXMiOiJleUpDVEU5RFMwVkVJam9pWm1Gc2MyVWlMQ0pYUVVsVVNVNUhYMFpQVWw5RlRVRkpURjlXUlZKSlJrbERRVlJKVDA0aU9pSm1ZV3h6WlNJc0lrSlNWVlJGWDBaUFVrTkZYMHhQUTB0RlJDSTZJbVpoYkhObElpd2lRVU5VU1ZaRklqb2lkSEoxWlNKOSIsImh0dHA6XC9cL3dzbzIub3JnXC9jbGFpbXNcL2xhc3RQd2RTZXRUaW1lU3RhbXAiOiIxNDkxMTcwMTg3Mjk3IiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvYXBwbGljYXRpb250aWVyIjoiVW5saW1pdGVkIiwiaHR0cDpcL1wvd3NvMi5vcmdcL2NsYWltc1wvZW1haWxhZGRyZXNzIjoiRGF2aWRfR2xhc3NlbmJ1cnlAVHJpbWJsZS5jb20ifQ.d2n4ioMqEVmkQVYRcHaAhfayA1tt6b_Py6TlnFJtS2gL_b-gyU2g9g00sz1xq4gywPPZENhM1o6FX8dAA-HnVg2OIfp-unFDvB-jHo1-VEQxUQ--Ii04z0fE5Ed7NJkQjC-tUOpJD-wL62bACxB1e9nrpW8nlZoPACUUP6k6zI8";
      var jwtToken = new TPaaSJWT(jwt);
    }
	*/
  }
}
