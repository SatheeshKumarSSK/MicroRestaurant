using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Micro.Web.SD;

namespace Micro.Web.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}