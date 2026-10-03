using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanHieuAPI : ControllerBase
    {
        private readonly RetailDbContext _db;

        public NhanHieuAPI(RetailDbContext db) { this._db = db; }

        [HttpGet]
        public async Task<IActionResult> layTatCa()
        {
            var nhanHieu = await _db.NhanHieus.AsNoTracking().OrderBy(x => x.MaNhanHieu).ToListAsync();
            return Ok(nhanHieu);
        }
    }
}
