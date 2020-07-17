using GetBlogPost.Configuration;
using GetBlogPost.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.Lambda.Core;
using GetBlogPost.Utility;
using Newtonsoft.Json;

namespace GetBlogPost.DataAccess
{
  public class SqlDataContext : ISqlDataContext, IDisposable
  {
    private readonly ILambdaConfiguration _lambdaConfiguration;
    private readonly IExceptionLogFormatter _exceptionLogFormatter;
    private NpgsqlConnection Connection { get; }

    public SqlDataContext(ILambdaConfiguration lambdaConfiguration, IExceptionLogFormatter exceptionLogFormatter)
    {
      _lambdaConfiguration = lambdaConfiguration;
      _exceptionLogFormatter = exceptionLogFormatter;

      Connection = CreateConnection();
    }

    // TODO - genericize the input model so that this can be reused in a Lambda Layer
    public BlogPost GetBlogPost(BlogPost blogpost)
    {
      try
      {
        using (var command = new NpgsqlCommand(
          @$"select bi.blogpost_id, bc.content, bi.slug, bi.title, json_agg(t.tag_name) as tags
            from public.blogpostinfo as bi
            inner join public.blogpostcontent as bc on bc.blogpost_id = bi.blogpost_id
            inner join public.blogpostid_tag as bt on bt.blogpost_id = bi.blogpost_id
            inner join public.tag t on t.tag_id = bt.tag_id
            where bi.blogpost_id = {blogpost.Id}
            group by bi.blogpost_id, bc.content, bi.slug, bi.title",
          Connection))
        { 
          Connection.Open();
          
          var reader = command.ExecuteReader();

          while (reader.Read())
          {
            blogpost.Slug = reader["slug"].ToString();
            blogpost.Title = reader["title"].ToString();
            blogpost.Tags = JsonConvert.DeserializeObject<List<string>>(reader["tags"].ToString());
            blogpost.Content = reader["content"].ToString();
          }
        }
      }
      catch (Exception ex)
      {
        LambdaLogger.Log(_exceptionLogFormatter.FormatExceptionLogMessage(ex));
      }

      Connection.Close();
      return blogpost;
    }

    private NpgsqlConnection CreateConnection()
    {
      if (Connection != null)
      {
        return Connection;
      }

      try
      {
        var section = _lambdaConfiguration.Configuration.GetSection("AppSettings");

        var server = section["Server"];
        var username = section["Username"];
        var database = section["Database"];
        var password = section["Password"];

        return new NpgsqlConnection(string.Format($"Database={database};Host={server};User ID={username};Password={password}"));
      }
      catch (Exception ex)
      {
        LambdaLogger.Log(_exceptionLogFormatter.FormatExceptionLogMessage(ex, new StringBuilder("ConnectionString was not retrieved from configuration, probably.")));
        throw;
      }
    }

    public void Dispose()
    {
      Connection.Dispose();
    }
  }
}
