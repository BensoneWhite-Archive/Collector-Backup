using Menu.Remix.MixedUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace TheCollector
{
    public class TheCollectorOptionsMenu : OptionInterface
    {
        public static Configurable<string> Damage { get; set; }

        private OpListBox DamageList;
        private OpLabel DamageLabel;

        public TheCollectorOptionsMenu()
        {
            Damage = config.Bind("Damage", "1. Monk");
        }

        public override void Update()
        {
            base.Update();
            Color colorOff = new Color(0.1451f, 0.1412f, 0.1529f);
            Color colorOn = new Color(0.6627f, 0.6431f, 0.698f);

            DamageList.greyedOut = false;
            DamageLabel.color = colorOn;
        }

        public override void Initialize()
        {
            var opTab1 = new OpTab(this, "Options");
            Tabs = new[] { opTab1 };

            OpContainer tab1Container = new(new Vector2(0, 0));
            opTab1.AddItems(tab1Container);

            UIelement[] UIArrayElements1 = new UIelement[]
            {
                new OpLabel(0f, 580f, "Options", true),

                DamageList = new OpListBox(Damage, new Vector2(10,425), 100, new List<ListItem>{ new ListItem("1. Monk"),new ListItem("2. Survivor"),new ListItem("3. Hunter") }) { description = Translate("Select your Damage (Monk as default)") },
                DamageLabel = new OpLabel(10f, 530f, Translate("Collector Damage")),

            };
            opTab1.AddItems(UIArrayElements1);
        }

    }
}
