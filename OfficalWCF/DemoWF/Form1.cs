using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoWF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            Category cate = new Category();
            DatabaseProviderFactory factory = new DatabaseProviderFactory();
            Database defaultDB = factory.Create("EFContext");
            string sqlCommand = "Select * from Categories where CategoryID=" + txtInputID.Text;
            DbCommand dbCommand = defaultDB.GetSqlStringCommand(sqlCommand);
            using (IDataReader dataReader = defaultDB.ExecuteReader(dbCommand))
            {
                if (dataReader.Read())
                {
                    txtID.Text = dataReader.GetInt32(0).ToString();
                    txtName.Text = dataReader.GetString(1);
                    txtDescription.Text = dataReader.GetString(2);
                }

            }
        }
    }
}
