namespace StudentManagementSystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using MongoDB.Driver;

    using StudentManagementSystem.Data.ViewModels.Schools;

    public class SchoolsController : ApiController
    {
        public SchoolsController(IMongoDatabase database)
        {
            this.SchoolCollection = database.GetCollection<SchoolViewModel>("Schools");
        }

        public IMongoCollection<SchoolViewModel> SchoolCollection { get; set; }

        // GET api/<controller>
        public async Task<IEnumerable<SchoolViewModel>> Get()
        {
            return await this.SchoolCollection.Find(FilterDefinition<SchoolViewModel>.Empty).ToListAsync();
        }

        // GET api/<controller>/5
        public string Get(string userName)
        {
            throw new NotImplementedException();
            // return await this.SchoolCollection.Find(new FilterDefinition<SchoolViewModel>()).ToListAsync();
        }
    }
}