using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;
using TotalLoss.Repository;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class TowingCompanyService
    {
        #region | Objects

        private IWorkRepository _workRepository;        
        private EmailService _emailService;        

        #endregion

        #region | Constructor 

        public TowingCompanyService(IWorkRepository workRepository,
                                    EmailService emailService)
        {
            this._workRepository = workRepository;            
            this._emailService = emailService;            
        }

        #endregion

        #region | Public 

        /// <summary>
        /// Cria uma Empresa de Guincho por dados informados
        /// </summary>
        /// <param name="towingCompany"></param>
        /// <returns></returns>
        public TowingCompany Create(Company insuranceCompany, TowingCompany towingCompany)
        {
            try
            {
                this._workRepository.BeginTransaction();

                //Adiciona o Id da company de seguros
                towingCompany.IdInsuranceCompany = insuranceCompany.Id;

                // Cria a Empresa de Guincho na base de dados 
                this._workRepository.TowingCompanyRepository.Create(towingCompany);

                // Configura Usuário a ser criado
                var user = new User()
                {
                    Company = new Company() { Id = towingCompany.Id },
                    Name = towingCompany.Name,
                    Login = towingCompany.Email,
                    Password = System.Web.Security.Membership.GeneratePassword(8, 1),
                    Status = true
                };

                // Cria Usuário para realizar login na aplicação
                this._workRepository.UserRepository.Create(user);

                // Busca configuração da Companhia Seguradora 
                InsuranceCompany configurationInsuranceCompany = this._workRepository.ConfigurationRepository.Find(insuranceCompany);

                // Envia E-mail à Empresa Guincho com Usuário/Senha para seu acesso
                this.SendMail(configurationInsuranceCompany, user);

                // Finaliza Escopo principal
                this._workRepository.Commit();

                return towingCompany;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Busca todos campos da Empresa de Guincho informada
        /// </summary>
        /// <param name="towingCompany"></param>
        /// <returns></returns>
        public TowingCompany GetTowingCompany(TowingCompany towingCompany)
        {
            TowingCompany _towingCompany = null;

            try
            {
                _towingCompany = _workRepository.TowingCompanyRepository.Find(towingCompany);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _towingCompany;
        }

        /// <summary>
        /// Retorna todas Empresas de Guinchos relaciondas com a Seguradora informada
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public IList<TowingCompany> ListByInsuranceCompany(Company insuranceCompany)
        {
            IList<TowingCompany> listTowingCompany = null;
            try
            {
                listTowingCompany = _workRepository.TowingCompanyRepository.ListByInsuranceCompany(insuranceCompany);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listTowingCompany;
        }

        /// <summary>
        /// Envia e-mail à Empresa de Guincho com Usuário/Senha para acesso ao painel de Incidente
        /// </summary>
        public void SendMail(InsuranceCompany insuranceCompany, User user)
        {
            try
            {
                // Carrega Corpo do E-mail que será enviado para a Empresa de Guincho
                string pathFileBodyMailTowing = string.Concat(System.Web.HttpRuntime.AppDomainAppPath, "Html\\BodyMailTowingCompany.html");
                string bodyMail = System.IO.File.ReadAllText(pathFileBodyMailTowing).Replace("[Login]", user.Login)
                                                                                    .Replace("[Password]", user.Password);
                
                // Envia e-mail à Empresa de Guincho com usuário para acessar seu painel                 
                _emailService.Send(new List<string>() { user.Login },
                                   null,
                                   "Pontuação Guincho",
                                   bodyMail,
                                   insuranceCompany.Image,
                                   null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Atualiza uma Empresa de Guincho por dados informados
        /// </summary>
        /// <param name="towingCompany"></param>
        /// <returns></returns>
        public TowingCompany Update(TowingCompany towingCompany)
        {
            try
            {
                _workRepository.BeginTransaction();

                // Atualiza a Empresa de Guincho na base de dados 
                _workRepository.TowingCompanyRepository.Update(towingCompany);

                // Comita a transação
                _workRepository.Commit();

                //retorna a Empresa de Guincho
                return towingCompany;
            }
            catch (Exception ex)
            {
                _workRepository.RollBack();

                throw ex;
            }            
        }

        #endregion
    }
}
