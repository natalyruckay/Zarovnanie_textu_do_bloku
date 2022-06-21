using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace zarovnani_do_bloku
{
    class Block
    {
        private char[] separators = new char[] { '\n', '\t', ' ' };
        public int max;

        public Block(int line_size)
        {
            max  = line_size;
        }

        public void Reader(string file_read, string file_write)
        {
            int current_number = 0;
            char symbol;
            string word = "";
            bool enter = false, white_symbol = true, paragraph = false;

            List<string> Words = new List<string>();

            try
            {
                //ak niečo výstupný súbor obsahuje, vymažem to
                StreamWriter write = new StreamWriter(file_write);
                write.Write("");
                write.Close();

                StreamReader read = new StreamReader(file_read);

                while (!read.EndOfStream)
                {
                    symbol = (char)read.Read();

                    if (!separators.Contains(symbol))
                    {
                        white_symbol = false;
                        enter = false;
                        word += symbol;
                    }

                    else
                    {
                        if (symbol == '\n')
                        {
                            if (enter)
                            {
                                enter = false;

                                if (Words.Count > 0)
                                {
                                    Writer(file_write, (max - Words.Count + 1), max, Words, paragraph);
                                    Words.Clear();
                                    paragraph = true;
                                }

                                current_number = 0;
                            }
                            else
                            {
                                enter = true;
                            }
                        }

                        if ((word.Length + current_number + Words.Count) < (max + 1) && word != "")
                        {
                            Words.Add(word);
                            current_number += word.Length;
                            word = "";
                            white_symbol = true;
                        }

                        else if (!white_symbol)
                        {
                            if (Words.Count > 0)
                            {
                                Writer(file_write, current_number, max, Words, paragraph);
                                Words.Clear();
                                paragraph = false;
                            }

                            Words.Add(word);
                            current_number = word.Length;
                            word = "";

                            white_symbol = true;
                        }
                    }
                }

                if (Words.Count > 0)
                {
                    Writer(file_write, (max - Words.Count + 1), max, Words, paragraph);
                    Words.Clear();
                    current_number = 0;
                    paragraph = false;
                }

                read.Close();
            }

            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }

        public void Writer(string file_write, int number_of_symbols, int max, List<string> Words, bool end_of_paragraph)
        {
            int spaces = 0, left_spaces = 0; //count = Words.Count;
            string space = "";

            if ((number_of_symbols + Words.Count == max + 1) && (number_of_symbols != max))
            {
                spaces = 1;
            }
            else if (Words.Count > 1)
            {
                spaces = (max - number_of_symbols) / (Words.Count - 1);
                left_spaces = (max - number_of_symbols) % (Words.Count - 1);
            }

            for (int i = 0; i < spaces; i++)
            {
                space += " ";
            }

            try
            {
                StreamWriter write = File.AppendText(file_write);

                if (end_of_paragraph)
                {
                    write.WriteLine();
                }

                for (int i = 0; i < Words.Count - 1; i++)
                {
                    write.Write(Words[i] + space);

                    if (i < left_spaces)
                    {
                        write.Write(" ");
                    }
                }

                write.Write(Words[Words.Count - 1]);
                write.WriteLine();
                write.Close();
            }
            catch
            {
                Console.WriteLine("File Error");
                return;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3 && (int.TryParse(args[2], out int n) && n > 0))
            {
                Block block = new Block(n);
                block.Reader(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("Argument Error");
            }
        }
    }
}
