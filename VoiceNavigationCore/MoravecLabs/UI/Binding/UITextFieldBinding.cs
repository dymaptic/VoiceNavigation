using System;
using MoravecLabs.Infrastructure;
using UIKit;

namespace MoravecLabs.UI.Binding
{
	public class UITextFieldBinding : BaseBinding
	{
		public UITextFieldBinding(BaseViewModel viewModel, string propertyName, UITextField textField)
		{
			viewModel.SubscribePropertyChanged(propertyName, () => { textField.Text = Convert.ToString(viewModel.GetPropertyByName(propertyName)); });


		}

		void HandleAction()
		{

		}
	}
}
