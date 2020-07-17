using System.Collections.Generic;
using Newtonsoft.Json;

namespace GetBlogPost.Models
{
  public class BlogPost
  {
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "content")]
    public string Content { get; set; }

    [JsonProperty(PropertyName = "slug")]
    public string Slug { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [JsonProperty(PropertyName = "tags")]
    public List<string> Tags { get; set; }

    [JsonProperty(PropertyName = "relatedPosts")]
    public List<BlogPost> RelatedPosts;

    [JsonConstructor]
    public BlogPost(int id, string content, string slug, string title, List<string> tags, List<BlogPost> relatedPosts)
    {
      Id = id;
      Content = content;
      Slug = slug;
      Title = title;
      Tags = tags;
      RelatedPosts = relatedPosts ?? new List<BlogPost>();
    }

    [JsonConstructor]
    public BlogPost() { }
  }
}