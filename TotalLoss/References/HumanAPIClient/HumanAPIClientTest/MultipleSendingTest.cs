using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HumanAPIClient.Enum;
using HumanAPIClient.Model;
using HumanAPIClient.Service;
using HumanAPIClient.Util.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HumanAPIClientTest
{
    
    
    /// <summary>
    ///This is a test class for MultipleSendingTest and is intended
    ///to contain all MultipleSendingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultipleSendingTest
    {
        private const String ACCOUNT = "conta";
        private const String CODE = "senha";

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void sendLoginFailTest() 
        {
		    ListResource msg = new ListResource("teste;teste", LayoutTypeEnum.TYPE_A);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(null, null);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com ACCOUNT / CODE invalidos >>");
			    List<String> ret = cliente.send(msg);
			    foreach (String result in ret) 
                {
                    Console.Out.WriteLine(result);
			    }
    			
			    Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
			    Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("Field \"" + MultipleSending.PARAM_ACCOUNT + "\" is required.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileTooLargeTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileTooLarge.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_A);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo muito grande >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File size exceeds the limit of 1MB.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileMalformedATest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedA.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_A);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout A) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileMalformedBTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedB.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_B);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
                Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout B) >>");
			    List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileMalformedDTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedD.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_D);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

            MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout D) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileMalformedETest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedE.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_E);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout E) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendFileMalformedEInvalidDateTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedEInvalidDate.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_E);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout E) - data invalida >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("Date invalid.", e.Message);
		    }
	    }
    	
        [TestMethod()]
	    public void sendListMalformedATest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedA.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }

		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_A);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com lista com problemas de formatacao (layout A) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendListMalformedBTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedB.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
    		
		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_B);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com lista com problemas de formatacao (layout B) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
                Console.Out.WriteLine(e.StackTrace);
                Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendListMalformedDTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedD.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
    		
		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_D);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try {
			    Console.Out.WriteLine("<< Teste de envio de SMS com lista com problemas de formatacao (layout D) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

                Assert.Fail("Deve jogar exception");
		    } catch (ClientHumanException e) {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendListMalformedETest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedE.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
    		
		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_E);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com lista com problemas de formatacao (layout E) >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }
    			
			    Assert.Fail("Deve jogar exception");
		    } 
            catch (ClientHumanException e) 
            {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.AreEqual("File format invalid.", e.Message);
		    }
	    }

        [TestMethod()]
	    public void sendListMalformedEInvalidDateTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileMalformedEInvalidDate.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
    		
		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_E);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo com problemas de formatacao (layout E) - data invalida >>");
			    List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }

			    Assert.Fail("Deve jogar exception");
		    } catch (ClientHumanException e) {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.AreEqual("Date invalid.", e.Message);
		    }
	    }

        [TestMethod()]
        public void sendFileOKTest() 
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileOKD.txt");
            FileInfo file = new FileInfo(path);
		    FileResource msg = new FileResource(file, LayoutTypeEnum.TYPE_D);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo OK >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }
    			
		    } catch (ClientHumanException e) {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.Fail("Nao deve jogar exception");
		    }
	    }

        [TestMethod()]
	    public void sendListOKTest()
        {
            String path = Path.Combine(Environment.CurrentDirectory, "TestFiles\\FileOKD.txt");
            StringBuilder sb = new StringBuilder();
            StreamReader sr = File.OpenText(path);
            String line = null;
            while ((line = sr.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
    		
		    ListResource msg = new ListResource(sb.ToString(), LayoutTypeEnum.TYPE_D);
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com arquivo OK >>");
                List<String> ret = cliente.send(msg);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }
    		
		    } 
            catch (ClientHumanException e) 
            {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.Fail("Nao deve jogar exception");
		    }
	    }

        [TestMethod()]
        public void checkMultipleTest() 
        {
		    MultipleSending cliente = new MultipleSending(ACCOUNT, CODE);

		    try
            {
			    Console.Out.WriteLine("<< Teste de verificacao de status de SMS >>");
			    String[] ids = new String[]{"TESTE0001","TESTE0002","TESTE0003"};
			    List<String> ret = cliente.query(ids);
                foreach (String result in ret)
                {
                    Console.Out.WriteLine(result);
                }
		    }
            catch (ClientHumanException e)
            {
			    Console.Out.WriteLine(e.StackTrace);
			    Assert.Fail("Nao deve jogar exception");
		    }
	    }
    }
}
