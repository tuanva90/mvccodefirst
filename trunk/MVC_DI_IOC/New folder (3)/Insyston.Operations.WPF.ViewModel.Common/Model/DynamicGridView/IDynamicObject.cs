using System.ComponentModel;

namespace WPF.DataTable.Models
{
    public interface IDynamicObject
    {
        T GetValue<T>(string propertyName);

        void SetValue<T>(string propertyName, T value);

        void OnPropertyChanged(PropertyChangedEventArgs args);
    }
}
