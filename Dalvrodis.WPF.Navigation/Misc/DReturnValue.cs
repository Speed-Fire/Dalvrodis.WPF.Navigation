using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalvrodis.WPF.Navigation.Misc
{
    /// <summary>
    /// Record of dialog result and its return value.
    /// </summary>
    /// <typeparam name="T">Type of return value.</typeparam>
    /// <param name="Result">Dialog result.</param>
    /// <param name="ReturnValue">Dialog return value.</param>
    public record DReturnValue<T>(bool? Result, T? ReturnValue);

	/// <summary>
	/// Dialog close callback.
	/// </summary>
	/// <param name="returnValue">Dialog return value.</param>
	public delegate void DialogCallback<T>(DReturnValue<T> returnValue);

	/// <summary>
	/// Dialog close callback.
	/// </summary>
	/// <param name="dialogResult">Dialog result.</param>
	public delegate void DialogCallback(bool? dialogResult);
}
