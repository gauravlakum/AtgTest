using System;
using System.Collections.Generic;
using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public interface IFailOverRepository
  {
    //List<FailOverLot> GetFailOverLotEntries();
    //bool IsFailedRequestsThreshHoldReached();
    void AddFailOver(FailOverLot failOverLot);
    List<FailOverLot> GetFailOvers();
    int  GetFailOverCount(DateTime startDate);
  }
}
