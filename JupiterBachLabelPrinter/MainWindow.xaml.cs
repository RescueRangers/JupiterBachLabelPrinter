using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JupiterBachLabelPrinter.Model;
using Newtonsoft.Json;

namespace JupiterBachLabelPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MasterItem selectedMasterItem;
        private Material selectedMaterial;
        private readonly string _zpl = @"^XA
^PQ<<Quantity>>
^CF0,80
^FO50,50^FD<<Master>>^FS
^CF0,60
^FO1150,50,1^FD<<Number>>^FS
^CF0,80
^FO50,190^FD<<Material>>^FS
^CF0,200
^FO100,290^FD<<Item>>^FS
^XZ";

        public ObservableCollection<MasterItem> MasterItems { get; set; }
        public MasterItem SelectedMasterItem
        {
            get => selectedMasterItem;
            set
            {
                selectedMasterItem = value;
                OnPropertyChanged(nameof(SelectedMasterItem));
            }
        }
        public Material SelectedMaterial
        {
            get => selectedMaterial; 
            set
            {
                selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        public int PrintQuantity { get; set; } = 4;
        public string PrinterIp { get; set; } = "172.25.194.77";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            CreateItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CreateItems()
        {
            using (var reader = new StreamReader("Jupiter-Bach-Items.json"))
            {
                var root = JsonConvert.DeserializeObject<Root>(reader.ReadToEnd());
                MasterItems = new ObservableCollection<MasterItem>(root.MasterItems);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMaterial == null || PrintQuantity < 1)
            {
                return;
            }

            if(!IPAddress.TryParse(PrinterIp, out _))
            {
                return;
            }
            var tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.Connect(PrinterIp, 9100);
            using (var writer = new StreamWriter(tcpClient.GetStream()))
            {
                if (SelectedMaterial.ComplexMaterial)
                {
                    foreach (var item in SelectedMaterial.Items.Reverse())
                    {
                        var zplData = _zpl.Replace("<<Master>>", item.MasterItem);
                        zplData = zplData.Replace("<<Quantity>>", $"{PrintQuantity}");
                        zplData = zplData.Replace("<<Number>>", item.SetNumber);
                        zplData = zplData.Replace("<<Material>>", SelectedMaterial.Name);
                        zplData = zplData.Replace("<<Item>>", item.Name);

                        writer.Write(zplData);
                        writer.Flush();
                    }
                }
                else
                {
                    foreach (var item in SelectedMaterial.Items.Reverse())
                    {
                        var zplData = _zpl.Replace("<<Master>>", SelectedMasterItem.Name);
                        zplData = zplData.Replace("<<Quantity>>", $"{PrintQuantity}");
                        zplData = zplData.Replace("<<Number>>", SelectedMasterItem.SetNumber);
                        zplData = zplData.Replace("<<Material>>", SelectedMaterial.Name);
                        zplData = zplData.Replace("<<Item>>", item.Name);

                        writer.Write(zplData);
                        writer.Flush();
                    }
                }
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9
                || e.Key == Key.Delete
                || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void IpTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 
                || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 
                || e.Key == Key.OemPeriod 
                || e.Key == Key.Delete 
                || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }
    }
}
