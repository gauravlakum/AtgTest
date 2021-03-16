using System;
using System.Collections.Generic;
using System.Linq;
using ATG.CodeTest.Models;


namespace ATG.CodeTest.Repositories
{
  public class FailOverRepository : IFailOverRepository
  {
    private readonly AppSettings _appSettings;
    private readonly List<FailOverLot> _failOverLotEntries = new List<FailOverLot>();

    public FailOverRepository(AppSettings appSettings)
    {
      _appSettings = appSettings;
    }

    public void AddFailOver(FailOverLot failOverLot)
    {
      _failOverLotEntries.Add(failOverLot);
    }


    public List<FailOverLot> GetFailOvers()
    {
      return _failOverLotEntries;
    }

    public int  GetFailOverCount(DateTime startDate)
    {
      return _failOverLotEntries.Count(failOverLotsEntry =>
        failOverLotsEntry.DateTime > startDate.AddMinutes(_appSettings.ExceptionThresholdPeriodInMin));
    }

    public bool IsFailedRequestsThreshHoldReached()
    {
      return _failOverLotEntries.Count(failOverLotsEntry =>
               failOverLotsEntry.DateTime > DateTime.Now.AddMinutes(_appSettings.ExceptionThresholdPeriodInMin)) >
             _appSettings.MaxFailedRequests;
    }
  }
}
