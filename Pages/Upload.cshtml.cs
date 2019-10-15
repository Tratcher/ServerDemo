using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServerDemo.Pages
{
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 25)]
    public class UploadModel : PageModel
    {
        private string uploadDir;

        public UploadModel(IWebHostEnvironment env)
        {
            uploadDir = Path.Combine(env.ContentRootPath, "uploaded");
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var output = System.IO.File.OpenWrite(Path.Combine(uploadDir, file.FileName));
            await file.CopyToAsync(output);
            return Redirect("/uploads");
        }
    }
}
