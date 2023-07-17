using EVABMS.AP.ConnectingString.Domain.Entities;
using EVABMS.AP.ConnectingString.Infrastructure;
using LogHelper.Nlog;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog.Fluent;
using Serilog;
using Serilog.Extensions.Hosting;
using System.Text.Json.Nodes;
using UtilityHelper;

namespace EVABMS_AP.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class ConnectingStringController : ControllerBase
    {
        readonly ILogger<ConnectingStringController> log;
        readonly IDiagnosticContext _diagnosticContext;

        public ConnectingStringController() { }
        public ConnectingStringController(ILogger<ConnectingStringController> logger, IDiagnosticContext diagnosticContext) 
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
        }

        #region [HttpGet("FileName")] QueryFilename
        /// <summary>
        /// ini檔名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("FileName")]
        public ApiResult<List<string>> FileName()
        {
            try
            {
                ConnectingStringRepository repository = new();
                List<string> files = repository.Getfilename(repository.address);
                return new ApiResult<List<string>>(files);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new ApiError<List<string>>("Exception", ex.Message);
            }
        }
        #endregion

        #region [HttpGet("FileData/{fileName}")] QueryFileData
        /// <summary>
        /// ini解碼內容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("FileData/{fileName}")]
        public ApiResult<List<ConnectingStringQuery>> FileData(string? fileName = null)
        {
            try
            {
                ConnectingStringRepository repository = new ConnectingStringRepository();
                string content = repository.Getfile(defaultININame: fileName);

                List<ConnectingStringQuery> connectingStringQueryModels = repository.InIDecrypt(content);

                return new ApiResult<List<ConnectingStringQuery>>(connectingStringQueryModels);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new ApiError<List<ConnectingStringQuery>>("Exception", ex.Message);
            }
        }
        #endregion

        #region [HttpPost("FileData")] FileDataUpload
        /// <summary>
        /// Api For UploadFile 
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("FileData")]
        public ApiResult<List<ConnectingStringQuery>> FileDataUpload(JsonObject insertJson)
        {
            try
            {
                ConnectingStringRepository repository = new();
                ConnectStringUpload query = ConnectStringUpload.Create(insertJson["filecontent"].ToString());
                string content = query.filecontent;

                List<ConnectingStringQuery> connectingStringQueryModels = repository.InIDecrypt(content);
                return new ApiResult<List<ConnectingStringQuery>>(connectingStringQueryModels);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new ApiError<List<ConnectingStringQuery>>("Exception", ex.Message);
            }
        }
        #endregion

        #region [HttpPost("Update")] UpdateFileData
        /// <summary>
        /// update data
        /// </summary>
        /// <param name="insertJson"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public ApiResult<bool> Update(JsonObject insertJson)
        {
            ApiResult<bool> apiResult = new();
            ConnectingStringRepository repository = new();
            string userid = insertJson["userID"].ToSafeString();
            List<ConnectingStringQuery> newmodels = JsonConvert.DeserializeObject<List<ConnectingStringQuery>>(insertJson["newmodel"].ToSafeString());
            try
            {
                if (newmodels.IsNullOrEmpty())
                {
                    apiResult.Succ = false;
                    return apiResult;
                }

                string content = repository.Getfile();

                List<ConnectingStringQuery> oldModels = repository.InIDecrypt(content);

                apiResult.Succ = repository.UpdateRecord(oldModels, newmodels, userid);
                if (apiResult.Succ == false) return new ApiError<bool>("Exception", "Export failed");


                apiResult.Succ = repository.InIFileManagement(repository.InIEncrypt(newmodels));
                apiResult.Message = apiResult.Succ ? $"Save Success. File Location : [thisServer]\\{repository.address}" : "Save failed from file creation.";
            }
            catch (IOException iox)
            {
                log.LogError(iox.ToString());
                apiResult = new ApiError<bool>("Exception", iox.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                apiResult = new ApiError<bool>("Exception", ex.Message);
            }
            return apiResult;

        }
        #endregion
    }
}