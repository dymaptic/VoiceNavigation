// <copyright file="BindableUITableViewDataSource.cs" company="Moravec Labs, LLC">
//     MIT License
//
//     Copyright (c) Moravec Labs, LLC.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </copyright>

namespace MoravecLabs.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Foundation;
    using UIKit;

    /// <summary>
    /// Bindable UI Table view data source.  Provides a simple event-driven way to bind 
    /// an observable collection to a UITableView
    /// </summary>
    /// <typeparam name="T">Type of value in the list.</typeparam>
    public class BindableUITableViewDataSource<T> : UITableViewDataSource
    {
        /// <summary>
        /// The get cell function a defined by the developer, called by the get cell on the UITableView.
        /// </summary>
        private Func<UITableView, NSIndexPath, ObservableCollection<T>, UITableViewCell> getCellFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MoravecLabs.UI.BindableUITableViewDataSource`1"/> class.
        /// </summary>
        /// <param name="tableView">Table view.</param>
        /// <param name="dataSource">Data source.</param>
        /// <param name="getCell">Get cell function.</param>
        public BindableUITableViewDataSource(UITableView tableView, ObservableCollection<T> dataSource, Func<UITableView, NSIndexPath, ObservableCollection<T>, UITableViewCell> getCell)
        {
            this.DataSource = dataSource;
            this.TableView = tableView;
            this.getCellFunction = getCell;

            this.DataSource.CollectionChanged += (sender, e) => 
            {
                UIApplication.SharedApplication.InvokeOnMainThread(delegate 
                {
                    this.TableView.ReloadData();
                });
            };
        }

        /// <summary>
        /// Gets the datasource
        /// </summary>
        /// <value>The data source.</value>
        public ObservableCollection<T> DataSource { get; private set; }

        /// <summary>
        /// Gets the table view that is connected to the datasource
        /// </summary>
        /// <value>The table view.</value>
        public UITableView TableView { get; private set; }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="indexPath">Index path.</param>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            return this.getCellFunction(tableView, indexPath, this.DataSource);
        }

        /// <summary>
        /// Number of rows in the section.
        /// </summary>
        /// <returns>The number of rows in the section.</returns>
        /// <param name="tableView">Table view.</param>
        /// <param name="section">The section in question.</param>
        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.DataSource.Count;
        }

        /// <summary>
        /// Number of sections.
        /// </summary>
        /// <returns>The of sections.</returns>
        /// <param name="tableView">Table view.</param>
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }
    }
}
