using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using TotalLoss.Service;
using TotalLoss.Repository;
using TotalLoss.Domain.Model;
using System.Collections;
using System.Collections.Generic;

namespace TotalLoss.Test
{
    [TestClass]
    public class IncidentAssessmentTest
    {
        IncidentAssessmentService _service;
        IncidentAssessment _model;
        string _guid = "789456";

        [TestInitialize]
        public void Initialize()
        {
            SqlConnection _conexao = new SqlConnection("Data Source=10.33.170.230;Initial Catalog=AudaFNOL_HM;Integrated Security=False;User Id=AudaFnol_USR_HM;Password=Audatex1;");
            IncidentAssessmentRepository _repository = new IncidentAssessmentRepository(_conexao);
            //_service = new IncidentAssessmentService(_repository, "", "", "");

            //_model = new IncidentAssessment()
            //{
            //    ClaimNumber = _guid,
            //    Configuration = new Configuration()
            //    {
            //        Id = 1,
            //        Name = "Allianz"
            //    },
            //    InsuredFone = "(11)9876-9865",
            //    InsuredName = "Simplicio",
            //    LicensePlate = "ABC-4321",
            //    Provider = "Mundial",
            //    WorkProvider = "Flavio Ota",
            //    WorkProviderFone = "(11)93216-9874"
            //};
        }

        [TestMethod]
        public void IncidentAssessment_Create()
        {
            //_service.Create(_model);

            //IncidentAssessment _incident = _service.FindByKey(_model.Key);
            //_model.Id = _incident.Id;

            //Assert.AreEqual(_guid, _incident.ClaimNumber);
        }

        [TestMethod]
        public void IncidentAssessment_GetAnswers()
        {
            //IList<Category> listAnswers = _service.GetAnswers(1000009);

            //Assert.AreEqual(1, listAnswers.Count);
        }

        [TestMethod]
        public void IncidentAssessment_List()
        {
            //_service.Create(_model);
            //_service.Create(_model);
            //_service.Create(_model);

            //IList<IncidentAssessment> _incident = _service. Search(1);

            //Assert.AreEqual(3, _incident.Count);
        }

        [TestCleanup]
        public void Finalize()
        {
            //IList<IncidentAssessment> _incident = _service.Search(1);

            //foreach (var item in _incident)
            //{
            //    if (!string.IsNullOrEmpty(item.ClaimNumber) &&
            //        item.ClaimNumber.Equals(_guid))
            //    {
            //        _service.Delete(item.Id);
            //    }
            //}
        }
    }
}
