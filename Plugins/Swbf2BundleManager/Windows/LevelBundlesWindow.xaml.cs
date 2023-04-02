using Frosty.Controls;
using Frosty.Core;
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

namespace BundleManagerRewrite
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class LevelBundlesPopup : FrostyDockableWindow
    {
        Dictionary<string, List<string>> swbf2Levels = new Dictionary<string, List<string>>()
        {
            {"Frontend Only", new List<string> {"win32/Levels/Frontend/Frontend" } },
            {"Prequel Maps", new List<string> { "win32/S5_1/Levels/MP/Geonosis_01/Geonosis_01", "win32/S6_2/Geonosis_02/Levels/Geonosis_02/Geonosis_02",
            "win32/Levels/MP/Kamino_01/Kamino_01", "win32/S7_1/Levels/Kamino_03/Kamino_03", "win32/Levels/MP/Naboo_01/Naboo_01", "win32/Levels/MP/Naboo_02/Naboo_02",
            "win32/S7_2/Levels/Naboo_03/Naboo_03", "win32/S8/Felucia/Levels/MP/Felucia_01/Felucia_01", "win32/Levels/MP/Kashyyyk_01/Kashyyyk_01", "win32/S7/Levels/Kashyyyk_02/Kashyyyk_02"} },
            {"Original Maps", new List<string> { "win32/S3/Levels/Kessel_01/Kessel_01", "win32/S9_3/Scarif/Levels/MP/Scarif_02/Scarif_02", "win32/Levels/MP/Tatooine_01/Tatooine_01", "win32/S9_3/Tatooine_02/Tatooine_02",
            "win32/S2_2/Levels/JabbasPalace_01/JabbasPalace_01", "win32/Levels/MP/Yavin_01/Yavin_01", "win32/Levels/MP/Hoth_01/Hoth_01", "win32/S9_3/Hoth_02/Hoth_02", "win32/S2/Levels/CloudCity_01/CloudCity_01",
            "win32/Levels/MP/Endor_01/Endor_01", "win32/S2_1/Levels/Endor_02/Endor_02" , "win32/S8_1/Endor_04/Endor_04", "win32/Levels/MP/DeathStar02_01/DeathStar02_01" } },
            {"Sequel Maps", new List<string> { "win32/Levels/MP/Jakku_01/Jakku_01", "win32/S9/Jakku_02/Jakku_02", "win32/Levels/MP/Takodana_01/Takodana_01", "win32/S9/Takodana_02/Takodana_02", 
            "win32/Levels/MP/StarKiller_01/StarKiller_01", "win32/S9/StarKiller_02/StarKiller_02", "win32/S9/StarKiller_02/StarKiller_02", "win32/S1/Levels/Crait_01/Crait_01", "win32/S9_3/Crait/Crait_02",
            "win32/S9/Paintball/Levels/MP/Paintball_01/Paintball_01", "win32/S9_3/COOP_NT_MC85/COOP_NT_MC85", "win32/S9_3/COOP_NT_FOSD/COOP_NT_FOSD"} },
            {"Space Maps", new List<string> { "win32/Levels/Space/SB_Kamino_01/SB_Kamino_01", "win32/Levels/Space/SB_DroidBattleShip_01/SB_DroidBattleShip_01", "win32/Levels/Space/SB_Fondor_01/SB_Fondor_01",
            "win32/Levels/Space/SB_Endor_01/SB_Endor_01", "win32/Levels/Space/SB_Resurgent_01/SB_Resurgent_01","win32/S1/Levels/Space/SB_SpaceBear_01/SB_SpaceBear_01" } },
            {"Geonosis Default", new List<string> {"win32/S5_1/Levels/MP/Geonosis_01/Geonosis_01" } },
            {"Geonosis Supremacy", new List<string> {"win32/S6_2/Geonosis_02/Levels/Geonosis_02/Geonosis_02" } },
            {"Kamino Default", new List<string> {"win32/Levels/MP/Kamino_01/Kamino_01" } },
            {"Kamino Supremacy", new List<string> {"win32/S7_1/Levels/Kamino_03/Kamino_03" } },
            {"Naboo Default", new List<string> {"win32/Levels/MP/Naboo_01/Naboo_01" } },
            {"Naboo Hangar", new List<string> {"win32/Levels/MP/Naboo_02/Naboo_02" } },
            {"Naboo Supremacy/Missions", new List<string> {"win32/S7_2/Levels/Naboo_03/Naboo_03" } },
            {"Felucia", new List<string> {"win32/S8/Felucia/Levels/MP/Felucia_01/Felucia_01" } },
            {"Kashyyyk Default", new List<string> {"win32/Levels/MP/Kashyyyk_01/Kashyyyk_01" } },
            {"Kashyyyk Supremacy/Missions", new List<string> {"win32/S7/Levels/Kashyyyk_02/Kashyyyk_02" } },
            {"Kessel", new List<string> {"win32/S3/Levels/Kessel_01/Kessel_01" } },
            {"Scarif", new List<string> {"win32/S9_3/Scarif/Levels/MP/Scarif_02/Scarif_02" } },
            {"Tatooine Default", new List<string> {"win32/Levels/MP/Tatooine_01/Tatooine_01" } },
            {"Tatooine Supremacy", new List<string> {"win32/S9_3/Tatooine_02/Tatooine_02" } },
            {"Jabba's Palace", new List<string> {"win32/S2_2/Levels/JabbasPalace_01/JabbasPalace_01" } },
            {"Yavin", new List<string> {"win32/Levels/MP/Yavin_01/Yavin_01" } },
            {"Hoth Default", new List<string> {"win32/Levels/MP/Hoth_01/Hoth_01" } },
            {"Hoth Supremacy", new List<string> {"win32/S9_3/Hoth_02/Hoth_02" } },
            {"Bespin", new List<string> {"win32/S2/Levels/CloudCity_01/CloudCity_01" } },
            {"Endor Default", new List<string> {"win32/Levels/MP/Endor_01/Endor_01" } },
            {"Endor Ewok Village", new List<string> {"win32/S2_1/Levels/Endor_02/Endor_02" } },
            {"Endor Night Alternate", new List<string> {"win32/S8_1/Endor_04/Endor_04" } },
            {"DeathStar 2", new List<string> {"win32/Levels/MP/DeathStar02_01/DeathStar02_01" } },
            {"Jakku Default", new List<string> {"win32/Levels/MP/Jakku_01/Jakku_01" } },
            {"Jakku Supremacy/Missions", new List<string> {"win32/S9/Jakku_02/Jakku_02" } },
            {"Takodana Default", new List<string> {"win32/Levels/MP/Takodana_01/Takodana_01" } },
            {"Takodana Supremacy/Missions", new List<string> {"win32/S9/Takodana_02/Takodana_02" } },
            {"Starkiller Base Default", new List<string> {"win32/Levels/MP/StarKiller_01/StarKiller_01" } },
            {"Starkiller Base Missions", new List<string> {"win32/S9/StarKiller_02/StarKiller_02" } },
            {"Crait Default", new List<string> {"win32/S1/Levels/Crait_01/Crait_01" } },
            {"Crait HvsV", new List<string> {"win32/S9_3/Crait/Crait_02" } },
            {"Ajan Kloss", new List<string> {"win32/S9/Paintball/Levels/MP/Paintball_01/Paintball_01" } },
            {"COOP MC85", new List<string> {"win32/S9_3/COOP_NT_MC85/COOP_NT_MC85" } },
            {"COOP FOSD", new List<string> {"win32/S9_3/COOP_NT_FOSD/COOP_NT_FOSD" } },
            {"Space - Kamino", new List<string> {"win32/Levels/Space/SB_Kamino_01/SB_Kamino_01" } },
            {"Space - Ryloth", new List<string> {"win32/Levels/Space/SB_DroidBattleShip_01/SB_DroidBattleShip_01" } },
            {"Space - Fondor", new List<string> {"win32/Levels/Space/SB_Fondor_01/SB_Fondor_01" } },
            {"Space - Endor", new List<string> {"win32/Levels/Space/SB_Endor_01/SB_Endor_01" } },
            {"Space - Star Destroyer", new List<string> {"win32/Levels/Space/SB_Resurgent_01/SB_Resurgent_01" } },
            {"Space - Unknown Regions", new List<string> {"win32/S1/Levels/Space/SB_SpaceBear_01/SB_SpaceBear_01" } },
        };

        public List<string> Bundles { get; private set; } = new List<string>();
        public string LevelName { get; private set; } = "";

        public LevelBundlesPopup()
        {
            InitializeComponent();
            foreach (string gamemode in swbf2Levels.Keys)
            {
                levelComboBox.Items.Add(gamemode);
            }
            levelComboBox.SelectedIndex = 0;
            string test = Config.Get<string>("BMO_Swbf2LevelCompleter", "null");
            if (levelComboBox.Items.Contains(test))
                levelComboBox.SelectedIndex = levelComboBox.Items.IndexOf(test);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            LevelName = levelComboBox.Text;
            Bundles = swbf2Levels[levelComboBox.Text];
            Config.Add("BMO_Swbf2LevelCompleter", LevelName);
            DialogResult = true;
            Close();
        }

        private void FrostyDockableWindow_FrostyLoaded(object sender, EventArgs e)
        {

        }
    }
}
