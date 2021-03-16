using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public interface IFailOverLotRepository
  {
    Lot GetLot(int id);
  }
}
