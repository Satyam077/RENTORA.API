using MongoDB.Driver;
using RENTARA.API.Models;
using RENTORA.API.Models;

namespace RENTORA.API.Models.MongoDB
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;

        private readonly IMongoDatabase _database;

        public MongoDbSettings(IConfiguration config)
        {
            try
            {
                var connectionString = config.GetConnectionString("MongoDb") ?? config["MongoDb:ConnectionString"] ?? "mongodb+srv://c59933290:SCAOuac9FtJyapjA@demo-drive-nexus.ekntl.mongodb.net/";
                var databaseName = config["MongoDb:Database"] ?? "RENTORA";

                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the application
                Console.WriteLine($"MongoDB connection failed: {ex.Message}");
                // You might want to use a fallback or in-memory database here
            }
        }
        public IMongoCollection<Registration> Users => _database.GetCollection<Registration>("Registration");
        public IMongoCollection<EmailTemplate> EmailTemplates => _database.GetCollection<EmailTemplate>("EmailTemplates");
        public IMongoCollection<PropertyModel> Properties => _database.GetCollection<PropertyModel>("Properties");
        public IMongoCollection<UnitModel> Units => _database.GetCollection<UnitModel>("Units");
    }

}
