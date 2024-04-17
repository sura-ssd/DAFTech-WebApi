using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QnAController : ControllerBase
    {
        private readonly ClientDbContext _context;

        public QnAController(ClientDbContext context)
        {
            _context = context;
        }

        [HttpGet("unanswered-count")]
        public async Task<IActionResult> GetUnansweredQuestionsCount()
        {
            var count = await _context.QuestionAnswers.CountAsync(q => !q.IsAnswered);
            return Ok(count);
        }


        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.QuestionAnswers.ToListAsync();
            

            // You can also include the corresponding answers in the result
            foreach (var question in questions)
            {
                if (question.IsAnswered)
                {
                    var answer = GetAnswerForQuestion(question.Id);
                    question.Answer = answer;
                   
                }
            }

            return Ok(questions);
        }

        // Helper method to get the answer for a specific question
        private string GetAnswerForQuestion(int questionId)
        {
            var answer = _context.AdminAnswers.FirstOrDefault(a => a.QuestionAnswerId == questionId);
            return answer?.Answer;
        }


        [HttpPost("questions")]
        public async Task<IActionResult> PostQuestion(QuestionAnswer question)
        {
            if (string.IsNullOrWhiteSpace(question.Question))
                return BadRequest("Question cannot be empty.");

            // Check if the client with the provided ClientId exists
            var client = await _context.Client.FindAsync(question.ClientId);
            if (client == null)
                return NotFound("Client not found.");

            question.Timestamp = DateTime.UtcNow;
            question.IsAnswered = false;

            _context.QuestionAnswers.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }



        [HttpGet("questions/{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _context.QuestionAnswers.FindAsync(id);

            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost("answers")]
        public async Task<IActionResult> PostAnswer(AdminAnswer answer)
        {
            var question = await _context.QuestionAnswers.FindAsync(answer.QuestionAnswerId);

            if (question == null)
                return NotFound("Question not found.");

            answer.Timestamp = DateTime.UtcNow;
            _context.AdminAnswers.Add(answer);

            question.IsAnswered = true;
            question.Answer = answer.Answer;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
        }

        [HttpGet("answers/{id}")]
        public async Task<IActionResult> GetAnswer(int id)
        {
            var answer = await _context.AdminAnswers.FindAsync(id);

            if (answer == null)
                return NotFound();

            return Ok(answer);
        }

        [HttpDelete("questions/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.QuestionAnswers.FindAsync(id);

            if (question == null)
                return NotFound("Question not found.");

            _context.QuestionAnswers.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(true); // Return true upon successful deletion.
        }


    }

}
