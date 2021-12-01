using SQLite;
using System.ComponentModel;

namespace Reklamacka.Models
{
	public class Store :INotifyPropertyChanged
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }					//!< Store's ID in database
		public string Name { get; set; }			//!< Store's name
		public string Link { get; set; }			//!< Store's website link
		public string Email { get; set; }			//!< Store's contact email
		public string PhoneNumber { get; set; }		//!< Store's contact number

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
