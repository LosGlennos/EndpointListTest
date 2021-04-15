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
            var inClusterConfig = KubernetesClientConfiguration.InClusterConfig();
            var client = new Kubernetes(inClusterConfig);

            var pods = await client.ListNamespacedPodAsync("gpe");

            var jsonList = new List<string>();
            foreach (var pod in pods.Items)
            {
                jsonList.Add(JsonSerializer.Serialize(pod.Status.HostIP));
            }

            return jsonList;
        }
    }
}