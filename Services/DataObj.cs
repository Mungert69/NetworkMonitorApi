using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Api.Services
{

       public class DataObj
    {
        private string _setupNetworkMonitor = "To setup a service monitor visit : https://freenetworkmonitor.click";

        private string _testedAddress = "";
        private ushort? _testedPort = 0;

        private ushort? responseTime = 0;
        private bool _resultSuccess = false;
        private string _resultStatus = "";

        public bool ResultSuccess { get => _resultSuccess; set => _resultSuccess = value; }
        public string ResultStatus { get => _resultStatus; set => _resultStatus = value; }
        public string TestedAddress { get => _testedAddress; set => _testedAddress = value; }
        public ushort? TestedPort { get => _testedPort; set => _testedPort = value; }
        public ushort? ResponseTime { get => responseTime; set => responseTime = value; }
        public string SetupNetworkMonitor { get => _setupNetworkMonitor; set => _setupNetworkMonitor = value; }
    }


}
 