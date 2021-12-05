/**
 * @brief View model of ArchivePage
 * 
 * @detail Show list of out of warranty goods bills
 * 
 * @file ArchiveViewModel.cs
 * @author Do Hung (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Reklamacka.Models;
using static Reklamacka.BaseModel;
using Reklamacka.Pages;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Reklamacka.ViewModels
{
	public class ArchiveViewModel : INotifyPropertyChanged
	{
		private List<ItemBill> lofBills;

		/// <summary>
		/// List of all bills in the database
		/// </summary>
		public List<ItemBill> LofBills
		{
			get => lofBills;
			set
			{
				lofBills = value;
				OnPropertyChanged(nameof(LofBills));
			}
		}

		private ObservableCollection<ItemBill> observeBills;

		/// <summary>
		/// Collection of displayable bills
		/// </summary>
		public ObservableCollection<ItemBill> ObserveBills
		{
			get => observeBills;
			set
			{
				observeBills = value;
				OnPropertyChanged(nameof(ObserveBills));
			}
		}

		private ItemBill selectedItem;

		/// <summary>
		/// Selected bill item
		/// </summary>
		public ItemBill SelectedItem
		{
			get => selectedItem;
			set
			{
				selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		/// <summary>
		/// Order mode sort by expiration date
		/// </summary>
		public bool OlderFirst { get; private set; }

		/// <summary>
		/// String used to filter out bills by name
		/// </summary>
		public string SearchedSubstring { get; set; }

		/// <summary>
		/// Command to select a bill
		/// </summary>
		public Command SelectBill { get; set; }

		/// <summary>
		/// Command to deselect a bill
		/// </summary>
		public Command DeleteSelected { get; set; }

		/// <summary>
		/// Command to open window to edit selected bill
		/// </summary>
		public Command EditSelected { get; set; }

		/// <summary>
		/// Filter bills by name
		/// </summary>
		public Command NameSearch { get; set; }

		/// <summary>
		/// Sort bills by expiration date
		/// </summary>
		public Command SortByDate { get; set; }

		public ArchiveViewModel(INavigation navig)
		{
			LofBills = new List<ItemBill>();
			SelectBill = new Command((e) =>
			{
				if (!(e is ItemBill item))
					return;
				item.IsSelected = !item.IsSelected;
			});
			EditSelected = new Command(async (e) =>
			{
				if (!(e is ItemBill item))
					return;

				await navig.PushAsync(new BillEditPage(navig, item.BillItem));
			});
			DeleteSelected = new Command(async () =>
			{
				LofBills.Where(item => item.IsSelected).ToList().ForEach(item => { BillsDB.DeleteItemAsync(item.BillItem); LofBills.Remove(item); });
				ObserveBills = new ObservableCollection<ItemBill>(LofBills.Where(item => item.IsVisible));
				await System.Threading.Tasks.Task.CompletedTask;
			});

			NameSearch = new Command(async () =>
			{
				// reset filter
				LofBills.ForEach(item => item.IsVisible = true);

				// filter bills by name while ignoring case
				// source: https://stackoverflow.com/a/15464440
				if (!string.IsNullOrWhiteSpace(SearchedSubstring))
					LofBills.Where(item => new CultureInfo("cs-CZ", false).CompareInfo.IndexOf(item.BillItem.ProductName, SearchedSubstring, CompareOptions.IgnoreCase) < 0).ToList().ForEach(item => item.IsVisible = false);

				ObserveBills = new ObservableCollection<ItemBill>(LofBills.Where(item => item.IsVisible).ToList());
				await System.Threading.Tasks.Task.CompletedTask;
			});

			SortByDate = new Command(async () =>
			{
				OlderFirst = !OlderFirst;
				if (OlderFirst)
					LofBills = LofBills.OrderByDescending(item => item.BillItem.ExpirationDate).ToList();
				else
					LofBills = LofBills.OrderBy(item => item.BillItem.ExpirationDate).ToList();

				ObserveBills = new ObservableCollection<ItemBill>(LofBills.Where(item => item.IsVisible));
				await System.Threading.Tasks.Task.CompletedTask;
			});
		}

		public async void OnAppearing()
		{
			LofBills.Clear();
			BillsDB.GetAllExpiredAsync().Result.ForEach(bill => LofBills.Add(new ItemBill(bill)));

			ObserveBills = new ObservableCollection<ItemBill>(LofBills.Where(item => item.IsVisible));
			await System.Threading.Tasks.Task.CompletedTask;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}