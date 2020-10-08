using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore_UI.WASM.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(IFileListEntry fileListEntry, MemoryStream msFile, string pictureName);

        public void RemoveFile(string pictureName);
    }
    
}
