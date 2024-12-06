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
using OpenAI.ObjectModels.ResponseModels;

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
            var authHeader = Request.Headers["x-rapidapi-proxy-secret"].FirstOrDefault();
            var altAuthHeader=Request.Headers["x-rapidapi-secret"].FirstOrDefault();
            if (authHeader != _apiService.OpenAIPluginServiceKey && altAuthHeader != _apiService.OpenAIPluginServiceKey)
            {
                _logger.LogError("Invalid RapidAPI Proxy Secret.");
                throw new UnauthorizedAccessException("Invalid RapidAPI Proxy Secret.");
            }
        }

        /// <summary>
        /// Check if api service is working should reply true.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>The result of service check</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the url is null or is unable to be converted to a URI</response>   
        [HttpGet("Check", Name = "Check")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ResultObj Check()
        {
            var result = new ResultObj();
            result.Message = " API : Check :";
            result.Success = true;

            try
            {
                AssertAuthHeader();
                result.Message +=" Api Service is running.";
                return result;
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check  is api service if running Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }




        // <snippet_CheckQuantum>
        /// <summary>
        /// Check if the supplied URL is using quantum-safe key encapsulation mechanisms.
        /// </summary>
        /// <remarks>
        /// To setup a quantum ready monitor visit : https://freenetworkmonitor.click/dashboard 
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
        [HttpPost("CheckQuantum", Name = "CheckQuantumOperation")]
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

        // <snippet_CheckSmtp>
        /// <summary>
        /// Check if the smtp server is responding with a helo message.
        /// </summary>
        /// <remarks>
        /// To setup an email service monitor visit : https://freenetworkmonitor.click/dashboard 
        /// 
        /// Sample request:
        ///
        ///     POST /Chat/CheckSmtp
        ///     {
        ///        "address": "smtp.gmail.com",
        ///        "port": 587
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
                        hostObject.EndPointType="smtp";
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

        // <snippet_CheckHttp>
        /// <summary>
        /// Check the response time and status of a website (HTTP).
        /// </summary>
        /// <remarks>
        /// To setup an HTTP service monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckHttp
        ///     {
        ///        "address": "https://www.cloudflare.com"
        ///     }
        /// </remarks>
        /// <returns>The result of the HTTP check.</returns>
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
                        hostObject.EndPointType="http";
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
        // </snippet_CheckHttp>

        // <snippet_CheckHttps>
        /// <summary>
        /// Check the response time and SSL certificate status of a website (HTTPS).
        /// </summary>
        /// <remarks>
        /// To setup an HTTPS service monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckHttps
        ///     {
        ///        "address": "https://www.cloudflare.com"
        ///     }
        /// </remarks>
        /// <returns>The result of the HTTPS check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or cannot be resolved</response>
        [HttpPost("CheckHttps", Name = "CheckHttpsOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckHttps([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckHttps :";
            AssertAuthHeader();
                        hostObject.EndPointType="https";
            try
            {
                result = await _apiService.CheckHttps(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check https. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckHttps>

        // <snippet_CheckDns>
        /// <summary>
        /// Perform a DNS lookup on the host address.
        /// </summary>
        /// <remarks>
        /// To setup a DNS monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckDns
        ///     {
        ///        "address": "google.com"
        ///     }
        /// </remarks>
        /// <returns>The result of the DNS check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or cannot be resolved</response>   
        [HttpPost("CheckDns", Name = "CheckDnsOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckDns([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckDns :";
            AssertAuthHeader();
                        hostObject.EndPointType="dns";
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

        // <snippet_CheckIcmp>
        /// <summary>
        /// Check the ping response time of a host (ICMP).
        /// </summary>
        /// <remarks>
        /// To setup a ping monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckIcmp
        ///     {
        ///        "address": "1.1.1.1"
        ///     }
        /// </remarks>
        /// <returns>The result of the ICMP ping check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or cannot be resolved</response>   
        [HttpPost("CheckIcmp", Name = "CheckIcmpOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckIcmp([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckIcmp :";
            hostObject.EndPointType="icmp";
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

        // <snippet_CheckNmap>
        /// <summary>
        /// Check connectivity using an Nmap-style connection test.
        /// </summary>
        /// <remarks>
        /// To setup an Nmap service monitor, visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckNmap
        ///     {
        ///        "address": "example.com",
        ///        "port": 80
        ///     }
        /// </remarks>
        /// <returns>The result of the Nmap check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or cannot be resolved</response>
        [HttpPost("CheckNmap", Name = "CheckNmapOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckNmap([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckNmap :";
            AssertAuthHeader();
                        hostObject.EndPointType="nmap";
            try
            {
                result = await _apiService.CheckNmap(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check nmap. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckNmap>

        // <snippet_CheckRawconnect>
        /// <summary>
        /// Check a raw socket connection to a given host and port.
        /// </summary>
        /// <remarks>
        /// To setup a raw socket monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckRawconnect
        ///     {
        ///        "address": "example.com",
        ///        "port": 443
        ///     }
        /// </remarks>
        /// <returns>The result of the Rawconnect check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address or port is null or invalid</response>
        [HttpPost("CheckRawconnect", Name = "CheckRawconnectOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckRawconnect([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckRawconnect :";
            AssertAuthHeader();
                        hostObject.EndPointType="rawconnect";
            try
            {
                result = await _apiService.CheckRawconnect(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check rawconnect. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckRawconnect>

        // <snippet_CheckCrawlSite>
        /// <summary>
        /// Crawl a website to generate traffic on the site. The crawl will browse internal links and scroll pages using a java script enabled web browser.
        /// </summary>
        /// <remarks>
        /// To setup a crawl site monitor visit : https://freenetworkmonitor.click/dashboard 
        ///
        /// Sample request:
        ///
        ///     POST /Chat/CheckCrawlSite
        ///     {
        ///        "address": "https://www.example.com"
        ///     }
        /// </remarks>
        /// <returns>The result of the CrawlSite check.</returns>
        /// <response code="201">Returns a result object</response>
        /// <response code="400">If the address is null or cannot be resolved</response>
        [HttpPost("CheckCrawlSite", Name = "CheckCrawlSiteOperation")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<TResultObj<DataObj>> CheckCrawlSite([FromBody] HostObject hostObject)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " API : CheckCrawlSite :";
            AssertAuthHeader();
                        hostObject.EndPointType="crawlsite";
            try
            {
                result = await _apiService.CheckCrawlSite(hostObject);
            }
            catch (Exception ex)
            {
                result.Message += " Error : Failed to check crawl site. Error was : " + ex.Message;
                result.Success = false;
            }
            _logger.LogInformation(result.Message);
            return result;
        }
        // </snippet_CheckCrawlSite>
    }
}
