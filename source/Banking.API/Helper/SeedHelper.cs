using System.Reflection;
using Newtonsoft.Json;

namespace Banking.API.Helper
{
    public static class SeedHelper
    {
        public static List<TEntity> SeedData<TEntity>(string fileName)
        {
            var dirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullPath = Path.Combine(dirPath!,"Data", fileName);

            Console.WriteLine($"Directory Path: {dirPath}");
            Console.WriteLine($"Full Path: {fullPath}");

            var result = new List<TEntity>();
            using (StreamReader reader = new StreamReader(fullPath))
            {
                string json = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<List<TEntity>>(json);
            }
     
            return result!;
        }
    }
}