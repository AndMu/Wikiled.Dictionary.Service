using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Dictionary.Legacy.Data
{
    public static class EntityExtension
    {
        static readonly Dictionary<Type, EdmEntityTypeAttribute> typeCache = new Dictionary<Type, EdmEntityTypeAttribute>();
        static readonly Dictionary<Type, string> keyCache = new Dictionary<Type, string>();

        public static string GetEntitySetName<T>(bool qualified = false)
        {
            EdmEntityTypeAttribute attrib = typeCache.GetSafeCreate(
                typeof(T),
                () => (EdmEntityTypeAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(EdmEntityTypeAttribute)));
            if (attrib == null)
            {
                return null;
            }
            return qualified ? (attrib.NamespaceName + "." + attrib.Name) : attrib.Name;
        }

        public static string GetKeyName<T>() 
        {
            var name = keyCache.GetSafeCreate(typeof(T),
                                   () =>
                                   (from prop in typeof(T).GetProperties()
                                    let attrib = (EdmScalarPropertyAttribute) Attribute.GetCustomAttribute(prop, typeof(EdmScalarPropertyAttribute))
                                    where attrib != null && attrib.EntityKeyProperty
                                    select prop.Name).Single());
            return name;
        }
    }
}
