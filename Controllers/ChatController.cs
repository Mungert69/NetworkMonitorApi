using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public ChatController(ILogger<ChatController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

         private void AssertAuthHeader()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            //_logger.LogWarning("authHeader is : "+authHeader);
            if (authHeader != $"Bearer {_apiService.OpenAIPluginServiceKey}")
            {
                throw new UnauthorizedAccessException();
            }
        }

        // <snippet_CheckQuantum>
        /// <summary>
        /// Check if the supplied URL is using quantum-safe key encapsulation mechanisms.
        /// </summary>
        /// <remarks>
        /// To setup a quantum ready monitor visit : https://freenetworkmonitor.click/dashboard to Login and create a free account then add hosts to start monitoring.
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckQuantum
        ///     {
        ///        "url": "https://cloudflare.com"
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of the quantum check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the url is null or is unable to be converted to a URI</response>   
        [HttpPost("CheckQuantum",Name = "CheckQuantumOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<QuantumDataObj>> CheckQuantum([FromBody] QuantumHostObject quantumHostObject)
        {
            var result = new TResultObj<QuantumDataObj>();
            result.Message = " API : CheckQuantum :";
            AssertAuthHeader();
            try
            {
                result = await _apiService.CheckQuantum(quantumHostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check quantum. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckQuantum>

        // A post method to call OpenAiService.CheckSmtp() to check if the supplied url has a running stmp service.
        // <snippet_CheckSmtp>
        /// <summary>
        /// Check if the stmp server is responding with a helo message.
        /// </summary>
        /// <remarks>
        /// To setup a email service monitor visit : https://freenetworkmonitor.click/dashboard to Login and create a free account then add hosts to start monitoring.
        ///     POST /Chat/CheckSmtp
        ///     {
        ///        "address": "smtp.gmail.com",
        ///        "port": 587,
        ///     }
        ///
        /// </remarks>
        /// <returns>The result of the smtp check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or is unable to be converted to a string</response>   
        [HttpPost("CheckSmtp", Name = "CheckSmtpOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckSmtp([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckSmtp :";
            AssertAuthHeader();
            try
            {
                result = await _apiService.CheckSmtp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check smtp. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckSmtp>

        // A post method to call OpenAiService.CheckHttp() to check if the supplied url has a running http service. 
        // <snippet_CheckHttp>
        /// <summary>
        /// Check the response time and status of a website.
        /// </summary>
        /// <remarks>
        /// To setup a http service monitor visit : https://freenetworkmonitor.click/dashboard to Login and create a free account then add hosts to start monitoring.
        ///     POST /Chat/CheckHttp
        ///     {
        ///        "address": "https://www.cloudflare.com"
        ///     }
        /// </remarks>
        /// <returns>The result is returned with the status of the check</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or is unable to be converted to a URI</response>   
        [HttpPost("CheckHttp", Name = "CheckHttpOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckHttp([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckHttp :";
            AssertAuthHeader();
            try
            {
                result = await _apiService.CheckHttp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check http. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }

        // A post method to call OpenAiService.CheckDns() to check a dns lookup on the host
        // <snippet_CheckDns>
        /// <summary>
        /// Perform a dns lookup on the host address.
        /// </summary>
        /// <remarks>
        /// To setup a dns monitor visit : https://freenetworkmonitor.click/dashboard to Login and create a free account then add hosts to start monitoring.
        ///     POST /Chat/CheckDns
        ///     {
        ///        "address": "google.com"
        ///     }
        /// </remarks>
        /// <returns>The result is returned with the status of the check</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or is unable to be converted to a URI</response>   

        [HttpPost("CheckDns", Name = "CheckDnsOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckDns([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckDns :";
            AssertAuthHeader();
            try
            {
                result = await _apiService.CheckDns(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check dns. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckDns>

        // a post method to call OpenAiService.CheckIcmp() to check if host responds to a ping.
           // <snippet_CheckIcmp>
        /// <summary>
        /// Check the ping response time of a host.
        /// </summary>
        /// <remarks>
        /// To setup a ping monitor visit : https://freenetworkmonitor.click/dashboard . You can Login to create a free account and then add hosts to monitor.
        ///     POST /Chat/CheckIcmp
        ///     {
        ///        "address": "1.1.1.1"
        ///     }
        /// </remarks>
        /// <returns>The result is returned with the status of the check</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or is unable to be converted to a URI</response>   

        [HttpPost("CheckIcmp", Name = "CheckIcmpOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckIcmp([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckIcmp :";
            AssertAuthHeader();
            try
            {
                result = await _apiService.CheckIcmp(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check icmp. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckIcmp>


    }

}
