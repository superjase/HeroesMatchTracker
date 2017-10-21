﻿using Heroes.Helpers;

namespace Heroes.Icons.Models
{
    public class Talent : TalentBase
    {
        public TalentTier Tier { get; set; }

        public override string ToString() => $"{Tier.GetFriendlyName()} | {ReferenceName}";
    }
}
