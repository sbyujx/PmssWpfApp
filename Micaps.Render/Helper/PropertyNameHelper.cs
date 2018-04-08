using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.Helper
{
    public static class PropertyNameHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
    }
}
