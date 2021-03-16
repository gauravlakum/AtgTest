using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public interface IArchivedRepository
  {
    Lot GetLot(int id);
  }
}
