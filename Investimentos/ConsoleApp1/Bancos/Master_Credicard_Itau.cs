using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

public static class Master_Credicard_Itau
{

    public static List<Extrato> Sync(Dictionary<string, DateTime> contas)
    {
        var conta = "Citibank Master";
        var extratos = new List<Extrato>();
        IWebDriver driver = null;

        try
        {
            UI.StartChrome(ref driver, 1);

            driver.Url = "https://www.credicard.com.br/";
            UI.WaitPageLoad(driver);


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home e login
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='header']/div/div/div[2]/div/div/a"));
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='loginModal']/div/div/div[2]/div[1]/div/div[1]/button"));
            UI.Wait(1);
            UI.SetTextBoxValue(driver, By.XPath("//*[@id='input']"), "cartao");
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='loginModal']/div/div/div[3]/button"));


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Senha
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(1);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='MSGBordaEsq']/table/tbody/tr/td/table/tbody/tr[2]/td[2]/a"));

            var teclado = UI.WaitForDisplayed(driver, By.XPath("//*[@id='TecladoFlutuanteBKL']/form/table/tbody/tr[2]/td[2]/span[2]/table[1]"), 20);
            IList<IWebElement> teclas = teclado.FindElements(By.TagName("td"));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            var senha = "senha";

            foreach (char c in senha)
            {
                foreach (var tecla in teclas)
                {
                    if (tecla.Text.Contains(" ou ") && tecla.Text.Contains(c.ToString()))
                    {
                        UI.ClickWithJavascript(driver, executor, tecla.FindElement(By.TagName("a")));
                    }
                }
            }

            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='TecladoFlutuanteBKL']/form/table/tbody/tr[2]/td[2]/span[2]/table[2]/tbody/tr/td[3]/a"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='menulinha1']/a[2]"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Cartões
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='menuarea']/div[2]/div[2]/p[1]/a"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Fatura atual
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            Master_Credicard_Itau.ReadTable(driver, conta, ref extratos);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='TRNcontainer01']/table[1]/tbody/tr[1]/td/table/tbody/tr/td[1]/a"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Fatura anterior
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            Master_Credicard_Itau.ReadTable(driver, conta, ref extratos);
            UI.ClickWhenDisplayed(driver, By.XPath("//*[@id='TRNcontainer01']/table[1]/tbody/tr[1]/td/table/tbody/tr/td[3]/a"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Próxima fatura
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(2);
            Master_Credicard_Itau.ReadTable(driver, conta, ref extratos);
        }
        catch (Exception ex)
        {
            UI.SaveHtmlAndPrint(driver, conta, ex);
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
            IList<IWebElement> tabelas = driver.FindElements(By.XPath("//*[@id='TRNcontainer01']/table"));

            foreach (var tabela in tabelas)
            {
                foreach (var tr in tabela.FindElements(By.TagName("tr")))
                {
                    IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                    if (tds.Count > 0 && tr.FindElement(By.TagName("td")).Text.Contains("/"))
                    {
                        var extrato = new Extrato();
                        extrato.conta = conta;

                        // Bug virada do ano
                        var ano = DateTime.Now.Year;

                        int n;
                        bool isNumeric = int.TryParse(tds[0].Text.Substring(3), out n);

                        if (isNumeric)
                        {
                            var mes = Convert.ToInt32(tds[0].Text.Substring(3));

                            if (mes > DateTime.Now.Month) // virou o ano
                            {
                                ano -= 1;
                            }

                            extrato.data = DateTime.ParseExact(tds[0].Text + "/" + ano.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            extrato.descricao = ExtratoHelper.GetDescricao(tds[1].Text);

                            if (extrato.descricao != null)
                            {
                                if (tds[2].Text.Contains("-"))
                                    extrato.credito = (decimal?)decimal.Parse(tds[2].Text.Replace("-", ""), NumberStyles.Currency, cultureBR);
                                else
                                    extrato.debito = -1 * (decimal?)decimal.Parse(tds[2].Text.Replace("-", ""), NumberStyles.Currency, cultureBR);

                                ExtratoHelper.AddExtrato(ref extratos, extrato);
                            }
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

