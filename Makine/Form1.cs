
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Makine
{
    public partial class Form1 : Form
    {
        DataTable dataTable = new DataTable();
        public Form1()
        {
            InitializeComponent();
            LoadData();
            PopulateMachineDropdown();
        }

        private void LoadData()
        {
            dataTable.Columns.Add("Makine", typeof(string));
            dataTable.Columns.Add("Tarih", typeof(DateTime));
            dataTable.Columns.Add("HedefMiktar", typeof(int));
            dataTable.Columns.Add("UretimMiktar", typeof(int));

            string[] lines = File.ReadAllLines(@"E:\C#\ödevler\SonÖdevler\Ödev1\MakineBilgiler.txt");

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts.Length == 4)
                {
                    string Makine = parts[0];
                    DateTime Tarih = DateTime.ParseExact(parts[1], "dd/MM/yyyy", null);
                    int HedefMiktar = int.Parse(parts[2]);
                    int UretimMiktar = int.Parse(parts[3]);
                    dataTable.Rows.Add(Makine, Tarih, HedefMiktar, UretimMiktar);
                }
            }
            dataGridView1.DataSource = dataTable;
        }
        //-----------------------------------------
        private void PopulateMachineDropdown()
        {                                                                                    //Distinct() tekrar edilen değirler siler 
            var machines = dataTable.AsEnumerable().Select(row => row.Field<string>("Makine")).Distinct().ToList();
            machines.Insert(0, "All");                                                       //ToList()    sonuç dizie  gönderir       
            comboBox1.DataSource = machines;
        }
        //-----------------------------------------
        private void UpdateGridAndChart()
        {
            string selectedMachine = comboBox1.SelectedItem.ToString();
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            var filteredRows = dataTable.AsEnumerable()
                .Where(row => (selectedMachine == "All" || row.Field<string>("Makine") == selectedMachine) &&
                              row.Field<DateTime>("Tarih") >= startDate &&
                              row.Field<DateTime>("Tarih") <= endDate);

            var filteredTable = filteredRows.CopyToDataTable();
            dataGridView1.DataSource = filteredTable;

            chart1.Series.Clear();
            var hedefMiktarSeries = new Series("Hedef Miktar")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.DateTime
            };
            var uretilenMiktarSeries = new Series("Uretilen Miktar")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.DateTime
            };

            foreach (DataRow row in filteredTable.Rows)
            {
                hedefMiktarSeries.Points.AddXY(row.Field<DateTime>("Tarih"), row.Field<int>("HedefMiktar"));
                uretilenMiktarSeries.Points.AddXY(row.Field<DateTime>("Tarih"), row.Field<int>("UretimMiktar"));
            }

            chart1.Series.Add(hedefMiktarSeries);
            chart1.Series.Add(uretilenMiktarSeries);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            UpdateGridAndChart();
        }



    }

}
