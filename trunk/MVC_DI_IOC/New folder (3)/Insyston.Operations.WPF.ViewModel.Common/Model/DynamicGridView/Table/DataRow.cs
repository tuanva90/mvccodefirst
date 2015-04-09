using System;
using System.Reflection;
using WPF.DataTable.Models;

namespace WPF.DataTable.Models
{
    public class DataRow
    {
        private readonly DataTable _owner;
        private object _rowObject;

        public DataRow(DataTable owner)
        {
            this._owner = owner;
        }

        public object this[string columnName]
        {
            get
            {
                return GetValue(columnName);
            }
            set
            {
                SetValue(columnName, value);
            }
        }

        public object RowObject
        {
            get
            {
                EnsureRowObject();
                return _rowObject;
            }
        }

        private void EnsureRowObject()
        {
            if (_rowObject == null)
            {
                _rowObject = (object)Activator.CreateInstance(_owner.TBaseClass);
            }
        }
        #region Private Methods
        private void SetValue(string propertyName, object value)
        {
            PropertyInfo prop = RowObject.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                prop.SetValue(RowObject, value, null);
            }
        }
        private object GetValue(string propertyName)
        {
            PropertyInfo prop = RowObject.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                return prop.GetValue(RowObject, null);
            }
            return null;
        }

        #endregion
    }
}
