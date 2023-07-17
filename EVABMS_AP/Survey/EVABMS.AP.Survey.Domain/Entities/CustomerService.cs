using Newtonsoft.Json;

namespace EVABMS.AP.Survey.Domain.Entities
{
    public class CustomerService : SurveyModel
    {
        public CustomerService() : base()
        {

        }
        [JsonConstructor]
        public CustomerService(Form? form, List<Section>? section) : base(form, section)
        {

        }

        #region 紀錄Parameter Setting中紀錄的問題ID
        /// <summary>
        /// 問題:搭機日期
        /// </summary>
        public long MFOQ_ID_FLTDate { get; private set; }
        /// <summary>
        /// 問題:搭機艙等
        /// </summary>
        public long MFOQ_ID_Class { get; private set; }

        public CustomerService SetParameterSetting(long MFOQ_ID_FLTDate, long MFOQ_ID_Class)
        {
            this.MFOQ_ID_FLTDate = MFOQ_ID_FLTDate;
            this.MFOQ_ID_Class = MFOQ_ID_Class;
            return this;
        }
        #endregion

        #region 找答案
        public DateTime? FindFlightDate()
        {
            if (section.IsNullOrEmpty()) return null;
            foreach (Section s in section)
            {
                foreach (Question q in s.question)
                {
                    if (q.id == MFOQ_ID_FLTDate)
                    {
                        return q.option.First().value.ToDateTime();
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
