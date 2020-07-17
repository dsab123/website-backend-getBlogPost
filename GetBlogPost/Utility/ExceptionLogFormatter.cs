using System;
using System.Text;

namespace GetBlogPost.Utility
{
  public class ExceptionLogFormatter : IExceptionLogFormatter
  {
    public string FormatExceptionLogMessage(Exception ex, StringBuilder builder = null)
    {
      builder = builder ?? new StringBuilder();

      builder.Append($"\tMessage: {ex.Message}");
      builder.AppendLine();

      builder.Append($"\tStacktrace: {ex.StackTrace}");
      builder.AppendLine();

      if (ex.InnerException != null)
      {
        builder.Append($"\tInner Exception: {ex.InnerException}");
        builder.AppendLine();
      }

      return builder.ToString();
    }
  }
}
