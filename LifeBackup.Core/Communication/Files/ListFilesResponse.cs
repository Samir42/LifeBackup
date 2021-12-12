using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeBackup.Core.Communication.Files
{
    public class ListFilesResponse
    {
        public string Key { get; set; }
        public string BucketName { get; set; }
        public string Owner { get; set; }
        public long Size { get; set; }
    }
}
