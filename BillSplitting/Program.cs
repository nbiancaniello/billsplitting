using System;
using System.IO;

namespace BillSplitting
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Validator.VerifyInputParameter(args);
                FileProcessor fileProcessor = new FileProcessor(args[0]);
                fileProcessor.Values = fileProcessor.RetrieveDataFromInputFile();
                if (Validator.ValidateDataFromInputFile(fileProcessor.Values))
                {
                    fileProcessor.DeleteExistingOutputFile();
                    fileProcessor.ProcessValues();
                    Console.WriteLine(@"The program finished with no errors and the file {0} has been created.",
                        fileProcessor.OutputFile);
                    Console.ReadKey();
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
    }
}
