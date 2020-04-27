using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brain.Model;
using Brain.Repositories;
using Brain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Brain.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BrainController : ControllerBase
    {
        private readonly IMemoryService memoryService;
        private readonly IBrainRepository brainRepository;

        public BrainController(IMemoryService memoryService, IBrainRepository brainRepository)
        {
            this.memoryService = memoryService;
            this.brainRepository = brainRepository;
        }

        [HttpPost("{id}")]
        public JsonResult Post(string id, [FromBody] SenseInputs senseInputs)
        {
            var predicted = memoryService.ManageSenseInputs(id, senseInputs);
            return new JsonResult(predicted);
        }

        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            var lookup = brainRepository.GetCurrentAssociationsLookup(id);
            return new JsonResult(lookup);
        }
    }
}
