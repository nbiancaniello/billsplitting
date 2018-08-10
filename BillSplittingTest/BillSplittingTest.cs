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
        private string[] _outputResult;

        [TestInitialize]
        public void Init_Parameters()
        {
            _args = new[] {@"C:\dev\test.txt"};
            _fileProcessor = new FileProcessor(_args[0])
            {
                Values = new [] { "2", "2", "10.50", "9.30", "3", "5.55", "7.20", "6.99", "0" }
            };
            _totalPerMember = new List<double>(){19.80,19.74};
            _outputResult = new[] {"$0.03","($0.03)"};
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
        public void TestSplitBillAndSaveOutputResult()
        {
            TestProcessValues();
            _fileProcessor.InputFile = @"C:\dev\test.txt.out";
            string[] result = _fileProcessor.RetrieveDataFromInputFile();
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != _outputResult[i])
                {
                    Assert.Fail("The values in output file are not correct");
                }
            }
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
