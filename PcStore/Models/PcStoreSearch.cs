using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PcStore.Models
{
    public class PcStoreSearch
    {
        private readonly PCBuilderContext _context;

        public PcStoreSearch(PCBuilderContext context)
        {
            _context = context;
        }

        public IEnumerable<ComponentBase> Search<T>(ObservableCollection<KeyValuePair<string, string>> filters) where T : ComponentBase
        {
            // Get all components of the specified type
            var components = GetAllComponentsGeneric<T>();

            // Prepare regex to separate comparison operator and value
            var regex = new Regex(@"([\<\<=\=\>=\>]*)(\d+(\.\d+)?)");

            // Filter the components
            foreach (var filter in filters)
            {
                var property = typeof(T).GetProperty(filter.Key);
                if (property != null && property.PropertyType == typeof(decimal) || property.PropertyType == typeof(double))  // ensure we're dealing with decimal
                {
                    var match = regex.Match(filter.Value);
                    if (match.Success)
                    {
                        var comparisonOperator = match.Groups[1].Value;
                        var value = double.Parse(match.Groups[2].Value, new CultureInfo("en-US"));

                        switch (comparisonOperator)
                        {
                            case "<":
                                components = components.Where(c => Convert.ToDouble(property.GetValue(c)) < value).ToList();
                                break;
                            case "<=":
                                components = components.Where(c => Convert.ToDouble(property.GetValue(c)) <= value).ToList();
                                break;
                            case "=":
                                components = components.Where(c => Convert.ToDouble(property.GetValue(c)) == value).ToList();
                                break;
                            case ">=":
                                components = components.Where(c => Convert.ToDouble(property.GetValue(c)) >= value).ToList();
                                break;
                            case ">":
                                components = components.Where(c => Convert.ToDouble(property.GetValue(c)) > value).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    components = components.Where(c => property.GetValue(c).ToString().Contains(filter.Value));
                }
            }

            return components;
        }

        public IEnumerable<ComponentBase> GetAllComponentsGeneric<T>() where T : ComponentBase
        {
            return _context.Set<T>().ToList();
        }

        public IEnumerable<ComponentBase> GetAllComponents(Type componentType)
        {
            var setMethod = typeof(PCBuilderContext).GetMethods()
            .FirstOrDefault(m => m.Name == "Set" && m.GetParameters().Length == 0);

            var genericSetMethod = setMethod.MakeGenericMethod(componentType);

            var dbSet = genericSetMethod.Invoke(_context, null);

            return ((IEnumerable)dbSet).Cast<ComponentBase>();
        }
    }
}
