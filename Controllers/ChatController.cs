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

        public ChatController(INetLoggerFactory loggerFactory, IApiService apiService)
        {
            _apiService = apiService;
            _logger = loggerFactory.GetLogger("ChatController");
        }

        // A post method to call OpenAiService.CheckQuantum() to check if the supplied url is a quantum ready.
        [HttpPost("CheckQuantum")]
        public async Task<ResultObj> CheckQuantum([FromBody] UrlObject urlObject)
        {
            var result = new ResultObj();
            result.Message = " API : CheckQuantum :";
            try
            {
                result = await _apiService.CheckQuantum(urlObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check quantum. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }

        // A post method to call OpenAiService.CheckSmtp() to check if the supplied url has a running stmp service.
        [HttpPost("CheckSmtp")]
        public async Task<ResultObj> CheckSmtp([FromBody] HostObject hostObject)
        {
            var result = new ResultObj();
            result.Message = " API : CheckSmtp :";
            try
            {
                result = await _apiService.CheckSmtp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check smtp. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }

        // A post method to call OpenAiService.CheckHttp() to check if the supplied url has a running http service. 
        [HttpPost("CheckHttp")]
        public async Task<ResultObj> CheckHttp([FromBody] HostObject hostObject)
        {
            var result = new ResultObj();
            result.Message = " API : CheckHttp :";
            try
            {
                result = await _apiService.CheckHttp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check http. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }

        // A post method to call OpenAiService.CheckDns() to check a dns lookup on the host
        [HttpPost("CheckDns")]
        public async Task<ResultObj> CheckDns([FromBody] HostObject hostObject)
        {
            var result = new ResultObj();
            result.Message = " API : CheckDns :";
            try
            {
                result = await _apiService.CheckDns(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check dns. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }

        // a post method to call OpenAiService.CheckIcmp() to check if host responds to a ping.
        [HttpPost("CheckIcmp")]
        public async Task<ResultObj> CheckIcmp([FromBody] HostObject hostObject)
        {
            var result = new ResultObj();
            result.Message = " API : CheckIcmp :";
            try
            {
                result = await _apiService.CheckIcmp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check icmp. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.Info(result.Message);
            return result;
        }
       

    }

}
