using System.Collections.Generic;
using System.Net.Http;
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
        private readonly IHttpClientFactory _clientFactory;

        public EndpointsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var inClusterConfig = KubernetesClientConfiguration.InClusterConfig();
            var client = new Kubernetes(inClusterConfig);

            var pods = await client.ListNamespacedPodAsync("gpe");
            var httpClient = _clientFactory.CreateClient();

            var resultList = new List<string>();
            foreach (var pod in pods.Items)
            {
                pod.Metadata.Labels.TryGetValue("app", out var deploymentName);
                if (deploymentName != null && deploymentName == "gpe-price")
                {
                    var result = await httpClient.GetAsync($"http://{pod.Status.PodIP}:8080/health");
                    var responseString = await result.Content.ReadAsStringAsync();
                    resultList.Add($"Pod {pod.Metadata.Name} response: {responseString}");
                }
            }



            return resultList;
        }
    }
}