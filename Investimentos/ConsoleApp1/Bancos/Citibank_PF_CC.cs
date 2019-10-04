



//// conta migrada para Itau e cartão de crédito para Credicard




//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.Support.UI;

//public static class Citibank_PF_CC 
//{
//    public static List<Extrato> Sync(Dictionary<string, DateTime> contas)
//    {
//        var conta = "Citibank Fe";
//        var extratos = new List<Extrato>();

//        IWebDriver driver = null;
//        var cultureBR = new CultureInfo("pt-BR");

//        try
//        {
//            UI.StartChrome(ref driver, 1);

//            driver.Url = "https://www.citibank.com.br/BRGCB/JPS/portal/Index.do";
//            UI.WaitPageLoad(driver);


//            // ------------------------------------------------------------------------------------------------------------------------------
//            // Tela: Marketing, as vezes aparece
//            // ------------------------------------------------------------------------------------------------------------------------------

//            if (UI.IsDisplayed(driver, By.Id("NH001_CL")))
//            {
//                UI.Click(driver, By.Id("NH001_CL"));
//                UI.WaitPageLoad(driver);
//            }


//            // ------------------------------------------------------------------------------------------------------------------------------
//            // Tela: Home
//            // ------------------------------------------------------------------------------------------------------------------------------

//            UI.SetTextBoxValue(driver, By.Name("username"), "login");
//            UI.SetTextBoxValue(driver, By.Name("password"), "");
//            UI.WaitForDisplayed(driver, By.Id("vkb"));

//            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[24]"));
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[20]"));
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[senha]"));
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[senha]"));
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[senha]"));
//            UI.ClickWithJavascript(driver, executor, By.XPath(".//*[@id='login-box']/map/area[senha]"));
//            UI.Click(driver, By.Id("link_avtEnterSite"));
//            UI.WaitPageLoad(driver);
//            UI.Wait(1);


//            //// ------------------------------------------------------------------------------------------------------------------------------
//            //// Tela: Home Logada
//            //// ------------------------------------------------------------------------------------------------------------------------------

//            //// Clica na conta bancária
//            //UI.ClickWhenDisplayed(driver, By.Id("cmlink_AccountNameLink"));
//            //UI.WaitPageLoad(driver);


//            //// ------------------------------------------------------------------------------------------------------------------------------
//            //// Tela: Conta bancária
//            //// ------------------------------------------------------------------------------------------------------------------------------

//            //// Clica no combo de filtro de período
//            //UI.WaitForDisplayed(driver, By.Id("buttonPanel"));
//            //var combo = driver.FindElement(By.Id("filterDropDown"));
//            //UI.ChangeAttribute(driver, executor, combo, "style", "display: block;");
//            //var options = new SelectElement(combo);
//            //options.SelectByText("Intervalo de datas");
//            //UI.ScrollDown(driver);

//            //// Preenche as datas
//            //UI.WaitForDisplayed(driver, By.Id("dateRangeFrom"));
//            //UI.SetTextBoxValue(driver, By.Id("dateRangeFrom"), contas[conta].ToString("dd/MM/yyyy"));
//            //UI.SetTextBoxValue(driver, By.Id("dateRangeTo"), DateTime.Today.ToString("dd/MM/yyyy"));
//            //UI.ClickWhenDisplayed(driver, By.Id("dateRangeButton_daterange"));

//            //// Espera o loading sumir
//            //UI.WaitUntilDisappear(driver, By.Id("transactionSpinner"));
//            //UI.ScrollDown(driver);

//            //// Clica no "Ver mais transações", se existir
//            //while (driver.FindElement(By.Id("transactionPanel")).Text.Contains("Ver mais transações"))
//            //{
//            //    UI.Click(driver, By.Id("cmlink_SeeMoreActivityLink"));
//            //    UI.WaitUntilDisappear(driver, By.Id("transactionSpinner"));
//            //    UI.ScrollDown(driver);
//            //}

//            //// Lê o extrato
//            //if (UI.IsDisplayed(driver, driver.FindElement(By.Id("postedTansactionTable")), By.TagName("tbody")))
//            //{
//            //    // Lê o extrato
//            //    IList<IWebElement> trs = driver.FindElement(By.Id("postedTansactionTable")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));

//            //    foreach (IWebElement tr in trs)
//            //    {
//            //        IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

//            //        var extrato = new Extrato();
//            //        extrato.conta = conta;
//            //        extrato.data = DateTime.ParseExact(tds[1].Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
//            //        extrato.descricao = ExtratoHelper.GetDescricao(tds[2].Text);

//            //        if (extrato.descricao != null)
//            //        {
//            //            decimal? nullable;
//            //            extrato.debito = string.IsNullOrWhiteSpace(tds[3].Text) ? ((decimal?)(nullable = null)) : new decimal?(decimal.MinusOne * decimal.Parse(tds[3].Text, NumberStyles.Currency, cultureBR));
//            //            extrato.credito = string.IsNullOrWhiteSpace(tds[4].Text) ? ((decimal?)(nullable = null)) : new decimal?(decimal.Parse(tds[4].Text, NumberStyles.Currency, cultureBR));
//            //            extrato.saldo = string.IsNullOrWhiteSpace(tds[5].Text) ? ((decimal?)(nullable = null)) : new decimal?(decimal.Parse(tds[5].Text, NumberStyles.Currency, cultureBR));
//            //            ExtratoHelper.AddExtrato(ref extratos, extrato);
//            //        }
//            //    }
//            //}

//            // Importa o cartão de crédito Master Fe e Lu
//            List<Extrato> masters = Citibank_PF_Master.Sync(contas, driver);
//            extratos.AddRange(masters);
//        }
//        catch (Exception ex)
//        {
//            UI.SaveHtmlAndPrint(driver, conta, ex);
//            throw ex;
//        }
//        finally
//        {
//            if (driver != null)
//            {
//                driver.Quit();
//                driver.Dispose();
//                driver = null;
//            }
//        }

//        // Retorna os registros
//        return extratos;
//    }
//}