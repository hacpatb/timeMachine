using System;
using System.Collections.Generic;
using System.Windows;


namespace Time_Machine
{
    /// <summary>
    /// Логика взаимодействия для addRecord.xaml
    /// </summary>
    public partial class addRecord : Window
    {
        int cardid = -1;
        string connStr;
        public addRecord(int id, string connStr1)
        {
            InitializeComponent();
            Dictionary<bool, string> dicExitEnter = new Dictionary<bool, string>();
            dicExitEnter.Add(false, "Ушел");
            dicExitEnter.Add(true, "Пришел");
            exitComboBox.ItemsSource = dicExitEnter;
            cardid = id;
            connStr = connStr1;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
           cSetTime t = new cSetTime();
           t.CTRLAREAID = 2;
           t.CARDID = cardid;
           t.PERSONID = cardid;
           t.IS_ENTRANCE = (exitComboBox.SelectedIndex == 0)? false : true;
           t.ADATETIME = new DateTime(myDate.SelectedDate.Value.Year, myDate.SelectedDate.Value.Month, myDate.SelectedDate.Value.Day, Convert.ToInt32(hoursText.Text), Convert.ToInt32(minText.Text), new Random().Next(1, 59));
           t.COMMENT = "";
           t.save(connStr: connStr);
           this.Close();
        }
    }
}
