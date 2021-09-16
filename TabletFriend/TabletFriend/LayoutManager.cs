﻿using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TabletFriend
{
	public class LayoutManager
	{
		private readonly MainWindow _window;

		public LayoutManager(MainWindow window)
		{
			_window = window;
			EventBeacon.Subscribe("files_changed", OnChanged);
			EventBeacon.Subscribe("change_layout", OnChangeLayout);
		}

		
		private void OnChanged(object[] args)
		{
			var sender = args[0];
			var fileSystemArgs = (FileSystemEventArgs)args[1];

			if (fileSystemArgs.FullPath == AppState.CurrentLayoutPath)
			{
				Application.Current.Dispatcher.Invoke(
					delegate
					{
						LoadLayout(fileSystemArgs.FullPath);
					}
				);
			}
		}

		private void OnChangeLayout(object[] obj)
		{
			var path = (string)obj[0];
			LoadLayout(path);
		}


		public void LoadLayout(string path)
		{
			if (AppState.CurrentLayout != null)
			{
				AppState.CurrentLayout.Dispose();
			}
			var layout = LayoutImporter.Import(path);

			if (layout == null)
			{
				return;
			}

			AppState.CurrentLayout = layout;
			UiFactory.CreateUI(AppState.CurrentLayout, _window);
			AppState.CurrentLayoutPath = path;
		}

	}
}
