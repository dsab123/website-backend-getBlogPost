using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using GetBlogPost.Configuration;
using GetBlogPost.DataAccess;
using GetBlogPost.Models;
using GetBlogPost.Utility;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.CamelCaseLambdaJsonSerializer))]

namespace GetBlogPost
{
  public class Function
  {
    public ISqlDataContext DataContext;
    private readonly IExceptionLogFormatter _exceptionLogFormatter;

    public Function()
    {
      var serviceCollection = new ServiceCollection();
      ConfigureServices(serviceCollection);
      var serviceProvider = serviceCollection.BuildServiceProvider();

      DataContext = serviceProvider.GetService<ISqlDataContext>();
      _exceptionLogFormatter = serviceProvider.GetService<IExceptionLogFormatter>();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="blogPost"></param>
    /// <returns></returns>
    public BlogPost FunctionHandler(BlogPost blogPost)
    {
      LambdaLogger.Log("GetBlogPost Lambda Started");

      try
      {
        LambdaLogger.Log("GetBlogPost Lambda finishing");
        DataContext.GetBlogPost(blogPost);

        return blogPost;
      }
      catch (Exception ex)
      {
        LambdaLogger.Log(_exceptionLogFormatter.FormatExceptionLogMessage(ex));
        throw;
      }
    }

    private void ConfigureServices(IServiceCollection serviceCollection)
    {
      serviceCollection.AddTransient<ILambdaConfiguration, LambdaConfiguration>();
      serviceCollection.AddTransient<ISqlDataContext, SqlDataContext>();
      serviceCollection.AddTransient<IExceptionLogFormatter, ExceptionLogFormatter>();
    }

    // used in local testing
    public static void Main()
    {
      var ret = new Function();
      ret.FunctionHandler(new BlogPost(1, "test", "test", "test", new List<string>{"tag"}, null));
    }
  }
}
