using System;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

namespace furl
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new AmazonCloudWatchLogsClient(Amazon.RegionEndpoint.USEast2);
            var response = client.GetLogEventsAsync(mkreq()).Result;
            dumpevts(response);
            while (response.NextForwardToken != null) {
                response = client.GetLogEventsAsync(mkreq(response.NextForwardToken)).Result;
                dumpevts(response);
            }
        }
        static void dumpevts(GetLogEventsResponse r) {
            foreach (var evt in r.Events) {
                Console.WriteLine($"{evt.Timestamp}: ${evt.Message}");
            }
        }
        static GetLogEventsRequest mkreq(string tok = null) {
            var r = new GetLogEventsRequest {
                LogStreamName = Environment.GetEnvironmentVariable("FURL_LOG_STREAM"),
                LogGroupName = Environment.GetEnvironmentVariable("FURL_LOG_GROUP"),
                Limit = 5000
            };
            if (tok != null) {
                r.NextToken = tok;
            }
            else {
                r.StartFromHead = true;
            }
            return r;
        }
    }
}
