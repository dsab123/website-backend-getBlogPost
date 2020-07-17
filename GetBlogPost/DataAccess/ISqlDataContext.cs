using System.Threading.Tasks;
using GetBlogPost.Models;

namespace GetBlogPost.DataAccess
{
  public interface ISqlDataContext
  {
    BlogPost GetBlogPost(BlogPost blogpost);
  }
}
