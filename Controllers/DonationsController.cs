using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FloodRelief.Api.Data;
using FloodRelief.Api.Dtos;
using FloodRelief.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using FloodRelief.Api.Services.Reports;


namespace FloodRelief.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // all endpoints require JWT
    public class DonationsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DonationsController(AppDbContext db)
        {
            _db = db;
        }


        private string? GetUserRole() =>
            User.FindFirstValue(ClaimTypes.Role);

        private int? GetUserCollectionPointId()
        {
            var cpClaim = User.FindFirst("collectionPointId")?.Value;
            return int.TryParse(cpClaim, out var id) ? id : (int?)null;
        }







        // POST: api/Donations
        [HttpPost]
        [Authorize(Roles = "Admin,Collector")]
        public async Task<ActionResult<DonationResponseDto>> CreateDonation(DonationCreateDto dto)
        {
            // Get current user info from JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Invalid user id in token.");
            }

            var role = User.FindFirstValue(ClaimTypes.Role);
            var collectionPointIdClaim = User.FindFirst("collectionPointId")?.Value;

            int? userCollectionPointId = null;
            if (int.TryParse(collectionPointIdClaim, out var cpIdParsed))
            {
                userCollectionPointId = cpIdParsed;
            }

            // Determine which collection point to use
            int collectionPointId;

            if (role == "Collector")
            {
                if (userCollectionPointId == null)
                    return BadRequest("Collector has no collection point assigned.");

                collectionPointId = userCollectionPointId.Value;
            }
            else // Admin
            {
                if (dto.CollectionPointId == null)
                    return BadRequest("Admin must specify CollectionPointId.");

                collectionPointId = dto.CollectionPointId.Value;
            }

            // Validate collection point exists
            var cp = await _db.CollectionPoints
                .FirstOrDefaultAsync(c => c.CollectionPointId == collectionPointId && c.IsActive);

            if (cp == null)
                return BadRequest("Invalid or inactive collection point.");

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized("User not found.");

            // Create entity
            var donation = new Donation
            {
                DonorName = dto.DonorName,
                DonorEmail = dto.DonorEmail,
                DonorPhone = dto.DonorPhone,
                ItemDescription = dto.ItemDescription,
                Quantity = dto.Quantity,
                WeightKg = dto.WeightKg,
                CollectionPointId = collectionPointId,
                CollectedByUserId = userId,
                CollectedAt = DateTime.UtcNow,
                Notes = dto.Notes
            };

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            // Map to response DTO
            var response = new DonationResponseDto
            {
                DonationId = donation.DonationId,
                DonorName = donation.DonorName,
                DonorEmail = donation.DonorEmail,
                DonorPhone = donation.DonorPhone,
                ItemDescription = donation.ItemDescription,
                Quantity = donation.Quantity,
                WeightKg = donation.WeightKg,
                CollectionPointId = donation.CollectionPointId,
                CollectionPointName = cp.Name,
                CollectedByUserId = user.UserId,
                CollectedByName = user.FullName,
                CollectedAt = donation.CollectedAt,
                Notes = donation.Notes
            };

            return CreatedAtAction(nameof(GetDonationById), new { id = donation.DonationId }, response);
        }

        // GET: api/Donations/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DonationResponseDto>> GetDonationById(int id)
        {
            var donation = await _db.Donations
                .Include(d => d.CollectionPoint)
                .Include(d => d.CollectedByUser)
                .FirstOrDefaultAsync(d => d.DonationId == id);

            if (donation == null) return NotFound();

            // Role-based visibility (collector only sees their own / point)
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Collector")
            {
                var cpClaim = User.FindFirst("collectionPointId")?.Value;
                if (!int.TryParse(cpClaim, out var cpId) || cpId != donation.CollectionPointId)
                {
                    return Forbid();
                }
            }

            var dto = new DonationResponseDto
            {
                DonationId = donation.DonationId,
                DonorName = donation.DonorName,
                DonorEmail = donation.DonorEmail,
                DonorPhone = donation.DonorPhone,
                ItemDescription = donation.ItemDescription,
                Quantity = donation.Quantity,
                WeightKg = donation.WeightKg,
                CollectionPointId = donation.CollectionPointId,
                CollectionPointName = donation.CollectionPoint.Name,
                CollectedByUserId = donation.CollectedByUserId,
                CollectedByName = donation.CollectedByUser.FullName,
                CollectedAt = donation.CollectedAt,
                Notes = donation.Notes
            };

            return Ok(dto);
        }

        // GET: api/Donations
        // Admin -> all; Collector -> only their collection point
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<DonationResponseDto>>> GetDonations(
        //    [FromQuery] DateTime? from,
        //    [FromQuery] DateTime? to,
        //    [FromQuery] int? collectionPointId)
        //{
        //    var role = User.FindFirstValue(ClaimTypes.Role);
        //    var cpClaim = User.FindFirst("collectionPointId")?.Value;
        //    int? userCpId = null;
        //    if (int.TryParse(cpClaim, out var cpParsed)) userCpId = cpParsed;

        //    var query = _db.Donations
        //        .Include(d => d.CollectionPoint)
        //        .Include(d => d.CollectedByUser)
        //        .AsQueryable();

        //    if (role == "Collector" && userCpId != null)
        //    {
        //        query = query.Where(d => d.CollectionPointId == userCpId.Value);
        //    }
        //    else if (role == "Admin" && collectionPointId != null)
        //    {
        //        query = query.Where(d => d.CollectionPointId == collectionPointId.Value);
        //    }

        //    if (from != null)
        //    {
        //        query = query.Where(d => d.CollectedAt >= from.Value);
        //    }

        //    if (to != null)
        //    {
        //        query = query.Where(d => d.CollectedAt <= to.Value);
        //    }

        //    var list = await query
        //        .OrderByDescending(d => d.CollectedAt)
        //        .ToListAsync();

        //    var result = list.Select(d => new DonationResponseDto
        //    {
        //        DonationId = d.DonationId,
        //        DonorName = d.DonorName,
        //        DonorEmail = d.DonorEmail,
        //        DonorPhone = d.DonorPhone,
        //        ItemDescription = d.ItemDescription,
        //        Quantity = d.Quantity,
        //        WeightKg = d.WeightKg,
        //        CollectionPointId = d.CollectionPointId,
        //        CollectionPointName = d.CollectionPoint.Name,
        //        CollectedByUserId = d.CollectedByUserId,
        //        CollectedByName = d.CollectedByUser.FullName,
        //        CollectedAt = d.CollectedAt,
        //        Notes = d.Notes
        //    });

        //    return Ok(result);
        //}



        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonationResponseDto>>> GetDonations(
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] int? collectionPointId)   // Admin can still use this later
        {
            var role = GetUserRole();
            var userCpId = GetUserCollectionPointId();

            var query = _db.Donations
                .Include(d => d.CollectionPoint)
                .Include(d => d.CollectedByUser)
                .AsQueryable();

            if (role == "Collector")
            {
                // 🔴 Collector ALWAYS restricted by JWT collectionPointId
                if (userCpId == null)
                    return Forbid("Collector has no collection point assigned.");

                query = query.Where(d => d.CollectionPointId == userCpId.Value);
            }
            else if (role == "Admin" && collectionPointId != null)
            {
                // 🟢 Admin optionally filters via query param
                query = query.Where(d => d.CollectionPointId == collectionPointId.Value);
            }

            if (from != null)
            {
                query = query.Where(d => d.CollectedAt >= from.Value);
            }

            if (to != null)
            {
                query = query.Where(d => d.CollectedAt <= to.Value);
            }

            var list = await query
                .OrderByDescending(d => d.CollectedAt)
                .ToListAsync();

            var result = list.Select(d => new DonationResponseDto
            {
                DonationId = d.DonationId,
                DonorName = d.DonorName,
                DonorEmail = d.DonorEmail,
                DonorPhone = d.DonorPhone,
                ItemDescription = d.ItemDescription,
                Quantity = d.Quantity,
                WeightKg = d.WeightKg,
                CollectionPointId = d.CollectionPointId,
                CollectionPointName = d.CollectionPoint.Name,
                CollectedByUserId = d.CollectedByUserId,
                CollectedByName = d.CollectedByUser.FullName,
                CollectedAt = d.CollectedAt,
                Notes = d.Notes
            });

            return Ok(result);
        }


    }
}
