using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminDbContext _adminDbContext;

        public AdminController(AdminDbContext adminDbContext)
        {
            _adminDbContext = adminDbContext;
        }

        [HttpGet]
        [Route("GetAdmin")]
        public async Task<IEnumerable<Admin>> GetAdmins()
        {
            var admins = await _adminDbContext.Admin.ToListAsync();

            // Construct URLs for the profile images
            foreach (var admin in admins)
            {
                if (!string.IsNullOrEmpty(admin.ProfileImage))
                {
                    admin.ProfileImage = $"{Request.Scheme}://{Request.Host}/Assets/{admin.ProfileImage}";
                }
            }

            var adminsWithId = admins.Select(admin => new
            {
                Id = admin.Id,
                AdminData = admin
            });

            return admins;
        }

        [HttpGet]
        [Route("GetTotalAdminCount")]
        public async Task<ActionResult<int>> GetTotalAdminCount()
        {
            var totalAdminCount = await _adminDbContext.Admin.CountAsync();
            return totalAdminCount;
        }


        [HttpPost]
        [Route("AddAdmin")]
        public async Task<ActionResult<Admin>> AddAdmin(
          [FromForm] Admin objAdmin,
          [FromForm] IFormFile profileImage)
        {

            // Check if username or email already exists
            var existingUser = await _adminDbContext.Admin
                .FirstOrDefaultAsync(c => c.Username == objAdmin.Username || c.Email == objAdmin.Email);

            if (existingUser != null)
            {
                // User with the same username or email already exists
                return Conflict("Username or email already exists");
            }

            // Generate a unique filename for the image
            var imageExtension = Path.GetExtension(profileImage.FileName);
            var imageFileName = $"{Guid.NewGuid()}{imageExtension}";
            var imagePath = Path.Combine("Assets", imageFileName);

            // Save the image with the unique filename
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            objAdmin.ProfileImage = imageFileName; // Store the filename, not the byte array

            _adminDbContext.Admin.Add(objAdmin);
            await _adminDbContext.SaveChangesAsync();
            return objAdmin;
        }


        [HttpPatch]
        [Route("UpdateAdmin/{id}")]
        public async Task<Admin> UpdateAdmin(Admin objAdmin)
        {
            _adminDbContext.Entry(objAdmin).State = EntityState.Modified;
            await _adminDbContext.SaveChangesAsync();
            return objAdmin;
        }
        [HttpDelete]
        [Route("DeleteAdmin/{id}")]
        public bool DeleteAdmin(int id)
        {
            bool a = false;
            var admin = _adminDbContext.Admin.Find(id);
            if (admin != null)
            {
                a = true;
                _adminDbContext.Entry(admin).State = EntityState.Deleted;
                _adminDbContext.SaveChanges();
            }
            else
            {
                a = false;
            }

            return a;

        }
    }
}
