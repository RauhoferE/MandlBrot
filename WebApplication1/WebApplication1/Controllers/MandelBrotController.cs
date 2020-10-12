using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using System.Drawing;
using NetworkLibraryMandelBrot;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MandelBrotController : ControllerBase
    {
        private IMandelbrotService service;
        private readonly ILogger<MandelBrotController> logger;
        public MandelBrotController(IMandelbrotService service, ILogger<MandelBrotController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        // GET: api/MandelBrot
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MandelBrot/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MandelBrot
        [HttpPost]
        public async Task<ActionResult<Bitmap>> Post([FromBody] MandelBrotRequest request)
        {
            logger.LogInformation("Client Connected");
            try
            {
                Bitmap bm = await this.service.GetMandelBrotBitmap(request);

                if (bm == null)
                {
                    logger.LogError("Bitmap is null");
                    return BadRequest("Ops something went wrong");
                }
                else
                {
                    logger.LogInformation("Return Bitmap");
                    MandelbrotBitMapAnswer bta = new MandelbrotBitMapAnswer();
                    
                    
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(ms, bm);
                        bta.Picture = ms.ToArray();
                    }
                    return Ok(bta);
                    
                }
            }
            catch (Exception)
            {
                logger.LogError("Exception is thrown in the service");
                return BadRequest("Ops something went wrong");
            }
            
        }

        // PUT: api/MandelBrot/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
