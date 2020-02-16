using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Linq;
using ConsoleApp1.Models;

public static class Poupanca
{
    public static List<Investimento> Sync()
    {
        var class_name = "Poupança";
        var investimentos = new List<Investimento>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        var ano_max = 2007;

        try
        {
            UI.StartChrome(ref driver);

            driver.Url = "https://portalbrasil.net/poupanca_mensal.htm";
            UI.WaitPageLoad(driver);

            IList<IWebElement> tables = driver.FindElements(By.XPath("//table"));

            foreach (var table in tables)
            {
                IList<IWebElement> trs = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));

                for (int i = 0; i < trs.Count; i++)
                {
                    int ano;
                    var tds = trs[i].FindElements(By.TagName("td"));
                    bool isNumeric = int.TryParse(tds[0].Text, out ano);

                    if (isNumeric && ano >= ano_max)
                    {
                        for (int j = 1; j <= 12; j++)
                        {
                            if (tds[j].Text != "-")
                            {
                                var investimento = new Investimento();
                                investimento.tipo = "Público";
                                investimento.nome = "Poupança";
                                investimento.mes = new DateTime(ano, j, 1);
                                investimento.valor = Convert.ToDouble(tds[j].Text);
                                investimentos.Add(investimento);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            UI.SaveHtmlAndPrint(driver, class_name, ex);
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
        return investimentos;
    }

}
