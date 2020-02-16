using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConsoleApp1.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

public static class Master_Credicard_Itau
{

    public static List<Extrato> Sync(List<Conta> contas)
    {
        var extratos = new List<Extrato>();
        IWebDriver driver = null;

        //return extratos;

        try
        {
            var conta = (from a in contas where a.classe.ToUpper() == "MASTER_CREDICARD_ITAU" select a).First();

            UI.StartChrome(ref driver, 1);

            driver.Url = "https://www.credicard.com.br/";
            UI.WaitPageLoad(driver);


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home e login
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='header']/div/div/div[2]/div/div/a"));
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='loginModal']/div/div/div[2]/div[1]/div/div[1]/button"));
            UI.Wait(1);
            UI.SetTextBoxValue(driver, By.XPath("//*[@id='input']"), conta.agencia);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='loginModal']/div/div/div[3]/button"));


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Senha
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(1);
            //UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='MSGBordaEsq']/table/tbody/tr/td/table/tbody/tr[2]/td[2]/a"));

            var teclado = UI.WaitForDisplayed(driver, By.XPath(".//div[@class='teclas clearfix']"), 20);

            IList<IWebElement> teclas = teclado.FindElements(By.TagName("a"));
            var digitos = new List<KeyValuePair<string, IWebElement>>();
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            var senha = conta.senha;

            foreach (char c in senha)
            {
                foreach (var tecla in teclas)
                {
                    if (tecla.Text.Contains(" ou ") && tecla.Text.Contains(c.ToString()))
                    {
                        UI.ClickWithJavascript(driver, executor, tecla);
                    }
                }
            }

            UI.ClickWithJavascript(driver, executor, By.XPath("//*[@id=\"a-acessar\"]"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id=\"boxCartoes\"]/div/div[2]/div[3]/a")); // Ver fatura

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Fatura atual
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);

            var combo = driver.FindElement(By.Name("dataFatura"));
            var combo_options = new SelectElement(combo);
            var options = new List<string>();

            // Lê os combos
            foreach (var option in combo_options.Options)
            {
                options.Add(option.Text);
            }

            options.Reverse();

            // Seleciona cada um deles
            //foreach (var option in options)
            //{
            //    if (combo_options.SelectedOption.Text != option)
            //    {
            //        combo_options.SelectByText(option);
            //        UI.WaitPageLoad(driver);
            //        UI.Wait(2);
            //    }

            //    Master_Credicard_Itau.ReadTable(driver, conta, ref extratos);
            //}


            // Sair
            UI.Click(driver, By.XPath("//*[@id=\"botao-sair-n\"]")); // Sair
            UI.WaitPageLoad(driver);
            UI.Wait(2);

            UI.Click(driver, By.XPath("//*[@id=\"get-out-confirmation\"]/section/a[2]")); // Confirmar
            UI.WaitPageLoad(driver);
        }

        catch (Exception ex)
        {
            UI.SaveHtmlAndPrint(driver, "Master Credicard Itau", ex);
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

        // Retorna os registros
        return extratos;
    }


    private static void ReadTable(IWebDriver driver, string conta, ref List<Extrato> extratos)
    {
        try
        {
            var cultureBR = new CultureInfo("pt-BR");
            IList<IWebElement> trs = driver.FindElements(By.XPath("//*[@id=\"idImpressaoOuPDF\"]/table/tbody/tr"));

            foreach (var tr in trs)
            {
                IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                if (tds.Count > 1 && tds[1].Text.Contains("/"))
                {
                    var extrato = new Extrato();
                    extrato.conta = conta;

                    // Bug virada do ano
                    var ano = DateTime.Now.Year;

                    decimal n;
                    bool isNumeric = decimal.TryParse(tds[3].Text, out n);

                    if (isNumeric)
                    {
                        var mes = Convert.ToInt32(tds[1].Text.Substring(3));

                        if (mes > DateTime.Now.Month) // virou o ano
                        {
                            ano -= 1;
                        }

                        extrato.data = DateTime.ParseExact(tds[1].Text + "/" + ano.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        extrato.descricao = ExtratoHelper.GetDescricao(tds[2].Text);

                        if (extrato.descricao != null)
                        {
                            extrato.debito = -1 * (decimal?)decimal.Parse(tds[3].Text.Replace("-", ""), NumberStyles.Currency, cultureBR);
                            ExtratoHelper.AddExtrato(ref extratos, extrato);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

