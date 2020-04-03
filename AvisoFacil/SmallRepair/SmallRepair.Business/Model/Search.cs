using SmallRepair.Management.Enum;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Business.Model
{
    public class Search
    {
        private Company _company;

        public string Plate { get; set; }

        public string BodyType { get; set; }

        public EnmAssessmentState? State { get; set; }

        public void SetCompany(Company company)
        {
            _company = company;
        }

        public Func<Assessment, bool> GetFunc()
        {
            Func<Assessment, bool> whereCondition = a
                                                    => (a.IdCompany == this._company.IdCompany &&
                                                        (this.State.HasValue ? a.State == this.State.Value : a.State == a.State) &&
                                                        (!string.IsNullOrEmpty(this.BodyType) ? a.BodyType == this.BodyType : a.BodyType == a.BodyType) &&
                                                        (!string.IsNullOrEmpty(this.Plate) ? a.Plate == this.Plate : a.Plate == a.Plate));
            return whereCondition;
        }
    }
}
