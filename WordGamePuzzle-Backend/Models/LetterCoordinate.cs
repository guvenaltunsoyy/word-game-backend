using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordGamePuzzle_Backend.Controllers;

namespace WordGamePuzzle_Backend.Models
{
    public class LetterCoordinate
    {
        public LetterCoordinate()
        {
            Coordinates = new List<Location>();
        }
        public Words WordModel { get; set; }
        public List<Location> Coordinates { get; set; }
    }

}
