using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.ExtensionFramework.Classes;
using ZzukBot.Game.Statics;
using ZzukBot.Objects;
using CustomClassTemplate.Data;
using CustomClassTemplate.Settings;

namespace CustomClassTemplate.Objects
{
    internal class Spellbook
    {
        private static Spell lastSpell = new Spell(string.Empty, -1, false, false);

        private List<Spell> spells;
       // public string ShieldName = "";      For Gui To chose shield!

        //--Healing Spells--//
        //---------------------------------------------------------------------------------------------------------Shield--//
        public static readonly Spell Shield = new Spell("Lightning Shield", 310, false, false,
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    Helpers.CanCast("Lightning Shield") && !Me.GotAura("Lightning Shield"), customAction:
                () =>
                {
                    //--Custom Action - SelfCasting--//
                    Helpers.TryBuff("Lightning Shield");
                });
        //----------------------------------------------------------------------------------------------------Healing Wave--//
        public static readonly Spell HealingWave = new Spell("Healing Wave", 320, false, false,
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    Me.HealthPercent <= 60 &&
                    Helpers.CanCast("Healing Wave"), customAction:
                () =>
                {
                    //--Custom Action - SelfCasting--//
                    Helpers.TryBuff("Healing Wave");
                });
        //---------------------------------------------------------------------------------------------Lesser Healing Wave--//
        public static readonly Spell LesserHealingWave = new Spell("Lesser Healing Wave", 330, false, false,
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    Helpers.CanCast("Lesser Healing Wave") && Me.HealthPercent <= 40, customAction:
                () =>
                {
                    //--Custom Action - SelfCasting--//
                    Helpers.TryBuff("Lesser Healing Wave");
                });

        //--Dmg Spells--//
        //--------------------------------------------------------------------------------------------------Lightning Bolt--//
        public static readonly Spell LightningBolt = new Spell("Lightning Bolt", 750, false, true,
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    Helpers.CanCast("Lightning Bolt")
                 && Me.ManaPercent >= 10, 
            customAction:
                () =>
                {
                    Helpers.TryCast("Lightning Bolt");                    
                });
        
        //------------------------------------------------------------------------------------------------------Earth Shock--//
        public static readonly Spell EarthShock = new Spell("Earth Shock", 700, false, true, true,
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    Helpers.CanCast("Earth Shock") && Target.Position.GetDistanceTo(Me.Position) <= 18
                && (!Helpers.CanCast("Flame Shock") || Target.GotDebuff("Flame Shock")), 
            customAction:
                () =>
                {
                    Helpers.TryCast("Earth Shock");                    
                });
        //-------------------------------------------------------------------------------------------------------Flame Shock--//
        public static readonly Spell FlameShock = new Spell("Flame Shock", 600, false, true, true, 
            isWanted:
                () =>
                //--What Parametters to take care of before casting--//
                    !Target.GotDebuff("Flame Shock") && Target.Position.GetDistanceTo(Me.Position) <= 20 && lastSpell.Priority != 600 &&
                    Helpers.CanCast("Flame Shock"), 
            customAction:
                () =>
                {
                    Helpers.TryCast("Flame Shock");                    
                });               
        //---------------------------------------------------------------------------------------------------Stoneskin Totem--//
        public static readonly Spell StoneskinTotem = new Spell("Stoneskin Totem", 125, true, true, true,
           isWanted:
               () =>
               //--What Parametters to take care of before casting--//
                   !Me.GotAura("Stoneskin") &&
                    Target.Position.GetDistanceTo(Me.Position) <= 20 &&
                   Helpers.CanCast("Stoneskin Totem"), customAction:
                () =>
                {                    
                        Helpers.TryCast("Stoneskin Totem");                                
                });
        //---------------------------------------------------------------------------------------------------Stoneclaw Totem--//
        public static readonly Spell ManaSpring = new Spell("Mana Spring Totem", 150, true, true, true,
           isWanted:
               () =>
               //--What Parametters to take care of before casting--//
                   !Me.GotAura("Mana Spring") &&
                    Target.Position.GetDistanceTo(Me.Position) <= 20 &&
                   Helpers.CanCast("Mana Spring Totem"), 
           customAction:
                () =>
                {
                    Helpers.TryCast("Mana Spring Totem");                    
                });
        //---------------------------------------------------------------------------------------------------Stoneclaw Totem--//
        public static readonly Spell GraceofAir = new Spell("Grace of Air Totem", 200, true, true, true,
           isWanted:
               () =>
               //--What Parametters to take care of before casting--//
                   !Me.GotAura("Grace of Air") &&
                    Target.Position.GetDistanceTo(Me.Position) <= 20 &&
                   Helpers.CanCast("Grace of Air Totem"),  
           customAction:
                () =>
                {
                    Helpers.TryCast("Grace of Air Totem");                    
                });
        
        //--If No Mana--//
        //--------------------------------------------------------------------------------------------------------Shoot Wand--//
        public static readonly Spell Attack = new Spell("Attack", 50, false, true, true, false,
            () => (!Helpers.CanCast("Lightning Bolt") && !Helpers.CanCast("Earth Shock") && !Helpers.CanCast("Flame Shock")) || Me.ManaPercent <= 15 && Helpers.CanCast("Attack"), customAction:
                () =>
                {   
                    ZzukBot.Game.Statics.Spell.Instance.Attack();
                });

        //-------------------------------------------------------------------------------------------------------------------//
        public Spellbook()
        {
            this.spells = new List<Spell>();
            this.InitializeSpellbook();
        }
        //-------------------------------------------------------------------------------------------------------------------//
        public IEnumerable<Spell> GetDamageSpells()
        {
            return Cache.Instance.GetOrStore("damageSpells", () => this.spells.Where(s => !s.IsBuff));
        }
        //-------------------------------------------------------------------------------------------------------------------//
        public IEnumerable<Spell> GetBuffSpells()
        {
            return Cache.Instance.GetOrStore("buffSpells", () => this.spells.Where(s => s.IsBuff && !s.DoesDamage));
        }

        
        //-------------------------------------------------------------------------------------------------------------------//
        public void UpdateLastSpell(Spell spell)
        {
            lastSpell = spell;
        }
        //-------------------------------------------------------------------------------------------------------------------//
        private void InitializeSpellbook()
        {
            foreach (var property in this.GetType().GetFields())
            {
                spells.Add(property.GetValue(property) as Spell);
            }

            spells = spells.OrderBy(s => s.Priority).ToList();
        }
        //-------------------------------------------------------------------------------------------------------------------//
        private static WoWUnit Me
        {
            get { return ObjectManager.Instance.Player; }
        }
        //-------------------------------------------------------------------------------------------------------------------//
        private static WoWUnit Target
        {
            get { return ObjectManager.Instance.Target; }
        }
        //-------------------------------------------------------------------------------------------------------------------//
        private static WoWUnit Pet
        {
            get { return ObjectManager.Instance.Pet; }
        }
        //-------------------------------------------------------------------------------------------------------------------//
    }
}
