using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BillSplitting
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                #region Parameter Verification
                verifyParameter(args);
                #endregion Parameter Verification

                //IEnumerable<string> vals = File.ReadAllLines(args[0]).Where(v => !string.IsNullOrEmpty(v));
                string[] vals = File.ReadAllLines(args[0]).Where(v => !string.IsNullOrEmpty(v)).ToArray();
                int members = 0, bills = 0;
                decimal amount = 0;
                List<decimal> subTotal = new List<decimal>();

                //foreach (var item in vals)
                for(int i=0;i<vals.Length;i++)
                {
                    if (vals[i] == "0")
                    {
                        break;
                    }

                    members = Int32.Parse(vals[i]);
                    for(int j = i + 1; j < vals.Length; j++)
                    {
                        bills = Int32.Parse(vals[j]);
                        for(int k = j+1; k < ((vals.Length - (vals.Length - (j+1))+bills)); k++)
                        {
                            amount += decimal.Parse(vals[k]);
                            i = k;
                        }
                        subTotal.Add(amount);
                        amount = 0;
                        j = i;
                        members--;

                        if(members == 0)
                        {

                            Console.WriteLine((subTotal.Sum() / subTotal.Count()).ToString());
                        }
                    }
                }

                #region Get File Name and Path
                string path = getDirectoryName(args[0]);
                string filename = getFileName(args[0]);
                #endregion Get File Name and Path
            }
            catch(IOException ioe)
            {
                Console.WriteLine(ioe);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }            
        }

        public static bool isDecimal(string val)
        {
            string decimalPattern = @"^[0-9]([.][0-9]{2,2})?$";
            Regex reg = new Regex(decimalPattern);
            return reg.Match(val).Success;
        }

        public static void verifyParameter(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: No argument has been passed.");
                throw new ArgumentNullException();
            }
        }

        public static string getDirectoryName(string arg)
        {
            return Path.GetDirectoryName(arg);
        }
        public static string getFileName(string arg)
        {
            return Path.GetFileNameWithoutExtension(arg);
        }
    }
}
