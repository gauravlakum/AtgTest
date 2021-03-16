using System;
using ATG.CodeTest.Models;
using ATG.CodeTest.Repositories;
using ATG.CodeTest.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace ATG.CodeTest.UnitTests
{
  [TestFixture]
  public class LotServiceTests
  {
    private AppSettings _appSettings;
    private IFailOverLotRepository _failOverLotRepository;
    private ILotRepository _lotRepository;
    private IArchivedRepository _archivedRepository;
    private IFailOverRepository _failOverRepository;
    private LotService _sut;
    private IFailOverService _failOverService;


    [SetUp]
    public void SetUp()
    {
      _appSettings = Substitute.For<AppSettings>();
      _failOverLotRepository = Substitute.For<IFailOverLotRepository>();
      _lotRepository = Substitute.For<ILotRepository>();
      _archivedRepository = Substitute.For<IArchivedRepository>();
      _failOverRepository = Substitute.For<IFailOverRepository>();
      _failOverService = Substitute.For<IFailOverService>();

      _sut = new LotService(_appSettings,
        _failOverLotRepository,
        _lotRepository,
        _archivedRepository,
        _failOverRepository,
        _failOverService);
    }

    #region GetLotTests

    [Test]
    public void GetLot_WhenValidIdProvidedAndArchiveFlagIsTrue_ShouldReturnArchivedLot()
    {
      //Arrange
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = true });

      //Act
      var lot = _sut.GetLot(1, true);


      //Assert
      Assert.AreEqual(lot.IsArchived, true);
    }

    [Test]
    public void GetLot_WhenValidIdAndArchiveFlagIsTrue_ShouldReturnLotFromArchiveLotStorage()
    {
      //Arrange
      _appSettings.MaxFailedRequests = 5;
      _appSettings.IsFailOverModeEnabled = false;
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = false });
      //Act
      var lot = _sut.GetLot(1, true);

      //Assert
      _archivedRepository.Received(1).GetLot(Arg.Any<int>());
      _lotRepository.Received(0).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(0).GetLot(Arg.Any<int>());

      //Don't need to check if system in fail over mode
      _failOverService.Received(0).IsInFailOverMode();

    }

    [Test]
    public void GetLot_WhenValidIdProvidedAndArchiveFlagIsFalse_ShouldReturnLotFromNormalLotStorage()
    {
      //Arrange
      _failOverService.IsInFailOverMode().Returns(false);
      _lotRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = false });
      //Act
      var lot = _sut.GetLot(1, false);


      //Assert
      _lotRepository.Received(1).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(0).GetLot(Arg.Any<int>());
      _archivedRepository.Received(0).GetLot(Arg.Any<int>());
    }

    [Test]
    public void GetLot_WhenValidIdProvidedAndArchiveFlagIsFalseButLotIsArchived_ShouldReturnLotFromArchivedLotStorage()
    {
      //Arrange
      _failOverService.IsInFailOverMode().Returns(false);
      _lotRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = true });
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = true });

      //Act
      var lot = _sut.GetLot(1, false);

      //Assert
      _lotRepository.Received(1).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(0).GetLot(Arg.Any<int>());
      _archivedRepository.Received(1).GetLot(Arg.Any<int>());
      Assert.AreEqual(lot.IsArchived, true);
    }

    [Test]
    public void GetLot_WhenValidIdProvideAndApplicationIsInFailOverMode_ShouldReturnLotFromFailOverLotStorage()
    {
      //Arrange
      _failOverService.IsInFailOverMode().Returns(true);
      _failOverLotRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = true });
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = true });
      //Act
      var lot = _sut.GetLot(1, false);

      //Assert
      _archivedRepository.Received(1).GetLot(Arg.Any<int>());
      _lotRepository.Received(0).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(1).GetLot(Arg.Any<int>());
    }

    [Test]
    public void GetLot_WheApplicationIsInFailOverModeButLotIsArchived_ShouldReturnLotFromArchiveOverLotStorage()
    {
      //Arrange
      _failOverService.IsInFailOverMode().Returns(true);
      _failOverLotRepository.GetLot(Arg.Any<int>()).Returns(new Lot { IsArchived = false });
      //Act
      var lot = _sut.GetLot(1, false);

      //Assert
      _archivedRepository.Received(0).GetLot(Arg.Any<int>());
      _lotRepository.Received(0).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(1).GetLot(Arg.Any<int>());
    }

    #endregion

    #region GetArchivedLot

    [Test]
    public void GetArchivedLot_WhenValidIdProvided_ShouldReturnArchivedLot()
    {
      //Arrange
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot() {IsArchived = true});
      //Act
      var lot = _sut.GetArchivedLot(1);

      //Assert
      Assert.AreEqual(lot.IsArchived, true);
    }

    [Test]
    public void GetArchivedLot_WhenValidIdProvided_ShouldReturnLotFromArchivedStorage()
    {
      //Arrange
      _archivedRepository.GetLot(Arg.Any<int>()).Returns(new Lot() { IsArchived = true });
      //Act
      var lot = _sut.GetArchivedLot(1);

      //Assert
      _archivedRepository.Received(1).GetLot(Arg.Any<int>());

    }

    #endregion

    [Test]
    public void GetLotWithPolly_WhenMaxFailedRequestsReached_ShouldReturnLotFromFailOverLotStorage()
    {
      //Arrange
      _appSettings.MaxFailedRequests = 5;
      _appSettings.IsFailOverModeEnabled = true;
      _lotRepository.GetLot(Arg.Any<int>()).Throws(new Exception("Exception occurred"));
      _failOverLotRepository.GetLot(Arg.Any<int>()).Returns((info) => new Lot { Id = info.ArgAt<int>(0) });

      //Act
      var lot = _sut.GetLotWithPolly(5, false);

      //Assert
      _lotRepository.Received(_appSettings.MaxFailedRequests + 1).GetLot(Arg.Any<int>());
      _failOverLotRepository.Received(1).GetLot(Arg.Any<int>());
      _archivedRepository.Received(0).GetLot(Arg.Any<int>());
      Assert.AreEqual(lot.Id, 5);

    }


  }
}
