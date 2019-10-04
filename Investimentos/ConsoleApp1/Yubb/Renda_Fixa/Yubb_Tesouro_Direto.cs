using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Linq;
using ConsoleApp1.Models;

public static class Yubb_Tesouro_Direto
{
    public static List<Yubb> Sync()
    {
        var arquivo_erro = "Yubb_Tesouro_Direto";
        var categoria = "Renda Fixa";
        var grupo = "Tesouro Direto";

        var fundos = new List<Yubb>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        try
        {
            UI.StartChrome(ref driver);

            driver.Url = "https://www.cut.org.br/indicadores/inflacao-mensal-ipca";
            UI.WaitPageLoad(driver);
        }
        catch (Exception ex)
        {
            UI.SaveHtmlAndPrint(driver, arquivo_erro, ex);
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
        return fundos;
    }
}
