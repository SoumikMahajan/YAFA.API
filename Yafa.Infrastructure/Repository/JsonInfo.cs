using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Infrastructure.Repository
{
    public class JsonInfo<T>
    {
        public string Status { get; set; } = string.Empty;
        public List<T> Response { get; set; } = new();
    }

    public class ResponseData<T>
    {
        public List<T> Response { get; set; } = new();
    }
}
