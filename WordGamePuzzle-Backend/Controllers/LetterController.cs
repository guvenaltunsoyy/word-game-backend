using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WordGamePuzzle_Backend.Data;
using WordGamePuzzle_Backend.Models;

namespace WordGamePuzzle_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LetterController : ControllerBase
    {
        private readonly ILogger<LetterController> _logger;
        private readonly DataContext _dataContext;

        public LetterController(ILogger<LetterController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public List<LetterModel> GetLetter()
        {
            return _dataContext.Letters.ToList();
        }

        // api/letter
        [HttpGet]
        public ActionResult GetLetters()
        {
            _logger.LogInformation(message: "GetLetters Called");
            try
            {
                return Ok(GetLetter());
            }
            catch (Exception e)
            {
                _logger.LogError(message: e.Message);
                return StatusCode(404, "An error has occurred");
            }
        }
    }
}