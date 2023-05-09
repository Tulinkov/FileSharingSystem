using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    /// <summary>
    /// Stub of authentication server.
    /// Accepts only one static key : 56351331F4F24B63BE5037FF4D2D7526
    /// </summary>
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string GOOD_KEY = "56351331F4F24B63BE5037FF4D2D7526";

        /// <summary>
        /// Checks if the key is valid
        /// </summary>
        /// <param name="key">Authentication key</param>
        /// <returns>Authentication result</returns>
        [HttpGet]
        public JsonResult Key(string key)
        {
            bool res = key.Equals(GOOD_KEY);
            return new JsonResult(res ? Ok() : Unauthorized());
        }
    }
}
