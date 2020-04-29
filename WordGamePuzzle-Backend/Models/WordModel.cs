using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordGamePuzzle_Backend.Models
{
    public class WordModel : Words
    {
        public WordModel()
        {
            Letters = new List<LetterModel>();
        }
        public List<LetterModel> Letters { get; set; }

    }
}
