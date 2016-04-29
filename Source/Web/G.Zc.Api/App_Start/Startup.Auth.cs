using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Threading;
using G.Util.Tool;
using G.Zc.Entity.Eap;
using G.Zc.Api.Biz;
using GOMFrameWork;
using System.Web.SessionState;

namespace G.Zc.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new BaseOAuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true,
                RefreshTokenProvider = new BaseRefreshTokenProvider()
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }

    public class BaseOAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public BaseOAuthorizationServerProvider() : base() { }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //string clientId;
            //string clientSecret;
            //context.TryGetBasicCredentials(out clientId, out clientSecret);

            //if (clientId == "1234" && clientSecret == "5678")
            //{
            //    context.Validated(clientId);
            //}
            //else
            //{
            //    context.SetError("客户端错误！");
            //}
            context.Validated();
            return base.ValidateClientAuthentication(context);
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            //_clientId = context.ClientId;

            //var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, "App"));
            //var props = new AuthenticationProperties(new Dictionary<string, string>
            //    {
            //        { "as:client_id", context.ClientId }
            //    });
            //var ticket = new AuthenticationTicket(oAuthIdentity, props);
            //context.Validated(ticket);
            context.Validated();
            return base.GrantClientCredentials(context);
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            CommonResult result = null;

            var authcode = context.Request.Query["authcode"];
            var truecode = System.Web.HttpRuntime.Cache["authcode"];
            if (authcode.ToLower() != truecode.ToString().ToLower())
            {
                result = new CommonResult() { ResultID = 0, Message = "验证码错误！" };
            }
            else
            {
                //验证用户名密码
                try
                {
                    result = LoginIn.Login(context.UserName, context.Password);
                }
                catch (Exception ex)
                {
                    result = new CommonResult() { ResultID = 0, Message = "用户登录发生错误！" };
                    G.Util.Tool.LogHelper.Logger.Fatal(result.Message, ex);
                }
            }

            if (result.ResultID == 0)
            {
                context.SetError(result.Message);
            }
            else
            {
                var user = result.Tag as EAP_User;

                var oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                oAuthIdentity.AddClaim(new Claim("UserID", user.UserID.ToString()));
                oAuthIdentity.AddClaim(new Claim("OrgId", user.OrgId.ToString()));
                oAuthIdentity.AddClaim(new Claim("POrgID_G", user.POrgID_G.ToString()));
                oAuthIdentity.AddClaim(new Claim("ORGTYPE_G", user.ORGTYPE_G.ToString()));
                oAuthIdentity.AddClaim(new Claim("RoleIDs_G", user.RoleIDs_G));

                var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
                context.Validated(ticket);
            }

            context.Response.Headers.Add("Access-Control-Allow-Origin", new string[] { "*" });

            return base.GrantResourceOwnerCredentials(context);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            //var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];

            //if (originalClient != _clientId)
            //{
            //    context.Rejected();
            //}
            //else
            //{
            //    var newId = new ClaimsIdentity(context.Ticket.Identity);
            //    newId.AddClaim(new Claim("newClaim", "refreshToken"));

            //    var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            //    context.Validated(newTicket);
            //}

            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);

            return base.GrantRefreshToken(context);
        }
    }

    public class BaseRefreshTokenProvider : AuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

        public override void Create(AuthenticationTokenCreateContext context)
        {
            string tokenValue = Guid.NewGuid().ToString("n");

            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(60);

            _refreshTokens[tokenValue] = context.SerializeTicket();

            context.SetToken(tokenValue);
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_refreshTokens.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }
    }
}
