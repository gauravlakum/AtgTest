using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public class LotRepository : ILotRepository
  {
    public Lot GetLot(int id)
    {
      return new Lot(){Id = id, IsArchived = false};
    }
  }
}
