using Micro.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}