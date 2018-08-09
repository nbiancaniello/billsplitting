using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace BillSplitting
{
    public class FileProcessor
    {
        private static readonly string DoubleOutputPattern = ConfigurationManager.AppSettings["doubleOutputPattern"];
        private static readonly string OutputFileExtension = ConfigurationManager.AppSettings["outputFileExtension"];
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string[] Values { get; set; }

        public FileProcessor(string inputFile)
        {
            InputFile = inputFile;
            OutputFile = Path.Combine(Path.GetDirectoryName(inputFile) + @"\" + Path.GetFileName(inputFile) + OutputFileExtension);
        }

        public bool ProcessValues()
        {
            try
            {
                int valuesLength = Values.Length;
                List<double> tripTotal = new List<double>();
                for (int i = 0; i < valuesLength; i++)
                {
                    if (Values[i] == Validator.ValidEofCharacter)
                    {
                        break;
                    }

                    var membersLeft = Int32.Parse(Values[i]);
                    double amount = 0;
                    for (int j = i + 1; j < valuesLength; j++)
                    {
                        int billsLoopLength = (j + 1) + Int32.Parse(Values[j]);
                        for (int k = j + 1; k < billsLoopLength; k++)
                        {
                            amount += double.Parse(Values[k]);
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

                return true;
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"An error ocurrred while fetching the array of values");
            }
        }

        public string[] RetrieveDataFromInputFile()
        {
            try
            {
                return File.ReadAllLines(InputFile).Where(v => !string.IsNullOrEmpty(v)).ToArray();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"An error ocurred while trying to read from input file");
            }
        }

        public bool SplitBillAndSave(List<double> totalPerMember)
        {
            try
            {
                double billTotalPerMember = (totalPerMember.Sum() / totalPerMember.Count());
                using (StreamWriter writer = File.AppendText(OutputFile))
                {
                    foreach (double total in totalPerMember)
                    {
                        double balance = total - billTotalPerMember;
                        writer.Write($"{balance.ToString(DoubleOutputPattern)}");
                        writer.WriteLine(string.Empty);
                        writer.WriteLine();
                    }

                    writer.WriteLine();
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"No permissions granted to write to output file.");
            }
        }

        public bool DeleteExistingOutputFile()
        {
            try
            {
                if (File.Exists(OutputFile))
                {
                    File.Delete(OutputFile);
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException($"No permissions granted to handle output file.");
            }
        }
    }
}
