using System.Linq;
using ATG.CodeTest.Models;
using ATG.CodeTest.Repositories;

namespace ATG.CodeTest.Services
{
  public class FailOverService : IFailOverService
  {
    private readonly IDateTimeService _dateTimeService;
    private readonly IFailOverRepository _failOverRepository;
    private readonly AppSettings _appSettings;

    public FailOverService(IDateTimeService dateTimeService,
      IFailOverRepository failOverRepository,
      AppSettings appSettings
    )
    {
      _dateTimeService = dateTimeService;
      _failOverRepository = failOverRepository;
      _appSettings = appSettings;
    }

    public bool IsInFailOverMode()
    {
      if (!_appSettings.IsFailOverModeEnabled) return false;

      //getting to many data, change repository to filter data with in required time frame
      //var fails = _failOverRepository.GetFailOverCount(_dateTimeService.Now());

      var failOvers = _failOverRepository.GetFailOvers();

      var failCount = failOvers.Count(fails => fails.DateTime > _dateTimeService.Now().AddMinutes(_appSettings.ExceptionThresholdPeriodInMin * -1));

      return failCount > _appSettings.MaxFailedRequests;

    }
  }
}
