using System;
using System.Windows.Forms;
using CustomClassTemplate.Settings;

namespace CustomClassTemplate.Gui
{
    public partial class CCGui : Form
    {
        public CCGui()
        {
            InitializeComponent();
            load();
        }


        private void save()
        {
            CustomClassSettings.Values.DrinkName = drinkTb.Text;
            CustomClassSettings.Values.FoodName = foodTb.Text;
            CustomClassSettings.Values.MeleeAttackRange = (int)meleeNud.Value;
            CustomClassSettings.Values.RangedAttackRange = (int)castNud.Value;
            CustomClassSettings.Values.WeaponEnchant = cbWeaponEnh.Text;
            CustomClassSettings.Values.Save();
        }

        private void load()
        {
            CustomClassSettings.Values.Load();
            cbWeaponEnh.Text = CustomClassSettings.Values.WeaponEnchant;
            drinkTb.Text = CustomClassSettings.Values.DrinkName;
            foodTb.Text = CustomClassSettings.Values.FoodName;
            meleeNud.Value = CustomClassSettings.Values.MeleeAttackRange;
            castNud.Value = CustomClassSettings.Values.RangedAttackRange;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            save();
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            load();
        }

        public void cbWeaponEnh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CCGui_Load(object sender, EventArgs e)
        {

        }
    }
}
