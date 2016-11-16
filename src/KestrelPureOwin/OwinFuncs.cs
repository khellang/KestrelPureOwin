using System.Collections.Generic;
using System.Threading.Tasks;

namespace KestrelPureOwin
{
    public delegate Task AppFunc(IDictionary<string, object> environment);

    public delegate AppFunc MidFunc(AppFunc next);

    public delegate void BuildFunc(MidFunc factory);
}
