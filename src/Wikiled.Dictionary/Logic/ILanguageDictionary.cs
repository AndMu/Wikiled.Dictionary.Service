using System.Threading.Tasks;
using Wikiled.Dictionary.Data;
using Wikiled.Redis.Persistency;

namespace Wikiled.Dictionary.Logic
{
    public interface ILanguageDictionary : IRepository
    {
        Task<TranslationResult> Translate(TranslationRequest request);
    }
}
