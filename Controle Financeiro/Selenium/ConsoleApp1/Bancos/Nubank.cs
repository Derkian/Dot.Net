using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using System.Linq;
using System.Net.Http;
using ConsoleApp1.Models;

public static class Nubank
{
    public static List<Extrato> Sync(List<Conta> contas)
    {
        var extratos = new List<Extrato>();
        var cultureBR = new CultureInfo("pt-BR");

        foreach (var conta in (from a in contas where a.classe.ToUpper(CultureInfo.InvariantCulture) == "NUBANK" select a).ToList())
        {
            IWebDriver driver = null;

            try
            {
                UI.StartChrome(ref driver, 4);

                driver.Url = "https://app.nubank.com.br/#/login";
                UI.WaitPageLoad(driver);


                // ------------------------------------------------------------------------------------------------------------------------------
                // Tela: Home
                // ------------------------------------------------------------------------------------------------------------------------------
                UI.SetTextBoxValue(driver, By.Id("username"), conta.conta);
                UI.SetTextBoxValue(driver, By.Id("input_001"), conta.senha);
                UI.Wait(1.5);
                UI.Click(driver, By.XPath("/html/body/navigation-base/div[1]/div/main/div[1]/div/div[1]/form/button"));
                //UI.WaitPageLoad(driver);
                UI.Wait(10);

                bool ToBreak = false;

                UI.Click(driver, By.ClassName("bills"));

                UI.Wait(10);
                
                IList<IWebElement> chargesList = new List<IWebElement>();

                chargesList = driver.FindElements(By.ClassName("charges-list"));                                    

                foreach (var charge in chargesList)
                {
                    if (ToBreak) break;
                    var tbodys = charge.FindElements(By.TagName("div"));

                    foreach (var tbody in tbodys)
                    {
                        try
                        {
                            #region DATA
                            IWebElement elementTime = tbody.FindElement(By.ClassName("time"));
                            IWebElement elementCell = elementTime.FindElement(By.ClassName("cell"));
                            IWebElement elementSpan = elementCell.FindElement(By.TagName("span"));

                            DateTime data = DateTime.Now;

                            var textoData = elementSpan.Text.Split(' ');
                            switch (textoData[1].ToUpper())
                            {
                                case "JAN":
                                    data = new DateTime(DateTime.Now.Year, 1, Convert.ToInt32(textoData[0]));
                                    break;
                                case "FEV":
                                    data = new DateTime(DateTime.Now.Year, 2, Convert.ToInt32(textoData[0]));
                                    break;
                                case "MAR":
                                    data = new DateTime(DateTime.Now.Year, 3, Convert.ToInt32(textoData[0]));
                                    break;
                                case "ABR":
                                    data = new DateTime(DateTime.Now.Year, 4, Convert.ToInt32(textoData[0]));
                                    break;
                                case "MAI":
                                    data = new DateTime(DateTime.Now.Year, 5, Convert.ToInt32(textoData[0]));
                                    break;
                                case "JUN":
                                    data = new DateTime(DateTime.Now.Year, 6, Convert.ToInt32(textoData[0]));
                                    break;
                                case "JUL":
                                    data = new DateTime(DateTime.Now.Year, 7, Convert.ToInt32(textoData[0]));
                                    break;
                                case "AGO":
                                    data = new DateTime(DateTime.Now.Year, 8, Convert.ToInt32(textoData[0]));
                                    break;
                                case "SET":
                                    data = new DateTime(DateTime.Now.Year, 9, Convert.ToInt32(textoData[0]));
                                    break;
                                case "OUT":
                                    data = new DateTime(DateTime.Now.Year, 10, Convert.ToInt32(textoData[0]));
                                    break;
                                case "NOV":
                                    data = new DateTime(DateTime.Now.Year, 11, Convert.ToInt32(textoData[0]));
                                    break;
                                case "DEZ":
                                    data = new DateTime(DateTime.Now.Year, 12, Convert.ToInt32(textoData[0]));
                                    break;
                            }
                            #endregion

                            IWebElement elementChargeData = tbody.FindElement(By.ClassName("charge-data"));
                            IWebElement elementdescription = elementChargeData.FindElement(By.ClassName("description"));

                            IWebElement elementamount = tbody.FindElement(By.ClassName("amount"));

                            var extrato = new Extrato();
                            extrato.conta = conta.nome;
                            extrato.data = data;
                            extrato.descricao = ExtratoHelper.GetDescricao(elementdescription.Text);

                            if (!string.IsNullOrEmpty(extrato.descricao))
                            {
                                var valor = decimal.Parse(elementamount.Text.Replace("R$", ""), NumberStyles.Currency, cultureBR);

                                if (valor > 0)
                                {
                                    extrato.debito = Math.Abs(valor) * -1;
                                    ExtratoHelper.AddExtrato(ref extratos, extrato);
                                }
                            }
                        }
                        catch { }
                    }
                }

                UI.Click(driver, By.XPath("/html/body/navigation-base/navigation-menu/div/nav/section/ul[1]/li[3]/a")); // Sair
                UI.WaitPageLoad(driver);
                UI.Wait(2);

                UI.Click(driver, By.ClassName("logout")); // Confirmar
                UI.WaitPageLoad(driver);
            }
            catch (Exception ex)
            {
                UI.SaveHtmlAndPrint(driver, "Nubank", ex);
                throw ex;
            }
            finally
            {
                if (driver != null)
                {
                    try { driver.Close(); } catch { }
                    driver.Quit();
                    driver.Dispose();
                    driver = null;
                }
            }
        }

        // Retorna os registros
        return extratos;
    }
}