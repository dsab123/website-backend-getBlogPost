using Microsoft.Extensions.Configuration;

namespace GetBlogPost.Configuration
{
  public interface ILambdaConfiguration
  {
    IConfiguration Configuration { get; }
  }
}
