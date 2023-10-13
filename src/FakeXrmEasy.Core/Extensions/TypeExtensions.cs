﻿using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;

namespace FakeXrmEasy.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true if the type is an OptionSetValue
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsOptionSet(this Type t)
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(OptionSetValue)
                   || t.IsEnum
                   || nullableType != null && nullableType.IsEnum;
        }

#if FAKE_XRM_EASY_9

        /// <summary>
        /// Returns true if the type is an OptionSetValueCollection
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsOptionSetValueCollection(this Type t)
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(OptionSetValueCollection);
        }
#endif

        /// <summary>
        /// Returns true if the type is a DateTime
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsDateTime(this Type t)
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(DateTime)
                   || nullableType != null && nullableType == typeof(DateTime);
        }

        /// <summary>
        /// Returns true if the type is a Nullable Enum
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullableEnum(this Type t)
        {
            return
                t.IsGenericType
                && t.GetGenericTypeDefinition() == typeof(Nullable<>)
                && t.GetGenericArguments()[0].IsEnum;
        }

        /// <summary>
        /// Gets the PropertyInfo for an attribute of an earlybound type
        /// </summary>
        /// <param name="earlyBoundType"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static PropertyInfo GetEarlyBoundTypeAttribute(this Type earlyBoundType, string attributeName)
        {
            var attributeInfo = earlyBoundType.GetProperties()
                .Where(pi => pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true).Length > 0)
                .Where(pi => (pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true)[0] as AttributeLogicalNameAttribute).LogicalName.Equals(attributeName))
                .FirstOrDefault();

            return attributeInfo;
        }
    }
}