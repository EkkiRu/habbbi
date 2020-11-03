using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace TestThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int numberPerson;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CharacterOutput(object sender, RoutedEventArgs e)
        {
            if (CharacterNumberTextBox.Text != null && int.TryParse(CharacterNumberTextBox.Text, out numberPerson))
            {
                var character = new Character();
                var url = $"https://swapi.dev/api/people/{numberPerson}/";
                Task.Run(() =>
                {
                    using (var context = new ApplicationContext())
                    {
                        character = context.Characters.Where(x => x.Url.Contains(url)).FirstOrDefault();
                        if (character == null)
                        {
                            Task.Run(() =>
                            {
                                using (var people = new WebClient())
                                {
                                    var response = people.DownloadString(url);
                                    var data = JsonConvert.DeserializeObject<Character>(response);
                                    context.Add(data);
                                }
                            });
                        }
                        Task.Run(() =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                FullNameLabel.Content = character.Name;
                                HeightLabel.Content = character.Height;
                                MassLabel.Content = character.Mass;
                                GenderLabel.Content = character.Gender;
                            });
                        });
                    }

                });

            }
        }
    }
}
