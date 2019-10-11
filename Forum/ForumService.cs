using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerDemo.Forum
{
    public class ForumService
    {
        private readonly string _storePath;
        private readonly List<ForumPost> _posts = new List<ForumPost>();
        private int _nextId = 1;

        public ForumService(IWebHostEnvironment env, IConfiguration configuration)
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

            Load();
        }

        // Only called from constructor, doesn't need to lock
        private void Load()
        {
            // TODO: make sure _nextId is larger than any existing ID
            if (File.Exists(_storePath))
            {
            }
        }

        // Called under lock
        private void Save()
        {
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
