using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ServerDemo.Forum
{
    public class ForumService
    {
        private readonly string _storePath;
        private readonly List<ForumPost> _posts = new List<ForumPost>();
        private readonly ILogger<ForumService> _logger;
        private int _nextId = 1;

        public ForumService(IWebHostEnvironment env, IConfiguration configuration, ILogger<ForumService> logger)
        {
            var contentRoot = env.ContentRootPath;
            _storePath = configuration["Forum:StorePath"];
            if (string.IsNullOrEmpty(_storePath))
            {
                _storePath = "forum.xml";
            }
            if (!Path.IsPathFullyQualified(_storePath))
            {
                _storePath = Path.Combine(contentRoot, _storePath);
            }
            _logger = logger;

            Load();
        }

        // Only called from constructor, doesn't need to lock
        private void Load()
        {
            try
            {
                if (!File.Exists(_storePath))
                {
                    _logger.LogError("The forum file does not exist at '{path}'.", _storePath);
                    return;
                }

                var doc = new XmlDocument();
                doc.Load(_storePath);
                var postSection = doc.DocumentElement.FirstChild;
                while (postSection != null)
                {
                    var post = new ForumPost();
                    post.User = postSection.Attributes["user"].Value;
                    post.Id = int.Parse(postSection.Attributes["id"].Value, CultureInfo.InvariantCulture);
                    post.PostedAt = DateTimeOffset.Parse(postSection.Attributes["postedAt"].Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    post.Content = postSection.FirstChild.Value;

                    _posts.Add(post);

                    if (post.Id >= _nextId)
                    {
                        _nextId = post.Id + 1;
                    }

                    postSection = postSection.NextSibling;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to load the forum file from '{path}'.", _storePath);
            }
        }

        // Called under lock
        private void Save()
        {
            // Intentionally doesn't catch exceptions so you can see what's wrong.
            var doc = new XmlDocument();
            var root = doc.CreateElement("posts");
            doc.AppendChild(root);
            foreach (var post in _posts)
            {
                var postSection = doc.CreateElement("post");
                postSection.SetAttribute("id", post.Id.ToString(CultureInfo.InvariantCulture));
                postSection.SetAttribute("user", post.User);
                postSection.SetAttribute("postedAt", post.PostedAt.ToString("o"));
                var content = doc.CreateTextNode(post.Content);
                postSection.AppendChild(content);

                root.AppendChild(postSection);
            }
            doc.Save(_storePath);
        }

        public IEnumerable<ForumPost> GetPosts()
        {
            lock (_posts)
            {
                return _posts.ToArray();
            }
        }

        public void AddPost(ForumPost forumPost)
        {
            lock (_posts)
            {
                forumPost.Id = _nextId++;
                forumPost.PostedAt = DateTimeOffset.UtcNow;
                _posts.Add(forumPost);
                Save();
            }
        }

        public void DeletePost(int id)
        {
            lock (_posts)
            {
                for (int i = 0; i < _posts.Count; i++)
                {
                    if (_posts[i].Id == id)
                    {
                        _posts.RemoveAt(i);
                        break;
                    }
                }
                Save();
            }
        }
    }
}
