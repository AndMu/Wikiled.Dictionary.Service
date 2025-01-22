using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Wikiled.Core.Standard.Api.Server;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Dictionary.Data;
using Wikiled.Dictionary.Logic;
using Wikiled.Server.Core.ActionFilters;

namespace Wikiled.Service.Controllers
{
    [Route("[controller]")]
    [RequestValidation]
    public class DictionaryController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ILanguageDictionary translation;

        public DictionaryController(ILanguageDictionary translation)
        {
            Guard.NotNull(() => translation, translation);
            this.translation = translation;
        }

        [HttpPost("Translate")]
        public async Task<ApiOkResponse> Translate([FromBody]TranslationRequest translationRequest)
        {
            logger.Debug("Translate {0}", translationRequest);
            var result = await translation.Translate(translationRequest).ConfigureAwait(false);
            return new ApiOkResponse(result);
        }

        [HttpGet("{from}/{to}/{word}")]
        public async Task<ApiOkResponse> Translate(Language from, Language to, string word)
        {
            TranslationRequest request = new TranslationRequest();
            request.From = from;
            request.To = to;
            request.Word = word;
            logger.Debug("Translate {0}", request);
            var result = await translation.Translate(request).ConfigureAwait(false);
            return new ApiOkResponse(result);
        }
    }
}
