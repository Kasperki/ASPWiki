using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASPWiki.Services
{
    //https://github.com/leonardr/olipy/tree/master/data/word-lists

    public class RouteGenerator : IRouteGenerator
    {
        private List<string> nouns, adjectives;
        private Random random;

        public RouteGenerator(IHostingEnvironment appEnvironment)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead("Resources/nouns.json")))
            {
                nouns = JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
            }

            using (StreamReader sr = new StreamReader(File.OpenRead("Resources/adjectives.json")))
            {
                adjectives = JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
            }

            random = new Random();
        }

        public string GenerateRoute()
        {
            string noun = nouns[random.Next(0, nouns.Count)];
            noun = noun.First().ToString().ToUpper() + noun.Substring(1);
            string adjective = adjectives[random.Next(0, adjectives.Count)];
            return adjective + noun;
        }
    }
}