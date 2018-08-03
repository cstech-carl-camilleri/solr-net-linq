﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Community.OData.Linq;
using Community.OData.Linq.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SolrNet.Linq;

namespace SolrNet.IntegrationOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ISolrOperations<Product> Solr { get; }

        public ValuesController(ISolrOperations<Product> solr)
        {
            Solr = solr ?? throw new ArgumentNullException(nameof(solr));
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return this.Ok(new string[]
            {
                "http://localhost:61452/api/values/1?$filter=price gt 100&$orderby=Popularity desc&$top=3&$skip=1",
                "http://localhost:61452/api/values/1?$filter=Popularity ne null",
                "http://localhost:61452/api/values/1?$filter=Popularity eq null",
                "http://localhost:61452/api/values/1?$filter=Categories/any(c: c eq 'electronics')",
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id, ODataQueryOptions odata)
        {
            return this.Ok(this.Solr.AsQuerable().OData().ApplyQueryOptionsWithoutSelectExpand(odata).ToSolrQueryResults());
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
