using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Linq;
using ConsoleApp1.Models;

public static class Inflacao
{
    public static List<Investimento> Sync()
    {
        var class_name = "Inflação";
        var investimentos = new List<Investimento>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        var ano_max = 2007;

        try
        {
            UI.StartChrome(ref driver);

            driver.Url = "https://www.cut.org.br/indicadores/inflacao-mensal-ipca";
            UI.WaitPageLoad(driver);

            UI.Wait(3);
            UI.ClickWhenDisplayed(driver, By.XPath("//div[@data-filter='max']"));
            UI.Wait(3);

            IList<IWebElement> lines = driver.FindElements(By.XPath("//div[@class='line']"));

            foreach (var line in lines)
            {
                var arr = line.Text.Replace("\r\n", "|").Split(Convert.ToChar("|"));

                if (arr.Length == 3)
                {
                    var mes_ano = arr[0].Split(Convert.ToChar(" "));

                    if (mes_ano.Length == 2)
                    {
                        int ano;
                        bool isNumeric = int.TryParse(mes_ano[1], out ano);

                        if (isNumeric && ano >= ano_max)
                        {
                            var mes = DateTime.ParseExact(mes_ano[0], "MMMM", new CultureInfo("pt-BR")).Month;
                            var investimento = new Investimento();
                            investimento.tipo = "Público";
                            investimento.nome = "Inflação";
                            investimento.mes = new DateTime(ano, mes, 1);
                            investimento.valor = Convert.ToDouble(arr[1]);
                            investimentos.Add(investimento);
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
