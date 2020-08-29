using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace azupdatesmonthlynewsletter
{
    public class FeedEntity : TableEntity
    {
        public static string Manual = "manual";
        public FeedEntity(string rowKey, string partitionKey)
        {
            RowKey = rowKey;
            PartitionKey = partitionKey;
        }

        public FeedEntity() { }

        public string Date { get; set; }

        public string title { get; set; }

        public string links { get; set; }

        public string Type { get; set; }
    }

    class Program
    {
        static Regex AppDevKeyWordsRegex = new Regex("(Entity Framework Core|App Service|C#|Functions|Logic Apps|Visual Studio|Application Insights|Signal R|gRPC|Web|Mobile|ASP.NET|CDN|Java|.NET|Azure Search|Azure Redis Cache|API Management|PHP)");

        static Regex InfraKeyWordsRegex = new Regex("(WVD|Virtual Desktop|ExpressRoute|Virtual Machine|VM|Azure Batch|Availability Zones|Azure Automation|Azure Site Recovery|Azure Log Analytics|Azure Monitor|Azure Cost Management|Load Balancer|DNS|Network|Traffic Manager|VNet|Azure Migrate|Reserved Instance|Log Analytics|SAP|Azure Advisor)");

        static Regex DataAIKeyWordsRegex = new Regex("(Azure Data Explorer|Datashare|catalogue|Cosmos DB|CosmosDB|SQL|IoT|Digital Twins|Azure Sphere|PowerBI|Power BI|Azure Synapse|Data Lake|Azure Analysis Services|Database|HDInsight|Data Factory|Stream Analytics|Time Series|MySQL|PostgreSQL|Data Warehouse|Event Grid|Event Hub|Service Bus|Databricks)");

        static Regex AIRegEx = new Regex("(ONNX|Immersive Reader|Machine Learning services|Batch AI|Cognitive Service|Cognitive Services|AI|Artificial Intelligence|Machine Learning|ML|ML Services|ML Studio|Machine Learning Studio|Bing APIs|Computer Vision API|Content moderator|Custom Services|Emotion API|Face API|Language Understanding|LUIS|Linguistic Analysis API|QnA Maker API|QnAMaker|TensorFlow|Speaker Recognition API|Text Analytics API|Translator Speech API|Form Recognizer|Translator Text API|Web Language Model API|Anamoly)");

        static Regex SecurityKeyWordsRegex = new Regex("(Security Center|Sentinel|Azure Active Directory|AAD|AD DS|SOC|Azure Information Protection|Azure AD|EMS|Traffic Analytics|Azure Advanced Threat Protection|DDoS|Azure Security Center|Cloud App Security|Application Security Groups|Intune)");

        static Regex ContainersRegEx = new Regex("(Container Registry|Kubernetes|Container|ACI|ACR|Docker|Container Instances|OpenShift|ACS|Azure Kubernetes Service|AKS|Web App for Containers)");

        static Regex HybridRegEx = new Regex("(Azure Stack|Arc|HCI|Stage Edge|Stack)");

        static Regex MediaRegEx = new Regex("(Azure Media|Rendering|Video Analytics|Media services|Media|Encoding|Live and On-Demand Streaming|Azure Media Player|Content Protection|Media Analytics|Video Indexer)");

        static Regex StorageRegEx = new Regex("(AzCopy|Storage|StorSimple|Data Lake Store|Blob Storage|Disk Storage|Managed Disks|Queue Storage|File Storage|Storage Explorer|Archive Storage|Block Blob|premium file|Data Box|Ultra Disk|Azure Page Blob|Azure Storage|Storsimple|Azure Backup|Azure File|NetApp|Azure Table|NFS|SMB)");

        static Regex DevOpsRegEx = new Regex("(DevOps|CI|CD|Github|DevOps tool integrations|Lab Services|DevTest Labs|VSTS|Azure Artifacts|Azure Pipelines|Git|Continous Deployment)");

        static Regex IgnoreRegEx = new Regex("(Azure Sphere|Azure Gov|Power Platform|Cumulative Update Preview|Chinese|Azure Maps|Spring|Genomics|Blockchain|Spatial Anchors|Kinect|Xamarin|Onedrive|M365|Service Fabric|Dew Drop|Mobile|TFS|SiteCore|Sitecore|WordPress|FHIR|App Center|GDPR|Open Service Broker|Bot Framework|Bot Service)");

        static void Main(string[] args)
        {
            string date = "2020-08";

            Console.WriteLine($"Current date: {blogArticleDate.ToString()}");

            var blogArticleContent = GetBlogArticleContent(date);

            System.IO.File.WriteAllText(@"D:\onlinekarate\azupdatesmonthlynewsletter\bin\Debug\sample.html", blogArticleContent);
        }
        private static CloudTable GetRssFeedsCloudTable()
        {
            var storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("mikkytest");
        }
        static string GetBlogArticleContent(DateTime currentDate)
        {
            var builder = new StringBuilder();

        
            builder.Append($"<body> <div class=\"separator\" style=\"clear:both;text-align:left;\"><a href=\"https://maheshfilesharing.blob.core.windows.net/staticimages/img1.PNG?sv=2019-02-02&st=2020-08-27T14%3A25%3A26Z&se=2023-03-01T14%3A25%3A00Z&sr=b&sp=r&sig=xfKsLe2tNzUBlJ07vIr8y9pEB2IbRUzphghK%2BWBOHPk%3D\" imageanchor=\"1\"style=\"margin-left:1em; margin-right:1em;\"><img border=\"0\" data-original-height=\"987\" data-original-width=\"836\" height=\"130\" src =\"https://maheshfilesharing.blob.core.windows.net/staticimages/img1.PNG?sv=2019-02-02&st=2020-08-27T14%3A25%3A26Z&se=2023-03-01T14%3A25%3A00Z&sr=b&sp=r&sig=xfKsLe2tNzUBlJ07vIr8y9pEB2IbRUzphghK%2BWBOHPk%3D\" width=\"1100\" /></a ></div >");

            //Body
            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var table = GetRssFeedsCloudTable();
            var query = new TableQuery<FeedEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, currentDate.ToString("yyyy-MM")));
            var results = table.ExecuteQuery(query).OrderByDescending(f => f.Date);
            var resultsCount = results.Count();

            var appDevStringBuilder = new StringBuilder();
            var infraStringBuilder = new StringBuilder();
            var dataAiStringBuilder = new StringBuilder();
            var securityStringBuilder = new StringBuilder();
            var containerStringBuilder = new StringBuilder();
            var devopsStringBuilder = new StringBuilder();
            var hybridStringBuilder = new StringBuilder();
            var miscStringBuilder = new StringBuilder();
            var storagestringBuilder = new StringBuilder();
            var mediastringBuilder = new StringBuilder();
            var aiStringBuilder = new StringBuilder();

            foreach (var feed in results)
            {
                var feedInHtml = $"<a href=\"{feed.links}\">{DateTime.Parse(feed.Date).ToString("dd/MMM")}</a> - {feed.title}";

                if (AppDevKeyWordsRegex.IsMatch(feed.title))
                {
                    appDevStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (ContainersRegEx.IsMatch(feed.title))
                {
                    containerStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (DataAIKeyWordsRegex.IsMatch(feed.title))
                {
                    dataAiStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (InfraKeyWordsRegex.IsMatch(feed.title))
                {
                    infraStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (SecurityKeyWordsRegex.IsMatch(feed.title))
                {
                    securityStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (StorageRegEx.IsMatch(feed.title))
                {
                    storagestringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (DevOpsRegEx.IsMatch(feed.title))
                {
                    devopsStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (AIRegEx.IsMatch(feed.title))
                {
                    aiStringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (MediaRegEx.IsMatch(feed.title))
                {
                    mediastringBuilder.Append($"<br />{feedInHtml}");
                }
                else if (HybridRegEx.IsMatch(feed.title))
                {
                    hybridStringBuilder.Append($"<br />{feedInHtml}");
                }
                else
                {
                    if(!IgnoreRegEx.IsMatch(feed.title))
                    miscStringBuilder.Append($"<br />{feedInHtml}");
                }
            }

            builder.Append($"<br /> <b> Microsoft Azure updates, news and announcements ({resultsCount} entries) for {currentDate.ToString("MMMM yyyy")}: </b>");

            builder.Append("<br /><br /><b>Infrastructure:</b>");
            HandleEmptyEntries(builder, infraStringBuilder);

            builder.Append("<br /><br /><b>Data Platform:</b>");
            HandleEmptyEntries(builder, dataAiStringBuilder);

            builder.Append("<br /><br /><b>Security:</b>");
            HandleEmptyEntries(builder, securityStringBuilder);

            builder.Append("<br /><br /><b>Containers:</b>");
            HandleEmptyEntries(builder, containerStringBuilder);

            builder.Append("<br /><br /><b>Storage:</b>");
            HandleEmptyEntries(builder, storagestringBuilder);

            builder.Append("<br /><br /><b>DevOps and GitHub:</b>");
            HandleEmptyEntries(builder,devopsStringBuilder);

            builder.Append("<br /><br /><b>Media:</b>");
            HandleEmptyEntries(builder,mediastringBuilder);

            builder.Append("<br /><br /><b>AI & Cognitive Service:</b>");
            HandleEmptyEntries(builder, aiStringBuilder);

            builder.Append("<br /><br /><b>Hybrid:</b>");
            HandleEmptyEntries(builder, hybridStringBuilder);

            builder.Append("<br /><br /><b>General:</b><br /> </body>");
            builder.Append(miscStringBuilder.ToString());

            return builder.ToString();
        }

        static void HandleEmptyEntries(StringBuilder builder,StringBuilder input)
        {
            if (input.ToString() != string.Empty)
                builder.Append("<br />" + input.ToString());
            else
                builder.Append(" No updates");
        }
    }
}
