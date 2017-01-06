using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Foundation;
using UIKit;

namespace MoravecLabs.UI
{
	//public class Binding<T> where T:NSObject
	//{
	//	private INotifyPropertyChanged _viewModel;
	//	private string _propertyName;
	//	private T _view;
	//	public Binding(INotifyPropertyChanged viewModel, string propertyName, T view)
	//	{
	//		this._viewModel = viewModel;
	//		this._propertyName = propertyName;
	//		this._view = view;

	//		viewModel.PropertyChanged += ViewModel_PropertyChanged;
	//		NSNotificationCenter.DefaultCenter.AddObserver(UITextField.TextFieldTextDidChangeNotification, HandleAction);
	//	}

	//	private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
	//	{
	//		if (e.PropertyName == this._propertyName)
	//		{
				
	//		}
	//	}

	//	private void HandleAction(NSNotification notification)
	//	{
	//		var field = notification.Object as T;

	//	}

	//}
	//public class ObservableValue<T>
	//{
	//	private T _item;
	//	private List<Action<T, T>> _subscribers = new List<Action<T, T>>();

	//	public ObservableValue(T item)
	//	{
	//		_item = item;
	//	}

	//	public bool Subscribe(Action<T, T> newSubscribe)
	//	{
	//		if (this._subscribers.Contains(newSubscribe))
	//			return false;

	//		this._subscribers.Add(newSubscribe);
	//		return true;
	//	}

	//	public bool UnSubscribe(Action<T, T> unSubscribe)
	//	{
	//		if (this._subscribers.Contains(unSubscribe))
	//		{
	//			this._subscribers.Remove(unSubscribe);
	//			return true;
	//		}
	//		return false;
	//	}

	//	public void UnSubscribeAll()
	//	{
	//		_subscribers.Clear();
	//	}

	//	public void Update(T newValue)
	//	{
	//		if (newValue.Equals(this._item))
	//			return;
	//		var oldValue = this._item;
	//		this._item = newValue;

	//		foreach (var s in this._subscribers)
	//		{
	//			s(oldValue, this._item);
	//		}
	//	}
	//}
	public class BindableUITableViewDataSource<T> : UITableViewDataSource
	{
		public ObservableCollection<T> DataSource { get; private set; }
		public UITableView TableView { get; private set; }
		private Func<UITableView, NSIndexPath, ObservableCollection<T>, UITableViewCell> _getCell;

		public BindableUITableViewDataSource( UITableView tableView, ObservableCollection<T> dataSource, Func<UITableView, NSIndexPath, ObservableCollection<T>, UITableViewCell> getCell)
		{
			this.DataSource = dataSource;
			this.TableView = tableView;
			_getCell = getCell;

			this.DataSource.CollectionChanged += (sender, e) => {
				UIApplication.SharedApplication.InvokeOnMainThread(delegate {
					this.TableView.ReloadData();
				});

			};
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return this._getCell(tableView, indexPath, this.DataSource);
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return this.DataSource.Count;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}
	}
}
