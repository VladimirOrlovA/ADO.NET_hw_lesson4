﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO.NET_hw_lesson4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string DbDataProvider;
        string connStr;
        string message;

        DbConnection connection;

        public MainWindow()
        {
            InitializeComponent();
        }

        void FillViewTable()
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(DbDataProvider);
            try
            {
                using (connection = dbFactory.CreateConnection())
                {
                    connection.ConnectionString = connStr;
                    connection.Open();
                    DbCommand cmd = dbFactory.CreateCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT TOP 100 * FROM newEquipment";

                    DbDataReader dataReader = cmd.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    dgViewTable.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                string[] msg = message.Split(new char[] { '\n' });
                message = msg[0];
                MessageBox.Show(message);
            }

        }

        private void RbtnSqlServer_Click(object sender, RoutedEventArgs e)
        {
            DbDataProvider = ConfigurationManager.AppSettings["SqlClient"];
            connStr = ConfigurationManager.ConnectionStrings["SqlClientConnStr"].ConnectionString;
            FillViewTable();
        }

        private void RbtnAccessDb_Click(object sender, RoutedEventArgs e)
        {
            DbDataProvider = ConfigurationManager.AppSettings["OleDb"];
            connStr = ConfigurationManager.ConnectionStrings["OleDbConnStr"].ConnectionString;
            FillViewTable();
        }

        private async void BtnDataLoad_Click(object sender, RoutedEventArgs e)
        {
            DbDataProvider = ConfigurationManager.AppSettings["SqlClient"];
            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(DbDataProvider);

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                connection = providerFactory.CreateConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["SqlClientConnStr"].ConnectionString;
                DbCommand cmd = providerFactory.CreateCommand();
                cmd.Connection = connection;
                cmd.CommandText = "Proc_ViewTableNewEquipment";
                cmd.CommandType = CommandType.StoredProcedure;

                await connection.OpenAsync();
                object state1 = connection.State;

                DbDataReader dataReader = await cmd.ExecuteReaderAsync();
                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                connection.Close();
                object state2 = connection.State;
                stopwatch.Stop();
                
                lbStatus.Content = $"State  {state1}-{state2} Runtime: {stopwatch.ElapsedMilliseconds}";

                dgViewTable1.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception exc)
            {
                message = exc.ToString();
                string[] msg = message.Split(new char[] { '\n' });
                message = msg[0];
                MessageBox.Show(message);
                lbStatus.Content = message;
            }
        }
    }
}
