using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConsoleApp1.Models;
using OpenQA.Selenium;

public static class Bradesco
{
    public static List<Extrato> Sync(List<Conta> contas)
    {
        var extratos = new List<Extrato>();
        IWebDriver driver = null;
        var cultureBR = new CultureInfo("pt-BR");

        return extratos;

        try
        {
            contas = (from a in contas where a.classe.ToUpper() == "BRADESCO" select a).ToList();
            if (contas.Count == 0) return extratos; // lista vazia

            UI.StartChrome(ref driver, 3);

            Bradesco.LogIn(driver, contas);

            // Conta corrente
            var cc_ultimos = Bradesco.ReadExtrato(driver, contas, cultureBR, TipoConta.Conta_Corrente, TipoLink.Ultimos_Lancamentos);
            var cc_range = Bradesco.ReadExtrato(driver, contas, cultureBR, TipoConta.Conta_Corrente, TipoLink.Por_Periodo);
            
            // Poupança
            var poup_ultimos = Bradesco.ReadExtrato(driver, contas, cultureBR, TipoConta.Poupanca, TipoLink.Ultimos_Lancamentos);
            var poup_range = Bradesco.ReadExtrato(driver, contas, cultureBR, TipoConta.Poupanca, TipoLink.Por_Periodo);

            Bradesco.LogOff(driver);

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

    private static void LogIn(IWebDriver driver, List<Conta> contas)
    {
        driver.Url = "https://banco.bradesco/html/classic/index.shtm";
        UI.WaitPageLoad(driver);

        // ------------------------------------------------------------------------------------------------------------------------------
        // Tela: Home
        // ------------------------------------------------------------------------------------------------------------------------------

        var conta = contas[0]; // Utiliza a primeira, pois trata-se da mesma conta, com poupança e corrente na lista
        var s_agencia = conta.agencia.Replace(".", "").Split(Convert.ToChar("-"))[0];
        var arr_conta = conta.conta.Replace(".", "").Split(Convert.ToChar("-"));
        var s_conta = arr_conta[0];
        var s_digito = arr_conta[1];

        UI.WaitForDisplayed(driver, By.Id("AGN"));
        UI.SetTextBoxValue(driver, By.Id("AGN"), s_agencia);
        UI.SetTextBoxValue(driver, By.Id("CTA"), s_conta);
        UI.SetTextBoxValue(driver, By.Id("DIGCTA"), s_digito);
        UI.Click(driver, By.ClassName("btn-ok"));
        UI.Wait(1);

        // Tela: Aparece às vezes, "Acesso não autorizado. Já existe um acesso em andamento para esta conta."
        if (UI.IsDisplayed(driver, By.Id("modalLogin")))
        {
            UI.Click(driver, By.Id("cancelarAcessoModalForm:_id178"));
            UI.WaitPageLoad(driver);
            Bradesco.LogIn(driver, contas);
        }

        // ------------------------------------------------------------------------------------------------------------------------------
        // Tela: Senha e Token
        // ------------------------------------------------------------------------------------------------------------------------------

        // Neste ponto aguarda digitar o Token
        UI.WaitForClickable(driver, By.Id("ul_teclado_virtual"), 300); // 5 minutos para digitar o token

        var teclado = UI.WaitForDisplayed(driver, By.Id("ul_teclado_virtual"));

        foreach (char c in conta.senha)
        {
            UI.Click(driver, teclado.FindElement(By.LinkText(c.ToString())));
        }

        UI.Wait(2);
        UI.Click(driver, By.XPath("//*[@id=\"loginbotoes:botaoAvancar\"]"));
        UI.WaitPageLoad(driver);
        UI.Wait(3);
    }

    public enum TipoLink { Ultimos_Lancamentos, Por_Periodo }
    public enum TipoConta { Conta_Corrente, Poupanca }

    public static List<Extrato> ReadExtrato(IWebDriver driver, List<Conta> contas, CultureInfo cultureBR, TipoConta tipoConta, TipoLink tipoLink)
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
            conta = (from a in contas where a.nome.ToUpper().EndsWith("CC") select a.nome).First(); // "Bradesco Lu CC";
            lnkExtratos[0].Click();
        }
        else if (tipoConta == TipoConta.Poupanca)
        {
            conta = (from a in contas where a.nome.ToUpper().EndsWith("POUP") select a.nome).First();
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

            var dataFim = DateTime.Today.AddDays(-1); // new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day - 1);
            //dataFim = dataFim.AddDays(-1);
            //dataFim = new DateTime(2019, 11, 10); // XXXXXXX

            if (diaFim > DateTime.Today.Day) dataFim = dataFim.AddMonths(-1); // caso hoje for 01/12 e o filtro máximo for 30/11. Corrige ficar como 30/12

            // Blur para fechar o calendário de data inicial
            UI.Blur(driver, executor, driver.FindElement(By.Id("formFiltroMensal:dataFinalDia")));
            UI.Wait(1);

            // Seta a data inicial
            var ultima_atulizacao = contas[0].ultima_atulizacao;
            var dataInicio = new DateTime(ultima_atulizacao.Year, ultima_atulizacao.Month, ultima_atulizacao.Day);
            if (dataInicio > dataFim) dataInicio = dataFim;

            //dataInicio = new DateTime(2019, 7, 1);
            //dataFim = new DateTime(2019, 7, 31);

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
