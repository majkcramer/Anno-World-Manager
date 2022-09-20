using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anno_World_Manager.model.helper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// 
    /// int abc = 123;
    /// var refabc = new VariableReference<int>(() => abc, x => { abc = x; });
    /// ... now you can pass around refabc, store it in a field, and so on
    /// refabc.Value = 456;
    /// Console.WriteLine(abc); // 456
    /// Console.WriteLine(refabc.Value); // 456
    /// 
    /// 
    /// 
    /// 
    /// </example>
    /// <remarks>
    /// https://stackoverflow.com/questions/2760087/storing-a-reference-to-an-object-in-c-sharp
    /// + https://stackoverflow.com/questions/24329012/store-reference-to-an-object-in-dictionary
    /// </remarks>
    public sealed class VariableReference<T>
    {
        private Func<T> getter;
        private Action<T> setter;
        public VariableReference(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value
        {
            get { return getter(); }
            set { setter(value); }
        }
    }
}
