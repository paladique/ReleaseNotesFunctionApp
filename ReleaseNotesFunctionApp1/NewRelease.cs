using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ReleaseNotesFunctionApp
{
    public static class NewRelease
    {
        [FunctionName("NewRelease")]
        public static async Task Run([HttpTrigger(WebHookType = "github")]HttpRequestMessage req, [Queue("release-queue",Connection = "ReleaseStorage")]ICollector<string> releaseQueueItem, TraceWriter log)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Extract GitHub release information from request body
            string releaseBody = data?.release?.body;
            string releaseName = data?.release?.name;
            string repositoryName = data?.repository?.full_name;

            //Format message and send to queue
            var releaseDetails = string.Format("{0}|{1}|{2}", releaseName, releaseBody, repositoryName);
            releaseQueueItem.Add(releaseDetails);
        }
    }
}
