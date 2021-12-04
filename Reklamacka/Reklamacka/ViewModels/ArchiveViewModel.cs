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

namespace Reklamacka.ViewModels
{
	public class ArchiveViewModel : INotifyPropertyChanged
	{
		private List<ItemBill> lofBills;
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
		public ItemBill SelectedItem
		{
			get => selectedItem;
			set
			{
				selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		public bool OlderFirst { get; private set; }
		public string SearchedSubstring { get; set; }

		public Command SelectBill { get; set; }
		public Command DeleteSelected { get; set; }
		public Command EditSelected { get; set; }
		public Command NameSearch { get; set; }
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
				if (!string.IsNullOrWhiteSpace(SearchedSubstring))
					LofBills.ForEach(item => item.IsVisible = item.BillItem.ProductName.Contains(SearchedSubstring));

				ObserveBills = new ObservableCollection<ItemBill>(LofBills.Where(item => item.IsVisible));
				await System.Threading.Tasks.Task.CompletedTask;
			});

			SortByDate = new Command(async () =>
			{
				OlderFirst = !OlderFirst;
				if (OlderFirst)
					LofBills = LofBills.OrderBy(item => item.BillItem.ExpirationDate).ToList();
				else
					LofBills = LofBills.OrderByDescending(item => item.BillItem.ExpirationDate).ToList();

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