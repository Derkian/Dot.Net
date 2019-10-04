using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Linq;

public static class Itau_PF_CC
{
    public static List<Extrato> Sync(Dictionary<string, DateTime> contas)
    {
        var conta = "Itau Fe";
        var extratos = new List<Extrato>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        try
        {
            UI.StartChrome(ref driver, 2);

            driver.Url = "https://www.itau.com.br/";
            UI.WaitPageLoad(driver);
            UI.Wait(2);


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"agencia\"]"), "agencia");
            UI.SetTextBoxValue(driver, By.XPath("//*[@id=\"conta\"]"), "conta");
            UI.Wait(2);
            UI.Click(driver, By.XPath("//*[@id=\"btnLoginSubmit\"]"));
            UI.Wait(2);


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Login
            // ------------------------------------------------------------------------------------------------------------------------------

            // Clica no nome FERNANDO
            UI.WaitPageLoad(driver);
            UI.Wait(2);

            // div class teclas clearfix
            var teclado = UI.WaitForDisplayed(driver, By.XPath(".//div[@class='teclas clearfix']"), 20);

            IList<IWebElement> teclas = teclado.FindElements(By.TagName("a"));
            var digitos = new List<KeyValuePair<string, IWebElement>>();
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            var senha = "senha";

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

            UI.ClickWithJavascript(driver, executor, By.Id("acessar"));

            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Home logada
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);
            UI.Wait(5);

            // Popup sobre o tour do site novo
            if (UI.IsDisplayed(driver, By.Id("divAcessTour")))
            {
                UI.ClickWhenClickable(driver, By.LinkText("Não, obrigado"));
            }

            UI.ChangeAttribute(driver, executor, driver.FindElement(By.XPath("//*[@id=\"varejo\"]/header/div/nav/ul/li/a")), "aria-expanded", "true");
            UI.ChangeAttribute(driver, executor, driver.FindElement(By.XPath("//*[@id=\"varejo\"]/header/div/nav/ul/li/div")), "class", "sub-mnu");
            UI.Wait(1);
            UI.ClickWithJavascript(driver, executor, By.XPath("//*[@id=\"varejo\"]/header/div/nav/ul/li/div/div/div[1]/ul[1]/li[3]/a"));
            UI.Wait(1);


            // ------------------------------------------------------------------------------------------------------------------------------
            // Tela: Extrato Conta Corrente
            // ------------------------------------------------------------------------------------------------------------------------------

            UI.WaitPageLoad(driver);


            TimeSpan span = DateTime.Today.Subtract(contas[conta]);
            var dias = span.TotalDays;

            UI.WaitIFrameLoad(driver, "CORPO");
            UI.WaitForDisplayed(driver, By.Name("periodoExtrato"));
            UI.Wait(1);

            var combo = driver.FindElement(By.Name("periodoExtrato"));
            var options = new SelectElement(combo);
            options.SelectByText("Período específico (até 17 anos) ");

            var dataFim = DateTime.Today;


            // Seta a data início e fim
            UI.SetTextBoxValue(driver, By.Id("MesInicial"), contas[conta].Month.ToString());
            UI.Wait(1);
            UI.SetTextBoxValue(driver, By.Id("AnoInicial"), contas[conta].Year.ToString());
            UI.Wait(1);
            UI.SetTextBoxValue(driver, By.Id("MesFinal"), dataFim.Month.ToString());
            UI.Wait(1);
            UI.SetTextBoxValue(driver, By.Id("AnoFinal"), dataFim.Year.ToString());
            UI.Wait(1);
            UI.Click(driver, By.CssSelector("[href*='JavaScript:enviaDados();']")); // clica em OK
            UI.Wait(1);

            bool ProximoMes = false;

            do
            {
                UI.WaitForDisplayed(driver, By.Id("buscaPesquisaOnline"));
                UI.Wait(2);

                try
                {
                    // Tenta por ID
                    IList<IWebElement> tablesID = driver.FindElements(By.Id("TabelaExtrato"));
                    ReadTable(ref driver, tablesID, conta, ref extratos, cultureBR);
                }
                catch
                {
                    // Ignora
                }

                // Tenta por Name
                IList<IWebElement> tablesName = driver.FindElements(By.Name("TabelaExtrato"));
                ReadTable(ref driver, tablesName, conta, ref extratos, cultureBR);

                // Próximo mês
                ProximoMes = UI.ElementExist(driver, By.LinkText("Próximo Mês >"));
                if (ProximoMes) UI.Click(driver, By.LinkText("Próximo Mês >"));

            } while (ProximoMes == true);
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


    private static void ReadTable(ref IWebDriver driver, IList<IWebElement> tables, string conta, ref List<Extrato> extratos, CultureInfo cultureBR)
    {
        foreach (var table in tables)
        {
            IList<IWebElement> trs = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));

            foreach (var tr in trs)
            {
                var tds = tr.FindElements(By.TagName("td"));

                if (tds.Count > 0)
                {
                    var s = tds[0].Text + "/" + DateTime.Today.Year.ToString();

                    if (UI.IsDate(s))
                    {
                        var extrato = new Extrato();
                        extrato.conta = conta;
                        extrato.data = DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        extrato.descricao = ExtratoHelper.GetDescricao(tds[3].Text);
                        if (extrato.descricao == null) continue;

                        s = tds[5].Text;

                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            var d = tds[6].Text;

                            if (d.Trim() == "")
                            {
                                if (tds[5].GetAttribute("class").ToUpper().Contains("NEG") || s.Contains("-"))
                                    extrato.debito = -1 * (decimal?)decimal.Parse(s.Replace("-", ""), NumberStyles.Currency, cultureBR);
                                else
                                    extrato.credito = (decimal?)decimal.Parse(s.Replace("-", ""), NumberStyles.Currency, cultureBR);
                            }
                            else
                            {
                                if (tds[5].GetAttribute("class").ToUpper().Contains("NEG") || d.Contains("-"))
                                    extrato.debito = -1 * (decimal?)decimal.Parse(s.Replace("-", ""), NumberStyles.Currency, cultureBR);
                                else
                                    extrato.credito = (decimal?)decimal.Parse(s.Replace("-", ""), NumberStyles.Currency, cultureBR);
                            }
                        }

                        extrato.saldo = string.IsNullOrWhiteSpace(tds[7].Text) ? (decimal?)null : decimal.Parse(tds[7].Text, NumberStyles.Currency, cultureBR);
                        ExtratoHelper.AddExtrato(ref extratos, extrato);
                    }
                }
            }
        }

    }

}