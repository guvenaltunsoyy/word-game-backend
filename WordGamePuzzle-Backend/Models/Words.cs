using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WordGamePuzzle_Backend.Models
{
    public class Words
    {
        public int? Id { get; set; }
        public string Word { get; set; }
        public int GroupId { get; set; }
    }
}
