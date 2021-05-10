using System;
using System.Collections.Generic;
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

namespace HomeWork_9
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Bots.Telegram tlg;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Tlg_Click(object sender, RoutedEventArgs e)
        {
            if(tlg == null)
                tlg = new Bots.Telegram();
            if (!tlg.Runniing)
            {
                tlg.Start();
                Tlg.Content = "Телеграм бот стоп.";
            }else
            {
                tlg.Stop();
                Tlg.Content = "Телеграм бот старт.";
            }
            
        }
    }
}
