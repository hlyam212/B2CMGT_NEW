using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Survey.Domain.Entities
{
    public class SurveyModel : BaseEntities
    {
        public Form? form { get; private set; }
        public List<Section>? section { get; private set; }

        public SurveyModel()
        {

        }

        [JsonConstructor]
        public SurveyModel(Form? form, List<Section>? section)
        {
            this.form = form;
            this.section = section;
        }

        public static SurveyModel Create(Form? form, List<Section>? section)
        {
            SurveyModel model = new SurveyModel(form, section);
            return model;
        }

        public bool Exist()
        {
            return form!=null && 
                   form?.id>0 &&
                   section.HasValue();
        }
    }
}
