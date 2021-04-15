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

        public EndpointsController(ILogger<EndpointsController> logger)
        {
            _logger = logger;
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            _client = new Kubernetes(config);
        }

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var pods = await _client.ListNamespacedPodAsync("gpe");

            var jsonList = new List<string>();
            foreach (var pod in pods.Items)
            {
                jsonList.Add(JsonSerializer.Serialize(pod));
            }

            return jsonList;
        }
    }
}
