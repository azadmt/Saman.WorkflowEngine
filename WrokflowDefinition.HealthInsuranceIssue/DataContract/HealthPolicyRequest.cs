using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrokflowDefinition.HealthInsuranceIssue.DataContract
{
    public class HealthPolicyRequest
    {
        public Guid PolicyHodler { get; set; }
        public decimal Ammount { get; set; }
        public List<Insured> Insureds { get; set; } = new();
        public List<Cover> Covers { get; set; } = new();
        public List<MedicalQuestions> MedicalQuestions { get; set; } = new();

        public static HealthPolicyRequest GenerateSample(decimal amount = 100_000, int underlyingDiseaseCount = 0, int withInvlidAgeCount = 0)
        {
            var model = new HealthPolicyRequest();
            model.Ammount = amount;

            for (int i = 1; i < 5; i++)
            {
                model.MedicalQuestions.Add(new MedicalQuestions { Code = i });
                model.Covers.Add(new Cover());
                model.Insureds.Add(new Insured() {BirthDate= DateTime.Now.AddYears(-30) });
            }
            if (withInvlidAgeCount > 0)
            {
                var items = model.Insureds.Take(withInvlidAgeCount);
                foreach (var item in items)
                {
                    item.BirthDate = DateTime.Now.AddDays(-80);
                }
            }

            if (underlyingDiseaseCount > 0)
            {
                var items = model.Insureds.Take(underlyingDiseaseCount);
                foreach (var item in items)
                {
                    item.HasUnderlyingDisease = true;
                }
            }
            model.PolicyHodler = model.Insureds.First().Id;

            return model;


        }
    }

    public class Insured
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool HasUnderlyingDisease { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Cover
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class MedicalQuestions
    {
        public int Code { get; set; }

    }
}
