using System.Security.Claims;
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
    [Authorize] // default: all endpoints require JWT unless overridden
    public class DonationFeedbacksController : ControllerBase
    {
        //private readonly AppDbContext _db;

        //public DonationFeedbacksController(AppDbContext db)
        //{
        //    _db = db;
        //}


        private readonly AppDbContext _db;

        public DonationFeedbacksController(AppDbContext db)
        {
            _db = db;
        }



        private string? GetUserRole() =>
            User.FindFirstValue(ClaimTypes.Role);

        // POST: api/DonationFeedbacks
        // Public-facing form: allow anonymous users to submit feedback
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<DonationFeedbackResponseDto>> CreateFeedback(DonationFeedbackCreateDto dto)
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.AddressLine1) ||
                string.IsNullOrWhiteSpace(dto.Postcode) ||
                string.IsNullOrWhiteSpace(dto.ItemsDescription))
            {
                return BadRequest("FullName, Email, AddressLine1, Postcode and ItemsDescription are required.");
            }

            var entity = new DonationFeedback
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone?.Trim(),
                AddressLine1 = dto.AddressLine1.Trim(),
                AddressLine2 = dto.AddressLine2?.Trim(),
                Postcode = dto.Postcode.Trim(),
                ItemsDescription = dto.ItemsDescription.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            _db.DonationFeedbacks.Add(entity);
            await _db.SaveChangesAsync();
            

            var response = new DonationFeedbackResponseDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                AddressLine1 = entity.AddressLine1,
                AddressLine2 = entity.AddressLine2,
                Postcode = entity.Postcode,
                ItemsDescription = entity.ItemsDescription,
                CreatedDate = entity.CreatedDate
            };

            return CreatedAtAction(nameof(GetFeedbackById), new { id = entity.Id }, response);
        }

        // GET: api/DonationFeedbacks/{id}
        // Admin only: view single feedback
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DonationFeedbackResponseDto>> GetFeedbackById(int id)
        {
            var feedback = await _db.DonationFeedbacks
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null) return NotFound();

            var dto = new DonationFeedbackResponseDto
            {
                Id = feedback.Id,
                FullName = feedback.FullName,
                Email = feedback.Email,
                Phone = feedback.Phone,
                AddressLine1 = feedback.AddressLine1,
                AddressLine2 = feedback.AddressLine2,
                Postcode = feedback.Postcode,
                ItemsDescription = feedback.ItemsDescription,
                CreatedDate = feedback.CreatedDate
            };

            return Ok(dto);
        }

        // GET: api/DonationFeedbacks
        // Admin grid list with optional filters
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DonationFeedbackResponseDto>>> GetFeedbacks(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? postcode)
        {
            var query = _db.DonationFeedbacks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(postcode))
            {
                query = query.Where(f => f.Postcode.Contains(postcode));
            }

            if (from != null)
            {
                query = query.Where(f => f.CreatedDate >= from.Value);
            }

            if (to != null)
            {
                query = query.Where(f => f.CreatedDate <= to.Value);
            }

            var list = await query
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();

            var result = list.Select(f => new DonationFeedbackResponseDto
            {
                Id = f.Id,
                FullName = f.FullName,
                Email = f.Email,
                Phone = f.Phone,
                AddressLine1 = f.AddressLine1,
                AddressLine2 = f.AddressLine2,
                Postcode = f.Postcode,
                ItemsDescription = f.ItemsDescription,
                CreatedDate = f.CreatedDate
            });

            return Ok(result);
        }
    }
}
