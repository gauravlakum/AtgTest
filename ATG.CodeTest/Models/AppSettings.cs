using System;
using System.Collections.Generic;
using System.Text;

namespace ATG.CodeTest.Models
{
  public class AppSettings
  {
    public bool IsFailOverModeEnabled = true;
    public int MaxFailedRequests  = 50;
    public int ExceptionThresholdPeriodInMin = 10;
  }
}
