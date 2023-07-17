using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonHelper.Services.CallApi
{
    public interface IConnect
    {

        /// <summary>
        /// 傳入Token
        /// </summary>
        /// <param name="_Jwt"></param>
        void SetJwt(string jwtToken);

        /// <summary>
        /// 傳入SessionId
        /// </summary>
        /// <param name="_SessionId"></param>
        void SetSessionId(string sessionId);

        /// <summary>
        /// Http Post
        /// </summary>
        /// <typeparam name="T">傳入參數物件泛型</typeparam>
        /// <param name="PostParameter">傳入參數物件</param>
        /// <param name="ControllerName">目標Controller 名稱</param>
        /// <returns></returns>
        Task<string> Post<T>(T postParameter, string controllerName, bool autoToken = false);

        /// <summary>
        /// Http Get 
        /// </summary>
        /// <param name="PostParameter">傳入參數字串 ex:"id=17&seq=3</param>
        /// <param name="ControllerName">目標Controller 名稱</param>
        /// <returns></returns>
        Task<string> Get(string postParameter, string controllerName, bool autoToken = false);

        Task<Tuple<bool, string>> PostSOAPXML(string URL, string PostContent);
    }
}
