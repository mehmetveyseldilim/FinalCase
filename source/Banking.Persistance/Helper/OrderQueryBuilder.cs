

using System.Reflection;
using System.Text;
using Banking.Shared;
using Banking.Shared.Exceptions;

namespace Banking.Persistance.Helper
{
    public class GenericQueryParameterProcessor
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var hashMap = GetAllProperties(propertyInfos);

            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                
                var trimmedQueryName = param.Trim();
                var propertyFromQueryName = trimmedQueryName.Split(" ")[0];


                var objectProperty = hashMap.FirstOrDefault(pi =>
                pi.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null) 
                {
                    throw new PropertyNotFoundException(ExceptionErrorMessages.PropertyNotFoundExceptionErrorMessage(propertyFromQueryName));
                }
                    

                var direction = param.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase) 
                    ? "descending" : "ascending";

                orderQueryBuilder.Append($"{objectProperty} {direction}, ");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return orderQuery;
        }

        private static HashSet<string> GetAllProperties(PropertyInfo[] properties) 
        {
            //* Create a new hashset for avoiding duplicated values and fast search operation
            var hashSet = new HashSet<string>();

            foreach (var property in properties)
            {
                string propertyName = property.Name;
                Type propertyType = property.PropertyType;
                string propertyTypeName = property.PropertyType.Name;
                bool isTypeGeneric = propertyType.IsGenericType;
                bool isPropertyTypeClass = propertyType.IsClass;
                bool isThePropertyTypeString = propertyType == typeof(string);


                // }


                // If it is a Navigational Property
                if(isPropertyTypeClass && !isThePropertyTypeString) 
                {
                    // Get Properties of Navigational Property
                    var navigationValueAndStringProperties = GetNonNavigationalProperties(propertyType);
                    
                    // Add it like Author.Name so  System.Linq.Dynamic.Core library can automatically 
                    // create the query
                    foreach (var navProperty in navigationValueAndStringProperties)
                    {
                        hashSet.Add($"{propertyTypeName}.{navProperty.Name}");
                    }
                }

                else if(isTypeGeneric) 
                {
                    var typeArg = property.PropertyType.GetGenericArguments()[0];
                    var typeArgName = typeArg.Name;
                    // bool isICollectionTypeFromGenericArgument = typeof(ICollection).IsAssignableFrom(propertyType);
                    bool isICollectionTypeFromGenericArgument = propertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
                    
                    if(isICollectionTypeFromGenericArgument)
                    {
                        hashSet.Add($"{typeArgName}s.Count()");
                    }

                }
                
                else
                { //~ It is not navigational property. It is either string or value type property.
                    hashSet.Add(propertyName);
                }
            }

            return hashSet;
        }

        //* Getting non-class (non-navigational basically) and string properties from a Type
        private static PropertyInfo[] GetNonNavigationalProperties(Type type) 
        {
            //~ Getting only value types and string type properties. 
            //~ So for Genre model ICollection<Book> Property is not selected
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string))
                                .ToArray();

            return propertyInfos;

        }

    }
}