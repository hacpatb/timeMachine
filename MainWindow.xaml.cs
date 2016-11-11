using System;
using System.Data;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

namespace Time_Machine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        cSetTime t;
        DataSet dataTimes = new DataSet();
        cSetTimeList timeList = new cSetTimeList();
        int cardid = -1;
     

        SolidColorBrush hb = new SolidColorBrush(Colors.Orange);
        SolidColorBrush nb = new SolidColorBrush(Colors.White);

        public MainWindow()
        {
            InitializeComponent();
            //nameListComboBox.ItemsSource = cSetTime.getPerson(connectionStringText.Text).Tables[0].DefaultView;

        }

        private void getListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nameListComboBox.ItemsSource = cSetTime.getPerson(connectionStringText.Text).Tables[0].DefaultView;
                if (nameListComboBox.Items.Count > 0) nameListComboBox.SelectedIndex = 1;

            }
            catch(Exception exc)
            {
                MessageBoxResult result = MessageBox.Show(exc.Message, "Ошибочка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            cSetTimeList s = new cSetTimeList();
            cardid = (int)((DataRowView)nameListComboBox.SelectionBoxItem).Row[0];
            timeDateGrid.ItemsSource = timeList.getTimeE(cardid:cardid,startDate: (DateTime?)startDateTimePicer.SelectedDate,endDate: (DateTime?)endDateTimePicer.SelectedDate, connStr: connectionStringText.Text).Tables[0].DefaultView; // dataset
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            int r = new Random().Next(1, 59);
            t.ADATETIME = new DateTime(myDate.SelectedDate.Value.Year, myDate.SelectedDate.Value.Month, myDate.SelectedDate.Value.Day, Convert.ToInt32(hoursText.Text), Convert.ToInt32(minText.Text),r);
            t.save(connStr: connectionStringText.Text);
           
        }

        private void init(int ID)
        {
            t = new cSetTime(ID, connStr: connectionStringText.Text);
            myDate.SelectedDate = t.ADATETIME;
            hoursText.Text = t.ADATETIME.Hour.ToString();
            minText.Text = t.ADATETIME.Minute.ToString();
            exitLable.Content = (t.IS_ENTRANCE == false)?"Ушел":"Пришел";
        }

        private void timeDateGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (timeDateGrid.SelectedItem != null)
            {
                int id = (int)(timeDateGrid.SelectedItem as DataRowView).Row[0];
                init(id);
            }

        }


        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addRecord recWin = new addRecord(cardid, connectionStringText.Text);
            recWin.ShowDialog();
        }

        
        private void timeDateGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataRowView time = (DataRowView)e.Row.DataContext;
            int num = Convert.ToInt32(time.Row[5]);
            if (num == 1)
            {
                e.Row.Background = hb;
            }
            else {
                e.Row.Background = nb;
            }
        }

       
    }
}
