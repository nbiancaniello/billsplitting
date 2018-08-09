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

        public void ProcessFile(string[] values)
        {
            int valuesLength = values.Length;
            List<decimal> tripTotal = new List<decimal>();
            for (int i = 0; i < valuesLength; i++)
            {
                if (values[i] == "0")
                {
                    break;
                }

                var membersLeft = Int32.Parse(values[i]);
                decimal amount = 0;
                for (int j = i + 1; j < valuesLength; j++)
                {
                    int billsLoopLength = (j + 1) + Int32.Parse(values[j]);
                    for (int k = j + 1; k < billsLoopLength; k++)
                    {
                        amount += decimal.Parse(values[k]);
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

        public string[] RetrieveStrings()
        {
            return File.ReadAllLines(InputFile).Where(v => !string.IsNullOrEmpty(v)).ToArray();
        }

        public bool ValidateInputFile(string[] values)
        {
            var val = values.Where(v => !Regex.IsMatch(v, "[0-9.]"));
            if (val.Any())
            {
                return false;
            }

            if (values[values.Length - 1] != "0")
            {
                return false;
            }

            return true;
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

        public void DeleteExistingOutputFile()
        {
            if (File.Exists(OutputFile))
            {
                File.Delete(OutputFile);
            }
        }
    }
}
