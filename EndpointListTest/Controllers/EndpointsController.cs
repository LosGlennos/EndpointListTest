using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using k8s;
using Microsoft.AspNetCore.Mvc;

namespace EndpointListTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndpointsController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var inClusterConfig = KubernetesClientConfiguration.InClusterConfig();
            var client = new Kubernetes(inClusterConfig);

            var pods = await client.ListNamespacedPodAsync("gpe");

            var jsonList = new List<string>();
            foreach (var pod in pods.Items)
            {
                jsonList.Add(JsonSerializer.Serialize(pod.Status.PodIP));
            }

            return jsonList;
        }
    }
}