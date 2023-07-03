using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Api.Services
{

        /// <summary>
        /// Object that contains details about the check.
        /// </summary>  
    public class DataObj
    {
       
        private string _setupFreeNetworkMonitor = "To setup a service monitor visit : https://freenetworkmonitor.click";

        private string _testedAddress = "";
        private ushort? _testedPort = 0;

        private ushort? responseTime = 0;
        private bool _resultSuccess = false;
        private string _resultStatus = "";

        /// <summary>
        /// Indicates whether the check was successful.
        /// </summary>  
        public bool ResultSuccess { get => _resultSuccess; set => _resultSuccess = value; }

        /// <summary>
        /// Instructions for setting up a free network monitor.
        /// </summary>  
        public string ResultStatus { get => _resultStatus; set => _resultStatus = value; }

        /// <summary>
        /// The address of the tested host.
        /// </summary>  
        public string TestedAddress { get => _testedAddress; set => _testedAddress = value; }

        /// <summary>
        /// The port used for the check.
        /// </summary>  
        public ushort? TestedPort { get => _testedPort; set => _testedPort = value; }

        /// <summary>
        /// The time taken to perform the test. This indicates the response time of the host service.
        /// </summary>  
        public ushort? ResponseTime { get => responseTime; set => responseTime = value; }

        /// <summary>
        /// Instructions for setting up a free network monitor.
        /// </summary>  
        public string SetupFreeNetworkMonitor { get => _setupFreeNetworkMonitor; set => _setupFreeNetworkMonitor = value; }
    }


}
