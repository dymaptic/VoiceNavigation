using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MoravecLabs.Infrastructure
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notifies the property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		public void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Notifies all properties changed.
		/// </summary>
		protected void NotifyAllPropertiesChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
		}

		/// <summary>
		/// Sets the specified value to the referenced storage field.  Only sets it if the newValue is different than the
		/// current value.
		/// </summary>
		/// <returns><c>true</c>, if property value was set, <c>false</c> otherwise.</returns>
		/// <param name="storageField">Storage field.</param>
		/// <param name="newValue">New value.</param>
		/// <param name="propertyName">Property name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected bool SetPropertyValue<T>(ref T storageField, T newValue, [CallerMemberName] string propertyName = "")
		{
			if (Equals(storageField, newValue))
				return false;

			storageField = newValue;
			this.NotifyPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// Returns the value of the specified property.
		/// </summary>
		/// <returns>The property by name.</returns>
		/// <param name="propertyName">Property name.</param>
		public object GetPropertyByName(string propertyName)
		{
			return this.GetType().GetProperty(propertyName).GetValue(this, null);
		}

		/// <summary>
		/// Subscribes the action to the PropertyChanged event, but only for the specified propertyName.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="callBack">Call back.</param>
		public void SubscribePropertyChanged(string propertyName, Action callBack)
		{
			this.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == propertyName)
				{
					callBack();
				}
			};
		}
		#endregion
	}
}
