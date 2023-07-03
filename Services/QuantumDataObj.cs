using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Api.Services
{
     /// <summary>
        /// Object that contains details about the quantum check.
        /// </summary>  
    public class QuantumDataObj
    {
        private string _setupQuantumMonitor=" To setup a quantum ready monitor visit : https://freenetworkmonitor.click";
        private string _quantumKeyExchange="";

        private string _testedUrl="";
        private bool _resultSuccess=false;
        private string _resultStatus="";

        /// <summary>
        /// Instructions for setting up a quantum-ready monitor.
        /// </summary>  
        public string SetupQuantumMonitor { get => _setupQuantumMonitor; set => _setupQuantumMonitor = value; }
        
        /// <summary>
        /// The quantum key exchange protocol used by the tested URL.
        /// </summary>  
        public string QuantumKeyExchange { get => _quantumKeyExchange; set => _quantumKeyExchange = value; }

        /// <summary>
        /// The URL that was tested for quantum safety.
        /// </summary>  
        public string TestedUrl { get => _testedUrl; set => _testedUrl = value; }
        
        /// <summary>
        /// Indicates whether the tested URL uses a quantum-safe protocol.
        /// </summary>  
        public bool ResultSuccess { get => _resultSuccess; set => _resultSuccess = value; }
        
        /// <summary>
        /// Describes the status of the quantum safety of the tested URL.
        /// </summary>  
        public string ResultStatus { get => _resultStatus; set => _resultStatus = value; }
    }
}