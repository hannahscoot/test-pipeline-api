using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace test_pipeline.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetTestData")]
        public List<DemoResult> GetTestData()
        {
            var returnobject = new List<DemoResult>();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = "192.168.50.30,1433"; //replace with ip of server
            builder.UserID = "SA";
            builder.Password = "Password123!";
            builder.InitialCatalog = "TestDB";
            builder.TrustServerCertificate = true;

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Demo";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DemoResult result = new DemoResult();
                            result.Id = (int)reader["Id"];
                            result.Name = (string)reader["Name"];
                            returnobject.Add(result);
                        }
                    }
                }
            }
            return returnobject;
        }

        public class DemoResult
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}