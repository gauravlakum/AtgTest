using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public class FailOverLotRepository : IFailOverLotRepository
  {
        public Lot GetLot(int id)
        {
            return new Lot();
        }
    }
}
