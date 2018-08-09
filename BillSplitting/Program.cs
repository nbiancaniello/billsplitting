using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BillSplitting
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                VerifyInputParameter(args);
                FileProcessor fileProcessor = new FileProcessor(args[0]);
                fileProcessor.Values = fileProcessor.RetrieveStrings();
                if (fileProcessor.ValidateInputFile(values))
                {
                    fileProcessor.DeleteExistingOutputFile();
                    fileProcessor.ProcessFile(values);
                    Console.WriteLine(@"The program finished with no errors and the file {0} has been created.",
                        fileProcessor.OutputFile);
                    Console.ReadLine();
                }
                else
                {

                }
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException);
            }
            catch (SystemException systemException)
            {
                Console.WriteLine(systemException);
            }
        }

        public static void VerifyInputParameter(string[] args)
        {
            if (args != null && args.Length != 0 && !string.IsNullOrEmpty(args[0]) &&
                !string.IsNullOrWhiteSpace(args[0])) return;
            Console.WriteLine($"Error: No argument has been passed.");
            throw new ArgumentNullException();
        }
    }
}
