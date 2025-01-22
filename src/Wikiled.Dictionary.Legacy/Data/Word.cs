using System;
using Wikiled.Dictionary.Legacy.Definitions;

namespace Wikiled.Dictionary.Legacy.Data
{
    public class Word : ICloneable
    {
        private string text;

        public Word()
        {   
        }

        public Word(string text, WorldLanguage language)
        {
            Text = text;
            Language = language;
        }

        public Word(int id, WorldLanguage language)
        {
            Id = id;
            Language = language;
        }

        public int? Id { get; set; }

        public long? GoogleCount { get; set; }

        public WorldLanguage Language { get; set; }

        public string Text
        {
            get => text;
            set => text = string.Intern(value.ToLower());
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            Word word = obj as Word;
            if (Language != word?.Language)
            {
                return false;
            }

            return string.Compare(Text, word.text, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override int GetHashCode()
        {
            int result = Text?.GetHashCode() ?? 0;
            result = 31 * result + Language.GetHashCode();
            return result;
        }

        public object Clone()
        {
            return new Word(Text, Language)
            {
                Id = Id
            };
        }

        public override string ToString()
        {
            var returnValue = $"<{Text}> ({Language})";
            if (Id.HasValue)
            {
                returnValue = Id.Value + ": " + text;
            }

            if (GoogleCount.HasValue)
            {
                returnValue += " google: " + GoogleCount.Value;
            }

            return returnValue;
        }
    }
}
