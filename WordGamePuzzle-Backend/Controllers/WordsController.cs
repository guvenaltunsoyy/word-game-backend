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
        private List<Words> _puzzleWords;
        public WordsController(ILogger<WordsController> logger, 
            DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }
        // GET: api/Words
        [HttpGet("all")]
        public ActionResult GetAllWordModels()
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

        // POST: api/Words/
        [HttpPost]
        public async Task<ActionResult> CreateWord([FromBody] List<Words> wordModels)
        {
            try
            {
                foreach (var wordModel in wordModels)
                {
                    wordModel.Level = wordModel.Word.Length;
                    _dataContext.Words.Add(wordModel);
                    _dataContext.SaveChanges();
                    _logger.LogInformation($"WORD ADDED :{wordModel.Word}");
                    Console.WriteLine("WORD ADDED :{0}", wordModel.Word);

                }

                return Ok(wordModels);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        // GET: api/Words/1/2
        [HttpGet("{level}/{wordCount}")]
        public ActionResult GetPuzzleWords(int level, int wordCount)
        {
            try
            {
                Random r = new Random();
                var randomGroupId = r.Next(0, 7);
                Console.WriteLine($"Random grup id {randomGroupId}");
                var allWords = GetAllWords();
                if (wordCount > 1)
                {
                    _puzzleWords = allWords.Where(x => x.Level == level && x.GroupId == randomGroupId).Take(wordCount - 1).ToList();
                    _puzzleWords.Insert(0, allWords.FirstOrDefault(x => x.GroupId == randomGroupId && x.Level == 6));
                }
                else
                {
                    _puzzleWords = allWords.Where(x => x.Level == level && x.GroupId == randomGroupId).Take(wordCount).ToList();
                }

                if (wordCount != _puzzleWords.Count)
                {
                    if (level == 5)
                    {
                        _puzzleWords.AddRange(allWords.Where(x=> x.Level == (level -1) && x.GroupId == randomGroupId).Take(wordCount - _puzzleWords.Count));
                    }
                   else
                    {
                        _puzzleWords.AddRange(allWords.Where(x => x.Level == (level+1) && x.GroupId == randomGroupId).Take(wordCount - _puzzleWords.Count));
                    }
                }

                var res = PuzzleProducer.Instance.GetMatris(9, 15, _puzzleWords);
                string jsonData = JsonConvert.SerializeObject(res);
                return Content(jsonData, "application/json");
            }
            catch (Exception e)
            {
                return StatusCode(404, e.Message);
            }
        }

        // GET: api/Words
        [HttpGet("")]
        public ActionResult GetWords()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(PuzzleProducer.Instance.GetLetterCoordinates());
                PuzzleProducer.Instance.GetLetterCoordinates().ForEach(x =>
                {
                    Console.WriteLine(x.WordModel.Word);
                });
                return Content(jsonData, "application/json");
            }
            catch (Exception e)
            {
                return StatusCode(404);
            }
        }
        public List<Words> GetAllWords()
        {
            List<Words> words = new List<Words>();
            var result = _dataContext.Words.ToList();
            result.ForEach(x=>
            {
                words.Add(new Words
                {
                    Id = x.Id,
                    Word = x.Word,
                    GroupId = x.GroupId,
                    Level =  x.Level
                });
            });
            return words;
        }
    }
}
