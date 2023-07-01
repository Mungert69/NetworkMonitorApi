using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMonitor.Objects;
using NetworkMonitor.Objects.ServiceMessage;
using NetworkMonitor.Connection;
using NetworkMonitor.Utils.Helpers;
using System.Web;
using Newtonsoft.Json;
using MetroLog;
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
        Task<ResultObj> CheckQuantum(string url);
    }
    public class ApiService : IApiService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CancellationToken _token;
        private readonly PingParams _pingParams;
        private string _frontEndUrl = "https://freenetworkmonior.click";
        public ApiService(IConfiguration config, INetLoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, CancellationTokenSource cancellationTokenSource)
        {
            _config = config;
            _frontEndUrl = _config["FrontEndUrl"] != null ? _config["FrontEndUrl"] : _frontEndUrl;
            _token = cancellationTokenSource.Token;
            _token.Register(() => OnStopping());
            _scopeFactory = scopeFactory;
            _logger = loggerFactory.GetLogger("ApiService");
            _pingParams = SystemParamsHelper.GetPingParams(_config);

        }

        // OnStopping() method
        public void OnStopping()
        {
            _logger.Info("OnStopping");
        }
        public async Task<ResultObj> CheckQuantum(UrlObject urlObj)
        {
            var result = new ResultObj();
            result.Message = " SERVICE : CheckQuantum : ";
            try
            {
                var monitorPingInfos = new List<MonitorPingInfo>();
                monitorPingInfos.Add(new MonitorPingInfo()
                {
                    Address = urlObj.Url,
                    Port=urlObj.Port,
                    EndPointType = "quantum",
                    Timeout = 10000,
                });

                var connectFactory = new ConnectFactory(_config, true);
                var netConnectCollection = new NetConnectCollection(_logger, _config, connectFactory);
                SemaphoreSlim semaphore = new SemaphoreSlim(1);
                await netConnectCollection.NetConnectFactory(monitorPingInfos, _pingParams, true, semaphore);
                var netConnect = netConnectCollection[0];
                await netConnect.Connect();
                result.Message += netConnect.MpiConnect.PingInfo.Status;
                result.Success = netConnect.MpiConnect.IsUp;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message += " Error : " + ex.Message;
                _logger.Error(result.Message);

            }
            return result;
        }
    }
}