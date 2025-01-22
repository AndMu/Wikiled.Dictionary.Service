using Wikiled.Dictionary.Legacy.Data;
using Wikiled.Redis.Persistency;

namespace Wikiled.Dictionary.Legacy.Definitions.Redis
{
    public interface IWordRepository : IRepository
    {
        void Save(Word word);

        void Save(SimpleTranslationResult translation);
    }
}
