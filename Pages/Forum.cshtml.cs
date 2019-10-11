using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServerDemo.Forum;

namespace ServerDemo.Pages
{
    public class ForumModel : PageModel
    {
        public ForumModel(ForumService forumService)
        {
            ForumService = forumService;
        }

        public ForumService ForumService { get; }

        public void OnGet()
        {
        }

        public void OnPost(ForumPost post)
        {
            ForumService.AddPost(post);
        }
    }
}
