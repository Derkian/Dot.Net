using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Linq;
using ConsoleApp1.Models;

public static class TesouroDireto
{
    public static List<Investimento> Sync()
    {
        var class_name = "Tesouro Direto";
        var investimentos = new List<Investimento>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        var min_ano = 2007;

        try
        {
            UI.StartChrome(ref driver);

            driver.Url = "http://www.tesouro.gov.br/-/balanco-e-estatisticas";
            UI.WaitPageLoad(driver);

            // TODO Derkian
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
