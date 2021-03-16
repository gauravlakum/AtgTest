using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using ATG.CodeTest.Models;
using ATG.CodeTest.Repositories;
using Polly;

namespace ATG.CodeTest.Services
{
  public class LotService : ILotService
  {
    private readonly AppSettings _appSettings;
    private readonly IFailOverLotRepository _failOverLotRepository;
    private readonly ILotRepository _lotRepository;
    private readonly IArchivedRepository _archivedRepository;
    private readonly IFailOverRepository _failOverRepository;
    private readonly IFailOverService _failOverService;


    //All different lot repo interface can inherit from single ILotBaseRepository 
    public LotService(AppSettings appSettings,
      IFailOverLotRepository failOverLotRepository,
      ILotRepository lotRepository,
      IArchivedRepository archivedRepository,
      IFailOverRepository failOverRepository,
      IFailOverService failOverService)
    {
      _appSettings = appSettings;
      _failOverLotRepository = failOverLotRepository;
      _lotRepository = lotRepository;
      _archivedRepository = archivedRepository;
      _failOverRepository = failOverRepository;
      _failOverService = failOverService;
    }

    public Lot GetLot(int id, bool isLotArchived)
    {

      if (isLotArchived)
      {
        return _archivedRepository.GetLot(id);
      }

      if (_failOverService.IsInFailOverMode())
      {
        var failOverLot = _failOverLotRepository.GetLot(id);
        return !failOverLot.IsArchived ? failOverLot : _archivedRepository.GetLot(id);
      }

      var lot = _lotRepository.GetLot(id);
      return !lot.IsArchived ? lot : _archivedRepository.GetLot(id);

    }

    public Lot GetArchivedLot(int id)
    {
      //if (_failOverService.IsInFailOverMode())
      //{
      //  return _failOverLotRepository.GetLot(id);
      //}

      return _archivedRepository.GetLot(id);

    }

    public Lot GetLotWithPolly(int id, bool isLotArchived)
    {
      var handler = Policy<Lot>.Handle<Exception>();
      var retry = handler.Retry(_appSettings.MaxFailedRequests,
        (exception, count) => { _failOverRepository.AddFailOver(new FailOverLot { DateTime = DateTime.Now }); });

      var fallback = handler.Fallback(fallbackAction: token => _failOverLotRepository.GetLot(id));

      var result = fallback.Wrap(retry).Execute(() =>
      {
        if (isLotArchived)
        {
          return _archivedRepository.GetLot(id);
        }

        return _lotRepository.GetLot(id);
      });

      return result;

    }


  }
}
