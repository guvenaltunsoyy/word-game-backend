using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WordGamePuzzle_Backend.Models;

namespace WordGamePuzzle_Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }

        public DbSet<Words> Words { get; set; }
        public DbSet<LetterModel> Letters { get; set; }
        public DbSet<WordLetterMappingModel> WordLetterMapping { get; set; }

    }
}
