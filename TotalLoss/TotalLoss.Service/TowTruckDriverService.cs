using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class TowTruckDriverService
    {
        #region | Objects

        private IWorkRepository _workRepository;

        #endregion

        #region | Constructor 

        public TowTruckDriverService(IWorkRepository workRepository)
        {
            this._workRepository = workRepository;
        }

        #endregion

        #region | Public 

        /// <summary>
        /// Cadastra um Motorista de Guincho relacionada com a Empresa de Guincho 
        /// </summary>
        /// <param name="towTruckDriver"></param>
        /// <returns></returns>
        public TowTruckDriver Create(TowTruckDriver towTruckDriver)
        {
            try
            {
                _workRepository.BeginTransaction();

                // Cria um Motorista de Guincho na base de dados 
                _workRepository.TowTruckDriverRepository.Create(towTruckDriver);

                // Comita a transação
                _workRepository.Commit();

                //retorna o Motorista de Guincho criado
                return towTruckDriver;
            }
            catch (Exception ex)
            {
                _workRepository.RollBack();

                throw ex;
            }
        }

        /// <summary>
        /// Busca Motorista de Guincho por ID informado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TowTruckDriver Find(int id)
        {
            TowTruckDriver _towTruckDriver = null;

            try
            {
                _towTruckDriver = _workRepository.TowTruckDriverRepository.Find(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }            

            return _towTruckDriver;
        }

        /// <summary>
        /// Retorna todos Motoristas de Guincho relacionados com a Empresa de Guincho informada
        /// </summary>
        /// <param name="towingCompany"></param>
        /// <returns></returns>
        public IList<TowTruckDriver> ListByTowingCompany(Company towingCompany)
        {
            IList<TowTruckDriver> listTowTruckDriver = null;
            try
            {
                listTowTruckDriver = _workRepository.TowTruckDriverRepository.ListByTowingCompany(towingCompany);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return listTowTruckDriver;
        }

        /// <summary>
        /// Atualiza um Motorista de Guincho informado
        /// </summary>
        /// <param name="towingCompany"></param>
        /// <returns></returns>
        public TowTruckDriver Update(TowTruckDriver towTruckDriver)
        {
            try
            {
                _workRepository.BeginTransaction();

                // Atualiza Motorista de Guincho na base de dados 
                _workRepository.TowTruckDriverRepository.Update(towTruckDriver);

                // Comita a transação
                _workRepository.Commit();

                //retorna o Motorista de Guincho atualizado
                return towTruckDriver;
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
