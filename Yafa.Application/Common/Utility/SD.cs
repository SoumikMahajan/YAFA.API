using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yafa.Application.Common.Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string AccessToken = "JWTToken";
        public enum ContentType
        {
            Json,
            MultipartFormData,
        }

        public static DateTime MinDate = new(1900, 1, 1, 0, 0, 0, 0);
    }
}
