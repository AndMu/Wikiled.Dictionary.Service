using System;

namespace Wikiled.Dictionary.Legacy.Data
{
    public interface IDataHolder
    {
        void Remove(string tag);
        T GetItem<T>(string resource);
        T GetItem<T>(string tag, T defaultValue);
        T GetCreateItem<T>(string tag) where T : new();
        T GetCreateItem<T>(string tag, Func<T> evaluator);
        void SetItem<T>(string tag, T value);
        void SetItem<T>(string tag, T value, T defaultValue);
        object this[string name] { get; set; }
    }
}