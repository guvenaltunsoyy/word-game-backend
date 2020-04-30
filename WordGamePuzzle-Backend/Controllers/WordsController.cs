using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WordGamePuzzle_Backend.Data;
using Newtonsoft.Json;
using WordGamePuzzle_Backend.Models;

namespace WordGamePuzzle_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly ILogger<WordsController> _logger;
        private readonly DataContext _dataContext;
        private readonly List<LetterModel> _letters;
        public WordsController(ILogger<WordsController> logger, 
            DataContext context)
        {
            _logger = logger;
            _dataContext = context;
            _letters = _dataContext.Letters.ToList();
        }
        // GET: api/Words
        [HttpGet]
        public ActionResult GetWords()
        {
            _logger.LogInformation(message:"GetAllWords Called");
            try
            {
                return Ok(GetAllWords());
            }
            catch (Exception e)
            {
                _logger.LogError(message:e.Message);
                return StatusCode(404, "An error has occurred");
            }
        }

        // GET: api/Words/id/5
        [HttpGet("id/{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            try
            {
                _logger.LogWarning(null,$"Get Word wıth {id} id");
                var result =  _dataContext.Words.FirstOrDefault(x=>x.Id==id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(404);
            }
        }

        // api/words/mapping
        [HttpGet("mapping")]
        public ActionResult GetMapping()
        {
            _logger.LogInformation(message: "mapping Called");
            try
            {
                return Ok(_dataContext.WordLetterMapping.ToList());
            }
            catch (Exception e)
            {
                _logger.LogError(message: e.Message);
                return StatusCode(404, "An error has occurred");
            }
        }

        // POST: api/Words
        [HttpPost]
        public async Task<ActionResult> CreateWord([FromBody] Words wordModel)
        {
            try
            {
                var word= _dataContext.Words.Add(wordModel);
                _dataContext.SaveChanges();

                foreach (char c in wordModel.Word)
                {
                    WordLetterMapping(wordModel.Id, c.ToString());
                    await Task.Delay(100);
                }

                return Ok(wordModel);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public ActionResult WordLetterMapping(int? wId, string l)
        {
            try
            {
                var letter = _letters.FirstOrDefault(x => x.Letter == l);
                _dataContext.WordLetterMapping.Add(new WordLetterMappingModel
                {
                    LetterId = letter.Id,
                    WordId = wId ?? 0
                });
                _dataContext.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500);
            }
        }


        // GET: api/Words/1/2
        [HttpGet("{groupId}/{wordCount}")]
        public ActionResult GetGameWords(int groupId, int wordCount)
        {
            try
            {
                var _words = GetAllWords().Where(x => x.GroupId == groupId).Take(wordCount).ToList();
                PuzzleProducer puzzle = new PuzzleProducer(7, 15, _words);
                var res = puzzle.GetMatris();
                string jsonData = JsonConvert.SerializeObject(res);
                return Content(jsonData, "application/json");
            }
            catch (Exception e)
            {
                return StatusCode(404);
            }
        }

        public List<WordModel> GetAllWords()
        {
            List<LetterModel> _lets;
            List<WordModel> words = new List<WordModel>();
            var result = _dataContext.Words.ToList();
            var mapping = _dataContext.WordLetterMapping.ToList();
            result.ForEach(x=>
            {
                var r = mapping.Where(y => y.WordId == x.Id).ToList();
                _lets = new List<LetterModel>();
                r.ForEach(s =>
                {
                    _lets.Add(_letters.FirstOrDefault(a=>a.Id==s.LetterId));
                });
                words.Add(new WordModel
                {
                    Id = x.Id,
                    Word = x.Word,
                    GroupId = x.GroupId,
                    Letters = _lets
                });
            });
            return words;
        }
    }
}
