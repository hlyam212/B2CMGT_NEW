using EVABMS.AP.Parameter.Domain.Entities;
using EVABMS.AP.Parameter.Infrastructure;
using EVABMS.AP.Survey.Domain.Entities;
using EVABMS.AP.Survey.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// SurveyController
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class FormCustomerService : ControllerBase
    {
        /// <summary>
        /// 依照搜尋條件找出Form資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryBasic/{id}/{lang}")]
        public ApiResult <SurveyModel> QueryBasic(long id, string lang)
        {
            ApiResult<SurveyModel> result = new ApiResult<SurveyModel>();
            SurveyRepository repository = new();
            try 
            {
                SurveyModel surveymodel = repository.QueryBasic(id, lang);
                if (surveymodel == null) return new ApiError<SurveyModel>(null, "Data Not Found");

                //return value
                result = new ApiResult<SurveyModel>(surveymodel);
            }
            catch (Exception ex) 
            {
                result = new ApiError<SurveyModel>(null, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("CheckBasicData")]
        public ApiResult<SurveyModel> CheckBasicData(JsonObject insertJson)
        {
            JObject odscmdata = (JObject)JsonConvert.DeserializeObject<object>(insertJson["ODS"].ToSafeString());
            Form form = JsonConvert.DeserializeObject<Form>(insertJson["form"].ToSafeString());
            List<Section> section = JsonConvert.DeserializeObject<List<Section>>(insertJson["section"].ToSafeString());
            ApiResult<SurveyModel> result = new ApiResult<SurveyModel>();
            SurveyRepository repository = new();

            try
            {
                //check soap status
                if (odscmdata["Status"]?.ToSafeString() != "OK") return new ApiError<SurveyModel>(null, "ODSCM Error");

                //check available days
                ParameterSetting quitirua = new ParameterSetting().SetQuery("EVABMS", "Survey", "AvailableDays");
                double days = new ParameterRepository().Query(quitirua).FirstOrDefault().value.ToInt32();
                var Now = odscmdata["TimeStamp"]?.ToSafeString();
                var DepDay = odscmdata["ReplyData"].FirstOrDefault()?["FltLists"].FirstOrDefault()?["Flt_Dt"].ToSafeString();
                if (repository.ComputingDays(Now, DepDay) > days) return new ApiError<SurveyModel>(null, "The time available for response has expired");

                //default Answer
                SurveyModel surveymodel = repository.QuerySurveyModel(form.id, form.default_lang);
                surveymodel.section[0] = section[0];//*
                return result = new ApiResult<SurveyModel>(surveymodel);
            }
            catch (Exception ex)
            {
                return new ApiError<SurveyModel>(null, ex.Message);
            }
        }

        /// <summary>
        /// 依照搜尋條件找出抽獎問卷資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryDrawCondition/{id}")]
        public ApiResult<long> QueryDrawCondition (long id)
        {
            ApiResult<long> result = new ApiResult<long>();
            SurveyRepository repository = new();

            long test = repository.QueryDrawCondition("2023/06/01", "2023/06/30");

            result = new ApiResult<long>(test);

            return result;

        }
    }
}
