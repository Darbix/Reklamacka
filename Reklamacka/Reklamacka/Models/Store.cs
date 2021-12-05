/**
 * @brief Store model
 * 
 * @file Store.cs
 * @author Do Hung (xdohun00)
 * @date 05/12/2021
 * 
 * This application serves as submission 
 * for a group project of class ITU at FIT, BUT 2021/2022
 */
using SQLite;
using System.ComponentModel;

namespace Reklamacka.Models
{
	public class Store :INotifyPropertyChanged
	{
		/// <summary>
		/// Auto-incrementing database key
		/// </summary>
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }
		/// <summary>
		/// Store's name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Store's website link
		/// </summary>
		public string Link { get; set; }
		/// <summary>
		/// Store's contact email
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// Store's contact number
		/// </summary>
		public string PhoneNumber { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
