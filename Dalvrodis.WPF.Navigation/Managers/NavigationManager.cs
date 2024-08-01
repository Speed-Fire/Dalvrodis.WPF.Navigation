using Synergy.WPF.Navigation.Components;
using Synergy.WPF.Navigation.Services;
using Synergy.WPF.Navigation.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Synergy.WPF.Navigation.Managers
{
	public class NavigationManager(IServiceProvider services)
	{
		#region Nested classes

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

		#endregion

		private readonly IServiceProvider _services = services;
		private readonly ConcurrentDictionary<object, NavigationChannel> _channels = [];

		private static Dispatcher? Dispatcher => Application.Current?.Dispatcher;

		/// <summary>
		/// Attaches specified navigation service to manager channel,
		/// or create new if not exists, via key.
		/// If there is attached view with the same key, that view and
		/// navigation service will be connected.
		/// </summary>
		/// <param name="key">Channel key.</param>
		/// <param name="navigationService">Navigation service.</param>
		/// <exception cref="InvalidOperationException">If some navigation service as already attached
		/// with this key, then you need to detach it first.
		/// </exception>
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

					Dispatcher?.Invoke(() => existing.Attacher?.Attach(frame));

					return existing;
				});
		}

		/// <summary>
		/// Detach navigation channel from manager. It detaches both navigation service and view.
		/// </summary>
		/// <param name="key">Channel key.</param>
		internal void Detach(object key)
		{
			if (!_channels.TryRemove(key, out var channel))
				return;

			if (channel.Frame is not null)
			{
				Dispatcher?.Invoke(() =>
				{
					channel.Attacher?.Detach(channel.Frame);
					channel.Frame.DataContext = null;
				});
			}

			channel.FrameVM?.Dispose();
		}

		/// <summary>
		/// Attaches view, where <see cref="UserControlFrame"/> will be put.
		/// <paramref name="detach"/> must actually detach <see cref="UserControlFrame"/> from your view!
		/// </summary>
		/// <param name="key">Channel key.</param>
		/// <param name="attach">Attaching delegate.</param>
		/// <param name="detach">Detaching delegate.</param>
		/// <exception cref="InvalidOperationException"></exception>
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
						Dispatcher?.Invoke(() => existing.Attacher.Attach(existing.Frame));

					return existing;
				});
		}
	}
}
