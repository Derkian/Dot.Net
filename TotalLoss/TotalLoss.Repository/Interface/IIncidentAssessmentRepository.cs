using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface IIncidentAssessmentRepository : IBaseRepository
    {
        bool AddAnswers(IncidentAssessment incidentAssessment, Question question);

        bool AddImage(IncidentAssessment incidentAssessment, IncidentAssessmentImage inicidentImage);

        void Create(IncidentAssessment incidentAssessment);

        bool DeleteAnswers(int idIncidentAssessment);

        IncidentAssessment Find(int idIncidentAssessment);

        IList<Category> GetAnswers(int idIncidentAssessment);

        IList<IncidentAssessment> ListByConfiguration(Configuration configuration);

        void Update(IncidentAssessment incidentAssessment);
    }
}
