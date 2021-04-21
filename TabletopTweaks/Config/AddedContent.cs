﻿
namespace TabletopTweaks.Config {
    class AddedContent {
        public bool AberrantBloodline = true;
        public bool CauldronWitchArchetype = true;
        public bool ElementalMasterArchetype = true;

        public void OverrideFixes(AddedContent userSettings) {
            AberrantBloodline = userSettings.AberrantBloodline;
            CauldronWitchArchetype = userSettings.CauldronWitchArchetype;
            ElementalMasterArchetype = userSettings.ElementalMasterArchetype;
        }
    }
}