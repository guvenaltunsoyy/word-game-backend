using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordGamePuzzle_Backend.Models
{
    public class WordLetterMappingModel
    {
        public int? Id { get; set; }
        public int WordId { get; set; }
        public int LetterId { get; set; }
    }
}
