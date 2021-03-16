using System;
using ATG.CodeTest.Models;
using Polly;

namespace ATG.CodeTest.Services
{
  public interface ILotService
  {
    Lot GetLot(int id, bool isLotArchived);
    Lot GetArchivedLot(int id);
    Lot GetLotWithPolly(int id, bool isLotArchived);
  }
}
