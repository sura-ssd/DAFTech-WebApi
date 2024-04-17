using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientDbContext _clientDbContext;

        public ClientController(ClientDbContext clientDbContext)
        {
          _clientDbContext = clientDbContext;
        }

        [HttpGet]
        [Route("GetRegionCounts")]
        public async Task<ActionResult<IEnumerable<RegionCount>>> GetRegionCounts()
        {
            var regionCounts = await _clientDbContext.Client
                .GroupBy(c => c.Region)
                .Select(group => new RegionCount
                {
                    Region = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return regionCounts;
        }

        [HttpGet]
        [Route("GetTotalClientCount")]
        public async Task<ActionResult<int>> GetTotalClientCount()
        {
            int totalClientCount = await _clientDbContext.Client.CountAsync();
            return totalClientCount;
        }

        [HttpGet]
        [Route("GetClient/{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _clientDbContext.Client.FindAsync(id);

            if (client == null)
            {
                return NotFound(); 
            }

            return client;
        }

        [HttpGet]
        [Route("GetClient")]
        public async Task<IEnumerable<Client>> GetClients()
        {
            return await _clientDbContext.Client.ToListAsync();
        }

        [HttpPost]
        [Route("AddClient")]
        public async Task<ActionResult<Client>> AddClient([FromBody] Client objClient)
        {
            // Check if username or email already exists
            var existingUser = await _clientDbContext.Client
                .FirstOrDefaultAsync(c => c.Username == objClient.Username || c.Email == objClient.Email);

            if (existingUser != null)
            {
                // User with the same username or email already exists
                return Conflict("Username or email already exists");
            }

            _clientDbContext.Client.Add(objClient);
            await _clientDbContext.SaveChangesAsync();
            return objClient;
        }

        [HttpPatch]
        [Route("UpdateClient/{id}")]
        public async Task<Client> UpdateClient(Client objClient)
        {
            _clientDbContext.Entry(objClient).State= EntityState.Modified;
            await _clientDbContext.SaveChangesAsync();
            return objClient;
        }

        [HttpDelete]
        [Route("DeleteClient/{id}")]
        public bool DeleteClient(int id)
        {
            bool a = false;
            var client= _clientDbContext.Client.Find(id);
            if (client != null)
            {
                a = true;
                _clientDbContext.Entry(client).State= EntityState.Deleted;
                _clientDbContext.SaveChanges();
            }
            else
            {
                a = false;
            }

            return a;

        }
    }
}
