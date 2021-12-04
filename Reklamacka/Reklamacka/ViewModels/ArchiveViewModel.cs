using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using Reklamacka.Models;
using static Reklamacka.BaseModel;
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

		public bool OlderFirst { get; private set; }
		public string SearchedSubstring { get; set; }

		public Command DeleteSelected;
		public Command EditSelected;
		public Command NameSearch;
		public Command SortByDate;

		public ArchiveViewModel()
		{
			LofBills = new List<ItemBill>();
			EditSelected = new Command(async () =>
			{
				// TODO:
				await System.Threading.Tasks.Task.CompletedTask;
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