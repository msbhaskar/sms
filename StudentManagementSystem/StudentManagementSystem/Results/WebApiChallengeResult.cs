namespace StudentManagementSystem.Web.Results
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class WebApiChallengeResult : IHttpActionResult
    {
        public WebApiChallengeResult(string loginProvider, ApiController controller)
        {
            this.LoginProvider = loginProvider;
            this.Request = controller.Request;
        }

        public string LoginProvider { get; }

        public HttpRequestMessage Request { get; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            this.Request.GetOwinContext().Authentication.Challenge(this.LoginProvider);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = this.Request;
            return Task.FromResult(response);
        }
    }
}
