using System;

namespace Synergy.WPF.Navigation.Misc
{
	public class GuidWrapper
	{
		internal Guid Guid { get; }

		public GuidWrapper()
		{
			Guid = new Guid();
		}
	}
}
