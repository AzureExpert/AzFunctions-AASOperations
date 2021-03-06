using System;
using System.Configuration;
using System.Net;
using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzFunctionApp
{
    using Models;

    /// <summary>
    /// Azure Function to process the specified partition for the specified table in the specified database.
    /// </summary>
    public static class ProcessTabularModelProcessPartitionAysnc
    {
        /// <summary>
        ///  Queues the request to process the specified partition in the specified table in the specified database.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        /// <param name="partitionName"></param>
        /// <param name="queue">Queue to place the procesing request</param>
        /// <param name="statusTable">Table to track the status of the processing request</param>
        /// <param name="log">Instance of log writer</param>
        /// <returns>Returns the tracking information for the procesing request</returns>
        [FunctionName("AsyncProcessPartition")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", 
            Route = "ProcessTabularModel/{databaseName}/tables/{tableName}/partitions/{partitionName}/async")]
                HttpRequestMessage req, 
                string databaseName,
                string tableName, 
                string partitionName,
                 [Queue("%ProcessPartitionQueue%", Connection = "AzureWebJobsStorage")] ICollector<QueueMessageProcesssTabular> queue,
                 [Table("%ProcessPartitionStatusTable%", Connection = "AzureWebJobsStorage")] ICollector<QueueMessageProcesssTabular> statusTable,
                TraceWriter log)
        {
            log.Info($"Received request queue processing of partition - {databaseName}/{tableName}/{partitionName}");

            string outputMediaType = ConfigurationManager.AppSettings["ProcessingTrackingOutputMediaType"];
           
            QueueMessageProcesssTabular queuedMessage = null;

            try
            {
                DateTime enqueuedDateTime = DateTime.UtcNow;
                string trackingId = Guid.NewGuid().ToString();

                queuedMessage = new QueueMessageProcesssTabular()
                {
                    TrackingId = trackingId,
                    EnqueuedDateTime = enqueuedDateTime,
                    Database = databaseName,
                    Tables = tableName,
                    TargetDate = DateTime.Now,
                    Parition = partitionName,
                    Status = "Queued",
                    PartitionKey = enqueuedDateTime.ToString("yyyy-MM-dd"),
                    RowKey = trackingId,
                    ETag = "*"
                };

                queue.Add(queuedMessage);
                statusTable.Add(queuedMessage);

                log.Info($"Successfully queued request to process partition - " +
                    $"{databaseName}//{tableName}/{partitionName} as {queuedMessage.PartitionKey}/{queuedMessage.RowKey}");
            }
            catch (Exception e)
            {
                log.Error($"Error trying queue request to process partition - {databaseName}/{tableName}/{partitionName}. Details : {e.ToString()}", e);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            
            return req.CreateResponse(HttpStatusCode.OK, queuedMessage.ToProcessingTrackingInfo());            
        }
    }
}
