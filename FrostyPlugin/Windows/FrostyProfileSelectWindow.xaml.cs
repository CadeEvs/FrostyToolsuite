using System.Collections.Generic;
using System.Windows;
using Frosty.Controls;

namespace Frosty.Core.Windows
{
    public partial class FrostyProfileSelectWindow
    {
        private List<FrostyConfiguration> configurations = new List<FrostyConfiguration>();
        
        public FrostyProfileSelectWindow()
        {
            InitializeComponent();
        }

        private void ProfileSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshConfigurationList();
        }
        
        private void RefreshConfigurationList()
        {
            configurations.Clear();

            foreach (string profile in Config.GameProfiles)
            {
                try
                {
                    configurations.Add(new FrostyConfiguration(profile));
                }
                catch (System.IO.FileNotFoundException)
                {
                    Config.RemoveGame(profile); // couldn't find the exe, so remove it from the profile list
                    Config.Save();
                }
            }

            ConfigurationListView.ItemsSource = configurations;
        }
    }
}