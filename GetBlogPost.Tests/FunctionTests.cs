using System.Collections.Generic;
using GetBlogPost;
using Moq;
using Xunit;
using GetBlogPost.DataAccess;
using GetBlogPost.Models;

namespace GetBlogPost.Tests
{
  public class FunctionTests
  {
    // we need to override the constructor so that we don't do any of that snazzy configuration,
    // and also to set the dataContext in an easier manner
    private class FunctionExtractAndOverride : Function
    {
      private readonly Mock<ISqlDataContext> _mockContext;

      public FunctionExtractAndOverride()
      {
        _mockContext = new Mock<ISqlDataContext>();
        _mockContext.Setup(c => c.GetBlogPost(It.IsAny<BlogPost>()))
          .Callback<BlogPost>((blogpost) =>
          {
            blogpost.Slug = "slug";
            blogpost.Content = "content";
            blogpost.Title = "title";
            blogpost.Tags = new List<string> { "one", "two" };
            blogpost.RelatedPosts = null;
          });
        
        DataContext = _mockContext.Object;
      }
    }

    [Fact]
    public void FunctionHandler_ContextReturnsValidList_JsonSerializationIsValid()
    {
      // Arrange
      var function = new FunctionExtractAndOverride();

      var blogpost = new BlogPost(4, null, null, null, null, null);
      
      // Act
      function.FunctionHandler(blogpost);

      // Assert
      Assert.Equal(4, blogpost.Id);
      Assert.Equal("title", blogpost.Title);
      Assert.Equal("content", blogpost.Content);
      Assert.Equal("slug", blogpost.Slug);
      Assert.Equal(new List<string> { "one", "two" }, blogpost.Tags);
      Assert.Null(blogpost.RelatedPosts);
    }
  }
}