using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BillSplitting.Poco
{
    public class InputFilePoco
    {
        static readonly string _decimalOutputPattern = "$##0.00;($##0.00);-\0-";
        static readonly string _outputFileExtension = ".out";
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string[] Values { get; set; }

        public InputFilePoco(string inputFile)
        {
            InputFile = inputFile;
            OutputFile = Path.Combine(Path.GetDirectoryName(inputFile) + @"\" + Path.GetFileName(inputFile) + _outputFileExtension);
        }

        public void ProcessFile()
        {
            int valuesLenght = Values.Length;
            List<decimal> tripTotal = new List<decimal>();
            for (int i = 0; i < valuesLenght; i++)
            {
                if (Values[i] == "0")
                {
                    break;
                }
                var membersLeft = Int32.Parse(Values[i]);
                decimal amount = 0;
                for (int j = i + 1; j < valuesLenght; j++)
                {
                    int billsLoopLenght = (j + 1) + Int32.Parse(Values[j]);
                    for (int k = j + 1; k < billsLoopLenght; k++)
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

        public void ValidateInputFile()
        {
            Values = File.ReadAllLines(InputFile).Where(v => !string.IsNullOrEmpty(v)).ToArray();
            if (!CheckForNumericValues())
            {
                Console.WriteLine($"Error: The file contains non numeric characters.");
                throw new NotFiniteNumberException();
            }
            if (Values[Values.Length - 1] != "0")
            {
                Console.WriteLine($"Error: The file does not contain EOF character (0)");
                throw new InvalidDataException();
            }
        }

        public void SplitBillAndSave(List<decimal> totalPerMember)
        {
            decimal billTotalPerMember = (totalPerMember.Sum() / totalPerMember.Count());
            using (StreamWriter writer = File.AppendText(OutputFile))
            {
                foreach (decimal total in totalPerMember)
                {
                    writer.Write($"{(total - billTotalPerMember).ToString(_decimalOutputPattern)}");
                    writer.WriteLine(string.Empty);
                    writer.WriteLine();
                }
                writer.WriteLine();
            }
        }

        public bool CheckForNumericValues()
        {
            foreach (string item in Values)
            {
                if (!Regex.IsMatch(item, "[0-9.]"))
                {
                    return false;
                }
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
