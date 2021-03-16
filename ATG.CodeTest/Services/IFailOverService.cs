using System.Collections.Generic;
using System.Text;
using NSubstitute;

namespace ATG.CodeTest.Services
{
  public interface IFailOverService
  {
    bool IsInFailOverMode();
  }
}
