using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppInfluxDB
{
    class Program
    {
        public static InfluxDbClient dbClient = new InfluxDbClient("http://23.100.86.157:8086/", "dbUser", "welcome@9876", InfluxData.Net.Common.Enums.InfluxDbVersion.Latest);
        static void Main(string[] args)
        {
            CreateDB();
            //InsertPoint();
            //BatchInsert();
            //ReadPoints();
            Console.ReadKey();
        }
        public static async Task InsertPoint()
        {
            // var dbClient = new InfluxDbClient("http://168.61.214.188:8086/", "dbUser", "welcome@9876", InfluxData.Net.Common.Enums.InfluxDbVersion.Latest);

            var pointToWrite = new Point()
            {
                Name = "reading", //Measurement Name
                Tags = new Dictionary<string, object>()
    {
        { "SensorId", 8 },
        { "SerialNumber", "00AF123B" }
    },
                Fields = new Dictionary<string, object>()
    {
        { "SensorState", "act" },
        { "Humidity", 431 },
        { "Temperature", 22.1 },
        { "Resistance", 34957 }
    },
                Timestamp = DateTime.UtcNow // optional (can be set to any DateTime moment)
            };
            try
            {
                var response = await dbClient.Client.WriteAsync(pointToWrite, "timeseriesdb");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static async Task CreateDB()
        {
            try
            {
                // var influxDbClient = new InfluxDbClient("http://168.61.214.188:8086/", "dbUser", "welcome@9876", InfluxData.Net.Common.Enums.InfluxDbVersion.Latest);
                var response = await dbClient.Database.CreateDatabaseAsync("timeseriesdb");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public static void BatchInsert()
        {
            var batchWriter = dbClient.Serie.CreateBatchWriter("timeseriesdb");

            List<Point> newPoints = new List<Point>();
            for (int i = 1; i <= 5000; i++)
            {
                Point newPoint = new Point()
                {
                    Name = "reading", //Measurement Name
                    Tags = new Dictionary<string, object>()
    {
        { "SensorId", i },
        { "SerialNumber", "00AF"+i.ToString()+"AB" }
    },
                    Fields = new Dictionary<string, object>()
    {
        { "SensorState", "act" },
        { "Humidity", i/3 },
        { "Temperature", i*0.002 },
        { "Resistance", i+1 }
    },
                    Timestamp = DateTime.UtcNow // optional (can be set to any DateTime moment)
                };
                newPoints.Add(newPoint);
            }

            batchWriter.AddPoints(newPoints);
            batchWriter.OnError += BatchWriter_OnError;
            batchWriter.Start();


        }

        public static void ReadPoints()
        {
            var query = "SELECT * FROM reading WHERE time > now() - 1h";
            var readresponse = dbClient.Client.QueryAsync(query, "timeseriesdb").Result;

            var resp = dbClient.Database.GetDatabasesAsync().Result;
        }
        private static void BatchWriter_OnError(object sender, Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
