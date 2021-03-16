using ATG.CodeTest.Models;

namespace ATG.CodeTest.Repositories
{
  public class ArchivedRepository : IArchivedRepository
  {
        public Lot GetLot(int id)
        {
            return new Lot(){Id =  id, IsArchived = true};
        }

  }
}
