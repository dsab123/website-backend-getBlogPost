using System;
using System.Text;

namespace GetBlogPost.Utility
{
  public interface IExceptionLogFormatter
  {
    string FormatExceptionLogMessage(Exception ex, StringBuilder builder = null);
  }
}
