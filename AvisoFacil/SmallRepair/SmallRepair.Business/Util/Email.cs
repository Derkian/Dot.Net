using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace SmallRepair.Business.Util
{
    public class Attachament
    {
        public string FileName { get; set; }

        public string MediaTypeNames { get; set; }

        public Stream File { get; set; }
    }

    public class SmtpConfig
    {
        public string From { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }
    }

    public class Email
    {
        private SmtpClient _SMTP = null;
        private string _From;

        public Email(SmtpConfig config)
        {
            this._SMTP = new SmtpClient(config.Address, config.Port);
            this._From = config.From;
        }

        ~Email()
        {
            if (this._SMTP != null)
            {
                this._SMTP.Dispose();
                this._SMTP = null;
            }
        }

        public bool Send(List<string> To,
                         List<string> Cc,
                         string Subject,
                         string Body,
                         IList<Attachament> Attachaments,
                         byte[] CompanyLogo = null,
                         byte[] SoleraLogo = null)
        {

            MemoryStream streamCompanyLogo = new MemoryStream();
            MemoryStream streamSoleraLogo = new MemoryStream();
            LinkedResource lkcompanyLogo = null;
            LinkedResource lkSoleraLogo = null;

            try
            {
                #region Mensagem

                using (MailMessage message = new MailMessage())
                {
                    //ADICIONA E-MAIL QUE ENVIARÁ
                    message.From = new MailAddress(this._From);

                    //TÍTULO DO E-MAIL
                    message.Subject = Subject;

                    //CONTEUDO EM HTML
                    message.IsBodyHtml = true;

                    //ADICIONAR DESTINATÁRIOS
                    #region DESTINATÁRIO
                    if (To == null)
                        throw new Exception("Informar o destinatário");

                    foreach (var _to in To)
                    {
                        message.To.Add(new MailAddress(_to));
                    }
                    #endregion

                    //ADICIONAR CÓPIAS
                    #region CÓPIAS
                    if (Cc != null)
                    {
                        foreach (var _cc in Cc)
                        {
                            message.CC.Add(new MailAddress(_cc));
                        }
                    }
                    #endregion

                    //LOGOTIPOS
                    #region LOGOS E CORPO

                    //CRIA VISÃO ALTERNATIVA
                    using (AlternateView view = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html))
                    {
                        //LOGO EMPRESA DE SEGUROS
                        #region LOGOTIPOS
                        if (SoleraLogo != null)
                        {
                            streamSoleraLogo = new MemoryStream(SoleraLogo);

                            lkSoleraLogo = new LinkedResource(streamSoleraLogo);
                            lkSoleraLogo.ContentType.MediaType = MediaTypeNames.Image.Jpeg;
                            lkSoleraLogo.TransferEncoding = TransferEncoding.Base64;
                            lkSoleraLogo.ContentId = "soleraLogo";

                            view.LinkedResources.Add(lkSoleraLogo);
                        }

                        if (CompanyLogo != null)
                        {
                            streamCompanyLogo = new MemoryStream(CompanyLogo);

                            lkcompanyLogo = new LinkedResource(streamCompanyLogo);
                            lkcompanyLogo.ContentType.MediaType = MediaTypeNames.Image.Jpeg;
                            lkcompanyLogo.TransferEncoding = TransferEncoding.Base64;
                            lkcompanyLogo.ContentId = "companyLogo";

                            view.LinkedResources.Add(lkcompanyLogo);
                        }

                        #endregion

                        //ADICONA AS IMAGENS E CORPO DO E-MAIL
                        message.AlternateViews.Add(view);

                        #region Attachament

                        foreach (Attachament attachFile in Attachaments)
                        {
                            //MediaTypeNames.Text.Plain
                            ContentType ct = new ContentType(attachFile.MediaTypeNames);
                            Attachment attach = new Attachment(attachFile.File, ct);
                            attach.ContentDisposition.FileName = attachFile.FileName;
                            message.Attachments.Add(attach);
                        }

                        #endregion

                        //ENVIA E-MAIL
                        this._SMTP.Send(message);
                    }

                    #endregion                    
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                streamSoleraLogo.Dispose();
                streamSoleraLogo.Flush();
                streamSoleraLogo = null;

                streamCompanyLogo.Dispose();
                streamCompanyLogo.Flush();
                streamCompanyLogo = null;

                if (lkcompanyLogo != null) lkcompanyLogo.Dispose();
                lkcompanyLogo = null;

                if (lkSoleraLogo != null) lkSoleraLogo.Dispose();
                lkSoleraLogo = null;
            }
        }
    }
}
