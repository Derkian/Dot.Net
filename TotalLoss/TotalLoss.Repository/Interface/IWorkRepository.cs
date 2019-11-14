using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Repository.Interface
{
    public interface IWorkRepository : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IConfigurationRepository ConfigurationRepository { get; }
        IHistoryRepository HistoryRepository { get; }
        IIncidentAssessmentRepository IncidentAssessmentRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        ISalvageCompanyRepository SalvageCompanyRepository { get; }
        ITowingCompanyRepository TowingCompanyRepository { get; }
        ITowTruckDriverRepository TowTruckDriverRepository { get; }
        IUserRepository UserRepository { get; }
        void BeginTransaction();
        void Commit();
        void RollBack();
    }
}
