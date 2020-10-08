using BlazorInputFile;
using Bookstore_UI.WASM.Contracts;
//using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Service
{
    //public class FileUpload : IFileUpload
    //{
    //    private readonly IWebHostEnvironment _env;

    //    public FileUpload(IWebHostEnvironment env)
    //    {
    //        _env = env;
    //    }

    //    public void RemoveFile(string pictureName)
    //    {
    //        var path = $"{_env.WebRootPath}\\uploads\\{pictureName}";

    //        if(File.Exists(path))
    //        {
    //            File.Delete(path);
    //        }
    //    }

    //    public async Task UploadFile(IFileListEntry fileListEntry, MemoryStream msFile, string pictureName)
    //    {
    //        try
    //        {
    //            //var ms = new MemoryStream();
    //            //await fileListEntry.Data.CopyToAsync(ms);

    //            var path = $"{_env.WebRootPath}\\uploads\\{pictureName}";

    //            using (FileStream fs = new FileStream(path, FileMode.Create))
    //            {
    //                msFile.WriteTo(fs);
    //            }

    //        }
    //        catch (Exception e)
    //        {

    //            throw;
    //        }
    //    }
    //}
}
