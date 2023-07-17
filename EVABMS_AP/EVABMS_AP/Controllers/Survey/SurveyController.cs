using EVABMS.AP.Survey.Domain.Entities;
using EVABMS.AP.Survey.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OracleHelper.TransactSql;
using System.Text.Json.Nodes;
using System.Transactions;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    /// <summary>
    /// SurveyController
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class SurveyController : ControllerBase
    {
        #region [HttpGet] QueryData
        /// <summary>
        /// 找出全部的formData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult<List<Form>> Query()
        {

            try
            {
                ApiResult<List<Form>> result = new();
                SurveyRepository repository = new();
                List<Form> data = repository.Query();

                return result = new ApiResult<List<Form>>(data);
            }
            catch (Exception ex)
            {
                return new ApiError<List<Form>>(null, ex.Message);
            }
        }
        #endregion

        #region [HttpGet][Route("SurveyModel/{id}")] QuerySurveyModel
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SurveyModel/{id}/{lang}")]
        public ApiResult<SurveyModel> QuerySurveyModel(long id, string lang)
        {
            ApiResult<SurveyModel> result = new ApiResult<SurveyModel>();
            SurveyRepository repository = new();
            try
            {
                SurveyModel surveymodel = repository.QuerySurveyModel(id, lang);
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
        #endregion

        #region [HttpGet][Route("InsertSurveyAns/{id}")] InsertSurveyAns
        /// <summary>
        /// 新增SurveyModel
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public ApiResult<bool> InsertSurveyAns(JsonObject insertJson)
        {
            ApiResult<bool> apiResult = new ApiResult<bool>();
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    OraDataService ora = new OraDataService();

                    SurveyModel insertData = JsonConvert.DeserializeObject<SurveyModel>(insertJson.ToSafeString());

                    apiResult.Succ = new SurveyRepository().InsertAns(insertData);

                    if (apiResult.Succ == false)
                    {
                        return apiResult = new ApiError<bool>("500", "Insert MGT_FORM_ANS、MGT_FORM_ANS_SECTION、MGT_FORM_ANS_QUESTION、MGT_FORM_ANS_OPTION failed.");
                    }

                    apiResult.Message = "Insert success.";
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;
        }
        #endregion
    }
}
