using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using k8s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EndpointListTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndpointsController : ControllerBase
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly Kubernetes _client;
        private readonly Kubernetes _specifiedFileClient;
        private readonly Kubernetes _inClusterClient;

        public EndpointsController(ILogger<EndpointsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {


            Kubernetes client = null;
            try
            {
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                client = new Kubernetes(config);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Failed to create client");
            }

            try
            {
                var specifiedFileConfig = KubernetesClientConfiguration.BuildConfigFromConfigFile(Environment.GetEnvironmentVariable("KUBECONFIG"));
                if (client == null)  client = new Kubernetes(specifiedFileConfig);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Failed to create specified file client");
            }

            try
            {
                var inClusterConfig = KubernetesClientConfiguration.InClusterConfig();
                if (client == null) client = new Kubernetes(inClusterConfig);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "Failed to create in cluster client");
            }

            if (client == null) throw new Exception("Unable to create any type of client");
            
            var pods = await client.ListNamespacedPodAsync("gpe");

            var jsonList = new List<string>();
            foreach (var pod in pods.Items)
            {
                jsonList.Add(JsonSerializer.Serialize(pod));
            }

            return jsonList;
        }
    }
}
