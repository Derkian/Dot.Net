using System;
using System.Collections.Generic;
using HumanAPIClient.Enum;
using HumanAPIClient.Model;
using HumanAPIClient.Service;
using HumanAPIClient.Util.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HumanAPIClientTest
{
    /// <summary>
    ///This is a test class for SimpleSendingTest and is intended
    ///to contain all SimpleSendingTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SimpleSendingTest
    {
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

        private const string ACCOUNT = "conta";
        private const string CODE = "senha";

        [TestMethod()]
        public void sendLoginFailTest() 
        {
		    SimpleMessage msg = new SimpleMessage();
		    msg.From = "Human";
		    msg.Message = "Teste de envio de SMS com account / code invalidos.";
		    msg.To = "550093429020";
		    msg.Callback = CallbackTypeEnum.INACTIVE;

		    SimpleSending cliente = new SimpleSending(null, null);

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
                Assert.AreEqual("Field \"" + SimpleSending.PARAM_ACCOUNT + "\" is required.", e.Message);
		    }
	    }

        [TestMethod()]
        public void sendSMSTooLargeTest() 
        {
		    SimpleMessage msg = new SimpleMessage();
		    msg.From = "Human";
		    msg.Message = "123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.";
            msg.To = "550093429020";
		    msg.Callback = CallbackTypeEnum.INACTIVE;

            SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS com MSG muito longo >>");
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
                Assert.AreEqual("Fields \"" + SimpleSending.PARAM_MSG + "\" + \"" + SimpleSending.PARAM_FROM + "\" can not exceed " + SimpleSending.BODY_MAX_LENGTH + " characters.", e.Message);
		    }
	    }

        [TestMethod()]
        public void sendSMSTooLarge2Test() 
        {
	        SimpleMessage msg = new SimpleMessage();
	        msg.Message = "123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123456789.123";
            msg.To = "550093429020";
	        msg.Callback = CallbackTypeEnum.INACTIVE;

	        SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

	        try 
            {
                Console.Out.WriteLine("<< Teste de envio de SMS com MSG muito longo e sem FROM >>");
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
		        Assert.AreEqual("Fields \"" + SimpleSending.PARAM_MSG + "\" + \"" + SimpleSending.PARAM_FROM + "\" can not exceed " + SimpleSending.BODY_MAX_LENGTH + " characters.", e.Message);
	        }
        }

        [TestMethod()]
        public void sendIdTooLargeTest()
        {
	        SimpleMessage msg = new SimpleMessage();
	        msg.From = "Human";
	        msg.Message = "Teste de envio de SMS com id invalido.";
            msg.To = "550093429020";
	        msg.Id = "123456789.123456789.1";
	        msg.Callback = CallbackTypeEnum.INACTIVE;

            SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

	        try 
            {
                Console.Out.WriteLine("<< Teste de envio de SMS com ID invalidos >>");
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
		        Assert.AreEqual("Field \"" + SimpleSending.PARAM_ID + "\" can not have more than " + SimpleSending.ID_MAX_LENGTH + " characters.", e.Message);
	        }
        }

        [TestMethod()]
        public void sendMissingToTest() 
        {
	        SimpleMessage msg = new SimpleMessage();
	        msg.From = "Human";
	        msg.Message = "Teste de envio de SMS sem destinatario.";
	        msg.Callback = CallbackTypeEnum.INACTIVE;

	        SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

	        try 
            {
                Console.Out.WriteLine("<< Teste de envio de SMS sem TO >>");
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
		        Assert.AreEqual("Field \"" + SimpleSending.PARAM_TO + "\" is required.", e.Message);
	        }
        }

        [TestMethod()]
        public void sendMissingMsgTest() 
        {
	        SimpleMessage msg = new SimpleMessage();
	        msg.From = "Human";
            msg.To = "550093429020";
	        msg.Callback = CallbackTypeEnum.INACTIVE;

            SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

	        try 
            {
                Console.Out.WriteLine("<< Teste de envio de SMS sem MSG >>");
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
		        Assert.AreEqual("Field \"" + SimpleSending.PARAM_MSG + "\" is required.", e.Message);
	        }
        }

        [TestMethod()]
        public void sendOKTest() 
        {
		    SimpleMessage msg = new SimpleMessage();
		    msg.From = "Human";
		    msg.Message = "Teste de envio de SMS OK";
		    msg.To = "550093429020";
		    msg.Id = "TESTE0001";
		    msg.Schedule = "27/05/2011 10:00:00";

            SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de envio de SMS OK >>");
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
        public void checkOneTest() 
        {
            Console.WriteLine("Version: {0}", Environment.Version.ToString());

		    SimpleSending cliente = new SimpleSending(ACCOUNT, CODE);

		    try 
            {
			    Console.Out.WriteLine("<< Teste de verificacao de status de SMS >>");
			    String id = "TESTE0001";
			    List<String> ret = cliente.query(id);
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
