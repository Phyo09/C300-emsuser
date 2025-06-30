using System;
using System.Collections.Generic;

namespace C300.Models
{
    public partial class Preference
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        public float LowestTemp { get; set; }
        public float HighestTemp { get; set; }
        public int TempUnit { get; set; }
        public int TempNoti { get; set; }
        public float LowestHumidity { get; set; }
        public float HighestHumidity { get; set; }
        public int HumUnit { get; set; }
        public int HumNoti { get; set; }
        public float LowestLight { get; set; }
        public float HighestLight { get; set; }
        public int LightUnit { get; set; }
        public int LightNoti { get; set; }
        public float LowestWeight { get; set; }
        public float HighestWeight { get; set; }
        public int WeightUnit { get; set; }
        public int WeightNoti { get; set; }
    }
}
