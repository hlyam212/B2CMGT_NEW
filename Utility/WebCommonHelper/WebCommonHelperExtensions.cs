using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebCommonHelper.Config;
using WebCommonHelper.Middlewares;
using WebCommonHelper.Services.Authenticaiton;
using WebCommonHelper.Services.CallApi;

namespace WebCommonHelper
{
    public static class WebCommonHelperExtensions
    {
        #region Common

        #region Add Services
        /// <summary>
        /// 註冊 Web 共用服務
        /// 1. Client 呼叫 WEB
        /// 2. Web 呼叫 AP
        /// </summary>
        public static void AddWebCommonHelper(this IServiceCollection services, bool addXml = false)
        {
            // 新增controller與移除controller回傳json大小寫預設規則
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new CommonHelper.DataTableConverter());
                options.JsonSerializerOptions.Converters.Add(new CommonHelper.DataSetConverter());
            });

            if (addXml)
            {
                services.AddControllers().AddXmlDataContractSerializerFormatters();
            }
        }
        #endregion

        #region Use Services
        public static void UseWebCommonHelper(this WebApplication app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
        #endregion

        #endregion

        #region Client

        #region Add Services
        /// <summary>
        /// 註冊 Client 相關的 服務
        /// 1. Jwt Token
        /// 2. Session
        /// </summary>
        public static void AddClientHelper(
            this IServiceCollection services, 
            IConfigurationRoot config,
            string sessionCookieName,
            string configSectionKey = "JwtSettings",
            bool changeJsMappings = false)
        {
            // 設一個預設時間針對Client端 Session Cookies
            JwtOptions jwtOptions = config.GetSection(configSectionKey).Get<JwtOptions>();
            if (jwtOptions == null)
            {
                throw new ArgumentNullException(nameof(jwtOptions));
            }
            double expiredMutinutes = jwtOptions.RefreshTokenValidityInHours * 60;

            // 註冊 Cookie
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = contex => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

            // 註冊 分散式記憶體快取物件，Session會用到
            services.AddDistributedMemoryCache();

            // 註冊 Seesion
            services.AddSession(options =>
            {
                options.Cookie.Name = sessionCookieName;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;                
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.IdleTimeout = TimeSpan.FromMinutes(expiredMutinutes); // 指出會話在放棄內容之前可以閒置的時間長度。 每個工作階段存取都會重設逾時。 請注意，這只適用於工作階段的內容，而不適用於Cookie。
            });

            // 註冊 HttpContextAccessor 服務，AddSingleton (整個 Process 只建立一個 Instance，任何時候都共用它。)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (changeJsMappings)
            {
                /* 避免以下錯誤
                 * "Failed to load module script: Expected a JavaScript module script but the server responded with a MIME type of "text/plain". Strict MIME type checking is enforced for module scripts per HTML spec."
                 * 這個錯誤訊息表示瀏覽器無法正確載入 JavaScript 模組，因為伺服器回應的 MIME 類型是 "text/plain" 而非 "application/javascript"。這是因為在 .NET 6 中預設會使用 "text/plain" MIME 類型來回應 JavaScript 檔案，而這不符合 HTML 規範中對模組載入的嚴格 MIME 類型檢查。
                 */
                services.Configure<StaticFileOptions>(options =>
                {
                    options.ContentTypeProvider = new FileExtensionContentTypeProvider
                    {
                        Mappings = { [".js"] = "application/javascript" }
                    };
                });
            }
        }
        #endregion

        #endregion

        #region Web

        #region Add Services
        /// <summary>
        ///  註冊 WEB層 服務
        /// </summary>
        public static void AddWebHelper(
            this IServiceCollection services,
            IConfigurationRoot config,
            bool withLdap = true,
            string configSectionKey = "JwtSettings",
            string apiSectionKey = "APISettings",
            bool isJwt = true)
        {
            if (withLdap)
            {
                // 針對 AD(Ldap)的Authentication 服務，進行注入
                if (OperatingSystem.IsWindows())
                {
                    services.AddScoped<IIdentity, LdapIdentity>();
                }

                services.AddScoped<IUserService, UserService>();
            }
            else
            {
                services.AddScoped<IAppService, AppService>();
            }

            // 註冊 Client JWT Token 服務
            services.Configure<JwtOptions>(config.GetSection(configSectionKey));
            if (isJwt)
            {
                services.AddScoped<IJwt, Jwt>();
            }
            else
            {
                services.AddScoped<IJwt, Jwe>();
            }

            // 註冊 呼叫 API 服務
            services.Configure<APISettings>(config.GetSection(apiSectionKey));
            services.AddScoped<IConnect, CallAPI>();
        }
        #endregion

        #region Use Services
        public static void UseWebHelper(this WebApplication app, bool withLdap = true)
        {
            app.UseCookiePolicy();
            app.UseSession();

            if (withLdap)
            {
                app.UseMiddleware<JwtMiddleware>();
            }
            else
            {
                app.UseMiddleware<OrderJwtMiddleware>();
            }
        }
        #endregion

        #endregion

        #region AP

        #region Add Services
        /// <summary>
        /// 註冊 AP層 的服務
        /// </summary>
        public static void AddApHelper(
            this IServiceCollection services,
            IConfigurationRoot config,
            string configSectionKey = "JwtSettings",
            string apiSectionKey = "APISettings",
            bool isJwt = true)
        {
            services.AddAuthorization();

            // 註冊 Cookie
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = contex => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // 設一個預設時間針對Client端 Session Cookies
            JwtOptions jwtOptions = config.GetSection(configSectionKey).Get<JwtOptions>();
            if (jwtOptions == null)
            {
                throw new ArgumentNullException(nameof(jwtOptions));
            }
            if (string.IsNullOrEmpty(jwtOptions.SecretKey) || string.IsNullOrEmpty(jwtOptions.Issuer))
            {
                throw new ArgumentNullException(nameof(jwtOptions.SecretKey));
            }
            double expiredMutinutes = jwtOptions.RefreshTokenValidityInHours * 60;

            // 註冊 分散式記憶體快取物件，Session會用到
            services.AddDistributedMemoryCache();

            // 註冊 Seesion
            services.AddSession(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.IdleTimeout = TimeSpan.FromMinutes(expiredMutinutes);
            });

            // 註冊 HttpContextAccessor 服務，AddSingleton (整個 Process 只建立一個 Instance，任何時候都共用它。)
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 註冊 Client JWT Token 服務
            services.Configure<JwtOptions>(config.GetSection(configSectionKey));
            if (isJwt)
            {
                services.AddScoped<IJwt, Jwt>();
            }
            else
            {
                services.AddScoped<IJwt, Jwe>();
            }

            // 註冊 jwt檢核設定  服務
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateActor = false,
                        ValidateAudience = false,//api接受者驗證
                        ValidateLifetime = true,//存活時間驗證
                        ValidateIssuerSigningKey = true,//SigningKey金鑰驗證
                        ValidIssuer = jwtOptions.Issuer,//Issuer(發布者)驗證
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey + "_AccessToken")),
                        ClockSkew = TimeSpan.Zero//時間偏移驗證
                    };
                });
				
			services.Configure<APISettings>(config.GetSection(apiSectionKey));
        }
        #endregion

        #region Use Services
        public static void UseApHelper(this WebApplication app)
        {
            app.UseSession();

            APISettings setting = app.Configuration.GetSection("APISettings").Get<APISettings>();
            if (setting.Api.doEncryptAndDecrypt)
            {
                // 前後台資料傳輸加解密Middleware
                app.UseMiddleware<DecryptMiddleware>();
                app.UseMiddleware<EncryptMiddleware>();
            }
        }
        #endregion

        #endregion
    }
}
