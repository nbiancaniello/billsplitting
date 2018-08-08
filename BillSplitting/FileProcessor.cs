using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BillSplitting
{
    public class FileProcessor
    {
        static readonly string _decimalOutputPattern = "$##0.00;($##0.00);-\0-";
        static readonly string _outputFileExtension = ".out";
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string[] Values { get; set; }

        public FileProcessor(string inputFile)
        {
            InputFile = inputFile;
            OutputFile = Path.Combine(Path.GetDirectoryName(inputFile) + @"\" + Path.GetFileName(inputFile) + _outputFileExtension);
        }

        public void ProcessFile()
        {
            try
            {
                int valuesLength = Values.Length;
                List<decimal> tripTotal = new List<decimal>();
                for (int i = 0; i < valuesLength; i++)
                {
                    if (Values[i] == "0")
                    {
                        break;
                    }

                    var membersLeft = Int32.Parse(Values[i]);
                    decimal amount = 0;
                    for (int j = i + 1; j < valuesLength; j++)
                    {
                        int billsLoopLength = (j + 1) + Int32.Parse(Values[j]);
                        for (int k = j + 1; k < billsLoopLength; k++)
                        {
                            amount += decimal.Parse(Values[k]);
                            i = k;
                        }

                        tripTotal.Add(amount);
                        amount = 0;
                        j = i;
                        membersLeft--;

                        if (membersLeft == 0)
                        {
                            SplitBillAndSave(tripTotal);
                            tripTotal.Clear();
                            break;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException();
            }
            
        }

        public void ValidateInputFile()
        {
            Values = File.ReadAllLines(InputFile).Where(v => !string.IsNullOrEmpty(v)).ToArray();

            if (!CheckForNumericValues())
            {
                Console.WriteLine($"Error: The file contains non numeric characters.");
                throw new InvalidDataException();
            }

            if (Values[Values.Length - 1] != "0")
            {
                Console.WriteLine($"Error: The file does not contain EOF character (0)");
                throw new InvalidDataException();
            }
        }

        public void SplitBillAndSave(List<decimal> totalPerMember)
        {
            try
            {
                decimal billTotalPerMember = (totalPerMember.Sum() / totalPerMember.Count());
                using (StreamWriter writer = File.AppendText(OutputFile))
                {
                    foreach (decimal total in totalPerMember)
                    {
                        decimal balance = total - billTotalPerMember;
                        writer.Write($"{balance.ToString(_decimalOutputPattern)}");
                        writer.WriteLine(string.Empty);
                        writer.WriteLine();
                    }

                    writer.WriteLine();
                }
            }
            catch
            {
                Console.WriteLine($"Error: a problem occurred while writing to output file.");
                throw new IOException();
            }
            
        }

        public bool CheckForNumericValues()
        {
            var val = Values.Where(v => !Regex.IsMatch(v, "[0-9.]"));
            if (val.Any())
            {
                return false;
            }
            return true;
        }

        public void DeleteExistingOutputFile()
        {
            if (File.Exists(OutputFile))
            {
                File.Delete(OutputFile);
            }
        }
    }
}
