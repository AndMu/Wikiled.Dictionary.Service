using System.Collections.Generic;
using System.Linq;
using NLog;
using Wikiled.Dictionary.Legacy.Database;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data.Factory
{
    public class TranslationSnapshotFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IManager manager;

        public TranslationSnapshotFactory(IManager manager)
        {
            this.manager = manager;
        }
        
        public TranslationSnapshot Construct(IEnumerable<InfoTable> tables)
        {
            log.Debug("Constructing");
            TranslationSnapshot snapshot = new TranslationSnapshot();
            snapshot.Information = (from item in tables
                select (new TranslationData(
                    item.FromLanguage.Convert(),
                    item.ToLanguage.Convert(),
                    item.Total))).ToArray();
            var left = snapshot.Information.Select(item => item.FromLanguage);
            var right = snapshot.Information.Select(item => item.ToLanguage);
            snapshot.Languages = left.Concat(right).Distinct().ToArray();
            snapshot.LatinLanguages = snapshot.Languages.Where(item => manager[item].Description.IsLatin).ToArray();
            snapshot.NonLatinLanguages = snapshot.Languages.Where(item => !manager[item].Description.IsLatin).ToArray();
            return snapshot;
        }
    }
}
