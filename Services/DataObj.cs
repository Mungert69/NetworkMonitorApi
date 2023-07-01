using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Api.Services
{
    public class DataObj
    {
        private string _setupQuantumMonitor=" To setup a quantum ready monitor visit : https://freenetworkmonitor.click";
        private string _quantumKeyExchange="";

        private string _testedUrl="";
        private bool _resultSuccess=false;
        private string _resultStatus="";

        public string SetupQuantumMonitor { get => _setupQuantumMonitor; set => _setupQuantumMonitor = value; }
        public string QuantumKeyExchange { get => _quantumKeyExchange; set => _quantumKeyExchange = value; }
        public string TestedUrl { get => _testedUrl; set => _testedUrl = value; }
        public bool ResultSuccess { get => _resultSuccess; set => _resultSuccess = value; }
        public string ResultStatus { get => _resultStatus; set => _resultStatus = value; }
    }
}