using System.Linq;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace MoreAuthenticStyles
{
	public class MoreAuthenticStyles : Mod
	{
		private MoreAuthenticStylesSettings settings;

		public MoreAuthenticStyles(ModContentPack content) : base(content)
		{
			settings = GetSettings<MoreAuthenticStylesSettings>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			if (listing.ButtonText("Add Techist to Empire"))
			{
				ModifyEmpireStyle(add: true);
			}

			if (listing.ButtonText("Remove Techist from Empire"))
			{
				ModifyEmpireStyle(add: false);
			}

			listing.End();
		}

		public override string SettingsCategory() => "More Authentic Styles";

		private void ModifyEmpireStyle(bool add)
		{
			Faction empire = Find.FactionManager.OfEmpire; // Get the in-game Empire faction
			if (empire == null)
			{
				Log.Error("Empire faction instance not found in the current game.");
				return;
			}

			if (empire.ideos == null || empire.ideos.PrimaryIdeo == null)
			{
				Log.Error("Empire faction has no ideology or PrimaryIdeo is null.");
				return;
			}

			StyleCategoryDef techist = DefDatabase<StyleCategoryDef>.GetNamed("Techist", false);
			if (techist == null)
			{
				Log.Error("Techist style definition not found.");
				return;
			}

			List<ThingStyleCategoryWithPriority> styles = empire.ideos.PrimaryIdeo.thingStyleCategories;

			if (add)
			{
				if (!styles.Any(s => s.category == techist))
				{
					styles.Add(new ThingStyleCategoryWithPriority(techist, 1f)); // Priority set to 1.0
					Messages.Message("Techist style added to the Empire faction.", MessageTypeDefOf.PositiveEvent, false);
				}
				else
				{
					Messages.Message("Empire already has the Techist style.", MessageTypeDefOf.RejectInput, false);
				}
			}
			else
			{
				if (styles.Any(s => s.category == techist))
				{
					styles.RemoveAll(s => s.category == techist);
					Messages.Message("Techist style removed from the Empire faction.", MessageTypeDefOf.NegativeEvent, false);
				}
				else
				{
					Messages.Message("Empire does not have the Techist style.", MessageTypeDefOf.RejectInput, false);
				}
			}

			// Trigger a re-sort or refresh of IdeoManager to reflect changes
			Find.IdeoManager.SortIdeos();
		}


	}
}

