using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using WordGamePuzzle_Backend.Models;

namespace WordGamePuzzle_Backend.Controllers
{
    public class PuzzleProducer
    {
        private int mDepth { get; set; }
        private int mWidth { get; set; }
        private string[,] matris;
        private bool isFirstWord;
        private List<Words> Words;
        private List<LetterCoordinate> letterCoordinates;

        PuzzleProducer()
        {
        }
        private static readonly object padlock = new object();
        private static PuzzleProducer instance = null;

        public static PuzzleProducer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new PuzzleProducer();
                    }

                    return instance;
                }
            }
        }


        public string[,] GetMatris(int depth = 7, int width = 15, List<Words> words = null)
        {
            mDepth = depth;
            mWidth = width;
            matris = new string[mDepth, mWidth];
            isFirstWord = true;
            Words = words;
            letterCoordinates = new List<LetterCoordinate>();
            Console.WriteLine($"Matris initialized. Word count :{Words.Count}");

            foreach (var word in Words)
            {
                Console.WriteLine($"Word : {word.Word}");
                letterCoordinates.Add(new LetterCoordinate
                {
                    WordModel = word
                });
                var res = WriteWord(word);
                if (!res)
                {
                    Console.WriteLine("Not written. Try again");
                    if (CheckMatrisVerticalToDown(word, 1, mWidth-2))
                    {
                        WriteWordToDown(word, 0, mWidth - 1);
                    }
                    else if (CheckMatrisVerticalToDown(word, 1, 1))
                    {
                        WriteWordToDown(word, 0, 0);
                    }
                    else if (CheckMatrisHorizontalToRight(word,1,1))
                    {
                        WriteWordToRight(word, 0, 0);
                    }
                    else if (CheckMatrisHorizontalToRight(word, depth-1, 1))
                    {
                        WriteWordToRight(word, depth-1, 1);
                    }
                    else
                    {
                        Console.WriteLine("Matris not available.");
                    }
                }
            }
            Console.WriteLine("Words finished.");
            PrintMatris(); 
            return matris;
        }

        bool WriteWord(Words word)
        {
            if (isFirstWord)
            {
                if (word.Word.Length != mWidth)
                {
                    for (int i = 0; i < word.Word.Length; i++)
                    {
                        matris[(mDepth / 3), ((mWidth / 5) + i)] = word.Word[i].ToString();
                        letterCoordinates
                            .FirstOrDefault(x => x.WordModel.Id == word.Id)
                            ?.Coordinates.Add(new Location
                            {
                                x = (mDepth / 3),
                                y = ((mWidth / 5) + i)
                            });
                    }

                    isFirstWord = false;
                    Console.WriteLine("First word created");
                    return true;
                }
            }
            else
            {
                for (int k = 0; k < word.Word.Length; k++)
                {
                    var letter = word.Word[k].ToString();
                    var locations = FindLetterLocationInMatris(letter);
                    if (locations.Count > 0)
                    {
                        foreach (var location in locations)
                        {
                            try
                            {
                                // check matris vertical
                                if (string.IsNullOrEmpty(matris[location.x - 1, location.y]) && string.IsNullOrEmpty(matris[location.x + 1, location.y]))
                                {
                                    if (k == 0 && CheckMatrisVerticalToDown(word, location.x, location.y))
                                    {
                                        if (WriteWordToDown(word, location.x, location.y))
                                        {
                                            Console.WriteLine("the word was written down");
                                            return true;
                                        }
                                    }
                                    else if (k == word.Word.Length - 1 &&
                                             CheckMatrisVerticalToUp(word, location.x, location.y))
                                    {
                                        if (WriteWordToUpwards(word, location.x, location.y))
                                        {
                                            Console.WriteLine("the word was written upwards");
                                            return true;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"ERROR Vertical {e.Message}");
                            }

                            try
                            {
                                if (string.IsNullOrEmpty(matris[location.x, location.y - 1]) &&
                                    string.IsNullOrEmpty(matris[location.x, location.y + 1])) // check matris horizontal
                                {
                                    if (k == 0 && CheckMatrisHorizontalToRight(word, location.x, location.y))
                                    {
                                        if (WriteWordToRight(word, location.x, location.y))
                                        {
                                            Console.WriteLine("the word was written right");
                                            return true;
                                        }
                                    }
                                    else if (k == word.Word.Length - 1 &&
                                             CheckMatrisHorizontalToLeft(word, location.x, location.y))
                                    {
                                        if (WriteWordToLeft(word, location.x, location.y))
                                        {
                                            Console.WriteLine("the word was written left");
                                            return true;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"ERROR Horizontal {e.Message}");
                            }
                        }
                    }
                }
            }

            return false;
        }

        bool CheckMatrisHorizontalToRight(Words word, int x, int y)
        {
            try
            {
                for (int i = 1; i < word.Word.Length; i++)
                {
                    if (!string.IsNullOrEmpty(matris[x, y + i]) || !string.IsNullOrEmpty(matris[(x + 1), (y + i)]) || !string.IsNullOrEmpty(matris[(x - 1), (y + i)]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool CheckMatrisHorizontalToLeft(Words word, int x, int y)
        {
            try
            {
                for (int i = 1; i < word.Word.Length; i++)
                {
                    if (!string.IsNullOrEmpty(matris[x, y - i]) || !string.IsNullOrEmpty(matris[x + 1, y - i]) || !string.IsNullOrEmpty(matris[x - 1, y - i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool WriteWordToRight(Words word, int x, int y)
        {
            try
            {
                for (int i = 0; i < word.Word.Length; i++)
                {
                    matris[x, y + i] = word.Word[i].ToString();
                    letterCoordinates
                        .FirstOrDefault(x => x.WordModel.Id == word.Id)
                        .Coordinates.Add(new Location
                        {
                            x = x,
                            y = y + i
                        });
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool WriteWordToLeft(Words word, int x, int y)
        {
            return WriteWordToRight(word, x, y - word.Word.Length + 1);
        }

        bool CheckMatrisVerticalToUp(Words word, int x, int y)
        {
            try
            {
                for (int i = 1; i < word.Word.Length; i++)
                {
                    if (!string.IsNullOrEmpty(matris[x - i, y]) || !string.IsNullOrEmpty(matris[x - i, y + 1]) || !string.IsNullOrEmpty(matris[x - i, y - 1]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool CheckMatrisVerticalToDown(Words word, int x, int y)
        {
            try
            {
                for (int i = 1; i < word.Word.Length; i++)
                {
                    if (!string.IsNullOrEmpty(matris[x + i, y]) || !string.IsNullOrEmpty(matris[x + i, y + 1]) || !string.IsNullOrEmpty(matris[x + i, y - 1]))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool WriteWordToDown(Words word, int x, int y)
        {
            try
            {
                for (int i = 0; i < word.Word.Length; i++)
                {
                    matris[x + i, y] = word.Word[i].ToString();
                    letterCoordinates
                        .FirstOrDefault(x=>x.WordModel.Id == word.Id)
                        .Coordinates.Add(new Location
                        {
                            x= x + i,
                            y = y
                        });
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool WriteWordToUpwards(Words word, int x, int y)
        {
            return WriteWordToDown(word, x - (word.Word.Length - 1), y);
        }

        List<Location> FindLetterLocationInMatris(string l)
        {
            List<Location> index = new List<Location>();
            for (int i = 0; i < mDepth; i++)
            {
                for (int j = 0; j < mWidth; j++)
                {
                    if (matris[i, j] == l)
                    {
                        index.Add(new Location
                        {
                            x = i,
                            y = j
                        });
                    }
                }
            }

            return index;
        }

        void PrintMatris()
        {
            for (int i = 0; i < mDepth; i++)
            {
                Console.Write($"{i + 1} | ");
                for (int j = 0; j < mWidth; j++)
                {
                    if (string.IsNullOrEmpty(matris[i, j]))
                    {
                        matris[i, j] = "0";
                    }
                    Console.Write($"{matris[i, j]}");
                }

                Console.WriteLine(" |");
            }
        }

        public List<LetterCoordinate> GetLetterCoordinates()
        {
            /*foreach (var letterCoordinate in letterCoordinates) 
            {
                letterCoordinate.Words.Letters = letterCoordinate.Words.Letters.OrderBy(x => x.Id).ToList();
            }*/
            return letterCoordinates;
        }
    }

    public class Location
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}