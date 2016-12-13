using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ASPWiki.Services.Generators
{
    public class GeneratorBase
    {
        protected Random random;

        protected List<string> nouns;

        public GeneratorBase()
        {
            random = new Random();

            using (StreamReader sr = new StreamReader(File.OpenRead("Resources/nouns.json")))
            {
                nouns = JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
            }
        }

        public string GetRandomName()
        {
            return nouns[random.Next(0, nouns.Count)];
        }

        protected T GetRandomEnum<T>()
        {
            return (T)(object)random.Next(1, Enum.GetNames(typeof(T)).Length + 1);
        }

        protected bool GetRandomBoolean(int propability = 50)
        {
            if (propability < 0 || propability > 100)
                throw new ArgumentOutOfRangeException("Propability out of range (0-100): " + propability);

            return random.Next(0, 100) > propability;
        }

        protected T GetRandomItemFromList<T>(List<T> list)
        {
            return list[random.Next(list.Count)];
        }

        public DateTime GetRandomDateTimeBetween(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null)
            {
                endDate = DateTime.Now;
            }

            if (startDate > endDate)
                throw new ArgumentException("startDate must be earlier than endDate");

            DateTime dateTime = startDate;

            var timeSpan = endDate - startDate;

            dateTime = dateTime.AddDays(random.Next(timeSpan.Value.Days - 1));

            dateTime = dateTime.AddHours(random.Next(24));
            dateTime = dateTime.AddMinutes(random.Next(60));
            dateTime = dateTime.AddSeconds(random.Next(60));

            return dateTime;
        }
    }
}
