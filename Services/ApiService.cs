using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMonitor.Objects;
using NetworkMonitor.Objects.ServiceMessage;
using NetworkMonitor.Connection;
using NetworkMonitor.Utils.Helpers;
using System.Web;
using Microsoft.Extensions.Logging;
using NetworkMonitor.Objects.Factory;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Globalization;
using System.IO;
namespace NetworkMonitor.Api.Services
{
    public interface IApiService
    {
        Task<TResultObj<QuantumDataObj>> CheckQuantum(UrlObject url);
        Task<TResultObj<DataObj>> CheckSmtp(HostObject host);
        Task<TResultObj<DataObj>> CheckHttp(HostObject host);
        Task<TResultObj<DataObj>> CheckDns(HostObject host);
        Task<TResultObj<DataObj>> CheckIcmp(HostObject host);

        string OpenAIPluginServiceKey { get; set; }

    }
    public class ApiService : IApiService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CancellationToken _token;
        private readonly PingParams _pingParams;
        private readonly NetConnectCollection _netConnectCollection;
        private ISystemParamsHelper _systemParamsHelper;
        private string _frontEndUrl = "https://freenetworkmonior.click";
        private string _openAIPluginServiceKey;

        public string OpenAIPluginServiceKey { get => _openAIPluginServiceKey; set => _openAIPluginServiceKey = value; }

        public ApiService(IConfiguration config, ILogger<ApiService> logger, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, CancellationTokenSource cancellationTokenSource, ISystemParamsHelper systemParamsHelper)
        {
            _config = config;
            _systemParamsHelper = systemParamsHelper;
            _frontEndUrl = _config["FrontEndUrl"] != null ? _config["FrontEndUrl"] : _frontEndUrl;
            OpenAIPluginServiceKey = _config["OpenAIPluginServiceKey"];
            if (OpenAIPluginServiceKey == null)
            {

                throw new ArgumentException(" Fatal error could not load OpenAIPluginServiceKey from appsettings.json");
            }
            _token = cancellationTokenSource.Token;
            _token.Register(() => OnStopping());
            _scopeFactory = scopeFactory;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _pingParams = _systemParamsHelper.GetPingParams();
            var netConnectConfig = new NetConnectConfig(_config);
            var connectFactory = new ConnectFactory(_logger, true);
            _netConnectCollection = new NetConnectCollection(_logger, netConnectConfig, connectFactory);
            if (_netConnectCollection != null)
                _netConnectCollection.SetPingParams(_pingParams);
            else
            {
                throw new ArgumentException(" Failed to create NetConnectCollection setup of it returned null");
            }


        }

        // OnStopping() method
        public void OnStopping()
        {
            _logger.LogInformation("OnStopping");
        }
        public async Task<TResultObj<QuantumDataObj>> CheckQuantum(UrlObject urlObj)
        {
            var result = new TResultObj<QuantumDataObj>();
            result.Message = " SERVICE : CheckQuantum : ";
            try
            {
                //var monitorPingInfos = new List<MonitorPingInfo>();
                //monitorPingInfos.Add();
                var monitorPingInfo = new MonitorPingInfo()
                {
                    Address = urlObj.Url,
                    Port = urlObj.Port,
                    EndPointType = "quantum",
                    Timeout = 20000,
                };

                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                // await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = _netConnectCollection.GetNetConnectInstance(monitorPingInfo);
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.Message;
                result.Success = netConnect.MpiConnect.IsUp;
                var data = new QuantumDataObj();
                data.TestedUrl = urlObj.Url;
                data.ResultSuccess = netConnect.MpiConnect.IsUp;
                if (netConnect.MpiConnect.PingInfo.RoundTripTime != UInt16.MaxValue) data.ResponseTime = netConnect.MpiConnect.PingInfo.RoundTripTime;
                else data.Timeout = netConnect.MpiStatic.Timeout;
                string[] splitData = result.Message.Split(':');
                if (splitData.Length > 3)
                {
                    data.QuantumKeyExchange = splitData[3];
                }
                if (splitData.Length > 2)
                {
                    data.ResultStatus = splitData[2];
                }
                result.Data = data;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.LogError(result.Message);

            }
            return result;
        }


        public async Task<TResultObj<DataObj>> CheckHttp(HostObject hostObj)
        {
            var result = new TResultObj<DataObj>();
            /*var urlObj= new UrlObject();
            urlObj.Url=hostObj.Address;
            // some convoluted logic due to way HttpConnect handles urls.
            if (hostObj.Port == 0) hostObj.Port = urlObj.Port;
            if (urlObj.Port==80 || urlObj.Port==443)
            {
                hostObj.Port = 0;
            }*/
            result.Message = " SERVICE : CheckHttp : ";
            try
            {
                //var monitorPingInfos = new List<MonitorPingInfo>();
                //monitorPingInfos.Add();
                var monitorPingInfo = new MonitorPingInfo()
                {
                    Address = hostObj.Address,
                    Port = hostObj.Port,
                    EndPointType = "http",
                    Timeout = hostObj.Timeout,
                };

                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                // await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = _netConnectCollection.GetNetConnectInstance(monitorPingInfo);
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.PingInfo.Status;
                result.Success = netConnect.MpiConnect.IsUp;
                var data = new DataObj();
                data.TestedAddress = netConnect.MpiStatic.Address;
                data.TestedPort = netConnect.MpiStatic.Port;
                if (netConnect.MpiConnect.PingInfo.RoundTripTime != UInt16.MaxValue) data.ResponseTime = netConnect.MpiConnect.PingInfo.RoundTripTime;
                else data.Timeout = netConnect.MpiStatic.Timeout;
                data.ResultSuccess = netConnect.MpiConnect.IsUp;
                string[] splitData = result.Message.Split(':');
                if (splitData.Length > 2)
                {
                    data.ResultStatus = splitData[2];
                }
                result.Data = data;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.LogError(result.Message);

            }
            return result;
        }
        public async Task<TResultObj<DataObj>> CheckSmtp(HostObject hostObj)
        {
            var result = new TResultObj<DataObj>();
            if (hostObj.Port == 0) hostObj.Port = 25;

            result.Message = " SERVICE : CheckSmtp : ";
            try
            {
                var monitorPingInfo = new MonitorPingInfo()
                {
                    Address = hostObj.Address,
                    Port = hostObj.Port,
                    EndPointType = "smtp",
                    Timeout = 20000,
                };

                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                // await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = _netConnectCollection.GetNetConnectInstance(monitorPingInfo);
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.PingInfo.Status;
                result.Success = netConnect.MpiConnect.IsUp;
                var data = new DataObj();
                data.TestedAddress = hostObj.Address;
                data.TestedPort = hostObj.Port;
                data.ResultSuccess = netConnect.MpiConnect.IsUp;
                if (netConnect.MpiConnect.PingInfo.RoundTripTime != UInt16.MaxValue) data.ResponseTime = netConnect.MpiConnect.PingInfo.RoundTripTime;
                else data.Timeout = netConnect.MpiStatic.Timeout;
                string[] splitData = result.Message.Split(':');
                if (splitData.Length > 2)
                {
                    data.ResultStatus = splitData[2];
                }
                result.Data = data;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.LogError(result.Message);

            }
            return result;
        }

        //Method to check icmp
        public async Task<TResultObj<DataObj>> CheckIcmp(HostObject hostObj)
        {
            var result = new TResultObj<DataObj>();

            result.Message = " SERVICE : CheckIcmp : ";
            try
            {
                var monitorPingInfo = new MonitorPingInfo()
                {
                    Address = hostObj.Address,
                    EndPointType = "icmp",
                    Timeout = 20000,
                };

                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                // await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = _netConnectCollection.GetNetConnectInstance(monitorPingInfo);
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.PingInfo.Status;
                result.Success = netConnect.MpiConnect.IsUp;
                var data = new DataObj();
                data.TestedAddress = hostObj.Address;
                data.TestedPort = hostObj.Port;
                data.ResultSuccess = netConnect.MpiConnect.IsUp;
                if (netConnect.MpiConnect.PingInfo.RoundTripTime != UInt16.MaxValue) data.ResponseTime = netConnect.MpiConnect.PingInfo.RoundTripTime;
                else data.Timeout = netConnect.MpiStatic.Timeout;
                string[] splitData = result.Message.Split(':');
                if (splitData.Length > 2)
                {
                    data.ResultStatus = splitData[2];
                }
                result.Data = data;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.LogError(result.Message);

            }
            return result;
        }

        //method to check dns lookup
        public async Task<TResultObj<DataObj>> CheckDns(HostObject hostObj)
        {
            var result = new TResultObj<DataObj>();
            result.Message = " SERVICE : CheckDns : ";
            try
            {
                var monitorPingInfo = new MonitorPingInfo()
                {
                    Address = hostObj.Address,
                    EndPointType = "dns",
                    Timeout = 20000,
                };

                //SemaphoreSlim semaphore = new SemaphoreSlim(1);
                // await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = _netConnectCollection.GetNetConnectInstance(monitorPingInfo);
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.PingInfo.Status;
                result.Success = netConnect.MpiConnect.IsUp;
                var data = new DataObj();
                data.TestedAddress = hostObj.Address;
                data.TestedPort = hostObj.Port;
                data.ResultSuccess = netConnect.MpiConnect.IsUp;
                if (netConnect.MpiConnect.PingInfo.RoundTripTime != UInt16.MaxValue) data.ResponseTime = netConnect.MpiConnect.PingInfo.RoundTripTime;
                else data.Timeout = netConnect.MpiStatic.Timeout;
                string[] splitData = result.Message.Split(new char[] { ':' }, 3);
                if (splitData.Length > 2)
                {
                    if (netConnect.MpiConnect.IsUp)
                        data.ResultStatus = "Resolved addresses : " + splitData[2];
                    else data.ResultStatus = splitData[2];
                }
                result.Data = data;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.LogError(result.Message);

            }
            return result;
        }
    }
}
