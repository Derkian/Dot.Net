using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Repository
{
    public class WorkRepository : IWorkRepository
    {
        #region Propriedade Protected
        private IDbConnection _Conexao;
        protected IDbConnection Conexao { get { if (this._Conexao.State == ConnectionState.Closed) this._Conexao.Open(); return this._Conexao; } }
        protected IDbTransaction Transacao { get; set; }
        #endregion

        #region Propriedades Privadas
        private string _connectionString = string.Empty;
        private ICategoryRepository _categoryRepository;
        private IConfigurationRepository _configurationRepository;
        private IHistoryRepository _historyRepository;
        private IIncidentAssessmentRepository _incidentAssessmentRepository;
        private IQuestionRepository _questionRepository;
        private ISalvageCompanyRepository _salvageCompanyRepository;
        private ITowingCompanyRepository _towingCompanyRepository;
        private ITowTruckDriverRepository _towTruckDriverRepository;
        private IUserRepository _userRepository;
        #endregion

        #region Construtor
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="connectionString"></param>
        public WorkRepository(string connectionString)
        {
            this._Conexao = new SqlConnection(connectionString);
            this._Conexao.Open();
        }

        ~WorkRepository()
        {
            this.Close();
        }
        #endregion

        #region Getters
        /// <summary>
        /// Retorna o repositorio Category
        /// </summary>
        public ICategoryRepository CategoryRepository
        {
            get
            {

                _categoryRepository = _categoryRepository ?? (_categoryRepository = new CategoryRepository(this.Conexao));
                _categoryRepository.Transacao = this.Transacao;

                return _categoryRepository;
            }
        }

        /// <summary>
        /// Retorna o repositorio Configuration
        /// </summary>
        public IConfigurationRepository ConfigurationRepository
        {
            get
            {
                _configurationRepository = _configurationRepository ?? (_configurationRepository = new ConfigurationRepository(this.Conexao));
                _configurationRepository.Transacao = this.Transacao;

                return _configurationRepository;
            }
        }

        /// <summary>
        /// Retorna o repositorio History
        /// </summary>
        public IHistoryRepository HistoryRepository
        {
            get
            {
                _historyRepository = _historyRepository ?? (_historyRepository = new HistoryRepository(this.Conexao));
                _historyRepository.Transacao = this.Transacao;

                return _historyRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio Incident
        /// </summary>
        public IIncidentAssessmentRepository IncidentAssessmentRepository
        {
            get
            {
                _incidentAssessmentRepository = _incidentAssessmentRepository ?? (_incidentAssessmentRepository = new IncidentAssessmentRepository(this.Conexao));

                _incidentAssessmentRepository.Transacao = this.Transacao;

                return _incidentAssessmentRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio Question
        /// </summary>
        public IQuestionRepository QuestionRepository
        {
            get
            {
                _questionRepository = _questionRepository ?? (_questionRepository = new QuestionRepository(this.Conexao));
                _questionRepository.Transacao = this.Transacao;

                return _questionRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio Salvage
        /// </summary>
        public ISalvageCompanyRepository SalvageCompanyRepository
        {
            get
            {
                _salvageCompanyRepository = _salvageCompanyRepository ?? (_salvageCompanyRepository = new SalvageCompanyRepository(this.Conexao));
                _salvageCompanyRepository.Transacao = this.Transacao;

                return _salvageCompanyRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio Towing
        /// </summary>
        public ITowingCompanyRepository TowingCompanyRepository
        {
            get
            {
                _towingCompanyRepository = _towingCompanyRepository ?? (_towingCompanyRepository = new TowingCompanyRepository(this.Conexao));
                _towingCompanyRepository.Transacao = this.Transacao;

                return _towingCompanyRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio TowTruck
        /// </summary>
        public ITowTruckDriverRepository TowTruckDriverRepository
        {
            get
            {
                _towTruckDriverRepository = _towTruckDriverRepository ?? (_towTruckDriverRepository = new TowTruckDriverRepository(this.Conexao));
                _towTruckDriverRepository.Transacao = this.Transacao;

                return _towTruckDriverRepository;
            }
        }

        /// <summary>
        /// Retorna repositorio User
        /// </summary>
        public IUserRepository UserRepository
        {
            get
            {
                _userRepository = _userRepository ?? (_userRepository = new UserRepository(this.Conexao));
                _userRepository.Transacao = this.Transacao;

                return _userRepository;
            }
        }
        #endregion

        #region Publico
        /// <summary>
        /// Cria uma transação
        /// </summary>
        public void BeginTransaction()
        {
            if (this.Conexao.State == ConnectionState.Closed)
                this.Conexao.Open();

            this.Transacao = this.Conexao.BeginTransaction();
        }

        /// <summary>
        /// Commit na transação
        /// </summary>
        public void Commit()
        {
            if (this.Transacao != null)
            {
                this.Transacao.Commit();
                this.Transacao.Dispose();
                this.Transacao = null;
            }
        }

        /// <summary>
        /// Rollback na transacao
        /// </summary>
        public void RollBack()
        {
            if (this.Transacao != null)
            {
                this.Transacao.Rollback();
                this.Transacao.Dispose();
                this.Transacao = null;
            }
        }

        /// <summary>
        /// Fecha a conexão
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
        #endregion

        #region Private
        /// <summary>
        /// Fecha a conexão
        /// </summary>
        private void Close()
        {
            if (this.Transacao != null)
            {
                this.Transacao.Dispose();
                this.Transacao = null;
            }
            if (this._Conexao != null)
            {
                this._Conexao.Dispose();
                this._Conexao = null;
            }
        }
        #endregion
    }
}
