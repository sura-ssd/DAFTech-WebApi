using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly ClientDbContext _context;

        public PublishController(ClientDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-publish-dates/{clientId}")]
        public async Task<IActionResult> GetPublishDates(int clientId)
        {
            // Check if the client exists
            var client = await _context.Client.FindAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Retrieve all timeframes associated with the client
            var timeframes = await _context.Timeframes
                .Where(tf => tf.ClientId == clientId)
                .OrderBy(tf => tf.StartDate)
                .ToListAsync();

            return Ok(timeframes);
        }


        [HttpPost("set-publish-date/{clientId}")]
        public async Task<IActionResult> SetPublishDate(int clientId, [FromBody] Timeframe timeframe)
        {
            if (timeframe.StartDate >= timeframe.EndDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            // Check if the client exists
            var client = await _context.Client.FindAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Create and save the new timeframe
            timeframe.ClientId = clientId;
            _context.Timeframes.Add(timeframe);
            await _context.SaveChangesAsync();

            return Ok("Publish date set successfully.");
        }

        [HttpPost("renew-publish-date/{clientId}")]
        public async Task<IActionResult> RenewPublishDate(int clientId)
        {
            // Check if the client exists
            var client = await _context.Client.FindAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Find the latest timeframe for the client
            var latestTimeframe = await _context.Timeframes
                .Where(tf => tf.ClientId == clientId)
                .OrderByDescending(tf => tf.EndDate)
                .FirstOrDefaultAsync();

            if (latestTimeframe == null)
            {
                return BadRequest("No existing publish date found for the client.");
            }

            // Calculate new start and end dates for renewal
            var renewalStartDate = latestTimeframe.EndDate.AddDays(1);
            var renewalEndDate = renewalStartDate.AddMonths(1);

            // Update the latest timeframe's end date with the renewed end date
            latestTimeframe.EndDate = renewalEndDate;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Publish date renewed successfully.", EndDate = renewalEndDate });
        }

        [HttpGet("check-end-date-notification/{clientId}")]
        public async Task<IActionResult> CheckEndDateNotification(int clientId)
        {
            // Check if the client exists
            var client = await _context.Client.FindAsync(clientId);
            if (client == null)
            {
                return NotFound("Client not found.");
            }

            // Find the latest timeframe for the client
            var latestTimeframe = await _context.Timeframes
                .Where(tf => tf.ClientId == clientId)
                .OrderByDescending(tf => tf.EndDate)
                .FirstOrDefaultAsync();

            if (latestTimeframe == null)
            {
                return BadRequest("No existing publish date found for the client.");
            }

            // Calculate the date one week from now
            var oneWeekFromNow = DateTime.Now.AddDays(7);

            if (latestTimeframe.EndDate <= oneWeekFromNow && latestTimeframe.EndDate >= DateTime.Now)
            {
                // End date is within one week from now and in the future, send a notification
                return Ok("End date is within one week. Send a notification to the user.");
            }

            return Ok("End date is not within one week.");
        }


    }
}
