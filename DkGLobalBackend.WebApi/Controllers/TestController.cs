using DkGLobalBackend.WebApi.Database;
using DkGLobalBackend.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DkGLobalBackend.WebApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        public TestController(InventoryDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetAll() 
        {
            //var user = new TestUser { UserName = "Tanvir" };
            var item = new TestItem { Name = "A4tech Mouse" };
            //user.TestItems.Add(item);
            //_context.TestItems.Add(item);
            //_context.TestUsers.Add(user);
            //var itemWithItemTo = await _context.TestItems.FirstOrDefaultAsync(x=>x.Id == 1);
            //var itemWithUserTo = await _context.TestUsers.FirstOrDefaultAsync(x=>x.Id == 4);

            //itemWithUserTo.TestItems.Add(itemWithItemTo);
            ////await _context.SaveChangesAsync();
            //var userWithItem = await _context.TestUsers.Include(u=>u.TestItems).ToListAsync();
            //var itemWithUser = await _context.TestItems.Include(u=>u.TestUsers).ToListAsync();
            return Ok();
        }
    }
}
