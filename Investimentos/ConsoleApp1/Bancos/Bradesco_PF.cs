using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;

public static class Bradesco_PF
{
    public static List<Extrato> Sync(Dictionary<string, DateTime> contas)
    {
        var extratos = new List<Extrato>();

        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        try
        {
            UI.StartChrome(ref driver, 3);

            Bradesco_PF.LogIn(driver);

            var cc_ultimos = Bradesco_PF.ReadExtrato(driver, contas, cultureBR, TipoConta.Conta_Corrente, TipoLink.Ultimos_Lancamentos);
            var cc_range = Bradesco_PF.ReadExtrato(driver, contas, cultureBR, TipoConta.Conta_Corrente, TipoLink.Por_Periodo);
            var poup_ultimos = Bradesco_PF.ReadExtrato(driver, contas, cultureBR, TipoConta.Poupanca, TipoLink.Ultimos_Lancamentos);
            var poup_range = Bradesco_PF.ReadExtrato(driver, contas, cultureBR, TipoConta.Poupanca, TipoLink.Por_Periodo);

            Bradesco_PF.LogOff(driver);

            if (cc_ultimos.Count > 0 && cc_range.Count > 0) cc_ultimos[cc_ultimos.Count - 1].saldo = cc_range[cc_range.Count - 1].saldo;
            if (poup_ultimos.Count > 0 && poup_range.Count > 0) poup_ultimos[poup_ultimos.Count - 1].saldo = poup_range[poup_range.Count - 1].saldo;

            extratos.AddRange(cc_ultimos);
            extratos.AddRange(cc_range);
            extratos.AddRange(poup_ultimos);
            extratos.AddRange(poup_range);
        }
        catch (Exception ex)
        {
            UI.SaveHtmlAndPrint(driver, "Bradesco", ex);
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


    private static void LogOff(IWebDriver driver)
    {
        driver.SwitchTo().DefaultContent();
        UI.ClickWithPerform(driver, UI.WaitForClickable(driver, By.Id("_id59")));
        UI.WaitPageLoad(driver);
    }

    private static void LogIn(IWebDriver driver)
    {
        driver.Url = "https://banco.bradesco/html/classic/index.shtm";
        UI.WaitPageLoad(driver);

        // ------------------------------------------------------------------------------------------------------------------------------
        // Tela: Home
        // ------------------------------------------------------------------------------------------------------------------------------

        UI.WaitForDisplayed(driver, By.Id("AGN"));
        UI.SetTextBoxValue(driver, By.Id("AGN"), "agencia");
        UI.SetTextBoxValue(driver, By.Id("CTA"), "conta");
        UI.SetTextBoxValue(driver, By.Id("DIGCTA"), "digito");
        UI.Click(driver, By.ClassName("btn-ok"));
        UI.Wait(1);

        // Tela: Aparece às vezes, "Acesso não autorizado. Já existe um acesso em andamento para esta conta."
        if (UI.IsDisplayed(driver, By.Id("modalLogin")))
        {
            UI.Click(driver, By.Id("cancelarAcessoModalForm:_id178"));
            UI.WaitPageLoad(driver);
            Bradesco_PF.LogIn(driver);
        }

        // ------------------------------------------------------------------------------------------------------------------------------
        // Tela: Senha e Token
        // ------------------------------------------------------------------------------------------------------------------------------

        // Neste ponto aguarda digitar o Token
        UI.WaitForClickable(driver, By.Id("ul_teclado_virtual"), 300); // 5 minutos para digitar o token

        var teclado = UI.WaitForDisplayed(driver, By.Id("ul_teclado_virtual"));
        UI.Click(driver, teclado.FindElement(By.LinkText("senha")));
        UI.Click(driver, teclado.FindElement(By.LinkText("senha")));
        UI.Click(driver, teclado.FindElement(By.LinkText("senha")));
        UI.Click(driver, teclado.FindElement(By.LinkText("senha")));
        UI.Wait(2);
        UI.Click(driver, By.XPath("//*[@id=\"loginbotoes:botaoAvancar\"]"));

        // Verifica se abriu a popup de usuário já logado
        //UI.WaitPageLoad(driver);
        //UI.Wait(1);

        //if (UI.IsDisplayed(driver, By.Id("modalLogin")))
        //{
        //    UI.ClickWhenClickable(driver, By.Id("cancelarAcessoModalForm:_id178"));
        //    UI.WaitPageLoad(driver);
        //    LogIn(driver);
        //    return;
        //}

        UI.WaitPageLoad(driver);
        UI.Wait(3);

        // Fechar a popup que aparecer
        //if (UI.IsDisplayed(driver, By.Id("modal_infra_estrutura")))
        //{
        //    UI.WaitIFrameLoad(driver, "modal_infra_estrutura");

        //    if (UI.IsDisplayed(driver, By.XPath("//a[@title = 'Fechar']")))
        //    {
        //        UI.ClickWhenClickable(driver, By.XPath("//a[@title = 'Fechar']"));
        //    }
        //}

        //if (UI.IsDisplayed(driver, By.XPath("//*[@id=\"entretela\"]/div/div[3]/a")))
        //{
        //    UI.Wait(1);
        //    UI.ClickWhenClickable(driver, By.XPath("//*[@id=\"entretela\"]/div/div[3]/a"));
        //}


    }

    public enum TipoLink { Ultimos_Lancamentos, Por_Periodo }
    public enum TipoConta { Conta_Corrente, Poupanca }

    public static List<Extrato> ReadExtrato(IWebDriver driver, Dictionary<string, DateTime> contas, CultureInfo cultureBR, TipoConta tipoConta, TipoLink tipoLink)
    {
        List<Extrato> extratos = new List<Extrato>();

        // Clica no menu "Saldos e Extratos"
        UI.Wait(1);
        driver.SwitchTo().DefaultContent();
        UI.Wait(1);
        UI.ClickWithPerform(driver, UI.WaitForClickable(driver, By.Id("topmenu_S")));
        UI.Wait(1);
        UI.WaitPageLoad(driver);

        // Clica no link da conta corrente ou ponpança, depende do parâmetro linkTitle
        UI.Wait(1);
        UI.WaitIFrameLoad(driver, "paginaCentral");
        UI.Wait(3);

        // Clica no link do extrato
        var lnkText = tipoLink == TipoLink.Ultimos_Lancamentos ? "Extrato (Últimos Lançamentos)" : "Extrato Mensal / Por Período";
        //UI.WaitForClickable(driver, By.LinkText(lnkText));

        var lnkExtratos = driver.FindElements(By.LinkText(lnkText));
        string conta = null;

        if (tipoConta == TipoConta.Conta_Corrente)
        {
            conta = "Bradesco Lu CC";
            lnkExtratos[0].Click();
        }
        else if (tipoConta == TipoConta.Poupanca)
        {
            conta = "Bradesco Lu Poup";
            lnkExtratos[1].Click();
        }

        if (tipoLink == TipoLink.Por_Periodo)
        {
            // Tela do extrato: clica no radio button 'A partir de:'
            UI.ClickWhenDisplayed(driver, By.Id("formFiltroMensal:opcaoData1"));
            UI.Wait(1);

            //Seta as datas iniciais
            UI.ClickWhenDisplayed(driver, By.Id("formFiltroMensal:dataInicialDia"));
            UI.Wait(1);

            // Blur para fechar o calendário de data inicial
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            UI.Blur(driver, executor, driver.FindElement(By.Id("formFiltroMensal:dataInicialDia")));
            UI.Wait(1);

            // Clica na data final - dia
            UI.WaitForClickable(driver, By.Id("formFiltroMensal:dataFinalDia"));
            UI.Wait(1);

            // Seleciona o último dia possível
            var divCalendar = UI.WaitForClickable(driver, By.Id("dp-popup"));
            IList<IWebElement> trsData = divCalendar.FindElement(By.ClassName("dp-calendar")).FindElement(By.ClassName("jCalendar")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
            int diaFim = 0;

            foreach (IWebElement tr in trsData)
            {
                IList<IWebElement> tds = tr.FindElements(By.TagName("td"));
                {
                    foreach (IWebElement td in tds)
                    {
                        if (td.GetAttribute("class").Contains("disabled")) break;
                        diaFim = Convert.ToInt32(td.Text);
                    }
                }
            }

            var dataFim = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1);
            dataFim = dataFim.AddDays(-1);

            //dataFim = new DateTime(2019, 8, 31); // XXXXXXX

            if (diaFim > DateTime.Today.Day) dataFim = dataFim.AddMonths(-1); // caso hoje for 01/12 e o filtro máximo for 30/11. Corrige ficar como 30/12

            // Blur para fechar o calendário de data inicial
            UI.Blur(driver, executor, driver.FindElement(By.Id("formFiltroMensal:dataFinalDia")));
            UI.Wait(1);

            // Seta a data inicial
            var dataInicio = new DateTime(contas[conta].Year, contas[conta].Month, contas[conta].Day);
            if (dataInicio > dataFim) dataInicio = dataFim;

            //UI.ClickWhenDisplayed(driver, By.Id("formFiltroMensal:dataInicialDia"));
            //UI.Wait(1);

            // Seta as datas
            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataInicialDia"), dataInicio.Day.ToString("00"));
            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataInicialMes"), dataInicio.Month.ToString("00"));
            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataInicialAno"), dataInicio.Year.ToString());

            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataFinalDia"), dataFim.Day.ToString("00"));
            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataFinalMes"), dataFim.Month.ToString("00"));
            UI.SetTextBoxValue(driver, By.Id("formFiltroMensal:dataFinalAno"), dataFim.Year.ToString());

            UI.Click(driver, By.Id("formFiltroMensal:dataInicialAno"));
            UI.Wait(1);
            UI.Blur(driver, executor, driver.FindElement(By.Id("formFiltroMensal:dataInicialAno")));
            UI.Wait(1);

            // Clica em buscar
            UI.Click(driver, By.Id("formFiltroMensal:botaoBuscar"));
        }

        // Aguarda carregar
        UI.WaitPageLoad(driver);

        // "Não há lançamentos/operações para o período selecionado."
        if (UI.IsDisplayed(driver, By.XPath("//table[@class='tabSaldosExtratos']")))
        {
            // Lê o extrato
            var table = UI.WaitForDisplayed(driver, By.XPath("//table[@class='tabSaldosExtratos']"));

            IList<IWebElement> trs = table.FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
            string data = null;

            foreach (IWebElement tr in trs)
            {
                IList<IWebElement> tds = tr.FindElements(By.TagName("td"));

                if (!string.IsNullOrWhiteSpace(tds[0].Text)) data = tds[0].Text;

                var extrato = new Extrato();
                extrato.conta = conta;
                extrato.data = DateTime.ParseExact(data, "dd/MM/yy", CultureInfo.InvariantCulture);
                if (extrato.data > DateTime.Now) continue;

                var spans = tds[1].FindElements(By.TagName("span"));
                string descricao = spans[0].Text.Trim();
                if (spans.Count > 1) descricao += " " + spans[1].Text.Trim();
                extrato.descricao = ExtratoHelper.GetDescricao(descricao);

                if (extrato.descricao != null)
                {
                    decimal? nullable;
                    extrato.credito = string.IsNullOrWhiteSpace(tds[3].Text) ? ((decimal?)(nullable = null)) : new decimal?(decimal.Parse(tds[3].Text, NumberStyles.Currency, cultureBR));
                    extrato.debito = string.IsNullOrWhiteSpace(tds[4].Text) ? ((decimal?)(nullable = null)) : new decimal?(-1 * decimal.Parse(tds[4].Text.Replace("-", "").Trim(), NumberStyles.Currency, cultureBR));
                    if (tipoLink == TipoLink.Por_Periodo) extrato.saldo = string.IsNullOrWhiteSpace(tds[5].Text) ? ((decimal?)(nullable = null)) : new decimal?(decimal.Parse(tds[5].Text.Replace("- ", "-"), NumberStyles.Currency, cultureBR));
                    ExtratoHelper.AddExtrato(ref extratos, extrato);
                }
            }
        }
        return extratos;
    }
}
