using Microsoft.VisualStudio.TestTools.UnitTesting;
using BillSplitting;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace BillSplittingTest
{
    [TestClass]
    public class BillSplittingTest
    {
        private FileProcessor _fileProcessor;
        private List<double> _totalPerMember;
        private string[] _args;

        [TestInitialize]
        public void Init_Parameters()
        {
            _args = new[] {@"C:\dev\bill.txt"};
            _fileProcessor = new FileProcessor(_args[0])
            {
                Values = new [] { "2", "2", "10.50", "9.30", "3", "5.55", "7.20", "6.99", "0" }
            };
            _totalPerMember = new List<double>(){19.50,19.74};
        }

        [TestMethod]
        public void TestDeleteExistingOutputFile()
        {
            _fileProcessor.DeleteExistingOutputFile();
            if (File.Exists(_fileProcessor.OutputFile))
            {
                Assert.Fail("The output file has not been deleted.") ;
            }
        }

        [TestMethod]
        public void TestInvalidDeleteExistingOutputFile()
        {
            Assert.ThrowsException<UnauthorizedAccessException>(() => _fileProcessor.DeleteExistingOutputFile());
        }

        [TestMethod]
        public void TestProcessValues()
        {
            Assert.IsTrue(_fileProcessor.ProcessValues());
        }

        [TestMethod]
        public void TestInvalidProcessValues()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => _fileProcessor.ProcessValues());
        }

        [TestMethod]
        public void TestSplitBillAndSave()
        {
            Assert.IsTrue(_fileProcessor.SplitBillAndSave(_totalPerMember));
        }

        [TestMethod]
        public void TestInvalidSplitBillAndSave()
        {
            Assert.ThrowsException<UnauthorizedAccessException>(() => _fileProcessor.SplitBillAndSave(_totalPerMember));
        }

        [TestMethod]
        public void TestCorrectValidateInputFile()
        {
            Assert.IsTrue(Validator.ValidateDataFromInputFile(_fileProcessor.Values));
        }

        [TestMethod]
        public void TestInvalidValidateInputFileNonNumericCharacter()
        {
            _fileProcessor.Values[0] = "A";
            Assert.ThrowsException<InvalidDataException>(() => Validator.ValidateDataFromInputFile(_fileProcessor.Values));
        }

        [TestMethod]
        public void TestInvalidValidateInputFileInvalidEof()
        {
            _fileProcessor.Values[_fileProcessor.Values.Length-1] = "";
            Assert.ThrowsException<InvalidDataException>(() => Validator.ValidateDataFromInputFile(_fileProcessor.Values));
        }

        [TestMethod]
        public void TestVerifyInputParameter()
        {
            Validator.VerifyInputParameter(_args);
        }

        [TestMethod]
        public void TestInvalidVerifyInputParameter()
        {
            _args[0] = "";
            Assert.ThrowsException<ArgumentNullException>(() => Validator.VerifyInputParameter(_args));
        }

        [TestMethod]
        public void TestRetrieveDataFromInputFile()
        {
            _fileProcessor.Values = _fileProcessor.RetrieveDataFromInputFile();
            Assert.IsTrue(_fileProcessor.Values.Any());
        }

        [TestMethod]
        public void TestInvalidRetrieveDataFromInputFileFileNotFoundException()
        {
            Assert.ThrowsException<FileNotFoundException>(() => _fileProcessor.RetrieveDataFromInputFile());
        }
    }
}
