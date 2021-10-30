using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SadConsole.Tests
{
    /// <summary>
    /// An attribute identical to [DynamicData], except for it also supports more convenient return types for the
    /// property or method specified.
    /// </summary>
    /// <remarks>
    /// The return type of the property or method specified can be any of the following:
    /// 1. IEnumerable of object[] (same as the built-in DynamicData)
    /// 2. IEnumerable of value tuples.  This is more convenient for specifying groups of items as parameters
    /// 3. IEnumerable of arbitrary objects, IFF the function that this attribute is being used on takes only a single parameter.
    /// </remarks>
    public class BetterDynamicDataAttribute : Attribute, ITestDataSource
    {
        private readonly string _dynamicDataSourceName;
        private Type _dynamicDataDeclaringType;
        private readonly DynamicDataSourceType _dynamicDataSourceType;

        /// <summary>
        /// Gets or sets the name of method used to customize the display name in test results.
        /// </summary>
        public string DynamicDataDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the declaring type used to customize the display name in test results.
        /// </summary>
        public Type DynamicDataDisplayNameDeclaringType { get; set; }

        /// <summary>
        /// Initializes and instance of the <see cref="DynamicDataAttribute"/> class.
        /// </summary>
        /// <param name="dynamicDataSourceName">
        /// The name of method or property having test data.
        /// </param>
        /// <param name="dynamicDataSourceType">
        /// Specifies whether the data is stored as property or in method.
        /// </param>
        public BetterDynamicDataAttribute(string dynamicDataSourceName,
                                          DynamicDataSourceType dynamicDataSourceType = DynamicDataSourceType.Property)
        {
            _dynamicDataSourceName = dynamicDataSourceName;
            _dynamicDataSourceType = dynamicDataSourceType;
        }

        /// <inheritdoc/>
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // Evaluate method or property we're using, just like [DynamicData] does
            object obj = EvaluateDataSource(methodInfo);

            // Convert whatever data we have to the IEnumerable<object[]> format required, or throw exception
            // if we didn't even get an enumerator.
            return obj switch
            {
                IEnumerable<object[]> objArray => objArray,
                IEnumerable enumerable => GetDataFromOtherEnumerable(enumerable),
                _ => throw new ArgumentException("Invalid type for dynamic data source field.", _dynamicDataSourceName)
            };
        }

        /// <inheritdoc/>
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (DynamicDataDisplayName != null)
            {
                MethodInfo declaredMethod = (DynamicDataDisplayNameDeclaringType
                                             ?? methodInfo.DeclaringType)!.GetTypeInfo()
                    .GetDeclaredMethod(DynamicDataDisplayName);
                ParameterInfo[] parameterInfoArray = declaredMethod != null
                    ? declaredMethod.GetParameters()
                    : throw new ArgumentNullException($"{DynamicDataSourceType.Method} {DynamicDataDisplayName}");

                if (parameterInfoArray.Length != 2 || parameterInfoArray[0].ParameterType != typeof(MethodInfo) ||
                    parameterInfoArray[1].ParameterType != typeof(object[]) ||
                    declaredMethod.ReturnType != typeof(string) || !declaredMethod.IsStatic || !declaredMethod.IsPublic)
                    throw new ArgumentNullException(
                        $"Method {DynamicDataDisplayName} must match the expected signature: public static {nameof(String)} {DynamicDataDisplayName}({string.Join(", ", nameof(MethodInfo), typeof(object[]).Name)})");

                return declaredMethod.Invoke(null, new object[] { methodInfo, data }) as string;
            }

            return data == null
                ? null
                : string.Format(CultureInfo.CurrentCulture, "{0} ({1})", methodInfo.Name,
                    string.Join(",", data.AsEnumerable()));
        }

        private object EvaluateDataSource(MethodInfo methodInfo)
        {
            _dynamicDataDeclaringType ??= methodInfo.DeclaringType;
            object obj = _dynamicDataSourceType switch
            {
                DynamicDataSourceType.Property => (_dynamicDataDeclaringType!.GetTypeInfo()
                                                       .GetDeclaredProperty(_dynamicDataSourceName) ??
                                                   throw new ArgumentNullException(
                                                       $"{DynamicDataSourceType.Property} {_dynamicDataSourceName}"))
                    .GetValue(null, null),
                DynamicDataSourceType.Method => (_dynamicDataDeclaringType!.GetTypeInfo()
                                                     .GetDeclaredMethod(_dynamicDataSourceName) ??
                                                 throw new ArgumentNullException(
                                                     $"{DynamicDataSourceType.Method} {_dynamicDataSourceName}"))
                    .Invoke(null, null),
                _ => null
            };

            if (obj == null)
                throw new ArgumentNullException(
                    $"Value returned by property or method {_dynamicDataSourceName} shouldn't be null");

            return obj;
        }

        private IEnumerable<object[]> GetDataFromOtherEnumerable(IEnumerable enumerator)
        {
            // Convert to IEnumerable<object[]>.  If we have a tuple, extract the values and place those in the array;
            // otherwise, just place the item in a single-element array (so we support functions w/ 1 parameter taking
            // Enumerable<ArbitraryObject>
            foreach (object item in enumerator)
            {
                if (item is ITuple tuple)
                {
                    List<object> objs = new List<object>(tuple.Length);
                    for (int i = 0; i < tuple.Length; i++)
                        objs.Add(tuple[i]);

                    yield return objs.ToArray();
                }
                else
                    yield return new[] { item };
            }
        }
    }
}
