using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Foundation;
using MoravecLabs.Infrastructure;

namespace MoravecLabs.Atom
{
	public class Atom<T> : AtomCore
	{
		private BaseViewModel _viewModel;
		private string _propertyName;

		private T _value;

		public T Value
		{
			get { return this.Get(); }
			set { this.Set(value); }
		}

		private List<Action<T, T>> _onChangeActions;

		public Atom(T value)
		{
			this._value = value;
			_onChangeActions = new List<Action<T, T>>();
		}

		public Atom(T value, BaseViewModel viewModel, string propertyName)
		{
			this._value = value;
			this._viewModel = viewModel;
			this._propertyName = propertyName;
			this._onChangeActions = new List<Action<T, T>>();
		}

		public bool Set(T value)
		{
			if (Equals(value, this._value))
				return false;

			var oldValue = this._value;
			this._value = value;

			//process events here.
			if (this._viewModel != null)
			{
				this._viewModel.NotifyPropertyChanged(this._propertyName);
			}
			this._onChangeActions.ForEach(a => a(oldValue, this._value));

			return true;
		}

		public T Get()
		{
			return this._value;
		}

		public void SubscribePropertyChanged(Action<T, T> callBack)
		{
			this._onChangeActions.Add(callBack);
		}

	}
}
