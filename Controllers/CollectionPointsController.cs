using FloodRelief.Api.Data;
using FloodRelief.Api.Dtos;
using FloodRelief.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FloodRelief.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CollectionPointsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CollectionPointsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CollectionPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectionPointDto>>> GetAll()
        {
            var cps = await _db.CollectionPoints
                .OrderBy(cp => cp.Name)
                .ToListAsync();

            var result = cps.Select(cp => new CollectionPointDto
            {
                CollectionPointId = cp.CollectionPointId,
                Name = cp.Name,
                Address = cp.Address,
                District = cp.District,
                Phone = cp.Phone,
                IsActive = cp.IsActive
            });

            return Ok(result);
        }

        // POST: api/CollectionPoints
        [HttpPost]
        public async Task<ActionResult<CollectionPointDto>> Create(CollectionPointDto dto)
        {
            var cp = new CollectionPoint
            {
                Name = dto.Name,
                Address = dto.Address,
                District = dto.District,
                Phone = dto.Phone,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.CollectionPoints.Add(cp);
            await _db.SaveChangesAsync();

            dto.CollectionPointId = cp.CollectionPointId;
            return CreatedAtAction(nameof(GetAll), new { id = cp.CollectionPointId }, dto);
        }
    }
}
