using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MetroLog;
using NetworkMonitor.Objects;
using NetworkMonitor.Objects.Factory;
using NetworkMonitor.Api.Services;

using System;
using System.Collections.Generic;


namespace NetworkMonitor.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ChatController : ControllerBase
    {
        private readonly ILogger _logger;
        private IApiService _apiService;

        public ChatController( INetLoggerFactory loggerFactory, IApiService apiService)
        {
            _apiService = apiService;
            _logger = loggerFactory.GetLogger("ChatController");
        }

        // A post method to call OpenAiService.CheckQuantum() to check if the supplied url is a quantum ready.
        // POST: api/Chat/CheckQuantum
        [HttpPost("CheckQuantum")]
        public async Task<ResultObj> CheckQuantum([FromBody] string url)
        {
            var result = new ResultObj();
            result.Message = " API : CheckQuantum :";
            try
            {
                result = await _apiService.CheckQuantum(url);
               
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check quantum. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }

              
    }
}
