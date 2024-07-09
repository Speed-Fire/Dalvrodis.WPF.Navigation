using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synergy.WPF.Navigation.Managers
{
	public class NavigationManager
	{
		private class FrameAttacher(
			Action<UserControlFrame> attach,
			Action<UserControlFrame> detach)
		{
			public Action<UserControlFrame> Attach { get; } = attach;
			public Action<UserControlFrame> Detach { get; } = detach;
		}

		private class NavigationChannel
		{
			public INavigationService? NavigationService { get; set; }
			public FrameAttacher? Attacher { get; set; }
			public UserControlFrame? Frame { get; set; }
			public UserControlFrameVM? FrameVM { get; set; }
		}

		private readonly IServiceProvider _services;

		private readonly ConcurrentDictionary<object, NavigationChannel> _channels = [];

		public NavigationManager(IServiceProvider services)
		{
			_services = services;
		}

		internal void Attach(object key, INavigationService navigationService)
		{
			var frameVM = new UserControlFrameVM(navigationService, _services);
			var frame = new UserControlFrame(frameVM);

			_channels.AddOrUpdate(key,
				key =>
				{
					return new()
					{
						NavigationService = navigationService,
						Frame = frame,
						FrameVM = frameVM,
					};
				},
				(key, existing) =>
				{
					if (existing.NavigationService is not null)
						throw new InvalidOperationException("Some navigation service is already attached to this key! Detach it firstly.");

					existing.NavigationService = navigationService;
					existing.Frame = frame;
					existing.FrameVM = frameVM;

					existing.Attacher?.Attach(frame);

					return existing;
				});
		}

		internal void Detach(object key)
		{
			if (!_channels.TryRemove(key, out var channel))
				return;

			if (channel.Frame is not null)
			{
				channel.Attacher?.Detach(channel.Frame);
				channel.Frame.DataContext = null;
			}

			channel.FrameVM?.Dispose();
		}

		public void Attach(
			object key,
			Action<UserControlFrame> attach,
			Action<UserControlFrame> detach)
		{
			_channels.AddOrUpdate(key,
				key =>
				{
					return new()
					{
						Attacher = new(attach, detach)
					};
				},
				(key, existing) =>
				{
					if (existing.Attacher is not null)
						throw new InvalidOperationException("Some view is already attached to this key!");

					existing.Attacher = new(attach, detach);

					if (existing.Frame is not null)
						existing.Attacher.Attach(existing.Frame);

					return existing;
				});
		}
	}
}
