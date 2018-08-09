using Microsoft.VisualStudio.TestTools.UnitTesting;
using BillSplitting;
using System.Collections.Generic;

namespace BillSplittingTest
{
    [TestClass]
    public class BillSplittingTest
    {

        private FileProcessor fileProcessor;
        private List<decimal> totalPerMember;
        private string[] args;
        private string[] values;

        [TestInitialize]
        public void Init_Parameters()
        {
            args = new string[1] {@"C:\dev\bill.txt"};
            fileProcessor = new FileProcessor(args[0]);
            values = new string[]{"2","2","10.50","9.30","3","5.55","7.20","6.99","0"};
        }

        [TestMethod]
        public void TestClearFile()
        {
            //fileProcessor.DeleteExistingOutputFile();
        }
        [TestMethod]
        public void TestProcessFile()
        {
            //fileProcessor.ProcessFile();
        }
        [TestMethod]
        public void TestSplitBillAndSave()
        {
            //fileProcessor.SplitBillAndSave();
        }
        [TestMethod]
        public void TestValidateInputFile()
        {

            Assert.IsFalse(fileProcessor.ValidateInputFile());
        }
        //public void TestVerifyInputParameter()
        //{
        //    Program.VerifyInputParameter();
        //}
    }
}
