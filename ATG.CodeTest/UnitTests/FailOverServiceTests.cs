using System;
using System.Collections.Generic;
using System.Text;
using ATG.CodeTest.Models;
using ATG.CodeTest.Repositories;
using ATG.CodeTest.Services;
using Castle.Components.DictionaryAdapter;
using NSubstitute;
using NUnit.Framework;

namespace ATG.CodeTest.UnitTests
{
  [TestFixture]
  public class FailOverServiceTests
  {
    private IFailOverService _sut;
    private IDateTimeService _dateTimeService;
    private IFailOverRepository _failOverRepository;
    private AppSettings _appSettings;

    [SetUp]
    public void SetUp()
    {
      _dateTimeService = Substitute.For<IDateTimeService>();
      _failOverRepository = Substitute.For<IFailOverRepository>();
      _appSettings = Substitute.For<AppSettings>();
      _sut = new FailOverService(_dateTimeService, _failOverRepository, _appSettings);
    }

    [Test]
    public void IsInFailOverMode_WhenFailOverModeIsFalse_ShouldReturnFalse()
    {
      //Arrange
      _appSettings.IsFailOverModeEnabled = false;

      //Act
      var result = _sut.IsInFailOverMode();

      //Assert
      Assert.AreEqual(false, result);

    }

    [Test]
    public void IsInFailOverMode_WhenFailOverModeIsTrueAndFailsCountExceedsThreshHold_ShouldReturnTrue()
    {
      //Arrange
      _appSettings.IsFailOverModeEnabled = true;
      _appSettings.MaxFailedRequests = 5;
      _dateTimeService.Now().Returns(DateTime.Now);
      _failOverRepository.GetFailOvers().Returns(new List<FailOverLot>()
      {
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)}
      });

      //Act
      var result = _sut.IsInFailOverMode();

      //Assert
      Assert.AreEqual(true, result);

    }

    [Test]
    public void IsInFailOverMode_WhenFailOverModeIsTrueAndFailsCountNotExceedsThreshHold_ShouldReturnTrue()
    {
      //Arrange
      _appSettings.IsFailOverModeEnabled = true;
      _appSettings.MaxFailedRequests = 5;
      _dateTimeService.Now().Returns(DateTime.Now);
      _failOverRepository.GetFailOvers().Returns(new List<FailOverLot>()
      {
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
        new FailOverLot {DateTime = DateTime.Now.AddMinutes(-5)},
      });

      //Act
      var result = _sut.IsInFailOverMode();

      //Assert
      Assert.AreEqual(false, result);

    }

  }
}
