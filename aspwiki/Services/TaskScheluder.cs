using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Services
{
    public class TaskScheluder
    {
        private readonly IWikiRepository wikiRepo;

        public TaskScheluder(IWikiRepository wikiRepo)
        {
            this.wikiRepo = wikiRepo;
        }

        public void DeleteOutDatedWikiPages()
        {
            var wikis = wikiRepo.GetAll();

            foreach (var wiki in wikis)
            {
                if (wiki.Public && wiki.DueDate < DateTime.Now)
                {
                    wikiRepo.Delete(wiki);
                }
            }
        }
    }
}
